using ClickableTransparentOverlay;
using ImGuiNET;
using Swed64;
using System.Numerics;



namespace FFXIV_Teleport
{
    public class Program : Overlay
    {
        List<SavePoint> savePoints = new List<SavePoint>(); // store our save points
        static Swed swed = new Swed("ffxiv_dx11"); // handle memory for process
        static IntPtr moduleBase; // hold module base
        static IntPtr posAddress; // hold address of position

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
            ImGui.Begin("Telport Menu");

            if (ImGui.Button("Save Current Position"))
            {
                SaveCurrentPosition();
            }

            ImGui.Separator();

            // Display existing save points and allow telport

            foreach(var savePoint in savePoints)
            {
                if (ImGui.Button($"Teleport to {savePoint.title}"))
                {
                    TeleportToPosition(savePoint.position);
                }
            }

            ImGui.End();

        }

        void SaveCurrentPosition()
        {
            // get current pos
            Vector3 currentPlayerPosition = GetCurrentPlayerPositon();
            // save point info
            string savePointTitle = "Save Point " + (savePoints.Count + 1);
            // create new savepoint and store it inout list
            SavePoint savePoint = new SavePoint { title = savePointTitle,position = currentPlayerPosition };
            savePoints.Add(savePoint);
        }

        void TeleportToPosition(Vector3 position)
        {
            swed.WriteVec(posAddress, position); // write new pos
        }

        Vector3 GetCurrentPlayerPositon()
        {
            return swed.ReadVec(posAddress); // read our address and return as vec
        }
    }
}