using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public abstract class Entity
    {
        public static readonly int TextureId = Textures.entity_atlas;

        public int ID;
        public int AtlasIndex;
        public Vector2i Position;
        public ActionList ActionList = new();

        public static Entity FromReader(NetDataReader reader)
        {
            int type = reader.GetInt();
            Entity entity = GetInstance[type]();

            entity.ID = reader.GetInt();
            entity.Position = (reader.GetInt(), reader.GetInt());
            entity.ActionList = ActionList.FromReader(reader);

            if(entity is ICombat c)
            {
                c.Read(reader);
            }

            return entity;
        }

        public void Write(NetDataWriter writer)
        {
            writer.Put(GetTypeIndex[this.GetType()]);
            writer.Put(ID);
            writer.Put(Position.X);
            writer.Put(Position.Y);
            ActionList.Write(writer);

            if(this is ICombat c)
            {
                c.Write(writer);
            }
        }

        public static Dictionary<Type, int> GetTypeIndex = new(){
          { typeof(Aspect), 0 },
          { typeof(Spider), 0 },
        };

        public static List<Func<Entity>> GetInstance = [
            () => new Aspect(),
            () => new Spider(),
        ];

        public void AddAction(SAction action)
        {
            ActionList.Actions.Add(action);
        }
    }


}