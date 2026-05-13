namespace EmployeeManagement.Model;

public enum TransactionType
{
    Credit = 1,
    Debit = 2
}

public class EmployeeTransaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal RunningBalance { get; set; }
}

public class TransactionCreateRequest
{
    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }
}

public class UserTransactionQueryParameters
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public class NetBalanceResponse
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal NetBalance { get; set; }
}