using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace Galaga_Exercise_1 {
    public class PlayerShot : Entity {
        private Game game;
        
        public PlayerShot (Game game, DynamicShape shape, IBaseImage image) 
            : base(shape, image) {
            this.game = game;
            Shape.AsDynamicShape().Direction.Y = 0.01f;
        }
    }
}