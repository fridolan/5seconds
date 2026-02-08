using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{

    public class Window : GameWindow
    {
        public static int Width = 1920;
        public static int Height = 1080;
        public static float aspectRatio = Width / (float)Height;

        public static int FrameRate = 0;

        public static Server Server;
        public static Client Client;

        public Window() : base(
            new GameWindowSettings
            {
                UpdateFrequency = FrameRate,
            },
            new NativeWindowSettings
            {
                Title = "Space Schmutz",
                ClientSize = (Width, Height),
                //WindowBorder = WindowBorder.Hidden,
                StartVisible = true,
                StartFocused = true,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3, 3),
                //Vsync = VSyncMode.On,
            })
        {
            CenterWindow();
        }

        protected override void OnLoad()
        {
            UpdateViewport();
            base.OnLoad();
            GL.ClearColor(0.02f, 0.02f, 0.02f, 1f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            HudRenderer.OnLoad();
            TextHandler.renderer = new TextHandler("fonts/Jersey15-Regular.ttf", 64, Width, Height);

            View.GameView.OnLoad();
            Client.Game = new Game();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            //GL.Viewport(0, 0, Width, Height);
            UpdateViewport();
        }

        private void UpdateViewport()
        {
            const float targetAspect = 16f / 9f;

            int fbWidth = FramebufferSize.X;
            int fbHeight = FramebufferSize.Y;

            float windowAspect = fbWidth / (float)fbHeight;

            int vpWidth, vpHeight, vpX, vpY;

            if (windowAspect > targetAspect)
            {
                // links / rechts
                vpHeight = fbHeight;
                vpWidth = (int)(vpHeight * targetAspect);
                vpX = (fbWidth - vpWidth) / 2;
                vpY = 0;
            }
            else
            {
                // oben / unten
                vpWidth = fbWidth;
                vpHeight = (int)(vpWidth / targetAspect);
                vpX = 0;
                vpY = (fbHeight - vpHeight) / 2;
            }

            GL.Viewport(vpX, vpY, vpWidth, vpHeight);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            AudioManager.Update(args.Time);
            Server?.Tick(args.Time);
            Client?.Tick();

            HandleInputs(args, KeyboardState, MouseState);
            Client.Game?.OnUpdateFrame(args);
        }

        private static double TimeAccumulator = 0;
        private static int FrameCount = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            aspectRatio = (float)Width / Height;
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            View.CurrentView.OnRenderFrame(args);
            Context.SwapBuffers();

            FrameCount++;
            TimeAccumulator += args.Time;
            if (TimeAccumulator >= 1.0)
            {
                String s = Window.Server == null ? "Client" : "Server";
                Console.WriteLine($"FPS: {FrameCount}, State: {Client.Game?.State}, {s}, Round: {Client.Game?.CurrentStage?.Round}");
                FrameCount = 0;
                TimeAccumulator -= 1.0;
            }
        }

        private static void HandleInputs(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
        {
            Input.keyboard = keyboard;
            Input.mouse = mouse;

            Input.wheelDelta = mouse.ScrollDelta;
            Input.mousePos = (mouse.Position.X, Window.Height - mouse.Position.Y);

            View.CurrentView.HandleInputs(args);

            if (Server == null && keyboard.IsKeyPressed(Keys.O))
                InitServer();

            if (Client == null && keyboard.IsKeyPressed(Keys.I))
                InitClient(Server != null);

            if (Client == null && keyboard.IsKeyPressed(Keys.P))
            {
                if (Server == null) InitServer();
                InitClient(true);
            }

            /* if (keyboard.IsKeyPressed(Keys.M))
            {
                ServerMessages.PlayerID(Server.bWriter, 0);
            } */
        }

        public static void InitServer()
        {
            Server = new Server();
            Server.Start();
            Console.WriteLine("Start Server");
        }

        public static void InitClient(bool local)
        {
            Client = new Client();
            Client.Start(local ? "localhost" : "idolfan.ddns.net");
            Console.WriteLine("Start Client");
            Client.Game = new Game();
        }

    }
}
