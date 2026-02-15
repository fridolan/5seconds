using OpenTK.Mathematics;

namespace fiveSeconds
{
    public interface IInputStartGoal
    {
        public Vector2i Start {get; set;}
        public Vector2i Goal {get; set;}
    }

    public interface IInputEntityID
    {
        public int EntityID {get; set;}
    }

    public interface IInputCancelOnDisplace
    {
        public bool CancelOnDisplace {get; set;}
    }

    public interface IInputRelative
    {
        public bool Relative {get; set;}
    }

    public interface IInputToEntityID
    {
        public int ToEntityID {get; set;}
    }
}