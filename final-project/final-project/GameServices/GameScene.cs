using final_project.GameObjects;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            double dx = _rightPlayer.Rect().X - _leftPlayer.Rect().X;
            double dy = _rightPlayer.Rect().Y - _leftPlayer.Rect().Y;
            double angle = Math.Atan2(dy, dx) * (180 / Math.PI);
            _leftPlayer.Image.Rotation = (float)angle;

            _rightPlayer.Image.Rotation = (float)(angle + 180);
        }
    }
}
