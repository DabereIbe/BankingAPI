namespace BankingAPI.Application.DTOs.Card;

public class IssueCardRequest
{
    public string CardholderName { get; set; } = string.Empty;
    public string CardType { get; set; } = "Visa"; // Visa, Mastercard
    public string Pin { get; set; } = string.Empty;
    public decimal DailyLimit { get; set; } = 5000;
    public decimal MonthlyLimit { get; set; } = 50000;
}