using System.ComponentModel.DataAnnotations;

namespace FileController.Data;
public record StatementPage
{
    [Key]
    public int Id { get; set; }
    public required int Number { get; set; }
    public required string Text { get; set; }
    public AccountStatement AccountStatement { get; set; }
}
