using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace fiveSeconds
{

    public static class Textures
    {

        public static int tile_atlas = LoadTexture("images/tile_atlas.png", false);
        public static int prop_atlas = LoadTexture("images/prop_atlas.png", false);
        public static int entity_atlas = LoadTexture("images/entity_atlas.png", false);
        public static int special_tile_atlas = LoadTexture("images/special_tile_atlas.png", false);
        public static int slider_head = LoadTexture("images/slider_head.png", false);
        public static int slightly_transparent_white = LoadTexture("images/slightly_transparent_white.png");
        public static int actions_slots = LoadTexture("images/actions_slots.png");
        public static int hp_bar = LoadTexture("images/hp_bar.png");
        public static int hp_bar_empty = LoadTexture("images/hp_bar_empty.png");
        public static int mana_bar = LoadTexture("images/mana_bar.png");
        public static int mana_bar_empty = LoadTexture("images/mana_bar_empty.png");
        public static int slot = LoadTexture("images/slot.png");
        public static int sword = LoadTexture("images/sword.png");
        public static int directplay_button = LoadTexture("images/directplay_button.png");
        public static int multiplayer_button = LoadTexture("images/multiplayer_button.png");
        public static int hud_color = LoadTexture("images/hud_color.png");
        public static int hud_transparent_color = LoadTexture("images/hud_transparent_color.png");
        public static int selection_color = LoadTexture("images/selection_color.png");
        public static int main_menu_background = LoadTexture("images/main_menu_background.png");

        public static TextureInfo INFO_hp_bar = new()
        {
            Margin = (5, 5),
            Size = (105, 26),
        };

        public static TextureInfo INFO_actions_slots = new()
        {
            Margin = (7,7),
            Size = (227, 30),
        };

        public static TextureInfo INFO_slot = new()
        {
            Margin = (5,5),
            Size = (32, 32)
        };

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

    public class TextureInfo
    {
        public Vector2 Size;
        public Vector2 Margin;
        public float SizeRatio => Size.X / Size.Y;
        public Vector2 Inner => Size - Margin * 2;
        public Vector2 MarginToSizeRatio => Margin / Size;
    }
}