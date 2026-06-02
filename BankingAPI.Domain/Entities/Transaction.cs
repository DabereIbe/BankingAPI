namespace BankingAPI.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public Guid? SenderAccountId { get; set; }
    public Guid? ReceiverAccountId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer
    public string Status { get; set; } = "Pending"; // Pending, Successful, Failed
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Account? SenderAccount { get; set; }
    public Account? ReceiverAccount { get; set; }
}