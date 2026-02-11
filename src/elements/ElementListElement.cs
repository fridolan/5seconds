using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class ElementListElement : WithBaseElement
    {
        public int BorderSize = 4;
        public int BorderTexture = Textures.hud_color;

        public string[] Strings;
        public BorderElement[] BorderElements;
        public bool Expanded = false;
        public int SelectedIndex = -1;
        public float TextSize = 1f;
        public string HeaderText = "";
        public float HeaderFactor = 1 / 0.7f;
        public Action<int> OnSelectAction = (index) => { };
        public int ElementTexture = -1;

        public void Render()
        {
            BorderElement.renderBorder(BaseElement, BorderSize, BorderTexture);

            // Header
            Text header = new Text
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

            if (SelectedIndex != -1)
            {
                TextHandler.renderer.elements.Add(new Text
                {
                    text = Strings[SelectedIndex],
                    scale = TextSize,
                    x = BaseElement.Position.X + BorderSize + 1,
                    y = BaseElement.Position.Y + BaseElement.Size.Y / 2 /* + - borderSize - 1 */,
                    alignX = TextAlignX.LEFT,
                    alignY = TextAlignY.CENTER,
                    color = (0, 1, 1),
                });

            }

            BorderElements = new BorderElement[Strings.Length];

            for (int i = 0; i < Strings.Length; i++)
            {
                BorderElements[i] = new BorderElement
                {
                    BaseElement = new HudElement
                    {
                        Position = BaseElement.Position + (i + 1) * new Vector2(0, BaseElement.Size.Y),
                        Size = BaseElement.Size,
                        TextureId = ElementTexture
                    },
                    borderSize = 4,
                    RenderBaseElement = ElementTexture != -1,
                };

                if (Expanded)
                {
                    BorderElements[i].Render();

                    TextHandler.renderer.elements.Add(new Text
                    {
                        text = Strings[i],
                        scale = TextSize,
                        x = BorderElements[i].BaseElement.Position.X + BorderSize + 1,
                        y = BorderElements[i].BaseElement.Position.Y + BorderElements[i].BaseElement.Size.Y / 2,
                        alignX = TextAlignX.LEFT,
                        alignY = TextAlignY.CENTER,
                        color = (0, 1, 1),
                    });

                }
            }

        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (Keybind.LEFTCLICK.IsPressed())
            {
                if (Expanded)
                {
                    SelectedIndex = HoveredIndex();
                    Expanded = false;
                    if (SelectedIndex != -1)
                        OnSelectAction(SelectedIndex);

                    return;
                } ////

                if (Hovered(out _) && Strings.Length > 0)
                {
                    Expanded = true;
                }
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }

        public int HoveredIndex()
        {
            for (int i = 0; i < BorderElements.Length; i++)
            {
                if (BorderElements[i].Hovered(out _)) return i;
            }

            return -1;
        }
    }
}