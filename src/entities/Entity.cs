using OpenTK.Mathematics;

namespace fiveSeconds
{
    public abstract class Entity
    {
        public static readonly int TextureId = Textures.entity_atlas;
        public static int IDCounter = 0; 

        public int AtlasIndex;
        public Vector2i Position;
        public int ID;
        public ActionList ActionList = new();

        public static Entity? GetByID(int id)
        {
            return Game.CurrentStage.EntityList.Find(e => e.ID == id);
        }
    }


}