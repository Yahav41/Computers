using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Objects
{
    public class Covers : GameObject
    {
        public enum CoverType { boxes, barrier, canister, foundation }
        public CoverType _coverType { get; private set; }
        private Random _random = new Random();
        public Covers(Canvas scene, CoverType type, double x, double y, double size) : base(scene, String.Empty, x, y, size)
        {
            _coverType = type;
            if (type == CoverType.boxes)
            {
                _image.Height = _image.Width * 0.6678;
                SetName("Covers/gameBoxes.png");
            }
            else if (type == CoverType.barrier)
            {
                _image.Height = _image.Width * 0.66616;
                SetName("Models/Covers/gameBarrier.png");
            }
            else if (type == CoverType.canister)
            {
                _image.Height = _image.Width * 1.678 / 2.5;
                SetName("Models/Covers/gameCanister.png");
            }
            else
            {
                _image.Height = _image.Width * 0.6675;
                SetName("Models/Covers/gameFoundation.png");
            }
            //RectangleHelper.DrawRectangle(_scene, _x, _y, _image.Width, _image.Height, Color.FromArgb(255, 0, 0, 255));
        }
        public override void Collide(GameObject gameObject)
        {
            if (gameObject is Covers cover)
            {
                _x = _x + _random.Next(-250, 250);
                _y = _y + _random.Next(-250, 250);
                Render();
            }
        }
    }
}
