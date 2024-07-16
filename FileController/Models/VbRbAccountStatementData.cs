using FileController.Data;
using FileController.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace FileController.Models;
public partial record VbRbAccountStatementData : BankAccountStatementData
{
    public VbRbAccountStatementData(PdfDocument pdfDocument) : base(pdfDocument) { }

    private protected override decimal GetStartValue()
    {
        Match startBounderyMatch = GetStartBounderyMatch();
        return GetMatchValue(startBounderyMatch);
    }

    private protected override decimal GetEndeValue()
    {
        Match endBounderyMatch = GetEndBounderyMatch();
        return GetMatchValue(endBounderyMatch);
    }

    private Match GetStartBounderyMatch()
    {
        return GetBounderyMatch();
    }

    private Match GetEndBounderyMatch()
    {
        Match endBounderyMatch = GetBounderyMatch().NextMatch();
        if (!endBounderyMatch.Success)
        {
            throw new ArgumentException("No end boundery found. Maybe the document is no valid account statement");
        }

        if (endBounderyMatch.NextMatch().Success)
        {
            throw new ArgumentException("Invalid content bounderies. Can only contain two content bounderies");
        }

        return endBounderyMatch;
    }

    private Match GetBounderyMatch()
    {
        if (!TryMatchRegex(PagesText, ContentBounderiesRegex, out Match? bounderyMatch))
        {
            throw new ArgumentException("No transaction content bounderies found");
        }
        return bounderyMatch;
    }

    private protected override AccountStatementTransactions GetAccountStatmentTransactions()
    {
        if (!TryMatchRegex(PagesText, TransactionRegex, out Match? transactionMatch))
        {
            throw new ArgumentException("No transactions found. Maybe the document is no valid account statement");
        }

        bool hasPageBoundery = TryMatchRegex(PagesText, PageContentBounderiesRegex, out Match? pageBounderiesMatch);
        Match startBounderyMatch = GetStartBounderyMatch();
        Match endBounderyMatch = GetEndBounderyMatch();

        AccountStatementTransactions transactions = [];

        const string transactionType = "Transaction";
        Match currentTransaction = transactionMatch;
        int transactionCounter = 0;
        while (currentTransaction.Success)
        {
            transactionCounter++;
            BankAccountStatementTransaction transaction = new()
            {
                TransactionType = currentTransaction.Groups[transactionType].Value,
                TransactionValue = GetMatchValue(currentTransaction),
                TransactionDate = GetTransactionDate(currentTransaction, CreationDate.Year)
            };

            Match nextTransaction = currentTransaction.NextMatch();
            int transactionInfoEndIndex;
            if (!nextTransaction.Success)
            {
                transactionInfoEndIndex = endBounderyMatch.Index;
            }
            else if (hasPageBoundery && pageBounderiesMatch!.Success && nextTransaction.Index > pageBounderiesMatch!.Index)
            {
                transactionInfoEndIndex = pageBounderiesMatch.Index;
                pageBounderiesMatch = pageBounderiesMatch.NextMatch();
            }
            else
            {
                transactionInfoEndIndex = nextTransaction.Index;
            }

            int transactionInfoStartIndex = currentTransaction.Index + currentTransaction.Value.Length;

            if (transactionInfoStartIndex == transactionInfoEndIndex)
            {
                transaction.TransactionInfo = "";
            }
            else
            {
                int transactionInfoLength = transactionInfoEndIndex - transactionInfoStartIndex;
                transaction.TransactionInfo = PagesText.Substring(transactionInfoStartIndex, transactionInfoLength).Trim(' ', '\n');
            }

            transactions.Add(transactionCounter, transaction);

            currentTransaction = nextTransaction;
        }

        return transactions;
    }

    private static DateOnly GetTransactionDate(Match transactionMatch, int year)
    {
        const string dueDate = "DueDate";
        const string dueDateFormat = "dd.MM.yyyy";
        string dueDateText = $"{transactionMatch.Groups[dueDate].Value}{year}";
        DateOnly transactionDate = DateOnly.ParseExact(dueDateText, dueDateFormat, CultureInfo.InvariantCulture);
        return transactionDate;
    }

    private protected override TimeOnly GetCreationTime()
    {
        if (!TryMatchRegex(PagesText, CreationDateTimeRegex, out Match? dateTimeMatch))
        {
            throw new ArgumentException("No creatione date time contained in text");
        }

        const string timeFormat = "HH:mm";
        string timeText = dateTimeMatch.Groups["time"].Value;
        return TimeOnly.ParseExact(timeText, timeFormat, CultureInfo.InvariantCulture);
    }

    private protected override DateOnly GetCreationDate()
    {
        if (!TryMatchRegex(PagesText, CreationDateTimeRegex, out Match? dateTimeMatch))
        {
            throw new ArgumentException("No creatione date time contained in text");
        }
        const string dateFormat = "dd.MM.yyyy";
        string dateText = dateTimeMatch.Groups["date"].Value;
        return DateOnly.ParseExact(dateText, dateFormat, CultureInfo.InvariantCulture);
    }

