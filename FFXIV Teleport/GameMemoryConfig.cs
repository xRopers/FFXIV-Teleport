using Swed64;
using System;

namespace FFXIV_Teleport
{
    public static class GameMemoryConfig
    {
     //   private static Swed swed = new Swed("ffxiv_dx11");
        private static readonly Swed swed = new Swed("ffxiv_dx11");

        // Lazy initialization of the module base address
        private static IntPtr moduleBase = IntPtr.Zero;

        // Define offsets as constants
        public const int BaseOffset = 0x025E3980;
        public const int PositionOffset = 0xB0;

        // Public property to access the Swed instance
        public static Swed SwedInstance => swed;

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
