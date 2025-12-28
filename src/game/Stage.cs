using OpenTK.Mathematics;

namespace fiveSeconds
{
    public abstract class Stage
    {
        public Tile[][] Tiles;
        public Entity?[][] Entities;
        public int Width;
        public int Height;
        public Mesh TileMesh;
        public Mesh EntityMesh;
        public List<Entity> EntityList = [];

        public bool EntityMeshDirty = false;
        public bool TileMeshDirty = false;

        public abstract void Generate();

        public void CreateTileMesh()
        {
            TileMesh = new();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Tile tile = Tiles[i][j];
                    TileMesh.RectAt((j, i), tile.AtlasIndex, (1, 1));
                }
            }

            TileMesh.UploadToGPU();
            TileMeshDirty = false;
        }

        public void CreateEntityMesh()
        {
            EntityMesh = new();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Entity? entity = Entities[i][j];
                    if(entity == null) continue;

                    EntityMesh.RectAt((j, i), entity.AtlasIndex, (1, 1));
                }
            }

            EntityMesh.UploadToGPU();
            EntityMeshDirty = false;
        }

        public bool AddEntity(Entity entity)
        {
            Vector2i Position = entity.Position;
            if(!ValidTilePos(Position)) return false;

            Entities[Position.Y][Position.X] = entity;
            EntityList.Add(entity);
            EntityMeshDirty = true;

            return true;
        }

        public bool RemoveEntity(Entity entity)
        {
            Vector2i Position = entity.Position;
            if(!ValidTilePos(Position)) return false;

            Entities[Position.Y][Position.X] = null;
            EntityList.Remove(entity);
            EntityMeshDirty = true;

            return true;
        }

        public bool MoveEntity(Entity entity, Vector2i newPosition)
        {
            if(!ValidTilePos(newPosition)) return false;

            Vector2i oldPosition = entity.Position;

            Entities[oldPosition.Y][oldPosition.X] = null;
            entity.Position = newPosition;
            Entities[newPosition.Y][newPosition.X] = null;
            EntityMeshDirty = true;

            return true;
        }

        public void MoveEntity(Vector2i Pos, Vector2i newPosition)
        {
            Entity? entity = Entities[Pos.Y][Pos.X];
            if (entity != null) MoveEntity(entity, newPosition);
        }

        public bool ValidTilePos(Vector2i Position, bool log = false)
        {
            bool valid = Position.X >= 0 && Position.X < Width
                && Position.Y >= 0 && Position.Y < Height;
            if(log) Console.WriteLine($"Invalid Tile Position {Position}");
    
            return valid;
        }
    }
}