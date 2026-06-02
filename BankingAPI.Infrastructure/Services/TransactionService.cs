using BankingAPI.Application.DTOs.Transaction;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Validators;
using BankingAPI.Domain.Entities;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BankingAPI.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public TransactionService(AppDbContext context, HttpClient httpClient, IConfiguration config)
    {
        _context = context;
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<TransactionResponse> DepositAsync(Guid accountId, DepositRequest request)
    {
        ValidationHelper.ValidateAmount(request.Amount);

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        if (account.Status == "Frozen")
            throw new InvalidOperationException("Cannot perform transaction on a frozen account");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            ReceiverAccountId = accountId,
            Amount = request.Amount,
            TransactionType = "Deposit",
            Status = "Successful",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        account.Balance += request.Amount;
        _context.Transactions.Add(transaction);
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        return MapToResponse(transaction);
    }

    public async Task<TransactionResponse> WithdrawAsync(Guid accountId, WithdrawRequest request)
    {
        ValidationHelper.ValidateAmount(request.Amount);

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        if (account.Status == "Frozen")
            throw new InvalidOperationException("Cannot perform transaction on a frozen account");

        if (account.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            SenderAccountId = accountId,
            Amount = request.Amount,
            TransactionType = "Withdrawal",
            Status = "Successful",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        account.Balance -= request.Amount;
        _context.Transactions.Add(transaction);
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        return MapToResponse(transaction);
    }

    public async Task<TransactionResponse> TransferAsync(Guid senderAccountId, TransferRequest request)
    {
        ValidationHelper.ValidateAmount(request.Amount);

        if (string.IsNullOrWhiteSpace(request.ReceiverAccountNumber))
            throw new InvalidOperationException("Receiver account number is required");

        var senderAccount = await _context.Accounts.FindAsync(senderAccountId);
        if (senderAccount == null)
            throw new InvalidOperationException("Sender account not found");

        if (senderAccount.Status == "Frozen")
            throw new InvalidOperationException("Cannot transfer from a frozen account");

        if (senderAccount.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.ReceiverAccountNumber);
        if (receiverAccount == null)
            throw new InvalidOperationException("Receiver account not found");

        if (receiverAccount.Status == "Frozen")
            throw new InvalidOperationException("Cannot transfer to a frozen account");

        if (senderAccountId == receiverAccount.Id)
            throw new InvalidOperationException("Cannot transfer to the same account");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            SenderAccountId = senderAccountId,
            ReceiverAccountId = receiverAccount.Id,
            Amount = request.Amount,
            TransactionType = "Transfer",
            Status = "Successful",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        senderAccount.Balance -= request.Amount;
        receiverAccount.Balance += request.Amount;

        _context.Transactions.Add(transaction);
        _context.Accounts.Update(senderAccount);
        _context.Accounts.Update(receiverAccount);
        await _context.SaveChangesAsync();

        return MapToResponse(transaction);
    }
    public async Task<TransactionResponse> InterBankTransferAsync(Guid senderAccountId, string bankCode, TransferRequest request)
    {
        ValidationHelper.ValidateAmount(request.Amount);

        if (string.IsNullOrWhiteSpace(request.ReceiverAccountNumber))
            throw new InvalidOperationException("Receiver account number is required");

        var senderAccount = await _context.Accounts.FindAsync(senderAccountId);
        if (senderAccount == null)
            throw new InvalidOperationException("Sender account not found");

        if (senderAccount.Status == "Frozen")
            throw new InvalidOperationException("Cannot transfer from a frozen account");

        if (senderAccount.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);

        var response = await _httpClient.GetAsync(new Uri($"https://api.paystack.co/bank/resolve?account_number={request.ReceiverAccountNumber}&bank_code={bankCode}"));
        var receiverAccount = JsonConvert.DeserializeObject<InterbankResponse>(await response.Content.ReadAsStringAsync());
        if (receiverAccount == null)
            throw new InvalidOperationException("Receiver account not found");

        if (!receiverAccount.status)
            throw new InvalidOperationException("Cannot transfer to this account");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = GenerateReferenceNumber(),
            SenderAccountId = senderAccountId,
            ReceiverAccountId = null,
            Amount = request.Amount,
            TransactionType = "Transfer",
            Status = "Successful",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        senderAccount.Balance -= request.Amount;

        _context.Transactions.Add(transaction);
        _context.Accounts.Update(senderAccount);
        await _context.SaveChangesAsync();

        return MapToResponse(transaction);
    }

    public async Task<IEnumerable<TransactionResponse>> GetAccountTransactionsAsync(Guid accountId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.SenderAccountId == accountId || t.ReceiverAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(MapToResponse);
    }

    public async Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync()
    {
        var transactions = await _context.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(MapToResponse);
    }

    private string GenerateReferenceNumber()
    {
        return "TXN" + DateTime.UtcNow.Ticks + new Random().Next(1000, 9999);
    }

    private TransactionResponse MapToResponse(Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            ReferenceNumber = transaction.ReferenceNumber,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType,
            Status = transaction.Status,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        };
    }
}