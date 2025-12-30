using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class ButtonElement : WithBaseElement
    {
        public int BorderSize = 4;
        public int BorderTexture = 666;
        public int HoverTexture = 666;
        public int ClickTexture = 666;

        public bool RenderBaseElement = false;
        public bool RenderBorder = true;
        public bool RenderHeader = true;
        public bool RenderInnerText = true;

        public string Text = "";
        public float TextSize = 1f;
        public Action SubmitAction = () => {};

        public float HeaderFactor = 1 / 0.7f;
        public string HeaderText = "";

        private float ClickTime = 0f;

        public void Render(float dt)
        {
            ClickTime -= dt;

            if (RenderBorder)
            {
                BorderElement.renderBorder(BaseElement, BorderSize, BorderTexture);
            }

            if (ClickTime > 0)
            {
                BorderElement.renderBorder(BaseElement, BorderSize, ClickTexture);
            }
            else if (Hovered(out _))
            {
                BorderElement.renderBorder(BaseElement, BorderSize, HoverTexture);
            }

            if (RenderHeader)
            {
                // Header
                Text header = new()
                {
                    text = HeaderText,
                    alignX = TextAlignX.CENTER,
                    alignY = TextAlignY.BOTTOM,
                    x = BaseElement.Position.X + BaseElement.Size.X / 2,
                    y = BaseElement.Position.Y - 10,
                    scale = TextSize * HeaderFactor,
                    color = (1, 1, 1),
                };
                TextHandler.renderer.elements.Add(header);
            }

            if (RenderBaseElement)
            {
                HudRenderer.renderer.elements.Add(new()
                {
                    Position = BaseElement.Position + (BorderSize, BorderSize),
                    Size = BaseElement.Size - (BorderSize * 2, BorderSize * 2),
                    TextureId = BaseElement.TextureId,
                    TexCoords = BaseElement.TexCoords,
                    Rotation = BaseElement.Rotation,
                });
            }

            if (RenderInnerText)
            {
                // Inner Text
                Text innerText = new()
                {
                    text = Text,
                    x = BaseElement.Position.X + BaseElement.Size.X / 2,
                    y = BaseElement.Position.Y + BaseElement.Size.Y / 2,
                    scale = TextSize,
                    color = (1, 1, 1),
                    alignX = TextAlignX.CENTER,
                    alignY = TextAlignY.CENTER
                };
                TextHandler.renderer.elements.Add(innerText);
            }
        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (Keybind.LEFTCLICK.IsPressed())
            {
                if (Hovered(out _))
                {
                    SubmitAction();
                    ClickTime = 0.3f;
                }
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }
    }
}