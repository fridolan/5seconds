namespace fiveSeconds
{
    public class Tile
    {
        public static readonly int TextureId = Textures.tile_atlas;

        public bool Walkable = false;
        public bool Solid = false;
        public int AtlasIndex;
    }

    public class Floor : Tile
    {
        public Floor()
        {
            Walkable = true;
            Solid = false;
        }

        public static Floor ABYSS = new()
        {
            AtlasIndex = 1,
            Walkable = false,
        };

        public static Floor STONE = new()
        {
            AtlasIndex = 0,
        };
    }

    public class Wall : Tile
    {
        public Wall()
        {
            Walkable = false;
            Solid = true;
        }

        public static Wall STONE = new()
        {
            AtlasIndex = 2,
        };
    }
}