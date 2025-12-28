using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class TextBoxElement() : WithBaseElement
    {
        public int borderSize = 4;
        public int borderTexture = Textures.hud_color;
        public bool renderBorder = true;

        public Text text;
        public bool visible = true;
        public bool multiline = true;

        public bool allowInput = false;
        public bool inputActive = false;
        public Action<string> submitAction = (s) => { };

        public void Render()
        {
            if (!visible) return;

            if (renderBorder)
            {
                BorderElement.renderBorder(BaseElement, borderSize, borderTexture);
            }

            if (multiline)
            {
                text.x = BaseElement.Position.X + borderSize + 8;
                text.y = BaseElement.Position.Y + borderSize + 4;
                text.lineWidth = (int)(BaseElement.Size.X - 2 * borderSize - 8);

                TextHandler.renderer.multilineElements.Add(text);
            }
            else
            {
                TextHandler.renderer.elements.Add(text);
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (allowInput)
            {
                if (inputActive)
                {
                    TextHandler.AddKeysToString(keyboard, ref text.text);
                    if (keyboard.IsKeyPressed(Keys.Enter))
                    {
                        submitAction(text.text);
                        inputActive = false;
                        Input.currentlyTexting = false;
                    }

                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        if (!Hovered(out _))
                        {
                            submitAction(text.text);
                            inputActive = false;
                            Input.currentlyTexting = false;
                        }
                    }

                } ////

                if (mouse.IsButtonPressed(MouseButton.Left))
                {
                    if (Hovered(out _))
                    {
                        inputActive = true;
                        Input.currentlyTexting = true;
                    }
                }
            }
            
        }
    }
}