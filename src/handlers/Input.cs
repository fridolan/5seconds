using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public static class Input
    {
        public static bool dragging = false;
        public static bool dragFinished = false;
        public static bool currentlyTexting = false;

        public static KeyboardState keyboard;
        public static MouseState mouse;
        public static Vector2 mousePos;
        public static Vector2 wheelDelta;

        
    }
}