using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class GameView : View
    {
        private Shader TileShader;

        public Vector2 CameraPos = new(0f, 0f);
        public float CameraAngle = 0f;

        public Vector2 PlayerRelativePosition = (1, 1); // To Middle, not Com / Position
        public float PlayerRelativeAngle = 0f;

        public override void OnLoad()
        {
            TileShader = new Shader("tileAtlasV.glsl", "tileAtlasF.glsl");
            Zoom = 1 / 8f;
        }

        public override void OnRenderFrame(FrameEventArgs args)
        {
            RenderStage();

            HudRenderer.Draw(true);
            TextHandler.renderer.Draw();
        }

        public void RenderStage()
        {
            if (Game.CurrentStage == null) return;

            Stage stage = Game.CurrentStage;

            if(stage.TileMeshDirty) stage.CreateTileMesh();
            if(stage.EntityMeshDirty) stage.CreateEntityMesh();

            Matrix4 projection = Matrix4.CreateOrthographic(2f * Window.aspectRatio / View.CurrentView.Zoom, 2f / View.CurrentView.Zoom, -10f, 10f);

            Matrix4 view =
                Matrix4.CreateTranslation(-CameraPos.X, -CameraPos.Y, 0f) *
                Matrix4.CreateRotationZ(-CameraAngle);

            TileShader.Use();
            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uProjection"), false, ref projection);
            GL.Uniform1(GL.GetUniformLocation(TileShader.Handle, "uTilesPerRow"), 8f);
            

            // Render Tiles
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Textures.floor_atlas);
            TileShader.SetInt("uAtlas", 0);

            Matrix4 model =
                Matrix4.CreateRotationZ(0f) *
                Matrix4.CreateTranslation(0, 0, 0f);

            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uModel"), false, ref model);

            GL.BindVertexArray(stage.TileMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, stage.TileMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

            // Render Entities
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Entity.TextureId);
            TileShader.SetInt("uAtlas", 0);

            GL.BindVertexArray(stage.EntityMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, stage.EntityMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

        }

        public override void HandleInputs(FrameEventArgs args)
        {
            float dT = (float)args.Time;

            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            //Vector2i tilePosition = ScreenToTilePosition(Input.mousePos, CameraAngle - Game.ships[0].Angle);

            // Zoom
            if (keyboard.IsKeyPressed(Keys.Y))
            {
                Zoom = Zoom / 2;
            }
            if (keyboard.IsKeyPressed(Keys.U))
            {
                Zoom = Zoom * 2;
            }

            float normalizingFactor =
                (keyboard.IsKeyDown(Keys.W) ^ keyboard.IsKeyDown(Keys.S))
                && (keyboard.IsKeyDown(Keys.A) ^ keyboard.IsKeyDown(Keys.D)) ? MathF.Sqrt(2) / 2f : 1;

            float magnitude = dT * normalizingFactor * 2;

            // Camera Movement
            if (keyboard.IsKeyDown(Keys.W))
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle + MathF.PI);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle + MathF.PI);
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle - MathF.PI / 2);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle - MathF.PI / 2);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle + MathF.PI / 2);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle + MathF.PI / 2);
            }

          /*   if(!((keyboard.IsKeyDown(Keys.W) || (keyboard.IsKeyDown(Keys.S))) || (keyboard.IsKeyDown(Keys.A) || (keyboard.IsKeyDown(Keys.D)))))
            {
                Console.WriteLine("nichts gedrückt");
            } */

            // Camera rotation
            if (keyboard.IsKeyDown(Keys.Q))
            {
                CameraAngle -= dT;
            }
            if (keyboard.IsKeyDown(Keys.E))
            {
                CameraAngle += dT;
            }

        }

        private Vector2i ScreenToTilePosition(Vector2 mousePos, float angle)
        {
            Vector2 m = (Window.Width / 2, Window.Height / 2) + RotateVector(mousePos - (Window.Width / 2, Window.Height / 2), angle);
            Vector2 offset = PlayerRelativePosition;

            Vector2 tile = (
                (int)Math.Floor((m.X / Window.Width * 2f - 1.0f) / Zoom * Window.aspectRatio + offset.X + 0.5f),
                (int)Math.Floor((m.Y - Window.Height / 2) / Window.Height * 2 / Zoom + offset.Y + 0.5f));

            /* tile = (
                Math.Max(Math.Min(tile.X, MapWidth - 1), 0),
                Math.Max(Math.Min(tile.Y, MapHeight - 1), 0)); */

            //Console.WriteLine($"Tile at {tile} {mousePos}");

            return ((int)tile.X, (int)tile.Y);
        }

        private static Vector2 RotateVector(Vector2 vec, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector2(
                vec.X * cos - vec.Y * sin,
                vec.X * sin + vec.Y * cos
            );
        }
    }
}