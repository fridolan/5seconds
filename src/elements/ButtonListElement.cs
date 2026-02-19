using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class ButtonListElement : WithBaseElement
    {
        public int BorderSize = 4;
        public int BorderTexture = Textures.hud_color;

        public string[] Strings = [];
        public int SelectedIndex = -1;
        public float TextSize = 1f;
        public string HeaderText = "";
        public float HeaderFactor = 1 / 0.7f;
        public Action<int> OnSelectAction = (index) => { };
        public int ElementTexture = -1;
        public Vector2 OuterMargin;
        public Func<int, ButtonElement> ButtonElementCallback;
        public float ElementHeight;

        private ButtonElement[] ButtonElements = [];

        public void GenerateElements(int count)
        {
            ButtonElements = new ButtonElement[count];
            for (int i = 0; i < count; i++)
            {
                ButtonElement element = ButtonElementCallback(i);
                if (element.BaseElement == null) element.BaseElement = new();

                element.BaseElement.Position = BaseElement.Position + OuterMargin + (0, ElementHeight * i);
                element.BaseElement.Size = (BaseElement.Size.X - OuterMargin.X * 2, ElementHeight);

                ButtonElements[i] = element;
            }
        }

        public void Render(float dT)
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

            }

            for (int i = 0; i < ButtonElements.Length; i++)
            {
                ButtonElement element = ButtonElements[i];
                if (element != null)
                {
                    element.Render(dT);
                }
            }

        }

        public void HandleInputs()
        {
            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            if (Keybind.LEFTCLICK.IsPressed())
            {
                SelectedIndex = HoveredIndex();
                if (SelectedIndex != -1)
                    OnSelectAction(SelectedIndex);
            }

            for (int i = 0; i < ButtonElements.Length; i++)
            {
                ButtonElement element = ButtonElements[i];
                if (element != null) element.HandleInputs();
            }
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }

        public int HoveredIndex()
        {
            for (int i = 0; i < ButtonElements.Length; i++)
            {
                if (ButtonElements[i].Hovered(out _)) return i;
            }

            return -1;
        }
    }
}