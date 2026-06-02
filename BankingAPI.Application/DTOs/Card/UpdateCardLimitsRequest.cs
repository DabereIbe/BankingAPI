namespace BankingAPI.Application.DTOs.Card;

public class UpdateCardLimitsRequest
{
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
}