using BankingAPI.Application.DTOs.Account;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Get your account details
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<AccountResponse>> GetMyAccount()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Get your account balance
    /// </summary>
    [HttpGet("balance")]
    public async Task<ActionResult<object>> GetBalance()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User not found"));
            var account = await _accountService.GetUserAccountAsync(userId);
            return Ok(new { accountNumber = account.AccountNumber, balance = account.Balance, currency = "USD" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }
}