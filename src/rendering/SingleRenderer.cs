using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace fiveSeconds
{

    public class SingleRenderer
    {
        public static Shader shader;

        private int vao;
        private int vbo;
        private int ebo;

        private int uniformProjectionLocation;
        private int uniformModelLocation;
        private int uniformTexCoordsLocation;
        private int uniformRotationLocation;

        private string vertexShaderPath;
        private string fragmentShaderPath;

        private Action<int> setCustomUniforms;

        public readonly List<HudElement> elements = [];
        public List<HudElement> worldElements = [];

        public SingleRenderer(
            string vertexShaderPath,
            string fragmentShaderPath,
            Action<int> setCustomUniforms)
        {
            this.vertexShaderPath = vertexShaderPath;
            this.fragmentShaderPath = fragmentShaderPath;
            this.setCustomUniforms = setCustomUniforms;
        }

        public void OnLoad()
        {
            shader = new Shader(vertexShaderPath, fragmentShaderPath);
            shader.Use();

            uniformProjectionLocation = GL.GetUniformLocation(shader.Handle, "u_Projection");
            uniformModelLocation = GL.GetUniformLocation(shader.Handle, "u_Model");
            uniformTexCoordsLocation = GL.GetUniformLocation(shader.Handle, "u_TexCoords");
            uniformRotationLocation = GL.GetUniformLocation(shader.Handle, "u_Rotation");

            float[] quadVertices =
            {
                // posX, posY, texU, texV (0..1)
                0f, 0f, 0f, 1f,
                1f, 0f, 1f, 1f,
                1f, 1f, 1f, 0f,
                0f, 1f, 0f, 0f,
            };

            uint[] indices = { 0, 1, 2, 2, 3, 0 };

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public void OnUnload()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shader.Handle);
        }

        public void Draw()
        {
            //GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(vao);
            shader.Use();

            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, -1, 1);
            GL.UniformMatrix4(uniformProjectionLocation, false, ref ortho);

            //Console.WriteLine($"Draw elements: {elements.Count}");

            foreach (var element in elements)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, element.TextureId);

                Matrix4 model = Matrix4.CreateScale(element.Size.X, element.Size.Y, 1f) *
                                Matrix4.CreateTranslation(element.Position.X, element.Position.Y, 0f);
                GL.UniformMatrix4(uniformModelLocation, false, ref model);

                GL.Uniform4(uniformTexCoordsLocation, element.TexCoords);

                GL.Uniform1(uniformRotationLocation, element.Rotation);

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }

            Matrix4 worldProj = Matrix4.CreateOrthographic(2f * Window.aspectRatio / View.CurrentView.Zoom, 2f / View.CurrentView.Zoom, -10f, 10f);
             GL.UniformMatrix4(uniformProjectionLocation, false, ref worldProj);

            foreach (var element in worldElements)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, element.TextureId);

                Matrix4 model = Matrix4.CreateScale(element.Size.X * View.CurrentView.Zoom, element.Size.Y * View.CurrentView.Zoom, 1f) *
                                Matrix4.CreateTranslation(element.Position.X * View.CurrentView.Zoom, element.Position.Y * View.CurrentView.Zoom, 0f);
                GL.UniformMatrix4(uniformModelLocation, false, ref model);

                GL.Uniform4(uniformTexCoordsLocation, element.TexCoords);

                GL.Uniform1(uniformRotationLocation, element.Rotation);

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }

            GL.BindVertexArray(0);

            //GL.Enable(EnableCap.DepthTest);

            elements.Clear();
            worldElements.Clear();
        }

        public static Vector4 GetTexCoordsTile(int tile)
        {
            float x = (tile % Textures.texturesPerRow) / (float)Textures.texturesPerRow;
            float y = (tile / Textures.texturesPerRow) / (float)Textures.texturesPerRow;
            return (x, y, x + 1f / Textures.texturesPerRow, y + 1f / Textures.texturesPerRow);
        }

        public static Vector4 GetTexCoordsProp(int prop)
        {
            float x = (prop % Textures.texturesPerRow) / (float)Textures.texturesPerRow;
            float y = (prop / Textures.texturesPerRow) / (float)Textures.texturesPerRow;
            return (x, y, x + 1f / Textures.texturesPerRow, y + 1f / Textures.texturesPerRow);
        }
    }
}