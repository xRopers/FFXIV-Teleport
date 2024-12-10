using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace FFXIV_Teleport
{
    public class Program : Overlay
    {
        private SavePointManager savePointManager = new SavePointManager();
        private TeleportFunctions teleportFunctions = new TeleportFunctions();
        private string inputX = "0.0", inputY = "0.0", inputZ = "0.0";
        private string savePointName = string.Empty;

        public static void Main(string[] args)
        {
            var program = new Program();
            program.savePointManager.LoadSavePoints();
            program.Start();
        }

        protected override void Render()
        {
            ImGui.Begin("Teleport Menu");

            teleportFunctions.RenderCurrentCoordinates();
            RenderSavePointInput();
            RenderButton("Save Current Position", () => savePointManager.SaveCurrentPosition(savePointName)); // Pass the savePointName here
            savePointManager.RenderSavePointDropdown();
            RenderButton("Clear Saved Points", savePointManager.ClearSavedPoints);
            RenderCoordinatesInput();

            ImGui.End();
        }

        private void RenderSavePointInput()
        {
            ImGui.InputText("Save Point Name", ref savePointName, 100);
        }

        private void RenderButton(string label, Action onClick)
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
                teleportFunctions.TeleportToInputPosition(inputX, inputY, inputZ);
            }
        }
    }
}
