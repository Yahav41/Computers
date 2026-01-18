using final_project.GameObjects;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace final_project.GameServices
{
    public class GameScene : Scene
    {
        private Players _leftPlayer;
        private Players _rightPlayer;
        public GameScene() : base()
        {
            Manager.Events.OnRun += RotatePlayers;
        }

        private void RotatePlayers()
        {
            // Always get fresh references - don't cache
            Players leftPlayer = getPlayer(true);
            Players rightPlayer = getPlayer(false);

            if (leftPlayer != null && rightPlayer != null)
            {
                Point center1 = new Point(rightPlayer.Rect().Left + rightPlayer.Rect().Width / 2, rightPlayer.Rect().Y + rightPlayer.Rect().Height / 2);
                Point center2 = new Point(leftPlayer.Rect().Left + leftPlayer.Rect().Width / 2, leftPlayer.Rect().Y + leftPlayer.Rect().Height / 2);

                double dx = center1.X - center2.X;
                double dy = center1.Y - center2.Y;
                double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

                leftPlayer.Image.Rotation = (float)angle - (float)Math.Atan2(45.9, Math.Sqrt(dx * dx + dy * dy)) * 180 / (float)Math.PI;
                rightPlayer.Image.Rotation = (float)angle + 180 - (float)Math.Atan2(45.9, Math.Sqrt(dx * dx + dy * dy)) * 180 / (float)Math.PI;
            }
        }


        public Players getPlayer(bool isLeft)
        {
            foreach (var obj in _gameObjects)
            {
                if (obj is Players player && player._isLeft == isLeft)
                    return player;
            }
            return null;
        }

        private Vector2 GetBulletStartPosition(Players player, double angle)
        {
            // Center of the player sprite
            var rect = player.Rect();
            float centerX = (float)(rect.Left + rect.Width / 2f);
            float centerY = (float)(rect.Top + rect.Height / 2f);

            // Local offset from center to pistol muzzle when unrotated.
            // Sprite is 80x40, center is (0,0), muzzle at right middle edge -> (40, 0)
            float localOffsetX = 40f;
            float localOffsetY = 0f;

            // Rotate the offset by 'angle'
            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            float rotatedOffsetX = localOffsetX * cosA - localOffsetY * sinA;
            float rotatedOffsetY = localOffsetX * sinA + localOffsetY * cosA;

            // World position of muzzle
            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;

            return new Vector2(muzzleX, muzzleY);
        }
    }
}
