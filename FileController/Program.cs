// See https://aka.ms/new-console-template for more information
using FileController.ConsoleArguments.Commands;
using FileController.ConsoleArguments.Settings;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Text;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

CommandApp app = new();
app.Configure(config =>
{
    config.AddBranch<BankAccountStatmentSetting>("bank", ascfg =>
    {
        ascfg.AddBranch<BankAccountStatmentSetting>("vbrb", asrcfg =>
        {
            asrcfg.AddCommand<VbRbReadCommand>("read")
            .WithExample("bank", "vbrb", "read", @"C:\example\account\statement\directory");
        });
    });
});
app.Run(args);
