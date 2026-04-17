using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ScmodCli.Commands;

public static class ProjectCreator
{
    public static async Task CreateAsync(string projectName)
    {
        if (!ValidateProjectName(projectName))
        {
            Console.Error.WriteLine($"Error: Invalid project name \"{projectName}\".");
            Environment.Exit(1);
            return;
        }

        var projectPath = Path.Combine(Directory.GetCurrentDirectory(), projectName);
        var absoluteProjectPath = Path.GetFullPath(projectPath);
        var absoluteCwd = Path.GetFullPath(Directory.GetCurrentDirectory());

        var normalizedCwd = absoluteCwd.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (!absoluteProjectPath.StartsWith(normalizedCwd + Path.DirectorySeparatorChar) &&
            !absoluteProjectPath.Equals(normalizedCwd, StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("Error: Project path escapes current directory.");
            Environment.Exit(1);
            return;
        }

        if (Directory.Exists(projectPath))
        {
            Console.Error.WriteLine($"Error: Directory \"{projectName}\" already exists.");
            Environment.Exit(1);
            return;
        }

        Console.WriteLine("Extracting template...");
        var templatePath = TemplateExtractor.GetTemplatePath();

        try
        {
            Console.WriteLine($"Copying template from {templatePath}...");
            CopyDirectory(templatePath, projectPath);
            Console.WriteLine($"✓ Copied template to: {projectName}");

            var replacements = new[] { ("SurvivalcraftMod", projectName) };

            ReplaceInFile(Path.Combine(projectPath, "SurvivalcraftMod.sln"), replacements);
            RenameFile(projectPath, "SurvivalcraftMod.sln", $"{projectName}.sln");
            Console.WriteLine($"✓ Renamed: SurvivalcraftMod.sln → {projectName}.sln");

            var csprojPath = Path.Combine(projectPath, "src", "SurvivalcraftMod.csproj");
            ReplaceInFile(csprojPath, new[]
            {
                ("SurvivalcraftMod", projectName),
                ("<AssemblyName>SurvivalcraftMod</AssemblyName>", $"<AssemblyName>{projectName}</AssemblyName>")
            });
            RenameFile(Path.Combine(projectPath, "src"), "SurvivalcraftMod.csproj", $"{projectName}.csproj");
            Console.WriteLine($"✓ Renamed: src/SurvivalcraftMod.csproj → src/{projectName}.csproj");

            var modinfoPath = Path.Combine(projectPath, "src", "modinfo.json");
            try
            {
                var modinfoJson = await File.ReadAllTextAsync(modinfoPath);
                var node = JsonNode.Parse(modinfoJson) ?? throw new InvalidOperationException("modinfo.json is empty.");

                if (node is JsonObject obj)
                    obj["Name"] = projectName;

                var updatedJson = node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(modinfoPath, updatedJson);
                Console.WriteLine($"✓ Updated: modinfo.json (Name: {projectName})");
            }
            catch
            {
                Console.Error.WriteLine("Error: Failed to parse modinfo.json.");
                Environment.Exit(1);
                return;
            }

            var class1Path = Path.Combine(projectPath, "src", "Class1.cs");
            if (File.Exists(class1Path))
            {
                ReplaceInFile(class1Path, new[] { ("namespace SurvivalcraftMod", $"namespace {projectName}") });
                Console.WriteLine("✓ Updated: Class1.cs namespace");
            }

            RenameFile(projectPath, "SurvivalcraftMod.DotSettings", $"{projectName}.DotSettings");
            Console.WriteLine($"✓ Renamed: SurvivalcraftMod.DotSettings → {projectName}.DotSettings");

            var gitInit = true;

            if (gitInit)
            {
                var gitignoreSrc = Path.Combine(templatePath, ".gitignore");
                var gitignoreDest = Path.Combine(projectPath, ".gitignore");

                if (File.Exists(gitignoreSrc) && !File.Exists(gitignoreDest))
                {
                    File.Copy(gitignoreSrc, gitignoreDest);
                    Console.WriteLine("✓ Copied: .gitignore");
                }

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = "init",
                        WorkingDirectory = projectPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var process = Process.Start(psi);
                    await process!.WaitForExitAsync();
                    Console.WriteLine("✓ Initialized git repository");
                }
                catch
                {
                    Console.WriteLine("⚠ git init failed (git may not be installed)");
                }
            }

            Console.WriteLine($"\n✓ Project \"{projectName}\" created successfully!");
        }
        finally
        {
            if (Directory.Exists(templatePath))
                Directory.Delete(templatePath, true);
        }
    }

    private static bool ValidateProjectName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (name.IndexOfAny(new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' }) >= 0)
            return false;

        if (name.StartsWith('.') || name.EndsWith('.'))
            return false;

        return true;
    }

    private static void CopyDirectory(string source, string destination)
    {
        if (!Directory.Exists(source))
            return;

        Directory.CreateDirectory(destination);

        foreach (var entry in new DirectoryInfo(source).EnumerateFileSystemInfos())
        {
            var srcPath = entry.FullName;
            var destPath = Path.Combine(destination, entry.Name);

            if (entry is DirectoryInfo)
                CopyDirectory(srcPath, destPath);
            else
                File.Copy(srcPath, destPath);
        }
    }

    private static void ReplaceInFile(string filePath, (string oldStr, string newStr)[] replacements)
    {
        var content = File.ReadAllText(filePath);
        foreach (var (oldStr, newStr) in replacements)
            content = content.Replace(oldStr, newStr);
        File.WriteAllText(filePath, content);
    }

    private static void RenameFile(string directory, string oldName, string newName)
    {
        var oldPath = Path.Combine(directory, oldName);
        var newPath = Path.Combine(directory, newName);
        if (File.Exists(oldPath))
            File.Move(oldPath, newPath);
    }
}
