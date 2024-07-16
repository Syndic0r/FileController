using UglyToad.PdfPig;

namespace FileController.Models;
public record BankAccountStatementFile<TData> where TData : BankAccountStatementData 
{
    public BankAccountStatementFile(string originatedFile, string pdfFileName, PdfDocument pdfDocument)
    {
        OriginatedFile = originatedFile;
        OriginatedFileExtension = Path.GetExtension(originatedFile);
        PdfFileName = pdfFileName;
        FileData = (TData)Activator.CreateInstance(typeof(TData), pdfDocument)!;
    }

    public string OriginatedFile { get; init; }
    public string OriginatedFileExtension { get; init; }
    public string PdfFileName { get; init; }
    public TData FileData { get; init; }
}
