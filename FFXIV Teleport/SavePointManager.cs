﻿using System.Text.Json;
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
