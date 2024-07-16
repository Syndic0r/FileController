namespace FileController.Models;
public class BankAccountStatementTransaction
{
    private string? _transactionType;
    public string TransactionType 
    { 
        get =>_transactionType ?? string.Empty;
        set
        {
            if (_transactionType != null)
            {
                throw new ArgumentException($"Cannot override {nameof(TransactionType)}");
            }
            _transactionType = value;
        } 
    }

    private decimal? _transactionValue;
    public decimal TransactionValue 
    { 
        get => _transactionValue.GetValueOrDefault();
        set
        {
            if (_transactionValue != null)
            {
                throw new ArgumentException($"Cannot override {nameof(TransactionValue)}");
            }
            _transactionValue = value;
        }
    }

    private DateOnly? _transactionDate;
    public DateOnly TransactionDate 
    { 
        get => _transactionDate.GetValueOrDefault();
        set
        {
            if (_transactionDate != null)
            {
                throw new ArgumentException($"Cannot override {nameof(TransactionDate)}");
            }
            _transactionDate = value;
        } 
    }

    private string? _transactionInfo;
    public string TransactionInfo 
    { 
        get => _transactionInfo ?? string.Empty;
        set
        {
            if (_transactionInfo != null)
            {
                throw new ArgumentException($"Cannot override {nameof(TransactionInfo)}");
            }
            _transactionInfo = value;
        }
    }
}
