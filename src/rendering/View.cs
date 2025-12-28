using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public abstract class View
    {

        public static View GameView = new GameView();


        public static View CurrentView = GameView;

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