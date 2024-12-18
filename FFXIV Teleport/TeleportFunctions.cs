﻿using System.Numerics;
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
