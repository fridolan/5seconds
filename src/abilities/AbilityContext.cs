using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class AbilityContext
    {
        public Vector2i HoveredTile;
        public Entity? HoveredEntity;
        public Stage Stage;
        public Entity SourceEntity;
        public bool ValidHover => Stage.ValidTilePos(HoveredTile);
    }
}