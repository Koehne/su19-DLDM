using System;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace Galaga_Exercise_1 {
    public class PlayerShot:Entity {
        private Game game;

        public PlayerShot(Game game, DynamicShape shape, IBaseImage image)
            : base(shape, image)
        {
            this.game = game;
        }

        public DynamicShape Dynamcshape { get; set; }

        public void Direction(Vec2F vec2F)
        {
            Shape.AsDynamicShape().Direction = vec2F;
        }

        public void Move()
        {
            var newPosition = Shape.AsDynamicShape().Direction;
            if (newPosition.X + Shape.Position.X > 1.0)
            {
                Shape.Position.X=0.80f;
                Console.WriteLine("hej1");
            }
            else if (newPosition.X + Shape.Position.X < 0.000001)
            {
                Shape.Position.X = 0.1f;
                Console.WriteLine("hej2");

            }
            {
                Shape.Move();
            }
        }
    }
}