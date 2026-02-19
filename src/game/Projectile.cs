using OpenTK.Mathematics;

namespace fiveSeconds
{
    public enum ProjectileMode
    {
        GOAL,
        VELOCITY,
    }

    public class Projectile
    {
        public Vector2i PosFP;
        public Vector2i GoalFP;
        public Vector2i VelocityFP;

        public int SpeedPercent;

        public ProjectileMode Mode = ProjectileMode.VELOCITY;

        public long TimePassed;
        public long Duration;
        public long NextTickTime;
        public long PreviousTickTime;

        public Vector2i SizePercent;
        public int TextureIndex;
        public Vector2 VisualPos;

        public bool FinishByDuration = true;

        public Action<Projectile, Mesh, float> DrawCallback;
        public Func<Projectile, long, bool> TickCallback;

        public static void Draw(Projectile projectile, Mesh mesh)
        {
            mesh.AtlasRect(-0.5f * ((Vector2)projectile.SizePercent / 100f) + projectile.VisualPos, (Vector2)projectile.SizePercent / 100f, projectile.TextureIndex);
        }

        public void Advance(float dT)
        {
            VisualPos += dT * (Vector2)VelocityFP / FP.SCALE;
        }

        public static void ExtraDraw(Projectile projectile, Mesh mesh, float dT, Vector2 offset, int textureIndex)
        {
            Vector2 visualPos = projectile.VisualPos;
            visualPos += dT * (Vector2)projectile.VelocityFP / FP.SCALE;
            mesh.AtlasRect(offset + -0.5f * ((Vector2)projectile.SizePercent / 100f) + visualPos, (Vector2)projectile.SizePercent / 100f, textureIndex);
        }

        public List<ICombat> FindHits()
        {
            Stage stage = Client.Game.CurrentStage;
            Vector2i corner1FP = PosFP - FP.ToFixed(SizePercent) / 100 / 2;
            Vector2i corner2FP = PosFP + FP.ToFixed(SizePercent) / 100 / 2;
            List<ICombat> hits = stage.EntityList.Where((e) =>
            {
                Vector2i posFixed = FP.ToFixed(e.Position);
                bool inside = posFixed.X >= corner1FP.X && posFixed.Y >= corner1FP.Y
                    && posFixed.X <= corner2FP.X && posFixed.Y <= corner2FP.Y;
                return inside;
            }).OfType<ICombat>().ToList();

            return hits;
        }
    }
}