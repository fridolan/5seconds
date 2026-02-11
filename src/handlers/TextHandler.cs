using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbTrueTypeSharp;

namespace fiveSeconds
{

    public enum TextAlignX
    {
        LEFT,
        RIGHT,
        CENTER,
    }

    public enum TextAlignY
    {
        TOP,
        BOTTOM,
        CENTER,
    }

    public struct Text
    {
        public string text;
        public float x;
        public float y;
        public float scale;
        public Vector3 color;
        public TextAlignX alignX;
        public TextAlignY alignY;
        public int lineWidth;
    }


    public class TextHandler
    {
        private const int FirstChar = 32;
        private const int CharCount = 224; // 32-127
        private const int AtlasSize = 1024;

        public List<Text> elements = new();
        public List<Text> multilineElements = new();

        private int _texture;
        private StbTrueType.stbtt_bakedchar[] _charData = new StbTrueType.stbtt_bakedchar[0];
        private int _vao, _vbo;
        private Shader _shader;

        private Matrix4 _ortho;

        public static TextHandler renderer;

        public TextHandler(string fontPath, float pixelHeight, int screenWidth, int screenHeight)
        {
            _shader = new Shader("text.vs", "text.fs");
            _ortho = Matrix4.CreateOrthographicOffCenter(0, screenWidth, screenHeight, 0, -1, 1);
            LoadFont(fontPath, pixelHeight);
            InitGLResources();
        }

        public void Draw()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];

