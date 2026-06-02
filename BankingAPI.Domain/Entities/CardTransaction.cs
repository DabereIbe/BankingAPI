namespace BankingAPI.Domain.Entities;

public class CardTransaction
{
    public Guid Id { get; set; }
    public Guid DebitCardId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Merchant { get; set; } = string.Empty;
    public string MerchantCategory { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty; // POS, Online, ATM, Transfer
    public string Status { get; set; } = "Pending"; // Pending, Approved, Declined
    public string DeclineReason { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public DebitCard DebitCard { get; set; } = null!;
}