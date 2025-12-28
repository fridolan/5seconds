using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class Mesh
    {
        public int Vao;
        public int Vbo;
        public int Ebo;
        public int IndexCount;
        public List<float> Vertices = [];
        public List<uint> Indices = [];
        public uint VertexOffset = 0;

        public static readonly Vector2[] texCoords = [
            (0,0),
            (1,0),
            (1,1),
            (0,1),
        ];

        public void RectAt(Vector2 pos, int index, Vector2 size, int rotation = 0)
        {
            float x = pos.X;
            float y = pos.Y;
            float sizeX = rotation % 2 == 0 ? size.X : size.Y;
            float sizeY = rotation % 2 == 0 ? size.Y : size.X;

            /* float[] quadVerts = {
                        x, y, texCoords[rotation % 4].X, texCoords[rotation % 4].Y, index,
                        x + sizeX, y, texCoords[(rotation + 1) % 4].X, texCoords[(rotation + 1) % 4].Y, index,
                        x + sizeX, y + sizeY, texCoords[(rotation + 2) % 4].X, texCoords[(rotation + 2) % 4].Y, index,
                        x, y + sizeY, texCoords[(rotation +3) % 4].X, texCoords[(rotation + 3) % 4].Y, index
                    }; */
            float[] quadVerts = {
                        x, y + sizeY, texCoords[rotation % 4].X, texCoords[rotation % 4].Y, index,
                        x + sizeX, y + sizeY, texCoords[(rotation + 1) % 4].X, texCoords[(rotation + 1) % 4].Y, index,
                        x + sizeX, y, texCoords[(rotation + 2) % 4].X, texCoords[(rotation + 2) % 4].Y, index,
                        x, y, texCoords[(rotation +3) % 4].X, texCoords[(rotation + 3) % 4].Y, index
                    };
            Vertices.AddRange(quadVerts);
            Indices.AddRange([
                VertexOffset, VertexOffset + 1, VertexOffset + 2,
                        VertexOffset + 2, VertexOffset + 3, VertexOffset
            ]);
            VertexOffset += 4;
        }

        public void UploadToGPU()
        {
            DeleteBuffers();
            Vao = GL.GenVertexArray();
            Vbo = GL.GenBuffer();
            Ebo = GL.GenBuffer();

            GL.BindVertexArray(Vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * sizeof(float), Vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 5 * sizeof(float);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, stride, 4 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);

            IndexCount = Indices.Count;
        }

        public void DeleteBuffers()
        {
            if (Vao != 0) GL.DeleteVertexArray(Vao);
            if (Vbo != 0) GL.DeleteBuffer(Vbo);
            if (Ebo != 0) GL.DeleteBuffer(Ebo);
        }

        public void Clear()
        {
            DeleteBuffers();
            Indices = [];
            Vertices = [];
            IndexCount = 0;
            VertexOffset = 0;
        }
    }
}