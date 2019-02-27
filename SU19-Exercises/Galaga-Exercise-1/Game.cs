using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Timers;

namespace Galaga_Exercise_1 {
    public class Game : IGameEventProcessor<object> {
        private Window win;
        private DIKUArcade.Timers.GameTimer gameTimer;

        public Game() {
            win = new Window( "Galaga", 500, 500);
            gameTimer = new GameTimer(50, 50);
        }

        public void GameLoop() {
            while(win.IsRunning()) {
                gameTimer.MeasureTime();
                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();
// Update game logic here
                }
                if (gameTimer.ShouldRender()) {
                    win.Clear();
// Render gameplay entities here
                    win.SwapBuffers();
                }
                if (gameTimer.ShouldReset()) {
// 1 second has passed - display last captured ups and fps
                    win.Title = "Galaga | UPS: " + gameTimer.CapturedUpdates +
                                ", FPS: " + gameTimer.CapturedFrames;
                }
            }
        }
        }
        
        
        public void KeyPress(string key){
        throw new NotImplementedException();
        }
        
        public void KeyRelease(string key){
            throw new NotImplementedException();
        }

        public void ProcessEvent(GameEventType eventType,
        GameEvent<object> gameEvent) {
            throw new NotImplementedException();
        }
   