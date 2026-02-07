using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class Keybind
    {
        private List<int> MouseButtons = [];
        private List<int> KeyboardButtons = [];
        private int ButtonCount => MouseButtons.Count + KeyboardButtons.Count;


        public static Keybind LEFTCLICK = new()
        {
            MouseButtons = [(int)MouseButton.Left],
            KeyboardButtons = [],
        };

        public static Keybind RIGHTCLICK = new()
        {
            MouseButtons = [(int)MouseButton.Right],
            KeyboardButtons = [],
        };

        public static Keybind UP = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.W],
        };

        public static Keybind DOWN = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.S],
        };

        public static Keybind LEFT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.A],
        };

        public static Keybind RIGHT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D],
        };

        public static Keybind ONE = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D1],
        };

        public static Keybind TWO = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D2],
        };

        public static Keybind THREE = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D3],
        };

        public static Keybind FOUR = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D4],
        };

        public static Keybind FIVE = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D5],
        };

        public static Keybind SIX = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D6],
        };

        public static Keybind SEVEN = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D7],
        };

        public static Keybind EIGHT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D8],
        };

        public static Keybind NINE = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D9],
        };

        public static Keybind ZERO = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.D0],
        };

        public static Keybind INTERACT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.E],
        };

        public static Keybind CONTROL = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.LeftControl],
        };

        public static Keybind SHIFT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.LeftShift],
        };

        public static Keybind ALT = new()
        {
            MouseButtons = [],
            KeyboardButtons = [(int)Keys.LeftAlt],
        };

        public static Keybind SCROLLDOWN = new()
        {
            MouseButtons = []
        };

        public bool IsPressed()
        {
            MouseState mouse = Input.mouse;
            KeyboardState keyboard = Input.keyboard;

            List<int> pressed = [.. MouseButtons.Where(b => mouse.IsButtonPressed((MouseButton)b)), .. KeyboardButtons.Where(b => keyboard.IsKeyPressed((Keys)b))];
            List<int> down = [.. MouseButtons.Where(b => mouse.IsButtonDown((MouseButton)b)), .. KeyboardButtons.Where(b => keyboard.IsKeyDown((Keys)b))];
            return (down.Count == ButtonCount) && pressed.Count > 0;
        }

        public bool IsDown()
        {
            MouseState mouse = Input.mouse;
            KeyboardState keyboard = Input.keyboard;

            List<int> down = [.. MouseButtons.Where(b => mouse.IsButtonDown((MouseButton)b)), .. KeyboardButtons.Where(b => keyboard.IsKeyDown((Keys)b))];
            return down.Count == ButtonCount;
        }

        public bool IsReleased()
        {
            MouseState mouse = Input.mouse;
            KeyboardState keyboard = Input.keyboard;

            List<int> down = [.. MouseButtons.Where(b => mouse.IsButtonDown((MouseButton)b)), .. KeyboardButtons.Where(b => keyboard.IsKeyDown((Keys)b))];
            List<int> released = [.. MouseButtons.Where(b => mouse.IsButtonReleased((MouseButton)b)), .. KeyboardButtons.Where(b => keyboard.IsKeyReleased((Keys)b))];
            return (down.Count + released.Count == ButtonCount) && released.Count > 0;
        }
    }
}