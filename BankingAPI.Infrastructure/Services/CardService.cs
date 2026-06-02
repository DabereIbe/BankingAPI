using BankingAPI.Application.DTOs.Card;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Validators;
using BankingAPI.Domain.Entities;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BankingAPI.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly AppDbContext _context;

    public CardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DebitCardResponse> IssueCardAsync(Guid accountId, IssueCardRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CardholderName))
            throw new InvalidOperationException("Cardholder name is required");

        if (!new[] { "Visa", "Mastercard" }.Contains(request.CardType))
            throw new InvalidOperationException("Invalid card type");

        if (request.DailyLimit <= 0 || request.MonthlyLimit <= 0)
            throw new InvalidOperationException("Card limits must be greater than zero");

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        // Check if user already has an active card
        var existingCard = await _context.DebitCards
            .FirstOrDefaultAsync(c => c.AccountId == accountId && c.Status == "Active");
        if (existingCard != null)
            throw new InvalidOperationException("User already has an active debit card");

        var cardNumber = GenerateCardNumber(request.CardType);
        var expiryDate = DateTime.UtcNow.AddYears(5);

        var card = new DebitCard
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            CardNumber = cardNumber,
            CardholderName = request.CardholderName,
            ExpiryMonth = expiryDate.Month.ToString("D2"),
            ExpiryYear = expiryDate.Year.ToString(),
            CVV = GenerateCVV(),
            Pin = Convert.ToBase64String(AesEncryption.Encrypt(request.Pin))!,
            Status = "Active",
            CardType = request.CardType,
            DailyLimit = request.DailyLimit,
            MonthlyLimit = request.MonthlyLimit,
            IssuedAt = DateTime.UtcNow,
            ExpiryDate = expiryDate
        };

        _context.DebitCards.Add(card);
        await _context.SaveChangesAsync();

        return MapToResponse(card);
    }

    public async Task<DebitCardResponse> GetCardAsync(Guid cardId)
    {
        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        return MapToResponse(card);
    }

    public async Task<IEnumerable<DebitCardResponse>> GetUserCardsAsync(Guid userId)
    {
        var cards = await _context.DebitCards
            .Where(c => c.Account.UserId == userId)
            .ToListAsync();

        return cards.Select(MapToResponse);
    }

    public async Task<CardDetailResponse> GetCardDetailsAsync(Guid cardId)
    {
        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        return new CardDetailResponse
        {
            Id = card.Id,
            CardNumber = card.CardNumber,
            CardholderName = card.CardholderName,
            ExpiryMonth = card.ExpiryMonth,
            ExpiryYear = card.ExpiryYear,
            CVV = card.CVV.Substring(card.CVV.Length - 3), // Last 3 digits only
            Status = card.Status,
            CardType = card.CardType,
            IssuedAt = card.IssuedAt,
            ExpiryDate = card.ExpiryDate
        };
    }

    public async Task BlockCardAsync(Guid cardId, BlockCardRequest request)
    {
        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        if (card.Status == "Blocked")
            throw new InvalidOperationException("Card is already blocked");

        card.Status = "Blocked";
        card.BlockedAt = DateTime.UtcNow;
        card.BlockReason = request.Reason;

        _context.DebitCards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task UnblockCardAsync(Guid cardId)
    {
        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        if (card.Status != "Blocked")
            throw new InvalidOperationException("Card is not blocked");

        card.Status = "Active";
        card.BlockedAt = null;
        card.BlockReason = null;

        _context.DebitCards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task<DebitCardResponse> UpdateCardLimitsAsync(Guid cardId, decimal dailyLimit, decimal monthlyLimit)
    {
        if (dailyLimit <= 0 || monthlyLimit <= 0)
            throw new InvalidOperationException("Card limits must be greater than zero");

        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        if (dailyLimit > monthlyLimit)
            throw new InvalidOperationException("Daily limit cannot exceed monthly limit");

        card.DailyLimit = dailyLimit;
        card.MonthlyLimit = monthlyLimit;

        _context.DebitCards.Update(card);
        await _context.SaveChangesAsync();

        return MapToResponse(card);
    }

    public async Task<CardTransactionResponse> ProcessTransactionAsync(Guid cardId, ProcessCardTransactionRequest request)
    {
        ValidationHelper.ValidateAmount(request.Amount);

        if (string.IsNullOrEmpty(request.Pin))
            throw new InvalidOperationException("Pin cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Merchant))
            throw new InvalidOperationException("Merchant name is required");

        var card = await _context.DebitCards.Include(c => c.Account).FirstOrDefaultAsync(c => c.Id == cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        if (card.Status != "Active")
            throw new InvalidOperationException($"Card is {card.Status} and cannot be used");
        var encryptedBytes = Convert.FromBase64String(card.Pin);

        var decryptedPin = AesEncryption.Decrypt(encryptedBytes);

        if (decryptedPin != request.Pin)
            throw new InvalidOperationException("Wrong Pin");

        if (card.ExpiryDate < DateTime.UtcNow)
        {
            card.Status = "Expired";
            _context.DebitCards.Update(card);
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("Card has expired");
        }

        // Check daily limit
        if (card.DailySpent + request.Amount > card.DailyLimit)
            throw new InvalidOperationException($"Daily limit exceeded. Available: {card.DailyLimit - card.DailySpent}");

        // Check monthly limit
        if (card.MonthlySpent + request.Amount > card.MonthlyLimit)
            throw new InvalidOperationException($"Monthly limit exceeded. Available: {card.MonthlyLimit - card.MonthlySpent}");

        // Check account balance
        if (card.Account.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        var transaction = new CardTransaction
        {
            Id = Guid.NewGuid(),
            DebitCardId = cardId,
            TransactionReference = GenerateTransactionReference(),
            Amount = request.Amount,
            Merchant = request.Merchant,
            MerchantCategory = request.MerchantCategory,
            TransactionType = request.TransactionType,
            Status = "Approved",
            TransactionDate = DateTime.UtcNow
        };

        // Update card spending
        card.DailySpent += request.Amount;
        card.MonthlySpent += request.Amount;

        // Update account balance
        card.Account.Balance -= request.Amount;

        _context.CardTransactions.Add(transaction);
        _context.DebitCards.Update(card);
        _context.Accounts.Update(card.Account);
        await _context.SaveChangesAsync();

        return MapToTransactionResponse(transaction);
    }

    public async Task<IEnumerable<CardTransactionResponse>> GetCardTransactionsAsync(Guid cardId)
    {
        var transactions = await _context.CardTransactions
            .Where(t => t.DebitCardId == cardId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return transactions.Select(MapToTransactionResponse);
    }

    public async Task ResetMonthlyLimitAsync(Guid cardId)
    {
        var card = await _context.DebitCards.FindAsync(cardId);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.MonthlySpent = 0;

        _context.DebitCards.Update(card);
        await _context.SaveChangesAsync();
    }

    private string GenerateCardNumber(string cardType)
    {
        var prefix = cardType == "Visa" ? "4" : "5";
        var random = new Random();
        var number = prefix + random.Next(100000000, 999999999).ToString() + random.Next(10000000, 99999999).ToString();
        return number;
    }

    private string HashPin(string pin)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(hashedBytes);
    }

    private string GenerateCVV()
    {
        var random = new Random();
        return random.Next(100, 999).ToString();
    }

    private string GenerateTransactionReference()
    {
        return "CARD" + DateTime.UtcNow.Ticks + new Random().Next(10000, 99999);
    }

    private DebitCardResponse MapToResponse(DebitCard card)
    {
        var maskedCardNumber = MaskCardNumber(card.CardNumber);
        return new DebitCardResponse
        {
            Id = card.Id,
            CardNumber = maskedCardNumber,
            CardholderName = card.CardholderName,
            ExpiryMonth = card.ExpiryMonth,
            ExpiryYear = card.ExpiryYear,
            Status = card.Status,
            CardType = card.CardType,
            DailyLimit = card.DailyLimit,
            MonthlyLimit = card.MonthlyLimit,
            DailySpent = card.DailySpent,
            MonthlySpent = card.MonthlySpent,
            IssuedAt = card.IssuedAt,
            ExpiryDate = card.ExpiryDate
        };
    }

    private CardTransactionResponse MapToTransactionResponse(CardTransaction transaction)
    {
        return new CardTransactionResponse
        {
            Id = transaction.Id,
            TransactionReference = transaction.TransactionReference,
            Amount = transaction.Amount,
            Merchant = transaction.Merchant,
            MerchantCategory = transaction.MerchantCategory,
            TransactionType = transaction.TransactionType,
            Status = transaction.Status,
            DeclineReason = transaction.DeclineReason,
            TransactionDate = transaction.TransactionDate
        };
    }

    private string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 8)
            return "****";

        var firstFour = cardNumber.Substring(0, 4);
        var lastFour = cardNumber.Substring(cardNumber.Length - 4);
        return $"{firstFour}****{lastFour}";
    }
}