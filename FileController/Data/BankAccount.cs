using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileController.Data;
public record BankAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string IBAN { get; set; }
    public required string Bankname { get; set; }
    public required int BLZ { get; set; }
    public required int Number { get; set; }
    public required string BIC { get; set; }
    public ICollection<AccountStatement> AccountStatements { get; set; }
}
