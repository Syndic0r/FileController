using FileController.Models;

namespace FileController.Data;
public class BankDbAccess : IDisposable
{
    private DbBankContext _context;
    private DirectoryInfo _filesDirectory;
    public BankDbAccess()
    {
        _context = new DbBankContext();
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string filesPath = Path.Combine(localAppData, AppDomain.CurrentDomain.FriendlyName);
        _filesDirectory = new DirectoryInfo(filesPath);

        if (!_filesDirectory.Exists) _filesDirectory.Create();
    }

    public AccountStatement? AddAccountStatement<TData>(BankAccountStatementFile<TData> statementFile) where TData : BankAccountStatementData
    {
        if (_context.AccountStatements.Any(s => s.Number == statementFile.FileData.StatementNumber))
        {
            return null;
        }

        BankAccount? bankAccount = _context.BankAccounts.Find(statementFile.FileData.IBAN);
        if (bankAccount == null)
        {
            bankAccount = new()
            {
                Bankname = statementFile.FileData.BankName,
                IBAN = statementFile.FileData.IBAN,
                BIC = statementFile.FileData.BIC,
                BLZ = statementFile.FileData.BLZ,
                Number = statementFile.FileData.AccountNumber
            };

            _context.BankAccounts.Add(bankAccount);
        }

        AccountStatement statement = new()
        {
            Account = bankAccount,
            CreationDate = statementFile.FileData.CreationDate,
            CreationTime = statementFile.FileData.CreationTime,
            FileName = statementFile.PdfFileName,
            Number = statementFile.FileData.StatementNumber,
            PagesText = statementFile.FileData.PagesText,
            ValueEnd = statementFile.FileData.AccountValueEnd,
            ValueStart = statementFile.FileData.AccountValueStart,
            Pages = GetStatementPages(statementFile.FileData.PageLines),
            Transactions = GetStatementTransactions(statementFile.FileData.Transactions)
        };

        _context.AccountStatements.Add(statement);
        _context.SaveChanges();

        return statement;
    }

    private static List<StatementTransaction> GetStatementTransactions(AccountStatementTransactions statementTransactions)
    {
        return statementTransactions.Select(t =>
        {
            return new StatementTransaction()
            {
                SortOrder = t.Key,
                Date = t.Value.TransactionDate,
                Type = t.Value.TransactionType,
                Value = t.Value.TransactionValue,
                Details = t.Value.TransactionInfo
            };
        }).ToList();
    }

    private static List<StatementPage> GetStatementPages(SortedDictionary<int, Lines> pages)
    {
        return pages.Select(p =>
        {
            return new StatementPage()
            {
                Number = p.Key,
                Text = p.Value.Text
            };
        }).ToList();
    }

    bool _disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
