using final_project.GameObjects;
using final_project.Objects;
using GameEngine;
using GameEngine.Objects;
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
        public GameScene _scene;
        public GameManager(GameScene scene, bool createCovers) : base(scene)
        {
            _scene = scene;
            CreateObjects(createCovers);
        }

        private void CreateObjects(bool createCovers)
        {
            if (createCovers)
            {
                CreateCovers();
            }
            for (int i = 1; i <= 15; i++)
            {
                _scene.AddObject(new Covers((Covers.CoverType)_random.Next(0, 4), _random.Next(100, 801), _random.Next(401), 100,_scene));
            }
            switch (GameConstants.leftPlayer)
            {
                case 0:
                    _scene.AddObject(new PistolPlayer(100, 200, _scene, true));
                    break;
                case 1:
                    _scene.AddObject(new RiflePlayer(100, 200, _scene, true));
                    break;
                case 2:
                    _scene.AddObject(new ShotgunPlayer(100, 200, _scene, true));
                    break;
                default:
                    throw new Exception("Invalid left player selection");
            }
            switch (GameConstants.rightPlayer)
            {
                case 0:
                    _scene.AddObject(new PistolPlayer(800, 200, _scene, false));
                    break;
                case 1:
                    _scene.AddObject(new RiflePlayer(800, 200, _scene, false));
                    break;
                case 2:
                    _scene.AddObject(new ShotgunPlayer(800, 200, _scene, false));
                    break;
                default:
                    throw new Exception("Invalid left player selection");
            }
        }

        private void CreateCovers()
        {
            List<Covers> covers = new List<Covers>();
            for (int i = 1; i <= 15; i++)
            {
                covers.Add(new Covers((Covers.CoverType)_random.Next(0, 4), _random.Next(100, 801), _random.Next(401), 100, _scene));
            }
            _scene.AddCovers(covers);

        }

        public int getBullets(bool isLeft)
        {
            return _scene.getPlayer(isLeft)._bullets;
        }
    }
}
