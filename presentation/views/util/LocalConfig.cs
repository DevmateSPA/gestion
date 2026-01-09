using System.IO;
using System.Text.Json;

public static class LocalConfig
{
    private static readonly string PathConfig =
        System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");

    public static string ObtenerImpresora()
    {
        if (!File.Exists(PathConfig))
            return "No seleccionada";

        var json = File.ReadAllText(PathConfig);
        var data = JsonSerializer.Deserialize<ConfigData>(json);

        return data?.ImpresoraSeleccionada ?? "No seleccionada";
    }

    public static void GuardarImpresora(string impresora)
    {
        var data = new ConfigData { ImpresoraSeleccionada = impresora };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(PathConfig, json);
    }

    private class ConfigData
    {
        public string ImpresoraSeleccionada { get; set; } = "No seleccionada";
    }
}
