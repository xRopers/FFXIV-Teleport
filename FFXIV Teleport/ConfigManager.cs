using System;
using System.IO;
using System.Text.Json;

namespace FFXIV_Teleport
{
    public class ConfigManager
    {
        private const string ConfigFilePath = "config.json";
        public static string GameExecutable { get; private set; } = "ffxiv_dx11";
        public static string GameId { get; private set; } = "ffxiv_dx11.exe";
        public static IntPtr ModuleBaseOffset { get; private set; } = IntPtr.Zero;

        static ConfigManager()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<Config>(json);
                    if (config != null)
                    {
                        GameExecutable = config.GameExecutable;
                        GameId = config.GameId;
                        ModuleBaseOffset = IntPtr.Parse(config.ModuleBaseOffset, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading config: {ex.Message}");
                }
            }
        }
    }

    public class Config
    {
        public string GameExecutable { get; set; } = "ffxiv_dx11";
        public string GameId { get; set; } = "ffxiv_dx11.exe";
        public string ModuleBaseOffset { get; set; } = "0x025E0980";
    }
}
