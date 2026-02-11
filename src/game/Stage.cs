using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public abstract class Stage
    {
        public Game Game;

        public Tile[][] Tiles;
        public Entity?[][] Entities;
        public int Width;
        public int Height;
        public Mesh TileMesh;
        public Mesh EntityMesh;
        public List<Entity> EntityList = [];
        public int EntityIDCounter = 0;
        public int seed = 0;

        public bool EntityMeshDirty = false;
        public bool TileMeshDirty = false;

        public List<ActionList> ActionLists;
        public List<ActionList> FromServerActionlists = [];

        private float roundTime = 0;
        public int Round = 0;

        public Entity? PlayerEntity => Window.Client == null ? null : EntityList.Find(e => e.ID == Window.Client.ControlledEntityID);

        public static Dictionary<Type, int> GetTypeIndex = new(){
          { typeof(Cave1), 0 },
        };

        public static List<Func<Stage>> GetInstance = [
            () => new Cave1(),
        ];

        public void Tick(float dT, bool first, out bool done)
        {
            roundTime += dT;
            List<ActionList> allActionLists = Window.Server != null ? GetAllActionLists() : FromServerActionlists;

            ActionLists = [.. allActionLists.Where(l => l is { Finished: false, Waiting: false })];
            List<ActionList> waitingActionLists = [.. allActionLists.Where(l => l is { Finished: false, Waiting: true })];
            // ActionLists that await a certain state. They will try to act after any other act.

            if (first && Window.Server != null)
            {
                Console.WriteLine("Server tick sends");
                ServerMessages.ActionLists(Window.Server.bWriter, ActionLists, Round);
                ServerMessages.SetGameState(Window.Server.bWriter, Client.Game.State, Client.Game.InputTimeLeft, Round);
            }

            if (ActionLists.Count > 0)
            {
                //Console.WriteLine($"ActionLists remaining {ActionLists.Count}");
                ActionList nextList = ActionLists.MinBy(l => l.GetNextTiming());
                if (nextList != null && nextList.GetNextTiming() <= roundTime)
                {
                    nextList.Act(Game);
                }

                waitingActionLists.ForEach(l => l.Act(Game));

                done = false;
            }
            else
            {
                Console.WriteLine("Round over");
                done = true;
                roundTime = 0;
                Round++;

                foreach (var list in GetAllActionLists())
                {
                    list.Reset();
                }
            }
        }

        private List<ActionList> GetAllActionLists()
        {
            return [.. EntityList.Select((e) => e.ActionList)];
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
            entity.ID = EntityIDCounter;
            EntityIDCounter++;
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

        public void ClearEntities()
        {
            Entities = new Entity[Height][];
            for (int i = 0; i < Height; i++)
            {
                Entities[i] = new Entity[Width];
                for (int j = 0; j < Width; j++)
                {
                    Entities[i][j] = null;
                }
            }
            EntityList.Clear();
            EntityMeshDirty = true;
        }

        public bool MoveEntity(Entity entity, Vector2i pos)
        {
            Vector2i newPosition = pos;

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
            Entity entity = GetEntityByID(entityID);
            if (entity == null) return false;
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
            //Console.WriteLine($"GetPathTo {origin}->{goal}");

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

        public List<Vector2i> GetPathTo(int entityID, Vector2i goal)
        {
            Entity entity = GetEntityByID(entityID);
            if (entity == null) return [];

            return GetPathTo(entity, goal);
        }

        public Entity? GetEntityByID(int id)
        {
            return Game.CurrentStage.EntityList.Find(e => e.ID == id);
        }

        public static Stage FromReader(NetDataReader reader)
        {
            int type = reader.GetInt();
            Stage stage = GetInstance[type]();
            stage.seed = reader.GetInt();
            stage.Width = reader.GetInt();
            stage.Height = reader.GetInt();

            return stage;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(GetTypeIndex[this.GetType()]);
            writer.Put(seed);
            writer.Put(Width);
            writer.Put(Height);
        }

        #endregion
    }
}