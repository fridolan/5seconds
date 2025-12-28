using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class WithBaseElement
    {
        public HudElement BaseElement;
        
        public Vector2 UpperLeft => BaseElement.Position;
        public Vector2 UpperRight => BaseElement.Position + (BaseElement.Size.X, 0);
        public Vector2 LowerLeft => BaseElement.Position + (0, BaseElement.Size.Y);
        public Vector2 LowerRight => BaseElement.Position + BaseElement.Size;
    }
}