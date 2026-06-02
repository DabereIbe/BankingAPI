using System.Globalization;

namespace BankingAPI.Application.Validators;

public static class ValidationHelper
{
    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Email is required");

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
                throw new InvalidOperationException("Invalid email format");
        }
        catch
        {
            throw new InvalidOperationException("Invalid email format");
        }
    }

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Password is required");

        if (password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters long");
    }

    public static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new InvalidOperationException("Full name is required");

        if (fullName.Length < 3)
            throw new InvalidOperationException("Full name must be at least 3 characters long");

        if (fullName.Length > 100)
            throw new InvalidOperationException("Full name must not exceed 100 characters");
    }

    public static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero");
    }
}