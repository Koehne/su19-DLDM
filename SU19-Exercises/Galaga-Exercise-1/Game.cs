using System.Collections.Generic;
using System.IO;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;

namespace Galaga_Exercise_1 {
    public class Game : IGameEventProcessor<object> {
        private Window win;
        private DIKUArcade.Timers.GameTimer gameTimer;

        private Player player;

        private GameEventBus<object> eventBus;

        private List<Enemy> enemies;
        private List<Image> enemyStrides; // TODO: change image to Image

        public List<PlayerShot> PlayerShots { get; private set; }
        public Image PlayerShotImage { get; }

        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private int explosionLength = 500;

        private Score score;

        public Game() {
            win = new Window("Galaga", 500, AspectRatio.R1X1);
            gameTimer = new GameTimer(60, 60);

            player = new Player(this,
                new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "Player.png")));

            eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType>() {
                GameEventType.InputEvent, // key press / key release
                GameEventType.WindowEvent, // messages to the window
            });
            win.RegisterEventBus(eventBus);
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);


            enemyStrides = ImageStride.CreateStrides(4,
                Path.Combine("Assets", "Images", "BlueMonster.png"));
            enemies = new List<Enemy>();

            PlayerShots = new List<PlayerShot>();
            PlayerShotImage = new Image(
                Path.Combine("Assets", "Images", "BulletRed2.png"));

            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(10);

            score = new Score(new Vec2F(0.1f, 0.8f), new Vec2F(0.2f, 0.2f));
        }

        private void AddEnemies() {
            for (int i = 0; i < 10; i++) {
                enemies.Add(new Enemy(this,
                    new DynamicShape(new Vec2F(0.1f * i, 0.8f), new Vec2F(0.1f, 0.1f)),
                    new ImageStride(80, enemyStrides)));
            }
        }

        private void IterateShots() {
            foreach (PlayerShot shot in PlayerShots) {
                shot.Shape.Move();
                if (shot.Shape.Position.Y > 1.0f) {
                    shot.DeleteEntity();
                } else {
                    foreach (Enemy enemy in enemies) {
                        CollisionData cd = CollisionDetection.Aabb(
                            (DynamicShape) shot.Shape, enemy.Shape);
                        if (cd.Collision) {
                            AddExplosion(enemy.Shape.Position.X, enemy.Shape.Position.Y,
                                enemy.Shape.Extent.X, enemy.Shape.Extent.Y);
                            enemy.DeleteEntity();
                            shot.DeleteEntity();
                            score.AddPoint(100);
                        }
                    }
                }
            }

            List<Enemy> newEnemies = new List<Enemy>();
            foreach (Enemy enemy in enemies) {
                if (!enemy.IsDeleted()) {
                    newEnemies.Add(enemy);
                }
            }

            enemies = newEnemies;

            List<PlayerShot> newPlayerShots = new List<PlayerShot>();
            foreach (PlayerShot shot in PlayerShots) {
                if (!shot.IsDeleted()) {
                    newPlayerShots.Add(shot);
                }
            }

            PlayerShots = newPlayerShots;
        }

        public void AddExplosion(float posX, float posY,
            float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                new ImageStride(explosionLength / 8, explosionStrides));
        }

        public void GameLoop() {
            AddEnemies();

            while (win.IsRunning()) {
                gameTimer.MeasureTime();
                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();
                    eventBus.ProcessEvents();

                    player.Move();

                    foreach (Enemy enemy in enemies) {
                        enemy.Shape.Move();
                    }

                    IterateShots();
                }

                if (gameTimer.ShouldRender()) {
                    win.Clear();

                    player.RenderEntity();

                    foreach (Enemy enemy in enemies) {
                        enemy.RenderEntity();
                    }

                    foreach (PlayerShot shot in PlayerShots) {
                        shot.RenderEntity();
                    }

                    explosions.RenderAnimations();

                    score.RenderScore();

                    win.SwapBuffers();
                }

                if (gameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps
                    win.Title = "Galaga | UPS: " + gameTimer.CapturedUpdates +
                                ", FPS: " + gameTimer.CapturedFrames;
                }
            }
        }


        private void KeyPress(string key) {
            switch (key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
            case "KEY_LEFT":
                player.Direction(new Vec2F(-0.01f, 0.0f));
                break;
            case "KEY_RIGHT":
                player.Direction(new Vec2F(0.01f, 0.0f));
                break;
            case "KEY_SPACE":
                player.Shoot();
                break;
            }
        }

        private void KeyRelease(string key) {
            switch (key) {
            case "KEY_LEFT":
                player.Direction(new Vec2F(0.0f, 0.0f));
                break;
            case "KEY_RIGHT":
                player.Direction(new Vec2F(0.0f, 0.0f));
                break;
            }
        }

        public void ProcessEvent(GameEventType eventType,
            GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                default:
                    break;
                }
            } else if (eventType == GameEventType.InputEvent) {
                switch (gameEvent.Parameter1) {
                case "KEY_PRESS":
                    KeyPress(gameEvent.Message);
                    break;
                case "KEY_RELEASE":
                    KeyRelease(gameEvent.Message);
                    break;
                }
            }
        }
    }
}