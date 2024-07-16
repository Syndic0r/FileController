using UglyToad.PdfPig.Content;

namespace FileController.Models;
public class Lines : SortedList<double, Line>
{
    public Lines() : base(Comparer<double>.Create((x, y) => y.CompareTo(x))) { }

    private string? _text;
    public string Text
    {
        get
        {
            _text ??= GetText();
            return _text;
        }
    }

    private string GetText()
    {
        List<string> lines = [];
        foreach(Line line in Values)
        {
            lines.Add(line.Text);
        }
        return string.Join("\n", lines);
    }
}
