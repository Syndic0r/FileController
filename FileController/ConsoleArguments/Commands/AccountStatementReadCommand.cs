using FileController.ConsoleArguments.Settings;
using FileController.Extensions;
using FileController.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace FileController.ConsoleArguments.Commands;
public abstract class AccountStatementReadCommand<TBankAccountStatementData> : Command<AccountStatmentReadSetting> where TBankAccountStatementData : BankAccountStatementData
{
    public override ValidationResult Validate(CommandContext context, AccountStatmentReadSetting settings)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(settings.FilesFolderPath);

        if (!files.Any())
        {
            return ValidationResult.Error($"No files found in provided directory {settings.FilesFolderPath}");
        }
        return ValidationResult.Success();
    }

    public override int Execute(CommandContext context, AccountStatmentReadSetting settings)
    {
        AnsiConsole.Progress()
            .AutoClear(true)
            .AutoRefresh(true)
            .RefreshRate(TimeSpan.FromMilliseconds(25))
            .Columns(
            [
                new PercentageColumn(),
                new SpinnerColumn(),
                new TaskDescriptionColumn()
            ])
            .UseRenderHook(RenderHook)
            .Start(ctx =>
            {
                ProgressTask? progressTask = null;
                return new FileHandler().CreateAccountStatements<TBankAccountStatementData>(settings.FilesFolderPath, prgrs =>
                {
                    progressTask ??= ctx.AddTask(prgrs.Message);
                    progressTask.Description = prgrs.Message;
                    progressTask.Value = prgrs.Progress;
                });
            });

        return 0;
    }

    private static IRenderable RenderHook(IRenderable renderable, IReadOnlyList<ProgressTask> progressTasks)
    {
        const string ESC = "\u001b";
        string escapeSequence;
        if (progressTasks.All(i => i.IsFinished))
        {
            escapeSequence = $"{ESC}]]9;4;0;100{ESC}\\";
        }
        else
        {
            var total = progressTasks.Sum(i => i.MaxValue);
            var done = progressTasks.Sum(i => i.Value);
            var percent = (int)(done / total * 100);
            escapeSequence = $"{ESC}]]9;4;1;{percent}{ESC}\\";
        }

        return new Rows(renderable, new ControlCode(escapeSequence));
    }
}
