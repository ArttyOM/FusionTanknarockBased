using Fusion;
using UnityEngine;

namespace FusionExamples.Tanknarok
{
    /// <summary>
    /// Our custom definition of an INetworkStruct. Keep in mind that
    /// * bool does not work (C# does not define a consistent size on different platforms)
    /// * Must be a top-level struct (cannot be a nested class)
    /// * Stick to primitive types and structs
    /// * Size is not an issue since only modified data is serialized, but things that change often should be compact (e.g. button states)
    /// </summary>
    public struct NetworkInputData : INetworkInput
    {
        public const byte BUTTON_FIRE_PRIMARY = 1 << 0;
        public const byte BUTTON_FIRE_SECONDARY = 1 << 1;
        public const byte READY = 1 << 6;

        public byte Buttons;

        public sbyte aimDirectionX;
        public sbyte aimDirectionY;

        public sbyte moveDirectionX;
        public sbyte moveDirectionY;

        public bool IsUp(uint button)
        {
            return IsDown(button) == false;
        }

        public bool IsDown(uint button)
        {
            return (Buttons & button) == button;
        }
    }
}