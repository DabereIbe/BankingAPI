namespace BankingAPI.Application.DTOs.Transaction;

public class DepositRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class TransferRequest
{
    public string ReceiverAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}