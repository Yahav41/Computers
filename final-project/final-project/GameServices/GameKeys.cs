using System;
using Windows.System;
using Windows.Storage;

namespace finalproject.GameServices
{
    public static class GameKeys
    {
        public static VirtualKey LeftPlayerLeft { get; set; } = VirtualKey.A;
        public static VirtualKey LeftPlayerRight { get; set; } = VirtualKey.D;
        public static VirtualKey LeftPlayerDown { get; set; } = VirtualKey.S;
        public static VirtualKey LeftPlayerUp { get; set; } = VirtualKey.W;
        public static VirtualKey LeftPlayerShoot { get; set; } = VirtualKey.F;
        public static VirtualKey LeftPlayerReload { get; set; } = VirtualKey.R;

        public static VirtualKey RightPlayerLeft { get; set; } = VirtualKey.Left;
        public static VirtualKey RightPlayerRight { get; set; } = VirtualKey.Right;
        public static VirtualKey RightPlayerDown { get; set; } = VirtualKey.Down;
        public static VirtualKey RightPlayerUp { get; set; } = VirtualKey.Up;
        public static VirtualKey RightPlayerShoot { get; set; } = VirtualKey.Shift;
        public static VirtualKey RightPlayerReload { get; set; } = VirtualKey.L;

        private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        public static void LoadFromSettings()
        {
            LeftPlayerLeft = ReadKey("LeftPlayerLeft", LeftPlayerLeft);
            LeftPlayerRight = ReadKey("LeftPlayerRight", LeftPlayerRight);
            LeftPlayerDown = ReadKey("LeftPlayerDown", LeftPlayerDown);
            LeftPlayerUp = ReadKey("LeftPlayerUp", LeftPlayerUp);
            LeftPlayerShoot = ReadKey("LeftPlayerShoot", LeftPlayerShoot);
            LeftPlayerReload = ReadKey("LeftPlayerReload", LeftPlayerReload);

            RightPlayerLeft = ReadKey("RightPlayerLeft", RightPlayerLeft);
            RightPlayerRight = ReadKey("RightPlayerRight", RightPlayerRight);
            RightPlayerDown = ReadKey("RightPlayerDown", RightPlayerDown);
            RightPlayerUp = ReadKey("RightPlayerUp", RightPlayerUp);
            RightPlayerShoot = ReadKey("RightPlayerShoot", RightPlayerShoot);
            RightPlayerReload = ReadKey("RightPlayerReload", RightPlayerReload);
        }

        public static void SaveToSettings()
        {
            WriteKey("LeftPlayerLeft", LeftPlayerLeft);
            WriteKey("LeftPlayerRight", LeftPlayerRight);
            WriteKey("LeftPlayerDown", LeftPlayerDown);
            WriteKey("LeftPlayerUp", LeftPlayerUp);
            WriteKey("LeftPlayerShoot", LeftPlayerShoot);
            WriteKey("LeftPlayerReload", LeftPlayerReload);

            WriteKey("RightPlayerLeft", RightPlayerLeft);
            WriteKey("RightPlayerRight", RightPlayerRight);
            WriteKey("RightPlayerDown", RightPlayerDown);
            WriteKey("RightPlayerUp", RightPlayerUp);
            WriteKey("RightPlayerShoot", RightPlayerShoot);
            WriteKey("RightPlayerReload", RightPlayerReload);
        }

        private static VirtualKey ReadKey(string name, VirtualKey defaultValue)
        {
            object value = Settings.Values[name];
            if (value is int intVal)
                return (VirtualKey)intVal;
            return defaultValue;
        }

        private static void WriteKey(string name, VirtualKey key)
        {
            Settings.Values[name] = (int)key;
        }
    }
}