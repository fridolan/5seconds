using OpenTK.Mathematics;

namespace fiveSeconds
{

    public class HudElement
    {
        public Vector2 Position;
        public Vector2 Size;
        public int TextureId;
        public Vector4 TexCoords = new(0, 0, 1, 1);
        public int Rotation = 0;
        
        public Vector2 UpperLeft => Position;
        public Vector2 UpperRight => Position + (Size.X, 0);
        public Vector2 LowerLeft => Position + (0, Size.Y);
        public Vector2 LowerRight => Position + Size;

        public bool Hovered(out Vector2 position)
        {
            Vector2 p = (Input.mousePos.X, Window.Height - Input.mousePos.Y);
            Vector2 aP = Position;
            Vector2 aS = Size;

            position = (p - aP) / aS;

           // Console.WriteLine($"Hov pos {position}");

            return position.X > 0 && position.Y > 0
            && position.X < 1 && position.Y < 1;
        }

        public void Render()
        {
            HudRenderer.renderer.elements.Add(new()
            {
                Position = Position,
                Size = Size,
                TextureId = TextureId,
                TexCoords = TexCoords,
                Rotation = Rotation,
            });
        }
    }
}