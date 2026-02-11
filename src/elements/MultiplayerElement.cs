using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class MultiplayerElement : WithBaseElement
    {
        public Vector2 outerMargin;
        public Vector2 innerMargin;

        private BorderElement card;
        private ButtonElement createLobbyButton;
        private ButtonElement joinLobbyText;
        private TextInputElement serverAddressInput;
        private ButtonElement joinLobbyButton;

        public MultiplayerElement(Vector2 outerMargin)
        {
            this.outerMargin = outerMargin;
            this.innerMargin = outerMargin / 2;
            card = new()
            {
                BaseElement = new()
                {
                    Position = outerMargin,
                    Size = (Window.Width / 3, Window.Width / 8 * 2 + outerMargin.Y),
                    TextureId = Textures.hud_transparent_color,
                },
                RenderBaseElement = true,
            };

            createLobbyButton = new()
            {
                BaseElement = new()
                {
                    Position = card.BaseElement.Position + innerMargin,
                    Size = (card.BaseElement.Size.X / 2 - innerMargin.X * 1.5f, card.BaseElement.Size.X / 8),
                },
                Text = "Create Lobby",
                TextSize = 0.8f,
                SubmitAction = () =>
                {
                    Window.InitServer();
                    Window.InitClient("localhost", true);
                    View.CurrentView = View.Lobby;
                },
            };

            joinLobbyText = new()
            {
                BaseElement = new()
                {
                    Position = createLobbyButton.LowerLeft/*  + (0,innerMargin.Y) */,
                    Size = (card.BaseElement.Size.X / 2 - innerMargin.X * 1.5f, card.BaseElement.Size.X / 12),
                },
                Text = "Join Lobby",
                RenderBorder = false,
                RenderHover = false,
                TextSize = 0.8f,
                RenderBorderBottom = true,
            };

            serverAddressInput = new()
            {
                BaseElement = new()
                {
                    Position = joinLobbyText.LowerLeft + (0, card.BaseElement.Size.X / 12 + innerMargin.Y),
                    Size = (card.BaseElement.Size.X - innerMargin.X * 2f, card.BaseElement.Size.X / 12),
                },
                HeaderText = "Server Address",
                HeaderFactor = 1f,
                TextSize = 0.8f,
                Text = "idolfan.ddns.net"
            };

            joinLobbyButton = new()
            {
                BaseElement = new()
                {
                    Position = serverAddressInput.LowerLeft + (0, innerMargin.Y),
                    Size = (card.BaseElement.Size.X / 2 - innerMargin.X * 1.5f, card.BaseElement.Size.X / 8),
                },
                Text = "Join",
                TextSize = 0.8f,
                SubmitAction = () =>
                {
                    Window.InitClient(serverAddressInput.Text, true);
                    Window.Client.ActionOnConnect = () =>
                    {
                        View.CurrentView = View.Lobby;
                        LobbyView.Host = false;
                    };
                },
            };
        }

        public void Render(float dT)
        {
            card.Render();
            createLobbyButton.Render(dT);
            joinLobbyText.Render(dT);
            serverAddressInput.Render();
            joinLobbyButton.Render(dT);
        }

        public void HandleInputs(float dT)
        {
            createLobbyButton.HandleInputs();
            joinLobbyButton.HandleInputs();
            serverAddressInput.HandleInputs(dT);
        }
    }
}