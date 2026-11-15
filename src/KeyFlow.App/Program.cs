using KeyFlow.App.Services;

namespace KeyFlow.App;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("--worker", StringComparison.OrdinalIgnoreCase))
            return WorkerProgram.Run(args);

        AppDataPaths.EnsureMigrated();
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        return 0;
    }
}
