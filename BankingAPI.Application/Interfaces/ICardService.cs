using BankingAPI.Application.DTOs.Card;

namespace BankingAPI.Application.Interfaces;

public interface ICardService
{
    Task<DebitCardResponse> IssueCardAsync(Guid accountId, IssueCardRequest request);
    Task<DebitCardResponse> GetCardAsync(Guid cardId);
    Task<IEnumerable<DebitCardResponse>> GetUserCardsAsync(Guid userId);
    Task<CardDetailResponse> GetCardDetailsAsync(Guid cardId);
    Task BlockCardAsync(Guid cardId, BlockCardRequest request);
    Task UnblockCardAsync(Guid cardId);
    Task<DebitCardResponse> UpdateCardLimitsAsync(Guid cardId, decimal dailyLimit, decimal monthlyLimit);
    Task<CardTransactionResponse> ProcessTransactionAsync(Guid cardId, ProcessCardTransactionRequest request);
    Task<IEnumerable<CardTransactionResponse>> GetCardTransactionsAsync(Guid cardId);
    Task ResetMonthlyLimitAsync(Guid cardId);
}