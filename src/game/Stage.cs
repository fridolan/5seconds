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


        private float roundTime = 0;
        private List<ActionList> actionLists;
        public void Tick(float dT, out bool done)
        {
            roundTime += dT;
            List<ActionList> allActionLists = [.. EntityList.Select((e) => e.ActionList)];
            actionLists = [.. allActionLists.Where(l => l.Finished == false)];
            if (actionLists.Count > 0)
            {
                //Console.WriteLine($"ActionLists remaining {}");
                ActionList nextList = actionLists.MinBy(l => l.GetNextTiming());
                if (nextList != null && nextList.GetNextTiming() <= roundTime)
                {
                    nextList.Act();
                }
                done = false;
            }
            else
            {
                Console.WriteLine("Round over");
                done = true;
                roundTime = 0;
                foreach (var list in allActionLists)
                {
                    list.Reset();
                }
            }
        }

        public abstract void Generate();
        #region Render
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
                    if (entity == null) continue;

                    EntityMesh.RectAt((j, i), entity.AtlasIndex, (1, 1));
                }
            }

            EntityMesh.UploadToGPU();
            EntityMeshDirty = false;
        }
        #endregion

        public bool AddEntity(Entity entity)
        {
            Vector2i Position = entity.Position;
            if (!ValidTilePos(Position)) return false;

            Entities[Position.Y][Position.X] = entity;
            EntityList.Add(entity);
            entity.ID = Entity.IDCounter;
            Entity.IDCounter++;
            EntityMeshDirty = true;

            return true;
        }

        public bool RemoveEntity(Entity entity)
        {
            Vector2i Position = entity.Position;
            if (!ValidTilePos(Position)) return false;

            Entities[Position.Y][Position.X] = null;
            EntityList.Remove(entity);
            EntityMeshDirty = true;

            return true;
        }

        public bool MoveEntity(Entity entity, Vector2i newPosition)
        {
            if (!ValidTilePos(newPosition)) return false;

            Vector2i oldPosition = entity.Position;

            Entities[oldPosition.Y][oldPosition.X] = null;
            entity.Position = newPosition;
            Entities[newPosition.Y][newPosition.X] = entity;
            EntityMeshDirty = true;

            return true;
        }

        public bool MoveEntity(int entityID, Vector2i newPosition)
        {
            Entity entity = Entity.GetByID(entityID);
            if(entity == null) return false;
            return MoveEntity(entity, newPosition);
        }

        public bool MoveEntity(Vector2i Pos, Vector2i newPosition)
        {
            Entity? entity = Entities[Pos.Y][Pos.X];
            if (entity != null) return MoveEntity(entity, newPosition);
            return false;
        }


        #region Helper
        public bool ValidTilePos(Vector2i? Position, bool log = false)
        {
            if (Position is not { } p) return false;

            bool valid = p.X >= 0 && p.X < Width
                && p.Y >= 0 && p.Y < Height;
            if (log && !valid) Console.WriteLine($"Invalid Tile p {p}");

            return valid;
        }

        public List<Vector2i> GetPathTo(Entity entity, Vector2i goal)
        {
            Vector2i origin = entity.Position;
            Console.WriteLine($"GetPathTo {origin}->{goal}");

            List<Vector2i> path = [origin];
            Vector2i difference = goal - origin;
            Vector2i directions = (Math.Sign(difference.X), Math.Sign(difference.Y));

            for (int i = 1; i <= difference.Abs().X; i++)
            {
                path.Add((origin.X + i * directions.X, origin.Y));
            }
            for (int i = 1; i <= difference.Abs().Y; i++)
            {
                path.Add((goal.X, origin.Y + i * directions.Y));
            }

            return path;
        }

        #endregion
    }
}