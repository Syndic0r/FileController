using System.ComponentModel.DataAnnotations;

namespace FileController.Data;
public record StatementTransaction
{
    [Key]
    public int Id { get; set; }
    public required int SortOrder { get; set; }
    public required string Type { get; set; }
    public required decimal Value { get; set; }
    public required string Details { get; set; }
    public required DateOnly Date { get; set; }
    public AccountStatement AccountStatement { get; set; }
}
