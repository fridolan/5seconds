namespace fiveSeconds
{
    public class Cave1 : Stage
    {
        public Cave1()
        {
            Generate();
        }

        public override void Generate()
        {
            Width = 32;
            Height = 32;
            Tiles = new Tile[Height][];
            for (int i = 0; i < Height; i++)
            {
                Tiles[i] = new Tile[Width];
                for (int j = 0; j < Width; j++)
                {
                    Tiles[i][j] = Floor.STONE;
                }
            }
            CreateTileMesh();

            Entities = new Entity[Height][];
            for (int i = 0; i < Height; i++)
            {
                Entities[i] = new Entity[Width];
                for (int j = 0; j < Width; j++)
                {
                    Entities[i][j] = null;
                }
            }

            AddEntity(new Spider()
            {
                Position = /* (Width / 3, Height / 3) */ (0,0)
            });

            CreateEntityMesh();
        }
    }
}