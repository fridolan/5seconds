using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class SliderElement : WithBaseElement
    {
        public HudElement SliderHead = new()
        {
            Position = (0, 0),
            Size = (Window.Width / 100, Window.Width / 100),
            TextureId = Textures.slider_head,
        };

        public Action<float> SubmitAction = (_) => { };
        private bool isPulled = false;
        public float Value = 0;
        public bool Horizontal = true;
        public float LineWidth = 5;

        public void Render()
        {
            HudElement line = new()
            {
                Position = Horizontal ? 
                (BaseElement.Position.X, BaseElement.Position.Y + BaseElement.Size.Y / 2 - LineWidth / 2)
                : (BaseElement.Position.X + BaseElement.Size.X / 2 - LineWidth / 2, BaseElement.Position.Y),
                Size = Horizontal ? (BaseElement.Size.X, LineWidth) : (LineWidth, BaseElement.Size.Y),
                TextureId = Textures.hud_color,
            };

            line.Render();
            Vector2 headMargin = (BaseElement.Size - SliderHead.Size) / 2;
            SliderHead.Position = Horizontal ?
                (BaseElement.Position.X + BaseElement.Size.X * Value - SliderHead.Size.X / 2, BaseElement.Position.Y + headMargin.Y) :
                (BaseElement.Position.X + headMargin.X, BaseElement.Position.Y + BaseElement.Size.Y * Value - SliderHead.Size.Y / 2);

            SliderHead.Render();
        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (Keybind.LEFTCLICK.IsPressed())
            {
                isPulled = Hovered(out _);
            }
            if (Keybind.LEFTCLICK.IsReleased())
            {
                isPulled = false;
            }

            if (isPulled)
            {
                Hovered(out Vector2 position);
                float rawValue = Horizontal ? position.X : position.Y;
                float newValue = Math.Clamp(rawValue, 0, 1);
                if (newValue != Value)
                {
                    Value = newValue;
                    SubmitAction(Value);
                }
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }
    }

}