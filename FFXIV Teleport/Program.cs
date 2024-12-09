using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FFXIV_Teleport
{
    public class Program : Overlay
    {
        private List<SavePoint> savePoints = new List<SavePoint>();
        private static Swed swed = new Swed("ffxiv_dx11");
        private static IntPtr moduleBase;
        private static IntPtr posAddress;

        // Coordinates input variables
        private string inputX = "0.0", inputY = "0.0", inputZ = "0.0";

        public static void Main(string[] args)
        {
            InitializeModule();
            var program = new Program();
            program.Start(); // Start rendering the overlay
        }

        private static void InitializeModule()
        {
            moduleBase = swed.GetModuleBase("ffxiv_dx11.exe");
            posAddress = swed.ReadPointer(moduleBase, 0x025E3980) + 0xB0; // Read position address
        }

        protected override void Render()
        {
            ImGui.Begin("Teleport Menu");

            RenderCurrentCoordinates();
            RenderButton("Save Current Position", SaveCurrentPosition);
            RenderSavePointDropdown();
            RenderButton("Clear Saved Points", ClearSavedPoints);
            RenderCoordinatesInput();

            ImGui.End();
        }

        private void RenderCurrentCoordinates()
        {
            Vector3 currentPlayerPosition = GetCurrentPlayerPosition();
            string positionText = $"(X: {currentPlayerPosition.X:F2}, Y: {currentPlayerPosition.Y:F2}, Z: {currentPlayerPosition.Z:F2})";

            ImGui.Text("Current Position:");
            ImGui.Text(positionText); // Display the current position
            ImGui.Separator();
        }

        private void RenderButton(string label, Action onClick)
        {
            if (ImGui.Button(label))
            {
                onClick.Invoke();
            }
            ImGui.Separator();
        }

        private void RenderSavePointDropdown()
        {
            if (savePoints.Count > 0)
            {
                int selectedSavePointIndex = -1;
                string[] savePointTitles = new string[savePoints.Count];

                for (int i = 0; i < savePoints.Count; i++)
                {
                    savePointTitles[i] = savePoints[i].title;
                }

                if (ImGui.Combo("Select Save Point", ref selectedSavePointIndex, savePointTitles, savePointTitles.Length) && selectedSavePointIndex != -1)
                {
                    TeleportToPosition(savePoints[selectedSavePointIndex].position);
                }
            }
            else
            {
                ImGui.Text("No save points available.");
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
                TeleportToInputPosition();
            }
        }

        private void SaveCurrentPosition()
        {
            Vector3 currentPlayerPosition = GetCurrentPlayerPosition();
            savePoints.Add(new SavePoint { title = $"Save Point {savePoints.Count + 1}", position = currentPlayerPosition });
        }

        private void TeleportToPosition(Vector3 position)
        {
            swed.WriteVec(posAddress, position);
        }

        private void TeleportToInputPosition()
        {
            if (TryParseCoordinates(out var targetPosition))
            {
                TeleportToPosition(targetPosition);
            }
            else
            {
                Console.WriteLine("Invalid input! Please enter valid numeric coordinates.");
            }
        }

        private bool TryParseCoordinates(out Vector3 position)
        {
            // Initialize variables with default values
            float x = 0f, y = 0f, z = 0f;

            // Try to parse the input values
            bool isValid = float.TryParse(inputX, out x) &&
                           float.TryParse(inputY, out y) &&
                           float.TryParse(inputZ, out z);

            // If parsing is successful, create a new Vector3, otherwise set position to default Vector3
            position = isValid ? new Vector3(x, y, z) : Vector3.Zero;
            return isValid;
        }

        private void ClearSavedPoints()
        {
            savePoints.Clear();
        }

        private Vector3 GetCurrentPlayerPosition()
        {
            return swed.ReadVec(posAddress);
        }
    }
}
