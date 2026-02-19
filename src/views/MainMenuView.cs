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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Window.Server.players.Add(0, new Player()
                {
                    ID = 0,
                    ClientId = 0,
                });
                Stage stage = Stage.GetStage(new LobbyInfo()
                {
                    Width = 32,
                    Height = 32,
                    Seed = 123,
                }, Client.Game);
                Window.Client.ControlledEntityID = Window.Server.PlayerList[0].Entity.ID;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Client.Game.CurrentStage = stage;
                CurrentView = GameView;
                Client.Game.SetState(GameState.GAMESTART);
                ServerMessages.SetLobbyInfo(Window.Server.bWriter);
                ServerMessages.GameStart(Window.Server.bWriter);
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


        public static BorderElement Card = new()
        {
            BaseElement = new()
            {
                Position = (MultiplayerButton.LowerLeft.X - outerMargin.X, -outerMargin.Y),
                Size = (MultiplayerButton.BaseElement.Size.X + outerMargin.X * 3, Window.Height) + outerMargin * 2,
                TextureId = Textures.hud_transparent_color,
            },
            RenderBaseElement = true,
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
            Card.Render();
            DirectPlayButton.Render(dT);

            if (multiplayerCardOpen)
            {
                multiplayerElement.Render(dT);
                MultiplayerButton.BorderTexture = MultiplayerButton.ClickTexture;
            }
            else
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