using OpenTK.Mathematics;

namespace fiveSeconds
{
    public abstract class Entity
    {
        public static readonly int TextureId = Textures.entity_atlas;

        public int AtlasIndex;
        public Vector2i Position;
    }


}