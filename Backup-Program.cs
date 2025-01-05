Program.cs

using ClickableTransparentOverlay;
using ImGuiNET;

namespace FFXIV_Teleport
{
    public class Program : Overlay
    {
        private readonly SavePointManager savePointManager = new();
        private string inputX = "0.00", inputY = "0.00", inputZ = "0.00";
        private string savePointName = string.Empty;

        public static void Main()
        {
            var program = new Program();
            program.savePointManager.LoadSavePoints();
            program.Start();
        }

        protected override void Render()
        {
            ImGui.Begin("Teleport Menu");

            TeleportFunctions.RenderCurrentCoordinates();
            RenderSavePointInput();
            RenderButton("Save Current Position", () => savePointManager.SaveCurrentPosition(savePointName));
            savePointManager.RenderSavePointDropdown();
            RenderButton("Clear Saved Points", savePointManager.ClearSavedPoints);
            RenderCoordinatesInput();

            ImGui.End();
        }

        private void RenderSavePointInput()
        {
            ImGui.InputText("Save Point Name", ref savePointName, 100);
        }

        private static void RenderButton(string label, Action onClick)
        {
            if (ImGui.Button(label))
            {
                onClick.Invoke();
            }
            ImGui.Separator();
        }

        private void RenderCoordinatesInput()
        {
            ImGui.PushItemWidth(50.0f);
            ImGui.InputText("X", ref inputX, 100);
            ImGui.InputText("Y", ref inputY, 100);
            ImGui.InputText("Z", ref inputZ, 100);
            ImGui.PopItemWidth();

            if (ImGui.Button("Teleport to Coordinates"))
            {
                TeleportFunctions.TeleportToInputPosition(inputX, inputY, inputZ);
            }
        }
    }
}

SavePoint.cs

using System.Numerics;

namespace FFXIV_Teleport
{
    public class SavePoint
    {
        public string? Title { get; set; }
        public Vector3 Position { get; set; }
    }
}

SavePointManager.cs

using System.Text.Json;
using ImGuiNET;

namespace FFXIV_Teleport
{
    public class SavePointManager
    {
        private static readonly string SaveFilePath = "savePoints.json";
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            Converters = { new Vector3JsonConverter() },
            WriteIndented = true
        };

        private List<SavePoint> savePoints = new();
        
        public void LoadSavePoints()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    savePoints = string.IsNullOrWhiteSpace(json)
                        ? new List<SavePoint>()
                        : JsonSerializer.Deserialize<List<SavePoint>>(json, jsonOptions) ?? new List<SavePoint>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading save points: {ex.Message}");
            }
        }

        public void SaveSavePoints()
        {
            try
            {
                string json = JsonSerializer.Serialize(savePoints, jsonOptions);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving save points: {ex.Message}");
            }
        }

        // Modify this method to accept a save point name
        public void SaveCurrentPosition(string savePointName)
        {
            if (string.IsNullOrWhiteSpace(savePointName))
            {
                savePointName = $"Save Point {savePoints.Count + 1}";
            }

            var currentPosition = GameMemoryConfig.SwedInstance.ReadVec(GameMemoryConfig.PositionAddress);
            savePoints.Add(new SavePoint { Title = savePointName, Position = currentPosition });
            SaveSavePoints();
        }

        public void RenderSavePointDropdown()
        {
            RenderSavePointDropdown(savePoints);
        }

        public static void RenderSavePointDropdown(List<SavePoint> savePoints)
        {
            if (savePoints.Count > 0)
            {
                int selectedIndex = -1;
                string[] titles = new string[savePoints.Count];

                for (int i = 0; i < savePoints.Count; i++)
                {
                    titles[i] = savePoints[i].Title;
                }

                if (ImGui.Combo("Select Save Point", ref selectedIndex, titles, titles.Length) && selectedIndex != -1)
                {
                    GameMemoryConfig.SwedInstance.WriteVec(GameMemoryConfig.PositionAddress, savePoints[selectedIndex].Position);
                }
            }
            else
            {
                ImGui.Text("No save points available.");
            }
            ImGui.Separator();
        }

        public void ClearSavedPoints()
        {
            savePoints.Clear();
            SaveSavePoints();
        }
    }
}

Vector3JsonConverter.cs

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;

namespace FFXIV_Teleport
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token.");

            reader.Read();
            var x = reader.GetSingle();
            reader.Read();
            var y = reader.GetSingle();
            reader.Read();
            var z = reader.GetSingle();
            reader.Read(); // EndArray token

            return new Vector3(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteEndArray();
        }
    }
}

TeleportFunctions.cs

using System.Numerics;
using ImGuiNET;

namespace FFXIV_Teleport
{
    public class TeleportFunctions
    {
        public static void RenderCurrentCoordinates()
        {
            var currentPosition = GameMemoryConfig.SwedInstance.ReadVec(GameMemoryConfig.PositionAddress);
            string positionText = $"(X: {currentPosition.X:F2}, Y: {currentPosition.Y:F2}, Z: {currentPosition.Z:F2})";
            ImGui.Text("Current Position");
            ImGui.Text(positionText);
            ImGui.Separator();
        }

        public static void TeleportToInputPosition(string inputX, string inputY, string inputZ)
        {
            if (TryParseCoordinates(inputX, inputY, inputZ, out var targetPosition))
            {
                TeleportToPosition(targetPosition);
            }
            else
            {
                Console.WriteLine("Invalid coordinates input.");
            }
        }

        public static bool TryParseCoordinates(string inputX, string inputY, string inputZ, out Vector3 position)
        {
            float y = 0f, z = 0f;

            bool isValid = float.TryParse(inputX, out float x) &&
                           float.TryParse(inputY, out y) &&
                           float.TryParse(inputZ, out z);

            position = isValid ? new Vector3(x, y, z) : Vector3.Zero;
            return isValid;
        }

        public static void TeleportToPosition(Vector3 position)
        {            
            GameMemoryConfig.SwedInstance.WriteVec(GameMemoryConfig.PositionAddress, position);
        }
    }
}

GameMemoryConfig.cs

using Swed64;

namespace FFXIV_Teleport
{
    public static class GameMemoryConfig
    {
        private static readonly Swed swed = new("ffxiv_dx11");

        // Lazy initialization of the module base address
        private static IntPtr moduleBase = IntPtr.Zero;

        // Define offsets as constants
        public const int BaseOffset = 0x025E0980;
        public const int PositionOffset = 0xB0;

        public static IntPtr ModuleBase
        {
            get
            {
                if (moduleBase == IntPtr.Zero)
                {
                    moduleBase = swed.GetModuleBase("ffxiv_dx11.exe");
                }
                return moduleBase;
            }
        }

        // Public property to access the Swed instance
        public static Swed SwedInstance => swed;

        // Helper to calculate address using the base and offsets
        public static IntPtr GetPointerOffset(params int[] offsets)
        {
            IntPtr address = ModuleBase;
            foreach (var offset in offsets)
            {
                address = swed.ReadPointer(address, offset);
            }
            return address;
        }

        // Get the position address directly
        public static IntPtr PositionAddress => GetPointerOffset(BaseOffset) + PositionOffset;
    }
}
