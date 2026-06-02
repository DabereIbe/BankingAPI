using BankingAPI.Application.DTOs.Card;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly IAccountService _accountService;

    public CardsController(ICardService cardService, IAccountService accountService)
    {
        _cardService = cardService;
        _accountService = accountService;
    }

    /// <summary>
    /// Issue a new debit card for your account
    /// </summary>
    [HttpPost("issue")]
    public async Task<ActionResult<DebitCardResponse>> IssueCard(IssueCardRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var card = await _cardService.IssueCardAsync(account.Id, request);
            return StatusCode(StatusCodes.Status201Created, card);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get your debit cards
    /// </summary>
    [HttpGet("my-cards")]
    public async Task<ActionResult<IEnumerable<DebitCardResponse>>> GetMyCards()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var cards = await _cardService.GetUserCardsAsync(userId);
            return Ok(cards);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get card details (with full card number and CVV)
    /// </summary>
    [HttpGet("{cardId}/details")]
    public async Task<ActionResult<CardDetailResponse>> GetCardDetails(Guid cardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var card = await _cardService.GetCardAsync(cardId);
            
            // Verify ownership
            var account = await _accountService.GetUserAccountAsync(userId);
            var userCards = await _cardService.GetUserCardsAsync(userId);
            if (!userCards.Any(c => c.Id == cardId))
                return Forbid();

            var details = await _cardService.GetCardDetailsAsync(cardId);
            return Ok(details);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Block your debit card
    /// </summary>
    [HttpPost("{cardId}/block")]
    public async Task<ActionResult> BlockCard(Guid cardId, BlockCardRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var userCards = await _cardService.GetUserCardsAsync(userId);
            if (!userCards.Any(c => c.Id == cardId))
                return Forbid();

            await _cardService.BlockCardAsync(cardId, request);
            return Ok(new { success = true, message = "Card blocked successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Unblock your debit card
    /// </summary>
    [HttpPost("{cardId}/unblock")]
    public async Task<ActionResult> UnblockCard(Guid cardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var userCards = await _cardService.GetUserCardsAsync(userId);
            if (!userCards.Any(c => c.Id == cardId))
                return Forbid();

            await _cardService.UnblockCardAsync(cardId);
            return Ok(new { success = true, message = "Card unblocked successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Update card spending limits
    /// </summary>
    [HttpPut("{cardId}/limits")]
    public async Task<ActionResult<DebitCardResponse>> UpdateLimits(Guid cardId, [FromBody] UpdateCardLimitsRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var userCards = await _cardService.GetUserCardsAsync(userId);
            if (!userCards.Any(c => c.Id == cardId))
                return Forbid();

            var card = await _cardService.UpdateCardLimitsAsync(cardId, request.DailyLimit, request.MonthlyLimit);
            return Ok(card);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Process a card transaction (POS, Online, ATM, etc.)
    /// </summary>
    [HttpPost("{cardId}/transaction")]
    public async Task<ActionResult<CardTransactionResponse>> ProcessTransaction(Guid cardId, ProcessCardTransactionRequest request)
    {
        try
        {
            var transaction = await _cardService.ProcessTransactionAsync(cardId, request);
            return StatusCode(StatusCodes.Status201Created, transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get card transaction history
    /// </summary>
    [HttpGet("{cardId}/transactions")]
    public async Task<ActionResult<IEnumerable<CardTransactionResponse>>> GetTransactions(Guid cardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var userCards = await _cardService.GetUserCardsAsync(userId);
            if (!userCards.Any(c => c.Id == cardId))
                return Forbid();

            var transactions = await _cardService.GetCardTransactionsAsync(cardId);
            return Ok(transactions);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }
}