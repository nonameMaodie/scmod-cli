using System.IO.Compression;

namespace ScmodCli;

public static class TemplateExtractor
{
    public static string GetTemplatePath()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "scmod-template-" + Guid.NewGuid().ToString("N"));
        ExtractTemplate(tempPath);
        return tempPath;
    }

    public static void ExtractTemplate(string destination)
    {
        var assembly = typeof(TemplateExtractor).Assembly;
        var resourceName = "ScmodCli.Template.Template.zip";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

        Directory.CreateDirectory(destination);
        ZipFile.ExtractToDirectory(stream, destination);
    }
}
