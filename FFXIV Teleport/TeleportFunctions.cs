using System.Numerics;
using ImGuiNET;

namespace FFXIV_Teleport
{
    public class TeleportFunctions
    {
        public void RenderCurrentCoordinates()
        {
            var currentPosition = GameMemoryConfig.SwedInstance.ReadVec(GameMemoryConfig.PositionAddress);
            string positionText = $"(X: {currentPosition.X:F2}, Y: {currentPosition.Y:F2}, Z: {currentPosition.Z:F2})";
            ImGui.Text("Current Position");
            ImGui.Text(positionText);
            ImGui.Separator();
        }

        public void TeleportToInputPosition(string inputX, string inputY, string inputZ)
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

        public bool TryParseCoordinates(string inputX, string inputY, string inputZ, out Vector3 position)
        {
            float x = 0f, y = 0f, z = 0f;

            bool isValid = float.TryParse(inputX, out x) &&
                           float.TryParse(inputY, out y) &&
                           float.TryParse(inputZ, out z);

            position = isValid ? new Vector3(x, y, z) : Vector3.Zero;
            return isValid;
        }

        public void TeleportToPosition(Vector3 position)
        {            
            GameMemoryConfig.SwedInstance.WriteVec(GameMemoryConfig.PositionAddress, position);
        }
    }
}
