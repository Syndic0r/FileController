using FileController.Data;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace FileController.Models;
public abstract record BankAccountStatementData
{
    private protected string _firstPageText;
    public BankAccountStatementData(PdfDocument pdfDocument)
    {
        PageLines = GetLines(pdfDocument);
        _firstPageText = PageLines[1].Text;
    }

    private string? _accountStatementNumber;
    public string StatementNumber 
    {
        get 
        {
            _accountStatementNumber ??= GetStatmentNumber();
            return _accountStatementNumber;
        }
    }

    private string? _bankName;
    public string BankName 
    {
        get
        {
            _bankName ??= GetBankName();
            return _bankName;
        } 
    }

    private int? _blz;
    public int BLZ 
    {
        get
        {
            _blz ??= GetBLZ();
            return _blz.Value;
        } 
    }

    private int? _accountNumber;
    public int AccountNumber 
    {
        get
        {
            _accountNumber ??= GetAccountNumber();
            return _accountNumber.Value;
        }
    }

    private string? _iban;
    public string IBAN 
    {
        get
        {
            _iban ??= GetIBAN();
            return _iban;
        }
    }

    private string? _bic;
    public string BIC 
    {
        get
        {
            _bic ??= GetBIC();
            return _bic;
        }
    }

    private DateOnly? _creationDate;
    public DateOnly CreationDate 
    {
        get
        {
            _creationDate ??= GetCreationDate();
            return _creationDate.Value;
        }
    }

    private TimeOnly? _creationTime;
    public TimeOnly CreationTime 
    {
        get
        {
            _creationTime ??= GetCreationTime();
            return _creationTime.Value;
        } 
    }

    private decimal? _startValue;
    public decimal AccountValueStart 
    {
        get
        {
            _startValue ??= GetStartValue();
            return _startValue.Value;
        }
    }

    private decimal? _endValue;
    public decimal AccountValueEnd 
    {
        get
        {
            _endValue ??= GetEndeValue();
            return _endValue.Value;
        }
    }

    private AccountStatementTransactions? _transactions;
    public AccountStatementTransactions Transactions 
    {
        get
        {
            _transactions ??= GetAccountStatmentTransactions();
            return _transactions;
        }
    }

    public string? _pagesText;
    public string PagesText 
    {
        get
        {
            _pagesText ??= GetPagesText();
            return _pagesText;
        }
    }
    public SortedDictionary<int, Lines> PageLines { get; }

    private string GetPagesText()
    {
        string text = "";
        for (int i = 1; i <= PageLines.Count; i++)
        {
            text += PageLines[i].Text;
        }
        return text;
    }

    private static SortedDictionary<int, Lines> GetLines(PdfDocument pdfDocument)
    {
        SortedDictionary<int, Lines> pageLines = [];

        foreach (Page page in pdfDocument.GetPages())
        {
            Lines lines = [];
            foreach (Word word in page.GetWords())
            {
                double topCoordinate = Math.Round(word.BoundingBox.Top, 0);
                double leftCoordinate = word.BoundingBox.Left;
                if (lines.TryGetValue(topCoordinate, out Line? line))
                {
                    line.Add(leftCoordinate, word);
                    lines[topCoordinate] = line;
                }
                else
                {
                    line ??= [];
                    line.Add(leftCoordinate, word);
                    lines.Add(topCoordinate, line);
                }
            }
            pageLines.Add(page.Number, lines);
        }

        return pageLines;
    }

    private protected abstract decimal GetStartValue();

    private protected abstract decimal GetEndeValue();

    private protected abstract AccountStatementTransactions GetAccountStatmentTransactions();

    private protected abstract TimeOnly GetCreationTime();

    private protected abstract DateOnly GetCreationDate();

    private protected abstract int GetBLZ();

    private protected abstract string GetBankName();

    private protected abstract string GetIBAN();

    private protected abstract string GetBIC();

    private protected abstract int GetAccountNumber();

    private protected abstract string GetStatmentNumber();
}
