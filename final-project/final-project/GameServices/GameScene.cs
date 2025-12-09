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
            if(_rightPlayer == _leftPlayer) 
            {
                _leftPlayer = (Players)_gameObjects.FirstOrDefault(p => p is Players);
                _rightPlayer = (Players)_gameObjects.FirstOrDefault(p => p is Players && p != _leftPlayer);
            }
            Point center1 = new Point(_rightPlayer.Rect().Left + _rightPlayer.Rect().Width / 2, _rightPlayer.Rect().Y + _rightPlayer.Rect().Height / 2);
            Point center2 = new Point(_leftPlayer.Rect().Left + _leftPlayer.Rect().Width / 2, _leftPlayer.Rect().Y + _leftPlayer.Rect().Height / 2);
            double dx = center1.X - center2.X;
            double dy = center1.Y - center2.Y;
            double angle = Math.Atan2(dy, dx) * (180 / Math.PI);
            _leftPlayer.Image.Rotation = (float)(angle - Math.Atan2(45.9, Math.Sqrt((dx * dx) + (dy * dy))));
            _rightPlayer.Image.Rotation = (float)(angle + 180 - Math.Atan2(45.9, Math.Sqrt((dx * dx) + (dy * dy))));
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
