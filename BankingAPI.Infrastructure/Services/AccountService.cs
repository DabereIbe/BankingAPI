using BankingAPI.Application.DTOs.Account;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AccountResponse> GetUserAccountAsync(Guid userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        return MapToResponse(account);
    }

    public async Task<decimal> GetAccountBalanceAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        return account.Balance;
    }

    public async Task FreezeAccountAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        account.Status = "Frozen";
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task UnfreezeAccountAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        account.Status = "Active";
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    private AccountResponse MapToResponse(Domain.Entities.Account account)
    {
        return new AccountResponse
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            AccountType = account.AccountType,
            Status = account.Status,
            CreatedAt = account.CreatedAt
        };
    }
}