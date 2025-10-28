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
        private Random _random = new Random();
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
        }
    }
}
