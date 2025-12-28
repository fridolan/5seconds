using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class TextInputElement : WithBaseElement
    {
        public int BorderSize = 4;
        public int BorderTexture = Textures.hud_color;
        public bool RenderBorder = true;
        public string Text = "";
        public float TextSize = 1f;
        public bool InputActive = false;
        public Action<string> SubmitAction = (s) => { };
        public Action<string> OnChange = (s) => {};

        public float HeaderFactor = 1 / 0.7f;
        public string HeaderText = "";
        public bool RenderHeader = true;
        public MouseButton TriggerOn = MouseButton.Left;

        public void Render()
        {
            if (RenderBorder)
                BorderElement.renderBorder(BaseElement, BorderSize, BorderTexture);

            if (RenderHeader)
            {
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

            // Inner Text
            Text innerText = new()
            {
                text = Text,
                x = BaseElement.Position.X + BaseElement.Size.X / 2,
                y = BaseElement.Position.Y + BaseElement.Size.Y / 2,
                scale = 0.7f,
                color = (1, 1, 1),
                alignX = TextAlignX.CENTER,
                alignY = TextAlignY.CENTER
            };
            TextHandler.renderer.elements.Add(innerText);

            if (InputActive)
            {
                // Left
                HudRenderer.renderer.elements.Add(
                    new HudElement
                    {
                        Position = BaseElement.Position,
                        Size = (BorderSize, BaseElement.Size.Y),
                        TextureId = Textures.selection_color,

                    }
                );
                // Top
                HudRenderer.renderer.elements.Add(
                    new HudElement
                    {
                        Position = BaseElement.Position,
                        Size = (BaseElement.Size.X, BorderSize),
                        TextureId = Textures.selection_color,
                    }
                );
                // Right
                HudRenderer.renderer.elements.Add(
                    new HudElement
                    {
                        Position = BaseElement.Position + (BaseElement.Size.X - BorderSize, 0),
                        Size = (BorderSize, BaseElement.Size.Y),
                        TextureId = Textures.selection_color,
                    }
                );
                // Bottom
                HudRenderer.renderer.elements.Add(
                    new HudElement
                    {
                        Position = BaseElement.Position + (0, BaseElement.Size.Y - BorderSize),
                        Size = (BaseElement.Size.X, BorderSize),
                        TextureId = Textures.selection_color,
                    }
                );
            }
        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (InputActive)
            {
                if(TextHandler.AddKeysToString(keyboard, ref Text))
                {
                    OnChange(Text);
                }
                if (keyboard.IsKeyPressed(Keys.Enter))
                {
                    SubmitAction(Text);
                    InputActive = false;
                    Input.currentlyTexting = false;
                }

                if (mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (!Hovered(out _))
                    {
                        SubmitAction(Text);
                        InputActive = false;
                        Input.currentlyTexting = false;
                    }
                }

            } ////

            if (mouse.IsButtonPressed(TriggerOn))
            {
                if (Hovered(out _))
                {
                    InputActive = true;
                    Input.currentlyTexting = true;
                }
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }
    }
}