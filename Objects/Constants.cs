using System.Collections.Generic;

namespace Objects
{
    public static class Constants
    {
        public const string EVENT_SOURCE = "X32Net";

        public static class DeviceNames
        {
            public const string AKAI_APC20 = "Akai APC20";
            public const string AKAI_APC20_2 = "Akai APC20 (2)";
        }

        public static class ControlChanges
        {
            public const string CC14 = "Controller 14";
            public const string CC47 = "Controller 47";
            public const string MV = "MainVolume";
        }

        public static readonly Dictionary<string, int> NoteMappings = new Dictionary<string, int>
        {
            { "F4", 53 },
            { "F#4", 54 },
            { "G4", 55 },
            { "G#4", 56 },
            { "A4", 57 },
            { "A#4", 58 },
            { "B4", 59 },
            { "E4|1", 59 },
            { "E4|2", 87 },
            { "E4|3", 88 },
            { "E4|4", 89 },
            { "E4|5", 90 },
            { "E4|6", 91 },
            { "E4|7", 92 },
            { "E4|8", 93 }
            //{ "C4", 60 },
            //{ "C#4", 61 },
            //{ "D4", 62 },
            //{ "D#4", 63 },
            //{ "A#6", 82 },
            //{ "B6", 83 },
            //{ "C7", 84 },
            //{ "C#7", 85 },
            //{ "D7", 86 }
            // TODO: handle E4 buttons
            //{ "E4", 87 },
            /*{ "E4", 89 },{ "E4", 90 },{ "E4", 91 },{ "E4", 92 },{ "E4", 93 },{ "E4", 94 },*/
        };

        public static readonly Dictionary<int, ApcButton> ButtonIdMappings = new Dictionary<int, ApcButton>
        {
            #region Clip LaunchButtonIdMappings
            // First row
            { 1, new ApcButton { Channel = 1, Note = 53 } },
            { 2, new ApcButton { Channel = 2, Note = 53 } },
            { 3, new ApcButton { Channel = 3, Note = 53 } },
            { 4, new ApcButton { Channel = 4, Note = 53 } },
            { 5, new ApcButton { Channel = 5, Note = 53 } },
            { 6, new ApcButton { Channel = 6, Note = 53 } },
            { 7, new ApcButton { Channel = 7, Note = 53 } },
            { 8, new ApcButton { Channel = 8, Note = 53 } },
            // Second row
            { 9, new ApcButton { Channel = 1, Note = 54 } },
            { 10, new ApcButton { Channel = 2, Note = 54 } },
            { 11, new ApcButton { Channel = 3, Note = 54 } },
            { 12, new ApcButton { Channel = 4, Note = 54 } },
            { 13, new ApcButton { Channel = 5, Note = 54 } },
            { 14, new ApcButton { Channel = 6, Note = 54 } },
            { 15, new ApcButton { Channel = 7, Note = 54 } },
            { 16, new ApcButton { Channel = 8, Note = 54 } },
            // Third row
            { 17, new ApcButton { Channel = 1, Note = 55 } },
            { 18, new ApcButton { Channel = 2, Note = 55 } },
            { 19, new ApcButton { Channel = 3, Note = 55 } },
            { 20, new ApcButton { Channel = 4, Note = 55 } },
            { 21, new ApcButton { Channel = 5, Note = 55 } },
            { 22, new ApcButton { Channel = 6, Note = 55 } },
            { 23, new ApcButton { Channel = 7, Note = 55 } },
            { 24, new ApcButton { Channel = 8, Note = 55 } },
            // Fourth row
            { 25, new ApcButton { Channel = 1, Note = 56 } },
            { 26, new ApcButton { Channel = 2, Note = 56 } },
            { 27, new ApcButton { Channel = 3, Note = 56 } },
            { 28, new ApcButton { Channel = 4, Note = 56 } },
            { 29, new ApcButton { Channel = 5, Note = 56 } },
            { 30, new ApcButton { Channel = 6, Note = 56 } },
            { 31, new ApcButton { Channel = 7, Note = 56 } },
            { 32, new ApcButton { Channel = 8, Note = 56 } },
            // Fifth row
            { 33, new ApcButton { Channel = 1, Note = 57 } },
            { 34, new ApcButton { Channel = 2, Note = 57 } },
            { 35, new ApcButton { Channel = 3, Note = 57 } },
            { 36, new ApcButton { Channel = 4, Note = 57 } },
            { 37, new ApcButton { Channel = 5, Note = 57 } },
            { 38, new ApcButton { Channel = 6, Note = 57 } },
            { 39, new ApcButton { Channel = 7, Note = 57 } },
            { 40, new ApcButton { Channel = 8, Note = 57 } },
            #endregion

            #region Scene Launch
            // TODO
            #endregion

            #region Stop Clips
            // TODO
            #endregion

            #region Activator, Solo/Cue, Record Arm 
            { 41, new ApcButton { Channel = 1, Note = 48 } }, // green only
            { 42, new ApcButton { Channel = 2, Note = 48 } },
            { 43, new ApcButton { Channel = 3, Note = 48 } },
            { 44, new ApcButton { Channel = 4, Note = 48 } },
            { 45, new ApcButton { Channel = 5, Note = 48 } },
            { 46, new ApcButton { Channel = 6, Note = 48 } },
            { 47, new ApcButton { Channel = 7, Note = 48 } },
            { 48, new ApcButton { Channel = 8, Note = 48 } },
            { 49, new ApcButton { Channel = 1, Note = 49 } }, // blue only
            { 50, new ApcButton { Channel = 2, Note = 49 } },
            { 51, new ApcButton { Channel = 3, Note = 49 } },
            { 52, new ApcButton { Channel = 4, Note = 49 } },
            { 53, new ApcButton { Channel = 5, Note = 49 } },
            { 54, new ApcButton { Channel = 6, Note = 49 } },
            { 55, new ApcButton { Channel = 7, Note = 49 } },
            { 56, new ApcButton { Channel = 8, Note = 49 } },
            { 57, new ApcButton { Channel = 1, Note = 50 } }, // red only
            { 58, new ApcButton { Channel = 2, Note = 50 } },
            { 59, new ApcButton { Channel = 3, Note = 50 } },
            { 60, new ApcButton { Channel = 4, Note = 50 } },
            { 61, new ApcButton { Channel = 5, Note = 50 } },
            { 62, new ApcButton { Channel = 6, Note = 50 } },
            { 63, new ApcButton { Channel = 7, Note = 50 } },
            { 64, new ApcButton { Channel = 8, Note = 50 } },
            #endregion

            #region Virtual Mute Buttons
            { 200, new ApcButton { Channel = 1, Note = 50 } }, // red only (virtual)
            { 201, new ApcButton { Channel = 2, Note = 50 } },
            { 202, new ApcButton { Channel = 3, Note = 50 } },
            { 203, new ApcButton { Channel = 4, Note = 50 } },
            { 204, new ApcButton { Channel = 5, Note = 50 } },
            { 205, new ApcButton { Channel = 6, Note = 50 } },
            { 206, new ApcButton { Channel = 7, Note = 50 } },
            { 207, new ApcButton { Channel = 8, Note = 50 } },
            #endregion
        };

    }
}
