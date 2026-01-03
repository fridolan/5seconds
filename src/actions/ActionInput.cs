using OpenTK.Mathematics;

namespace fiveSeconds
{
    public interface IStartGoalInput
    {
        public Vector2i Start {get; set;}
        public Vector2i Goal {get; set;}
    }

    public interface IEntityIDInput
    {
        public int EntityID {get; set;}
    }

    public interface ICancelOnDisplaceInput
    {
        public bool CancelOnDisplace {get; set;}
    }

    public interface IRelativeInput
    {
        public bool Relative {get; set;}
    }

    public interface IToEntityIDInput
    {
        public int ToEntityID {get; set;}
    }
}