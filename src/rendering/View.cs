using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public abstract class View
    {

        public static GameView GameView = new GameView();
        public static MainMenuView MainMenuView = new MainMenuView();
        public static LobbyView Lobby = new LobbyView();

        public static View CurrentView = MainMenuView;

        public float Zoom = 1/4.0f;
        
        public virtual void OnRenderFrame(FrameEventArgs args)
        {
            
        }

        public virtual void OnLoad()
        {

        }

        public virtual void HandleInputs(FrameEventArgs args)
        {
            
        }
    }

}