using FileController.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace FileController.ConsoleArguments.Settings;
public class AccountStatmentReadSetting : BankAccountStatmentSetting
{
    [CommandArgument(0, "<FilesPath>")]
    [Description("Folder to account statement files.")]
    public string FilesFolderPath { get; init; }

    public override ValidationResult Validate()
    {
        if (!Directory.Exists(FilesFolderPath))
        {
            return ValidationResult.Error($"{FilesFolderPath} is not a valid diretory!");
        }

        return ValidationResult.Success();
    }
}
