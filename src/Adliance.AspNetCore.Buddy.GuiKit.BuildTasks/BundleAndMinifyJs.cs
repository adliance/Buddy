using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using NUglify;
using NUglify.JavaScript;

namespace Adliance.AspNetCore.Buddy.GuiKit.BuildTasks;

public class BundleAndMinifyJs : Microsoft.Build.Utilities.Task
{
    [Required] public ITaskItem[] Files { get; set; } = [];

    private string BaseDirectory { get; set; } = "";

    public override bool Execute()
    {
        var success = true;

        var bundles = Files.GroupBy(f => Resolve(f.GetMetadata("OutputFile")));
        foreach (var bundle in bundles) success &= ProcessBundle(bundle.Key, [.. bundle]);

        return success;
    }

    private bool ProcessBundle(string outputFile, ITaskItem[] inputFiles)
    {
        if (string.IsNullOrWhiteSpace(outputFile))
        {
            Log.LogError("A JsBundle item is missing required OutputFile metadata.");
            return false;
        }

        var sb = new StringBuilder();
        foreach (var file in inputFiles)
        {
            var content = File.ReadAllText(file.GetMetadata("FullPath"));
            var minified = Uglify.Js(content, new CodeSettings
            {
                TermSemicolons = true
            });
            if (minified.HasErrors)
            {
                foreach (var error in minified.Errors)
                    Log.LogWarning($"JS error in {Path.GetFileName(file.GetMetadata("FullPath"))} ({error.StartLine}:{error.StartColumn}) - using unminified content: {error.Message}");
                sb.Append(content);
            }
            else
            {
                sb.Append(minified.Code);
            }
        }

        var dir = Path.GetDirectoryName(outputFile);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(outputFile, sb.ToString());

        Log.LogMessage(MessageImportance.Normal, $"JS bundle written to \"{outputFile}\".");
        return true;
    }

    private string Resolve(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "";
        if (Path.IsPathRooted(path) || string.IsNullOrEmpty(BaseDirectory)) return path;
        return Path.GetFullPath(path, BaseDirectory);
    }
}
