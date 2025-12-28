using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class Page
    {
        public String Text = "";
    }

    public class JournalElement
    {
        private int currentLeftPage = 0;

        private static List<Page> pages = [
            new(){
                Text = Localization.Get("journal.introduction"),
            },
            new(){
                Text = Localization.Get("journal.ghostIntroduction"),
            },
            new(){
                Text = Localization.Get("journal.test1"),
            },
            new(){
                Text = Localization.Get("journal.test2"),
            },
            new(){
                Text = Localization.Get("journal.test3"),
            }
        ];

        private HudElement JournalPageElement = new()
        {
            Position = (0, 0),
            Size = (Window.Width, Window.Width / 2),
            TextureId = Textures.journal_page
        };

        private TextBoxElement LeftTextBox = new()
        {
            BaseElement = new()
            {
                Position = (1f / 32 * Window.Width, 2f / 32 * Window.Width),
                Size = (14f / 32 * Window.Width, 13f / 16 * Window.Width / 2),
            },
            multiline = true,
            text = new()
            {
                alignX = TextAlignX.LEFT,
                alignY = TextAlignY.TOP,
                scale = 0.5f,
                text = pages[0].Text,
            },
        };

        private TextBoxElement RightTextBox = new()
        {
            BaseElement = new()
            {
                Position = (Window.Width - 15f / 32 * Window.Width, 2f / 32 * Window.Width),
                Size = (14f / 32 * Window.Width, 13f / 16 * Window.Width / 2),
            },
            multiline = true,
            text = new()
            {
                alignX = TextAlignX.LEFT,
                alignY = TextAlignY.TOP,
                scale = 0.5f,
                text = pages[1].Text,
            },
        };


        public void Render()
        {
            LeftTextBox.text.text = (currentLeftPage < pages.Count) ? pages[currentLeftPage].Text : "";
            RightTextBox.text.text = (currentLeftPage + 1 < pages.Count) ? pages[currentLeftPage + 1].Text : "";
            HudRenderer.renderer.elements.Add(JournalPageElement);
            LeftTextBox.Render();
            RightTextBox.Render();
        }

        private bool turnNextPage = false;
        private bool turnPreviousPage = false;
        private Vector2 swipeStartPosition;

        public void HandleInputs()
        {
            if (Input.mouse.IsButtonPressed(MouseButton.Left))
            {
                turnPreviousPage = LeftTextBox.Hovered(out _);
                turnNextPage = RightTextBox.Hovered(out _);
                swipeStartPosition = Input.mousePos;
            }

            if (Input.mouse.IsButtonReleased(MouseButton.Left))
            {
                int direction = (int)(Input.mousePos - swipeStartPosition).X;
                if (turnNextPage && direction < 0)
                {
                    currentLeftPage += 2;
                }
                if (turnPreviousPage && direction > 0)
                {
                    if(currentLeftPage > 1) currentLeftPage -= 2;
                }
            }
        }
    }


}