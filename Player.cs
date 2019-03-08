using System;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace Galaga_Exercise_1
{
    public class Player : Entity
    {
        private Game game;
        private Image billede;
        public Player(Game game, DynamicShape shape, IBaseImage image)
            : base(shape, image){
            this.game = game;
            
            billede = new Image(Path.Combine("Assets", "Images", "BulletRed2.png"));
        }

        public void Direction(Vec2F vec2F)
        {
            Shape.AsDynamicShape().Direction = vec2F;
        }

        public void Move()
        {
            var newPosition = Shape.AsDynamicShape().Direction;
            if (newPosition.X + Shape.Position.X >= 0.95) {
                Shape.Position.X = 0.85f;
                 Console.WriteLine("Can not move further left");
            } else if (newPosition.X + Shape.Position.X < 0.0) {
                Shape.Position.X = 0.05f;
                Console.WriteLine("Can not move further right");
            }
            Shape.Move();
        }

        public void Playershot() {
            PlayerShot playerShot = new PlayerShot(game, new DynamicShape(Shape.Position.X + 0.048f, Shape.Position.Y + 0.1f, 0.008f, 0.027f,
                0.0f, 0.01f) ,billede);
            game.playerShots.Add(playerShot);
        }
    }
}