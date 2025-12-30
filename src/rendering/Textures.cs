using OpenTK.Graphics.OpenGL4;

namespace fiveSeconds
{

    public static class Textures
    {

        public static int tile_atlas = LoadTexture("images/tile_atlas.png", false);
        public static int prop_atlas = LoadTexture("images/prop_atlas.png", false);
        public static int entity_atlas = LoadTexture("images/entity_atlas.png", false);
        public static int special_tile_atlas = LoadTexture("images/special_tile_atlas.png", false);
        public static int slider_head = LoadTexture("images/slider_head.png", false);

        public const int texturesPerRow = 8;
        public const int tile_pixel_size = 8;
        public const int texture_count = texturesPerRow * texturesPerRow;

        public static int LoadTexture(string path, bool flip = true)
        {
            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(flip ? 1 : 0);

            using var stream = File.OpenRead(path);
            var image = StbImageSharp.ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return tex;

        }
    }
}