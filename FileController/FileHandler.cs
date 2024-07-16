using FileController.Data;
using FileController.Exceptions;
using FileController.Models;
using System.IO.Compression;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;

namespace FileController;
public class FileHandler
{
    private readonly DirectoryInfo _filesDirectory;
    public FileHandler()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string filesPath = Path.Combine(localAppData, AppDomain.CurrentDomain.FriendlyName);
        _filesDirectory = new DirectoryInfo(filesPath);

        if (!_filesDirectory.Exists) _filesDirectory.Create();
    }

    public List<BankAccountStatementFile<TData>> CreateAccountStatements<TData>(string filesPath, Action<ProgressReport> reportProgressCallback) where TData : BankAccountStatementData
    {
        reportProgressCallback(new ProgressReport(1, $"Reading files from {filesPath}"));
        string[] files = Directory.GetFiles(filesPath);

        double progressPerFile = 99 / (double)files.Length;
        List<BankAccountStatementFile<TData>> accountStatementFiles = [];

        HashSet<string> addedAccountStatements = [];
        using BankDbAccess dbAccess = new();

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];

            if (!File.Exists(file)) continue;

            double currentProgress = (i + 1) * progressPerFile;
            reportProgressCallback(new ProgressReport(currentProgress, $"Processing file {file}"));

            if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                using ZipArchive archive = ZipFile.OpenRead(file);

                double progressPerZipEntry = progressPerFile / archive.Entries.Count;

                for (int j = 0; j < archive.Entries.Count; j++)
                {
                    ZipArchiveEntry entry = archive.Entries[j];
                    if (Path.GetExtension(entry.FullName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        double currentZipProgress = currentProgress + (progressPerZipEntry * (j + 1));
                        reportProgressCallback(new ProgressReport(currentZipProgress, $"Processing file {entry.FullName}"));

                        using Stream fs = entry.Open();
                        using PdfDocument pdfDoc = GetPdfDocument(fs);
                        BankAccountStatementFile<TData> accountStatmentFile = new(file, entry.FullName, pdfDoc);

                        if (!IsStatementUnique(accountStatmentFile, addedAccountStatements))
                        {
                            reportProgressCallback(new ProgressReport(currentZipProgress, $"Skipped file {entry.FullName}"));
                            continue;
                        }

                        if (IsAccountStatementFileHandled(accountStatmentFile, pdfDoc))
                        {
                            accountStatementFiles.Add(accountStatmentFile);
                        }
                    }
                }
            }

            if (Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using FileStream fs = File.OpenRead(file);
                using PdfDocument pdfDoc = GetPdfDocument(fs);
                string fileName = Path.GetFileName(file);
                BankAccountStatementFile<TData> accountStatmentFile = new(file, fileName, pdfDoc);

                if (IsStatementUnique(accountStatmentFile, addedAccountStatements))
                {
                    if (IsAccountStatementFileHandled(accountStatmentFile, pdfDoc))
                    {
                        accountStatementFiles.Add(accountStatmentFile);
                    }
                }
                else
                {
                    reportProgressCallback(new ProgressReport(currentProgress, $"Skipped file {fileName}"));
                }
            }

            MoveFileToBackup(file);
        }

        reportProgressCallback(new ProgressReport(100, "Processing files finisched"));
        return accountStatementFiles;
    }

    private void MoveFileToBackup(string file)
    {
        if (!File.Exists(file)) return;

        DirectoryInfo backupDir = _filesDirectory.CreateSubdirectory("Backup");
        string filename = Path.GetFileName(file);
        string newFilename = Path.Combine(backupDir.FullName, filename);
        File.Move(file, newFilename);
    }

    private bool IsAccountStatementFileHandled<TData>(BankAccountStatementFile<TData> statementFile, PdfDocument pdfDocument) where TData : BankAccountStatementData
    {
        using BankDbAccess dbAccess = new();
        AccountStatement? statement = dbAccess.AddAccountStatement(statementFile);
        if (statement != null)
        {
            WritePdf(statement.Id, pdfDocument, statement.Number);
            return true;
        }
        return false;
    }

    private void WritePdf(int statementId, PdfDocument document, string fileName)
    {
        using PdfDocumentBuilder builder = new();
        builder.AddPage(document, document.NumberOfPages);

        DirectoryInfo currentFileDirectory = _filesDirectory.CreateSubdirectory(statementId.ToString());
        string pdfFilePath = Path.Combine(currentFileDirectory.FullName, fileName.Replace('/', '_') + ".pdf");

        FileInfo pdfFile = new(pdfFilePath);
        using FileStream fs = pdfFile.Create();

        byte[] fileBytes = builder.Build();
        fs.Write(fileBytes);
        fs.Flush();
    }

    private static bool IsStatementUnique<TData>(BankAccountStatementFile<TData> statementFile, HashSet<string> addedAccountStatments) where TData : BankAccountStatementData
    {
        try
        {
            if (!addedAccountStatments.Add(statementFile.FileData.StatementNumber))
            {
                return false;
            }
        }
        catch (InvalidAccountStatementException)
        {
            return false;
        }
        return true;
    }

    private static PdfDocument GetPdfDocument(Stream fileStream)
    {
        MemoryStream ms = new();
        fileStream.CopyTo(ms);
        ms.Position = 0;
        return PdfDocument.Open(ms);
    }
}
