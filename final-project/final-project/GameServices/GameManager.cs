using final_project.GameObjects;
using final_project.Objects;
using GameEngine;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace final_project.GameServices
{
    public class GameManager : Manager
    {
        private GameScene _scene;
        public GameManager(GameScene scene) : base(scene)
        {
            _scene = scene;
            CreateObjects();
        }

        private void CreateObjects()
        {
            for (int i = 1; i <= 15; i++)
            {
                _scene.AddObject(new Covers((Covers.CoverType)_random.Next(0, 4), _random.Next(100, 801), _random.Next(401), 100));
            }
            _scene.AddObject(new Players(Players.PlayerType.rifle, true, 50, 200, 80));
            _scene.AddObject(new Players(Players.PlayerType.pistol, false, 400, 400, 80));
        }
    }
}
