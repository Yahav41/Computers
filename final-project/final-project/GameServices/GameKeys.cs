using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace final_project.GameServices
{
    public static class ControlKeys
    {
        public static VirtualKey LeftPlayerLeft
        { get; set; } = VirtualKey.A;
        public static VirtualKey LeftPlayerRight
        { get; set; } = VirtualKey.D;
        public static VirtualKey LeftPlayerDown
        { get; set; } = VirtualKey.S;
        public static VirtualKey LeftPlayerUp
        { get; set; } = VirtualKey.W;
        public static VirtualKey LeftPlayerShoot
        { get; set; } = VirtualKey.F;
        public static VirtualKey LeftPlayerReload
        { get; set; } = VirtualKey.R;


        public static VirtualKey RightPlayerLeft
        { get; set; } = VirtualKey.Left;
        public static VirtualKey RightPlayerRight
        { get; set; } = VirtualKey.Right;
        public static VirtualKey RightPlayerDown
        { get; set; } = VirtualKey.Down;
        public static VirtualKey RightPlayerUp
        { get; set; } = VirtualKey.Up;
        public static VirtualKey RightPlayerShoot
        { get; set; } = VirtualKey.Shift;
        public static VirtualKey RightPlayerReload
        { get; set; } = VirtualKey.L;
    }
}
