namespace fiveSeconds
{
    public class Tile
    {
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

        };

        public static Floor STONE = new()
        {
            AtlasIndex = 0
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
            
        };
    }
}