                RenderText(element);
            }
            for (int i = 0; i < multilineElements.Count; i++)
            {
                var element = multilineElements[i];

                RenderTextMultiline(element);
            }

            elements.Clear();
            multilineElements.Clear();
        }

        private void LoadFont(string path, float pixelHeight)
        {
            byte[] ttf = File.ReadAllBytes(path);
            _charData = new StbTrueType.stbtt_bakedchar[CharCount];
            byte[] bitmap = new byte[AtlasSize * AtlasSize];

            StbTrueType.stbtt_BakeFontBitmap(ttf, 0, pixelHeight, bitmap, AtlasSize, AtlasSize, FirstChar, _charData.Count(), _charData);

            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, AtlasSize, AtlasSize, 0,
                          PixelFormat.Red, PixelType.UnsignedByte, bitmap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Console.WriteLine($"texture {_texture}");
        }

        private void InitGLResources()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4 * 1000, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0); // pos
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1); // uv
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public unsafe void RenderText(Text textO)
        {

            string text = textO.text;
            float x = textO.x;
            float y = textO.y;
            float scale = textO.scale;
            Vector3 color = textO.color;
            TextAlignX alignX = textO.alignX;
            TextAlignY alignY = textO.alignY;

            GL.UseProgram(_shader.Handle);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _shader.SetMatrix4("projection", _ortho);
            _shader.SetVector3("textColor", color);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "fontTexture"), 0); // Texture unit 0 ////

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);

            List<float> vertices = new List<float>();
            float scaledX = 0;
            float scaledY = 0;

            float offsetX = 0;
            float offsetY = 0;

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            StbTrueType.stbtt_aligned_quad[] qs = new StbTrueType.stbtt_aligned_quad[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c < FirstChar || c >= FirstChar + CharCount) continue;

                fixed (StbTrueType.stbtt_bakedchar* charDataPtr = _charData)
                {
                    StbTrueType.stbtt_aligned_quad q;
                    StbTrueType.stbtt_GetBakedQuad(charDataPtr, AtlasSize, AtlasSize, c - FirstChar, &scaledX, &scaledY, &q, 1);

                    minY = MathF.Min(minY, q.y0);
                    maxY = MathF.Max(maxY, q.y1);
                    minX = MathF.Min(minX, q.x0);
                    maxX = MathF.Max(maxX, q.x1);

                    qs[i] = q;
                }
            }

            float width = (maxX - minX);
            float height = (maxY - minY);

            // Console.WriteLine($"text: {text}, Width: {width}, X {minX} {maxX}, Y {minY} {maxY}, sX {scaledX}, sY {scaledY}");

            if (alignX == TextAlignX.LEFT)
            {
                offsetX = minX;
            }
            else if (alignX == TextAlignX.CENTER)
            {
                offsetX = +maxX / 2 /* width / 2 + minX */;
            }
            else if (alignX == TextAlignX.RIGHT)
            {
                offsetX = +maxX /* width + minX */;
            }

            if (alignY == TextAlignY.BOTTOM)
            {
                offsetY = 0;
            }
            else if (alignY == TextAlignY.CENTER)
            {
                offsetY = +minY / 2;
            }
            else if (alignY == TextAlignY.TOP)
            {
                offsetY = +minY /* - height + maxY */;
            }

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c < FirstChar || c >= FirstChar + CharCount) continue;
                StbTrueType.stbtt_aligned_quad q = qs[i];

                qs[i] = q;

                float x0 = x + (q.x0 - offsetX/* - minX */) * scale /* - offsetX */;
                float x1 = x + (q.x1 /* - minX */ - offsetX) * scale /* - offsetX */;
                float y0 = y + (q.y0 /* - maxY */ - offsetY) * scale /* - offsetY */;
                float y1 = y + (q.y1 /* - maxY */ - offsetY) * scale /* - offsetY */;

                // Zwei Dreiecke (6 Vertices à 4 Werte: x, y, s, t)
                vertices.AddRange(new float[]
                {
                x0, y0, q.s0, q.t0,
                x1, y0, q.s1, q.t0,
                x0, y1, q.s0, q.t1,

                x0, y1, q.s0, q.t1,
                x1, y0, q.s1, q.t0,
                x1, y1, q.s1, q.t1
                });

            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Count * sizeof(float), vertices.ToArray());
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count / 4);

        }

        public unsafe void RenderTextMultiline(Text textO)
        {

            string text = textO.text;
            float x = textO.x;
            float y = textO.y;
            float rightLimit = x + textO.lineWidth;
            float scale = textO.scale;
            Vector3 color = textO.color;
            TextAlignX alignX = textO.alignX;
            TextAlignY alignY = textO.alignY;

            GL.UseProgram(_shader.Handle);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _shader.SetMatrix4("projection", _ortho);
            _shader.SetVector3("textColor", color);
            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "fontTexture"), 0); // Texture unit 0 ////

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);

            List<float> vertices = new List<float>();
            float xPos = 0;
            float yPos = 0;

            float offsetX = 0;
            float offsetY = 0;

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            StbTrueType.stbtt_aligned_quad[] qs = new StbTrueType.stbtt_aligned_quad[text.Length];

            List<float> spaceXPositions = new();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c < FirstChar || c >= FirstChar + CharCount) continue;

                fixed (StbTrueType.stbtt_bakedchar* charDataPtr = _charData)
                {
                    StbTrueType.stbtt_aligned_quad q;
                    StbTrueType.stbtt_GetBakedQuad(charDataPtr, AtlasSize, AtlasSize, c - FirstChar, &xPos, &yPos, &q, 1);

                    minY = MathF.Min(minY, q.y0);
                    maxY = MathF.Max(maxY, q.y1);
                    minX = MathF.Min(minX, q.x0);
                    maxX = MathF.Max(maxX, q.x1);

                    q.x0 *= scale;
                    q.x1 *= scale;
                    q.y0 *= scale;
                    q.y1 *= scale;

                    qs[i] = q;

                    if (c == ' ' || i == text.Length - 1) spaceXPositions.Add(q.x0);

                }
            }

            float width = (maxX - minX);
            float height = (maxY - minY);

            //Console.WriteLine($"text: {text}, Width: {width}, X {minX} {maxX}, Y {minY} {maxY}, sX {scaledX}, sY {scaledY}");

            if (alignX == TextAlignX.LEFT)
            {
                offsetX = minX;
            }
            else if (alignX == TextAlignX.CENTER)
            {
                offsetX = textO.lineWidth / 2 /* width / 2 + minX */;
            }
            else if (alignX == TextAlignX.RIGHT)
            {
                offsetX = textO.lineWidth /* width + minX */;
            }

            if (alignY == TextAlignY.BOTTOM)
            {
                offsetY = 0;
            }
            else if (alignY == TextAlignY.CENTER)
            {
                offsetY = +minY / 2 * scale;
            }
            else if (alignY == TextAlignY.TOP)
            {
                offsetY = +minY * scale /* - height + maxY */;
            }

            float currentHeight = 0;
            float lineXOffset = 0;

            float startX = x - offsetX;
            float endX = x - (offsetX + width);

            int currentSpace = 0;

            float previousX1 = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\n')
                {
                    lineXOffset = previousX1;
                    currentHeight += height * scale;
                }

                if (c < FirstChar || c >= FirstChar + CharCount) continue;
                StbTrueType.stbtt_aligned_quad q = qs[i];

                qs[i] = q;
                previousX1 = q.x1;
                float x0 = q.x0/*  * scale */;
                float x1 = q.x1 /* * scale */;
                float y0 = (q.y0 - offsetY) /* * scale */;
                float y1 = (q.y1 - offsetY) /* * scale */;

                float nextSpacePosition = spaceXPositions[currentSpace] - lineXOffset;
                if (nextSpacePosition > textO.lineWidth)
                {
                    lineXOffset = q.x0;
                    currentHeight += height * scale;
                }

                if (c == ' ') currentSpace += 1;

                float x0l = x0 + startX - lineXOffset;
                float x1l = x1 + startX - lineXOffset;
                float y0l = y0 + y + currentHeight;
                float y1l = y1 + y + currentHeight;

                // Zwei Dreiecke (6 Vertices à 4 Werte: x, y, s, t)
                vertices.AddRange(new float[]
                {
                x0l, y0l, q.s0, q.t0,
                x1l, y0l, q.s1, q.t0,
                x0l, y1l, q.s0, q.t1,

                x0l, y1l, q.s0, q.t1,
                x1l, y0l, q.s1, q.t0,
                x1l, y1l, q.s1, q.t1
                });

            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Count * sizeof(float), vertices.ToArray());
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count / 4);

        }

        public static bool AddKeysToString(KeyboardState keyboard, ref string str, ref float timeSinceLastRemove)
        {
            bool isBackspaceUp = timeSinceLastRemove > 0.15f;
            //if()
            int initialLength = str.Length;
            bool changed = false;
            for (int i = 0; i < 26; i++)
            {
                if (keyboard.IsKeyPressed(Keys.A + i))
                {
                    if (!keyboard.IsKeyDown(Keys.LeftShift))
                        str += (char)('a' + i);
                    else
                        str += (char)(Keys.A + i);
                }
            }
            if (keyboard.IsKeyPressed(Keys.Space)) str += " ";
            if (keyboard.IsKeyPressed(Keys.Backspace) || (keyboard.IsKeyDown(Keys.Backspace) && isBackspaceUp))
            {
                timeSinceLastRemove = 0;
                if (str.Length > 0) {
                    str = str.Substring(0, str.Length - 1);
                }
                changed = true;
            }
            for (int i = 0; i < 9; i++)
            {
                if (keyboard.IsKeyPressed(Keys.D0 + i))
                {
                    str += (char)(Keys.D0 + i);
                }
            }
            if (keyboard.IsKeyPressed(Keys.Period))
            {
                str+= ".";
            }
            return changed || initialLength != str.Length;
        }
    }
}