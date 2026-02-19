using LiteNetLib.Utils;
using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class FireballAction : Ability
    {
        public int DamagePercent = 100;
        public int SpeedPercent = 550;
        public int SizePercent = 300;
        public override int Icon => Textures.fireball;

        public Entity Entity;

        public FireballAction(Random random)
        {

        }

        public override void Begin(AbilityAction action)
        {
            StartGoalInput input = (StartGoalInput)action.Input;
            Entity = stage.GetEntityByID(input.EntityID);
            if (Entity is not ICombat) throw new Exception("MeleeAttackEntityAction Entity not ICombat entity");
            
            input.Start = Entity.Position;

            Vector2i tileDistance = input.Goal - input.Start;
            Vector2i directionFixed = FP.Normalize(FP.ToFixed(tileDistance), out int lengthFixed);
            Vector2i velocityFixed = directionFixed * SpeedPercent / 100;
            

            Console.WriteLine($"Creating Fireball with tD {tileDistance}, dF {directionFixed}, vF {velocityFixed}, lF {lengthFixed}");

            stage.Projectiles.Add(new()
            {
                Mode = ProjectileMode.VELOCITY,
                VelocityFP = velocityFixed,
                Duration = ((long)(lengthFixed * 100 / SpeedPercent) * 1_000_000) >> FP.FRACTION_BITS,
                PosFP = FP.ToFixed(input.Start) + (FP.SCALE / 2, FP.SCALE / 2),
                VisualPos = new Vector2(0.5f, 0.5f) + input.Start,
                SizePercent = (SizePercent, SizePercent),
                DrawCallback = (effect, mesh, dT) =>
                {
                    if(Client.Game.State == GameState.UPDATE)
                    effect.Advance(dT);
                    float speedOffsetMult = 100f / MathF.Pow(SpeedPercent, 0.92f);
                    Projectile.ExtraDraw(effect, mesh, -3.5f / (float)SizePercent * 100f * speedOffsetMult, (0, 0), EffectAtlasIndeces.FIREBALL_TRAIL_2);
                    Projectile.ExtraDraw(effect, mesh, -1.75f / (float)SizePercent * 100f * speedOffsetMult, (0, 0), EffectAtlasIndeces.FIREBALL_TRAIL_1);
                    Projectile.Draw(effect, mesh);
                },
                TickCallback = (proj, dT) =>
                {
                    //effect.PosFP = (FP.SCALE / 2, FP.SCALE / 2) + FP.ToFixed(input.Start) + (Vector2)(input.Goal - input.Start) * (effect.TimePassed / effect.Duration);
                    proj.PosFP += FP.Mult(proj.VelocityFP, FP.Div((int)dT, 1_000_000));
                    Console.WriteLine($"Fireball Pos updated {(Vector2)proj.PosFP / FP.SCALE} {dT} {proj.Duration} {proj.VisualPos}");
                    proj.NextTickTime += 1_000_000 / 10;

                    List<ICombat> newHits = proj.FindHits().Where((h) => !action.AlreadyHit.Contains(h)).ToList();
                    newHits.ForEach((h) =>
                    {
                        action.AlreadyHit.Add(h);
                        CombatContext combatContext = new()
                        {
                            Source = (ICombat)Entity,
                            Target = h,
                            Damage = new()
                            {
                                Amount = 20 * DamagePercent / 100,
                                Type = DamageType.FIRE,
                            }
                        };
                        Damage.DamageC(combatContext);
                    });
                    return false;
                },
                NextTickTime = action.RoundTime,
                PreviousTickTime = action.RoundTime,

            });

            action.Finished = true;
        }

        public override AbilityInput GetNewAbilityInput() => new StartGoalInput();
    }
}