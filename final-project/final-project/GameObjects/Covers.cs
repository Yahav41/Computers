using final_project.GameServices;
using GameEngine.Objects;
using System;

namespace final_project.Objects
{
    public class Covers : GameObject
    {
        public enum CoverType { boxes, barrier, canister, foundation }

        public CoverType CoverKind { get; private set; }

        private readonly Random _random = new Random();
        private readonly GameScene _scene;

        public Covers(CoverType type, double x, double y, double size, GameScene scene)
            : base(string.Empty, x, y, size)
        {
            CoverKind = type;

            switch (type)
            {
                case CoverType.boxes:
                    Image.Height = Image.Width * 0.6678;
                    SetSprite("Models/Covers/gameBoxes.png");
                    break;
                case CoverType.barrier:
                    Image.Height = Image.Width * 0.66616;
                    SetSprite("Models/Covers/gameBarrier.png");
                    break;
                case CoverType.canister:
                    Image.Height = Image.Width * 1.678 / 2.5;
                    SetSprite("Models/Covers/gameCanister.png");
                    break;
                case CoverType.foundation:
                default:
                    Image.Height = Image.Width * 0.6675;
                    SetSprite("Models/Covers/gameFoundation.png");
                    break;
            }

            _scene = scene;
        }

        public override void OnCollide(GameObject gameObject)
        {
            if (gameObject is Covers)
            {
                X = X + _random.Next(-250, 250);
                Y = Y + _random.Next(-250, 250);
                Render();
            }
        }

        public override void Render()
        {
            if (X < 0 || X > 1050 || Y < 0 || Y > 400)
            {
                _scene.RemoveObject(this);
            }

            base.Render();
        }
    }
}