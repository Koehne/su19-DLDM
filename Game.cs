using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.EventBus;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using DIKUArcade.Timers;
using Galaga_Exercise_1;
public class Game : IGameEventProcessor<object> {
    private readonly GameEventBus<object> eventBus;
    private readonly GameTimer gameTimer; //3.1
    private readonly Player player;
    private readonly Window win; //3.1
    private List<Image> enemyStrides;
    private List<Enemy> enemies;
    public List<PlayerShot> playerShots;
    private List<Image> explosionStrides;
    private AnimationContainer explosions;
    private int explosionLength = 500;
    private Score _score;

    public Game() //3.1
    {
        win = new Window("Galaga", 500, 500); //3.1
        gameTimer = new GameTimer(60, 60); //3.1
      
        explosionStrides = ImageStride.CreateStrides(8,
            Path.Combine("Assets", "Images", "Explosion.png"));
        explosions = new AnimationContainer(8);
        
        enemyStrides = ImageStride.CreateStrides(4,
            Path.Combine("Assets", "Images", "BlueMonster.png"));
        enemies = new List<Enemy>();
        
        player = new Player(this,
            new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
            new Image(Path.Combine("Assets", "Images", "Player.png")));
        
        playerShots = new List<PlayerShot>();
        
        _score= new Score(new Vec2F(0.005f,-0.17f),new Vec2F(0.2f,0.2f) );
        
        eventBus = new GameEventBus<object>();
        eventBus.InitializeEventBus(new List<GameEventType> {
            GameEventType.InputEvent, // key press / key release
            GameEventType.WindowEvent // messages to the window
        });
        win.RegisterEventBus(eventBus);
        eventBus.Subscribe(GameEventType.InputEvent, this);
        eventBus.Subscribe(GameEventType.WindowEvent, this);
        
        // Look at the file and consider why we place the number '4' here.

    }

    public void AddExplosion(float posX, float posY,
        float extentX, float extentY) {
        explosions.AddAnimation(
            new StationaryShape(posX, posY, extentX, extentY), explosionLength,
            new ImageStride(80/*Milliseconds virker ikke*/, explosionStrides));
        }

    public void ProcessEvent(GameEventType eventType,
        GameEvent<object> gameEvent) {
        if (eventType == GameEventType.WindowEvent) {
            switch (gameEvent.Message) {
            case "CLOSE_WINDOW":
                win.CloseWindow();
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

    public void AddEnemies() {
        List<float> em = new List<float>(){0.1f,0.2f,0.3f,0.4f,0.5f,0.6f,0.7f,0.8f};
        foreach (var obj in em) {
            var n1 = new Enemy(this,
                new DynamicShape(new Vec2F(obj, 0.9f), new Vec2F(0.1f, 0.1f)),
                new ImageStride(80, enemyStrides));
            enemies.Add(n1);
        }
    }
   
    
    public void IterateShots() {
        foreach (var shot in playerShots) {
           
            foreach (var enemy in enemies) {
                if (CollisionDetection.Aabb(shot.Shape.AsDynamicShape(),enemy.Shape).Collision) {
                    shot.DeleteEntity();
                    enemy.DeleteEntity();
                    AddExplosion(enemy.Shape.Position.X,enemy.Shape.Position.Y,0.1f,0.1f);
                    _score.AddPoint();
                    }
                
            }
            shot.Shape.Move();
            if (shot.Shape.Position.Y > 1.0f) {
                shot.DeleteEntity();
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
        foreach (PlayerShot playerShot in playerShots) {
            if (!playerShot.IsDeleted()) {
                newPlayerShots.Add(playerShot);
            }
        }
        playerShots = newPlayerShots;
    }

   public void GameLoop() //3.1 
    {;
        AddEnemies();
       
        while (win.IsRunning()) {
            gameTimer.MeasureTime();
            while (gameTimer.ShouldUpdate()) {
                player.Move();
                win.PollEvents();
            }

            if (gameTimer.ShouldRender()) {
                win.Clear();
                explosions.RenderAnimations();

                foreach (var enemy in enemies) {
                    enemy.RenderEntity();
                }
                IterateShots();
                _score.RenderScore(); 

                foreach (PlayerShot obj in playerShots) {
                    obj.RenderEntity();                    
                }
                
                player.RenderEntity(); //3.1
                win.SwapBuffers();
                eventBus.ProcessEvents(); //3.1
                
            }

            if (gameTimer.ShouldReset()) {
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
            player.Playershot();
            Console.WriteLine(playerShots.Count);
            Console.WriteLine(playerShots[0].Shape.Position);
            break;
        }
    }

    public void KeyRelease(string key) {
        switch (key) {
        case "KEY_LEFT":
            player.Direction(new Vec2F(0.0f,
                0.0f)); //hvis vi skal lave events når man releaser en key, så kan man bare ændre på den her parameter
            //Console.WriteLine(player.Shape.Position);
            //  Console.WriteLine("test KeyRelease KEY_LEFT");
            //   player.Move();

            break;
        case "KEY_RIGHT":
            player.Direction(new Vec2F(0.0f,
                0.0f)); //hvis vi skal lave events når man releaser en key, så kan man bare ændre på den her parameter
            //Console.WriteLine(player.Shape.Position);
            //  Console.WriteLine("test KeyRelease KEY_RIGHT");
            //  player.Move();

            break;
        }
    }
}