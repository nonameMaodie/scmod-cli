using System.CommandLine;
using ScmodCli.Commands;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var nameArg = new Argument<string>("name")
        {
            Description = "Name of the project"
        };

        var newCommand = new Command("new", "Create a new Survivalcraft mod project")
        {
            nameArg
        };
        newCommand.SetAction(async (parseResult, ct) =>
        {
            var name = parseResult.GetValue(nameArg) ?? string.Empty;
            await ProjectCreator.CreateAsync(name);
            return 0;
        });

        var rootCommand = new RootCommand("Survivalcraft mod scaffolding CLI tool")
        {
            newCommand
        };

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}
