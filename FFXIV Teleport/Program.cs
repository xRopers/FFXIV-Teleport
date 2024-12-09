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
        List<SavePoint> savePoints = new List<SavePoint>(); // store our save points
        static Swed swed = new Swed("ffxiv_dx11"); // handle memory for process
        static IntPtr moduleBase; // hold module base
        static IntPtr posAddress; // hold address of position

        // Variables for new teleport coordinates input
        string inputX = "0.0";
        string inputY = "0.0";
        string inputZ = "0.0";

        public static void Main(string[] args)
        {
            moduleBase = swed.GetModuleBase("ffxiv_dx11.exe"); // we had the main module here, (exe)
            posAddress = swed.ReadPointer(moduleBase, 0x025E3980) + 0xB0; //"ffxiv_dx11.exe"+025E3980 + B0, old address
            Program program = new Program();
            program.Start(); // run the render method
        }

        protected override void Render()
        {
            // create window stuff
            ImGui.Begin("Teleport Menu");

            if (ImGui.Button("Save Current Position"))
            {
                SaveCurrentPosition();
            }

            ImGui.Separator();

            // Display existing save points and allow teleport
            foreach (var savePoint in savePoints)
            {
                if (ImGui.Button($"Teleport to {savePoint.title}"))
                {
                    TeleportToPosition(savePoint.position);
                }
            }

            ImGui.Separator();

            // Textbox for X, Y, Z coordinates input
            ImGui.InputText("X Coordinate", ref inputX, 100);
            ImGui.InputText("Y Coordinate", ref inputY, 100);
            ImGui.InputText("Z Coordinate", ref inputZ, 100);

            // Button to teleport to the new position entered in the input fields
            if (ImGui.Button("Teleport to Coordinates"))
            {
                TeleportToInputPosition();
            }

            ImGui.End();
        }

        void SaveCurrentPosition()
        {
            // get current pos
            Vector3 currentPlayerPosition = GetCurrentPlayerPositon();
            // save point info
            string savePointTitle = "Save Point " + (savePoints.Count + 1);
            // create new savepoint and store it in the list
            SavePoint savePoint = new SavePoint { title = savePointTitle, position = currentPlayerPosition };
            savePoints.Add(savePoint);
        }

        void TeleportToPosition(Vector3 position)
        {
            swed.WriteVec(posAddress, position); // write new pos
        }

        void TeleportToInputPosition()
        {
            // Try to parse the X, Y, Z input coordinates
            if (float.TryParse(inputX, out float x) &&
                float.TryParse(inputY, out float y) &&
                float.TryParse(inputZ, out float z))
            {
                // If parsing is successful, create a new vector and teleport to it
                Vector3 targetPosition = new Vector3(x, y, z);
                TeleportToPosition(targetPosition);
            }
            else
            {
                Console.WriteLine("Invalid input! Please enter valid numeric coordinates.");
            }
        }

        Vector3 GetCurrentPlayerPositon()
        {
            return swed.ReadVec(posAddress); // read our address and return as vec
        }
    }
}
