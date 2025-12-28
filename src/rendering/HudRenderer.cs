namespace fiveSeconds
{
    public class HudRenderer
    {

        public static SingleRenderer renderer;

        public static void OnLoad()
        {
            renderer = new SingleRenderer(
                "hud.vs",
                "hud.fs",
                shaderHandle =>
                { }
            );

            renderer.OnLoad();
        }

        public static void OnUnload() => renderer.OnUnload();

        public static void Draw(bool camMoved) => renderer.Draw();
    }
}