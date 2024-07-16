using System.ComponentModel.DataAnnotations;

namespace FileController.Data;
public record AccountStatement
{
    [Key]
    public int Id { get; set; }
    public required string FileName { get; init; }
    public required string Number { get; set; }
    public required decimal ValueStart { get; set; }
    public required decimal ValueEnd { get; set; }
    public required DateOnly CreationDate { get; set; }
    public required TimeOnly CreationTime { get; set; }
    public required string PagesText { get; set; }
    public BankAccount Account { get; set; }
    public ICollection<StatementTransaction> Transactions { get; set; }
    public ICollection<StatementPage> Pages { get; set; }
}
