namespace fiveSeconds
{
    public abstract class UIElement
    {
        public abstract void Render(float dt);
        public abstract void HandleInputs();
    }
}