namespace fiveSeconds
{
    public class Cave1 : Stage
    {
        public Cave1()
        {

        }

        public override void Generate()
        {
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
                Position = /* (Width / 3, Height / 3) */ (0, 0)
            });

            List<Player> players = Window.Server != null ? Window.Server.players.Values.ToList() : Window.Client.Players;
            for (int i = 0; i < players.Count; i++)
            {
                Entity entity = new Aspect()
                {
                    Position = (3 + i * 2, 3 + i * 2),
                };
                players[i].Entity = entity;
                AddEntity(entity);
            }



            CreateEntityMesh();
        }
    }
}