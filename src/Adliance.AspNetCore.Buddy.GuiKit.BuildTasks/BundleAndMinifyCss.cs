using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

[assembly: InternalsVisibleTo("Adliance.AspNetCore.Buddy.GuiKit.BuildTasks.Test")]

namespace Adliance.AspNetCore.Buddy.GuiKit.BuildTasks;

public class BundleAndMinifyCss : Microsoft.Build.Utilities.Task
{
    [Required] public ITaskItem[] Files { get; set; } = [];

    /// Base directory used to resolve relative paths in OutputFile metadata.
    private string BaseDirectory { get; set; } = "";

    public override bool Execute()
    {
        var success = true;

        // Group by OutputFile, preserving declaration order within each bundle.
        var bundles = Files.GroupBy(f => Resolve(f.GetMetadata("OutputFile")));
        foreach (var bundle in bundles) success &= ProcessBundle(bundle.Key, [.. bundle]);

        return success;
    }

    private bool ProcessBundle(string outputFile, ITaskItem[] inputFiles)
    {
        if (string.IsNullOrWhiteSpace(outputFile))
        {
            Log.LogError("A CssBundle item is missing required OutputFile metadata.");
            return false;
        }

        var sb = new StringBuilder();
        foreach (var file in inputFiles)
        {
            var content = File.ReadAllText(file.GetMetadata("FullPath"));
            sb.Append(MinifyCss(content));
        }

        var dir = Path.GetDirectoryName(outputFile);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(outputFile, sb.ToString());

        Log.LogMessage(MessageImportance.Normal, $"CSS bundle written to \"{outputFile}\".");
        return true;
    }

    private string Resolve(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "";
        if (Path.IsPathRooted(path) || string.IsNullOrEmpty(BaseDirectory)) return path;
        return Path.GetFullPath(path, BaseDirectory);
    }

    // Alternation is ordered: comments and quoted strings are consumed before whitespace,
    // so their content is never touched by the \s+ branch.
    private static readonly Regex MinifyPattern = new(
        @"/\*.*?\*/|([""'])(?:\\.|(?!\1).)*\1|\s+",
        RegexOptions.Compiled | RegexOptions.Singleline);

    internal static string MinifyCss(string css) => MinifyPattern.Replace(css, m => m.Value[0] is '"' or '\'' ? m.Value : "");
}
