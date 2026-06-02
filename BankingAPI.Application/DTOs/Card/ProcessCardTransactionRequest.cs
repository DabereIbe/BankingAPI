namespace BankingAPI.Application.DTOs.Card;

public class ProcessCardTransactionRequest
{
    public string Merchant { get; set; } = string.Empty;
    public string MerchantCategory { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty; // POS, Online, ATM, Transfer
    public string Pin { get; set; } = string.Empty;
}