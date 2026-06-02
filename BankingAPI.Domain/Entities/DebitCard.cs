namespace BankingAPI.Domain.Entities;

public class DebitCard
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string CardNumber { get; set; } = string.Empty; // Masked in responses
    public string CardholderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty; // MM format
    public string ExpiryYear { get; set; } = string.Empty; // YYYY format
    public string CVV { get; set; } = string.Empty; // Masked/encrypted
    public string Pin { get; set; } = string.Empty; // Encrypted
    public string Status { get; set; } = "Active"; // Active, Blocked, Expired, Closed
    public string CardType { get; set; } = "Visa"; // Visa, Mastercard
    public decimal DailyLimit { get; set; } = 5000; // Daily spending limit
    public decimal MonthlyLimit { get; set; } = 50000; // Monthly spending limit
    public decimal DailySpent { get; set; } = 0;
    public decimal MonthlySpent { get; set; } = 0;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public DateTime? BlockedAt { get; set; }
    public string? BlockReason { get; set; }
    
    // Navigation properties
    public Account Account { get; set; } = null!;
    public ICollection<CardTransaction> CardTransactions { get; set; } = new List<CardTransaction>();
}