using LiteNetLib.Utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace fiveSeconds
{
    public class LobbyView : View
    {
        private static Vector2 outerMargin = (Window.Width / 60, Window.Width / 60);
        private static Vector2 innerMargin = outerMargin;
        public static List<Player> Players = [];
        public static bool Host = true;

        public LobbyInfo Info = new();

        private BorderElement card;
        private HudElement background;
        private ButtonElement startGameButton;
        private ButtonElement closeGameButton;
        private TextInputElement widthInput;
        private TextInputElement widthText;
        private TextInputElement heightInput;
        private TextInputElement heightText;
        private TextInputElement seedInput;
        private TextInputElement seedText;

        public LobbyView()
        {
            background = new()
            {
                Position = (0, 0),
                Size = (Window.Width, Window.Height),
                TextureId = Textures.main_menu_background,
            };

            card = new()
            {
                BaseElement = new()
                {
                    Position = outerMargin,
                    Size = (Window.Width, Window.Height) - outerMargin * 2,
                    TextureId = Textures.hud_transparent_color,
                },
                RenderBaseElement = true,
            };

            startGameButton = new()
            {
                BaseElement = new()
                {
                    Position = card.LowerRight - innerMargin - (card.BaseElement.Size.X / 6, card.BaseElement.Size.X / 16),
                    Size = (card.BaseElement.Size.X / 6, card.BaseElement.Size.X / 16),
                    TextureId = Textures.directplay_button,
                },
                Text = "Start Game",
                TextSize = 1f,
                RenderBaseElement = true,
                TextColor = (1f, 0.7f, 0.6f),
                RenderBadEffect = true,
                BadEffectColor = (0.95f, 0.45f, 0f),
                BadEffectMultiplier = 1.035f,
                SubmitAction = () =>
                {
                    Stage stage = Stage.GetStage(Info, Client.Game);
                    Window.Client.ControlledEntityID = Window.Server.PlayerList[0].Entity.ID;
                    Client.Game.CurrentStage = stage;
                    CurrentView = GameView;
                    Client.Game.SetState(GameState.GAMESTART);
                    ServerMessages.SetLobbyInfo(Window.Server.bWriter);
                    ServerMessages.GameStart(Window.Server.bWriter);
                    
                }
            };

            closeGameButton = new()
            {
                BaseElement = new()
                {
                    Position = card.LowerLeft + (innerMargin.X, -innerMargin.Y) - (0, card.BaseElement.Size.X / 16),
                    Size = (card.BaseElement.Size.X / 6, card.BaseElement.Size.X / 16),
                    TextureId = Textures.multiplayer_button,
                },
                Text = Host ? "Close Lobby" : "Leave Lobby",
                TextSize = 1f,
                RenderBaseElement = true,
                TextColor = (1f, 0.7f, 0.6f),
                RenderBadEffect = true,
                BadEffectColor = (0.95f, 0.45f, 0f),
                BadEffectMultiplier = 1.035f,
                SubmitAction = () =>
                {
                    Window.Client?.Stop();
                    Window.Server?.Stop();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    Window.Server = null;
                    Window.Client = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    CurrentView = MainMenuView;
                }
            };

            widthText = new()
            {
                BaseElement = new()
                {
                    Position = card.UpperLeft + innerMargin,
                    Size = (card.BaseElement.Size.X / 14, card.BaseElement.Size.X / 32),
                },
                RenderBorder = false,
                Text = "Width",
            };
            widthInput = new()
            {
                BaseElement = new()
                {
                    Position = widthText.UpperRight + (innerMargin.X, 0),
                    Size = (card.BaseElement.Size.X / 6, card.BaseElement.Size.X / 32),
                },
                Text = "64",
                SubmitAction = (s) =>
                {
                    if (int.TryParse(s, out int newValue))
                    {
                        Info.Width = newValue;
                        ServerMessages.SetLobbyInfo(Window.Server.bWriter);
                    }
                }
            };

            heightText = new()
            {
                BaseElement = new()
                {
                    Position = widthText.LowerLeft + (0, innerMargin.Y),
                    Size = widthText.BaseElement.Size,
                },
                RenderBorder = false,
                Text = "Height",
            };
            heightInput = new()
            {
                BaseElement = new()
                {
                    Position = heightText.UpperRight + (innerMargin.X, 0),
                    Size = widthInput.BaseElement.Size,
                },
                Text = "64",
                SubmitAction = (s) =>
                {
                    if (int.TryParse(s, out int newValue))
                    {
                        Info.Height = newValue;
                        ServerMessages.SetLobbyInfo(Window.Server.bWriter);
                    }
                }
            };

            seedText = new()
            {
                BaseElement = new()
                {
                    Position = heightText.LowerLeft + (0, innerMargin.Y),
                    Size = heightText.BaseElement.Size,
                },
                RenderBorder = false,
                Text = "Seed",
            };
            seedInput = new()
            {
                BaseElement = new()
                {
                    Position = seedText.UpperRight + (innerMargin.X, 0),
                    Size = heightInput.BaseElement.Size,
                },
                Text = "123",
                SubmitAction = (s) =>
                {
                    if (int.TryParse(s, out int newValue))
                    {
                        Info.Seed = newValue;
                        ServerMessages.SetLobbyInfo(Window.Server.bWriter);
                    }
                }
            };
        }

        public override void HandleInputs(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (Host)
            {
                startGameButton.HandleInputs();
                widthInput.HandleInputs(dT);
                heightInput.HandleInputs(dT);
                seedInput.HandleInputs(dT);
            }
            closeGameButton.HandleInputs();

        }

        public override void OnLoad()
        {

        }

        private float lobbyUpdateTimer = 0;
        private void ServerUpdate(float dT)
        {
            lobbyUpdateTimer += dT;
            if (lobbyUpdateTimer > 1)
            {
                lobbyUpdateTimer -= 1;
                ServerMessages.SetLobbyInfo(Window.Server.bWriter);
            }
        }

        public override void OnRenderFrame(FrameEventArgs args)
        {
            float dT = (float)args.Time;
            if (Window.Server != null) ServerUpdate(dT);

            if (!Host)
            {
                widthInput.Text = $"{Info.Width}";
                heightInput.Text = $"{Info.Height}";
                seedInput.Text = $"{Info.Seed}";
            }

            background.Render();
            card.Render();
            if (Host)
            {
                startGameButton.Render(dT);
            }
            closeGameButton.Render(dT);
            widthInput.Render();
            widthText.Render();
            heightInput.Render();
            heightText.Render();
            seedInput.Render();
            seedText.Render();

            HudRenderer.Draw(true);
            TextHandler.renderer.Draw();

            //MenuBaseRender.OnRenderFrame(args);
        }

    }

    public class LobbyInfo
    {
        public int Width = 64;
        public int Height = 64;
        public int Seed = 123;

        public static LobbyInfo FromReader(NetDataReader reader)
        {
            LobbyInfo newLobbyInfo = new()
            {
                Width = reader.GetInt(),
                Height = reader.GetInt(),
                Seed = reader.GetInt(),
            };

            return newLobbyInfo;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(Width);
            writer.Put(Height);
            writer.Put(Seed);
        }
    }
}