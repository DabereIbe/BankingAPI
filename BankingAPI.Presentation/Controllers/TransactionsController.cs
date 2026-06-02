using BankingAPI.Application.DTOs.Transaction;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public TransactionsController(ITransactionService transactionService, IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    /// <summary>
    /// Deposit funds into your account
    /// </summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<TransactionResponse>> Deposit(DepositRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var result = await _transactionService.DepositAsync(account.Id, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Withdraw funds from your account
    /// </summary>
    [HttpPost("withdraw")]
    public async Task<ActionResult<TransactionResponse>> Withdraw(WithdrawRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var result = await _transactionService.WithdrawAsync(account.Id, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Transfer funds to another account
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<TransactionResponse>> Transfer(TransferRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var result = await _transactionService.TransferAsync(account.Id, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }
    /// <summary>
    /// Transfer funds to an account in a different bank
    /// </summary>
    [HttpPost("inter-bank-transfer")]
    public async Task<ActionResult<TransactionResponse>> InterBankTransfer(TransferRequest request, string bankCode)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var result = await _transactionService.InterBankTransferAsync(account.Id, bankCode, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get your transaction history
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionHistory()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            var transactions = await _transactionService.GetAccountTransactionsAsync(account.Id);
            return Ok(transactions);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }
}