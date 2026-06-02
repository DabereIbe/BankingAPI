using BankingAPI.Application.DTOs.Account;

namespace BankingAPI.Application.Interfaces;

public interface IAccountService
{
    Task<AccountResponse> GetUserAccountAsync(Guid userId);
    Task<decimal> GetAccountBalanceAsync(Guid accountId);
    Task FreezeAccountAsync(Guid accountId);
    Task UnfreezeAccountAsync(Guid accountId);
}
