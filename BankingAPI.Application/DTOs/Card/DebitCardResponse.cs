namespace BankingAPI.Application.DTOs.Card;

public class DebitCardResponse
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } = string.Empty; // Masked: 4111****1111
    public string CardholderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal DailySpent { get; set; }
    public decimal MonthlySpent { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
}