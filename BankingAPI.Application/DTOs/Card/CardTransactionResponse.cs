namespace BankingAPI.Application.DTOs.Card;

public class CardTransactionResponse
{
    public Guid Id { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Merchant { get; set; } = string.Empty;
    public string MerchantCategory { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DeclineReason { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
}