using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Data;
using BankingAPI.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;
    private readonly IAdminService _adminService;

    public AdminController(ITransactionService transactionService, IAccountService accountService, IAdminService adminService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
        _adminService = adminService;
    }

    /// <summary>
    /// Get all registered users
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers()
    {
        var users = await _adminService.GetUsers();
        return Ok(new { success = true, data = users, count = users.Count });
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult> GetAllTransactions()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(new { success = true, data = transactions, count = transactions.Count() });
    }

    /// <summary>
    /// Freeze a user account
    /// </summary>
    [HttpPut("accounts/{id}/freeze")]
    public async Task<ActionResult> FreezeAccount(Guid id)
    {
        try
        {
            await _accountService.FreezeAccountAsync(id);
            return Ok(new { success = true, message = "Account frozen successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Unfreeze a user account
    /// </summary>
    [HttpPut("accounts/{id}/unfreeze")]
    public async Task<ActionResult> UnfreezeAccount(Guid id)
    {
        try
        {
            await _accountService.UnfreezeAccountAsync(id);
            return Ok(new { success = true, message = "Account unfrozen successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse { Success = false, Message = ex.Message });
        }
    }
}