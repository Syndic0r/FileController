using UglyToad.PdfPig.Content;

namespace FileController.Models;
public class Line : SortedList<double, Word>
{
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
        List<string> words = [];
        foreach (Word word in Values) 
        {
            words.Add(word.Text);
        }
        return string.Join(" ", words);
    }
}
