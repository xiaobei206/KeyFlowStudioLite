using System.IO;

namespace KeyFlow.App.Services;

/// <summary>
/// Owns the current per-user data location and performs a non-destructive
/// import from the previous internal product directory.
/// </summary>
internal static class AppDataPaths
{
    private static readonly string LocalAppData =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    // Kept only as a migration source so existing 1.0.0 data is not stranded.
    private static readonly string LegacyRootDirectory =
        Path.Combine(LocalAppData, "Anjian2026");

    public static string RootDirectory => Path.Combine(LocalAppData, "KeyFlowStudioLite");
    public static string ScriptsDirectory => Path.Combine(RootDirectory, "Scripts");
    public static string SettingsPath => Path.Combine(RootDirectory, "settings.json");

    public static void EnsureMigrated()
    {
        Directory.CreateDirectory(RootDirectory);
        if (!Directory.Exists(LegacyRootDirectory))
            return;

        // Never overwrite data already created by the new version, or delete
        // the source directory: both choices keep rollback non-destructive.
        CopyMissingFiles(LegacyRootDirectory, RootDirectory);
    }

    private static void CopyMissingFiles(string sourceRoot, string destinationRoot)
    {
        foreach (var sourceFile in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceRoot, sourceFile);
            var destinationFile = Path.Combine(destinationRoot, relativePath);
            if (File.Exists(destinationFile))
                continue;

            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(sourceFile, destinationFile, overwrite: false);
        }
    }
}
