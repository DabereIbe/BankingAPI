using BankingAPI.Application.DTOs.Transaction;

namespace BankingAPI.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponse> DepositAsync(Guid accountId, DepositRequest request);
    Task<TransactionResponse> WithdrawAsync(Guid accountId, WithdrawRequest request);
    Task<TransactionResponse> TransferAsync(Guid senderAccountId, TransferRequest request);
    Task<TransactionResponse> InterBankTransferAsync(Guid senderAccountId, string bankCode, TransferRequest request);
    Task<IEnumerable<TransactionResponse>> GetAccountTransactionsAsync(Guid accountId);
    Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync();
}
