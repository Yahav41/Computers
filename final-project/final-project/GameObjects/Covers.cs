using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GameEngine.Objects;

namespace final_project.Objects
{
    public class Covers : GameObject
    {
        public enum CoverType { boxes, barrier, canister, foundation }
        public CoverType _coverType { get; private set; }
        private Random _random = new Random();
        public Covers(CoverType type, double x, double y, double size) : base(String.Empty, x, y, size)
        {
            _coverType = type;
            if (type == CoverType.boxes)
            {
                Image.Height = Image.Width * 0.6678;
                SetName("Covers/gameBoxes.png");
            }
            else if (type == CoverType.barrier)
            {
                Image.Height = Image.Width * 0.66616;
                SetName("Models/Covers/gameBarrier.png");
            }
            else if (type == CoverType.canister)
            {
                Image.Height = Image.Width * 1.678 / 2.5;
                SetName("Models/Covers/gameCanister.png");
            }
            else
            {
                Image.Height = Image.Width * 0.6675;
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
        public override void Render()
        {
            if (_x < 0 || _x > 1050 || _y < 0 || _y > 400)
            {
                this.Collisional = false;
                SetName(" ");
            }
            base.Render();
        }
    }
}
