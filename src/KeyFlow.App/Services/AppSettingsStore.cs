using System.Text.Json;
using KeyFlow.App.Models;

namespace KeyFlow.App.Services;

public sealed class AppSettingsStore
{
    private readonly string _path = AppDataPaths.SettingsPath;

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_path)) return new AppSettings();
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path)) ?? new AppSettings();
        }
        catch { return new AppSettings(); }
    }

    public void Save(AppSettings value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temp, _path, overwrite: true);
    }
}
