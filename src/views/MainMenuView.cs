using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public class MainMenuView : View
    {
        private static Vector2 outerMargin = (Window.Width / 60, Window.Width / 60);
        private static bool multiplayerCardOpen = false;

        private static HudElement Background = new()
        {
            Position = (0, 0),
            Size = (Window.Width, Window.Height),
            TextureId = Textures.main_menu_background,
        };

        public static ButtonElement DirectPlayButton = new()
        {
            BaseElement = new()
            {
                Position = (Window.Width / 3 * 2 - outerMargin.X, outerMargin.Y),
                Size = (Window.Width / 3, Window.Width / 8),
                TextureId = Textures.directplay_button,
            },
            Text = "Direct Play",
            TextSize = 1.6f,
            RenderBaseElement = true,
            TextColor = (1f, 0.7f, 0.6f),
            RenderBadEffect = true,
            BadEffectColor = (0.95f, 0.45f, 0f),
            BadEffectMultiplier = 1.035f,
            SubmitAction = () =>
            {
                if (Window.Server == null) Window.InitServer();
                Window.InitClient("localhost", true);
                CurrentView = GameView;
            }
        };

        public static ButtonElement MultiplayerButton = new()
        {
            BaseElement = new()
            {
                Position = DirectPlayButton.LowerLeft + (0, outerMargin.Y),
                Size = (Window.Width / 3, Window.Width / 8),
                TextureId = Textures.multiplayer_button,
            },
            Text = "Multiplayer",
            TextSize = 1.6f,
            RenderBaseElement = true,
            TextColor = (0.7f, 1f, 0.6f),
            RenderBadEffect = true,
            BadEffectColor = (0.45f, 0.95f, 0f),
            BadEffectMultiplier = 1.035f,
            SubmitAction = () =>
            {
                multiplayerCardOpen = !multiplayerCardOpen;
            }
        };

        private static MultiplayerElement multiplayerElement = new(outerMargin);

        public override void HandleInputs(FrameEventArgs args)
        {
            DirectPlayButton.HandleInputs();
            MultiplayerButton.HandleInputs();
            if (multiplayerCardOpen)
            {
                multiplayerElement.HandleInputs((float)args.Time);
            }
        }

        public override void OnLoad()
        {

        }

        public override void OnRenderFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            Background.Render();
            DirectPlayButton.Render(dT);
            
            if (multiplayerCardOpen)
            {
                multiplayerElement.Render(dT);
                MultiplayerButton.BorderTexture = MultiplayerButton.ClickTexture;
            } else
            {
                MultiplayerButton.BorderTexture = Textures.hud_color;
            }
            MultiplayerButton.Render(dT);

            HudRenderer.Draw(true);
            TextHandler.renderer.Draw();

            //MenuBaseRender.OnRenderFrame(args);
        }
    }
}