    private protected override int GetBLZ()
    {
        if (!TryMatchRegex(_firstPageText, BLZRegex, out Match? blzMatch))
        {
            throw new ArgumentException("No bic contained on first page");
        }

        string blzText = blzMatch.Groups["blzNum"].Value.Replace(" ", "");
        int blz = int.Parse(blzText);
        return blz;
    }

    private protected override string GetBankName()
    {
        if (!TryMatchRegex(_firstPageText, BankNameRegex, out Match? bankNameMatch))
        {
            throw new ArgumentException("No bank name contained on first page");
        }

        string bankName = bankNameMatch.Value;
        return bankName;
    }

    private protected override string GetIBAN()
    {
        if (!TryMatchRegex(_firstPageText, IBANRegex, out Match? ibanMatch))
        {
            throw new ArgumentException("No iban contained on first page");
        }

        string iban = ibanMatch.Groups["IBAN"].Value;
        return iban;
    }

    private protected override string GetBIC()
    {
        if (!TryMatchRegex(_firstPageText, BICRegex, out Match? bicMatch))
        {
            throw new ArgumentException("No bic contained on first page");
        }

        string bic = bicMatch.Groups["BIC"].Value;
        return bic;
    }

    private protected override int GetAccountNumber()
    {
        if (!TryMatchRegex(_firstPageText, AccountNumberRegex, out Match? accountNumberMatch))
        {
            throw new ArgumentException("No account number contained on first page");
        }

        string accountNumberText = accountNumberMatch.Groups["AccountNum"].Value.Replace(" ", "");
        int accountNumber = int.Parse(accountNumberText);
        return accountNumber;
    }

    private protected override string GetStatmentNumber()
    {
        if (!TryMatchRegex(_firstPageText, StatementNumberRegex, out Match? accountStatementNumMatch))
        {
            throw new InvalidAccountStatementException();
        }

        string accountStatmentNumber = accountStatementNumMatch.Value;
        return accountStatmentNumber;
    }

    private static decimal GetMatchValue(Match accountValueMatch)
    {
        const string accountValue = "Value";
        const string sollHaben = "SollHaben";
        string accountValueText = accountValueMatch.Groups[accountValue].Value;
        string sollHabenText = accountValueMatch.Groups[sollHaben].Value;
        decimal value = decimal.Parse(accountValueText);

        if (sollHabenText.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
            value *= -1;
        }
        return value;
    }

    private static bool TryMatchRegex(string text, Func<Regex> regex, [NotNullWhen(true)] out Match? match)
    {
        match = regex().Match(text);

        if (!match.Success)
        {
            match = null;
            return false;
        }
        return true;
    }

    [GeneratedRegex(@"^(([a-zA-ZäöüÄÖÜß]+\s)+)(?<date>(([0-9]{2}.){2})([0-9]{4}))\s(?<Value>\b(((\d{1,3}\.)+)?(?(7)\d{3}|(?<!\.)\d{1,3}),\d{2})\b)(\s)?(?<SollHaben>H|S)$", RegexOptions.Multiline)]
    private static partial Regex ContentBounderiesRegex();

    [GeneratedRegex(@"Übertrag auf Blatt\s\d\s(?<Value>\b(((\d{1,3}\.)+)?(?(2)\d{3}|(?<!\.)\d{1,3}),\d{2})\b)\s+(?<SollHaben>H|S)$", RegexOptions.Multiline)]
    private static partial Regex PageContentBounderiesRegex();

    [GeneratedRegex(@"^(((?<DueDate>(\d{2}\.){2})\s)(\d{2}\.){2})\s((?<Transaction>([a-zA-ZäöüÄÖÜß\/\-.0-9]+\s?)+))\s(?<Value>\b(((\d{1,3}\.)+)?(?(8)\d{3}|(?<!\.)\d{1,3}),\d{2})\b)\s+(?<SollHaben>H|S)$", RegexOptions.Multiline)]
    private static partial Regex TransactionRegex();

    [GeneratedRegex(@"^(\w+\s)+(?<date>(\d{2}.){2}\d{4})\s(?<time>(\d{2}:?){2})(\s\w+\s\d){2}$", RegexOptions.Multiline)]
    private static partial Regex CreationDateTimeRegex();

    [GeneratedRegex(@"\b[0-9]{1,3}\/[0-9]{4}\b", RegexOptions.Multiline)]
    private static partial Regex StatementNumberRegex();

    [GeneratedRegex(@"Kontonummer\s(?<AccountNum>\d+)$", RegexOptions.Multiline)]
    private static partial Regex AccountNumberRegex();

    [GeneratedRegex(@"^.*bank.*$", RegexOptions.Multiline)]
    private static partial Regex BankNameRegex();

    [GeneratedRegex(@"BLZ\s(?<blzNum>(\d{2,3}\s?){3})$", RegexOptions.Multiline)]
    private static partial Regex BLZRegex();

    [GeneratedRegex(@"IBAN:\s(?<IBAN>[A-Z]{2}\d{2}\s(\d{4}\s){4}\d{2})", RegexOptions.Multiline)]
    private static partial Regex IBANRegex();

    [GeneratedRegex(@"BIC:\s(?<BIC>[A-Z0-9]{11})$", RegexOptions.Multiline)]
    private static partial Regex BICRegex();
}
