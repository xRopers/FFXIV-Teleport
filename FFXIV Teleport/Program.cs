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
