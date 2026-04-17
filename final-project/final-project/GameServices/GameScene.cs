using final_project.GameObjects;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using System.Numerics;
using Windows.Foundation;

namespace final_project.GameServices
{
    public class GameScene : Scene
    {
        public GameScene() : base()
        {
            Manager.Events.OnRun += RotatePlayers;
        }

        private void RotatePlayers()
        {
            var leftPlayer = GetPlayer(true);
            var rightPlayer = GetPlayer(false);

            if (leftPlayer == null || rightPlayer == null)
                return;

            Point center1 = new Point(
                rightPlayer.Bounds().Left + rightPlayer.Bounds().Width / 2,
                rightPlayer.Bounds().Top + rightPlayer.Bounds().Height / 2);

            Point center2 = new Point(
                leftPlayer.Bounds().Left + leftPlayer.Bounds().Width / 2,
                leftPlayer.Bounds().Top + leftPlayer.Bounds().Height / 2);

            double dx = center1.X - center2.X;
            double dy = center1.Y - center2.Y;
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

            double distance = Math.Sqrt(dx * dx + dy * dy);
            double offsetAngle = Math.Atan2(45.9, distance) * 180 / Math.PI;

            leftPlayer.Image.Rotation = (float)(angle - offsetAngle);
            rightPlayer.Image.Rotation = (float)(angle + 180 - offsetAngle);
        }

        public Player GetPlayer(bool isLeft)
        {
            foreach (GameObject obj in GameObjectsSnapshot)
            {
                if (obj is Player player && player.IsLeft == isLeft)
                    return player;
            }
            return null;
        }

        private Vector2 GetBulletStartPosition(Player player, double angle)
        {
            var rect = player.Bounds();
            float centerX = (float)(rect.Left + rect.Width / 2f);
            float centerY = (float)(rect.Top + rect.Height / 2f);

            float localOffsetX = 40f;
            float localOffsetY = 0f;

            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            float rotatedOffsetX = localOffsetX * cosA - localOffsetY * sinA;
            float rotatedOffsetY = localOffsetX * sinA + localOffsetY * cosA;

            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;

            return new Vector2(muzzleX, muzzleY);
        }
    }
}