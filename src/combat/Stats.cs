using LiteNetLib.Utils;

namespace fiveSeconds
{
    public class BasicStat
    {
        public int Index;

        public static readonly BasicStat MaxHealth = new()
        {
            Index = 0,
        };

        public static readonly BasicStat CurrentHealth = new()
        {
            Index = 1,
        };

        public static readonly BasicStat AttackSpeed = new()
        {
            Index = 2,
        };

        public static readonly BasicStat MovementSpeed = new()
        {
            Index = 3,
        };

        public static readonly BasicStat Armor = new()
        {
            Index = 4,
        };

        public static readonly BasicStat MaxMana = new()
        {
            Index = 5,
        };

        public static readonly BasicStat CurrentMana = new()
        {
            Index = 6,
        };

        public const int TypeCount = 7;

        public static implicit operator int(BasicStat type)
        {
            return type.Index;
        }

        public static float[] FilledArray(float value)
        {
            float[] array = new float[TypeCount];
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }
    }

    public class Stats
    {
        public float MaxHealth { get => Basic[BasicStat.MaxHealth]; set => Basic[BasicStat.MaxHealth] = value; }
        public float CurrentHealth { get => Basic[BasicStat.CurrentHealth]; set => Basic[BasicStat.CurrentHealth] = value; }
        public float AttackSpeed { get => Basic[BasicStat.AttackSpeed]; set => Basic[BasicStat.AttackSpeed] = value; }
        public float MovementSpeed { get => Basic[BasicStat.MovementSpeed]; set => Basic[BasicStat.MovementSpeed] = value; }
        public float Armor { get => Basic[BasicStat.Armor]; set => Basic[BasicStat.Armor] = value; }
        public float MaxMana { get => Basic[BasicStat.MaxMana]; set => Basic[BasicStat.MaxMana] = value; }
        public float CurrentMana { get => Basic[BasicStat.CurrentMana]; set => Basic[BasicStat.CurrentMana] = value; }

        public float HealthPercentage => CurrentHealth / MaxHealth;
        public float ManaPercentage => CurrentMana / MaxMana;


        public float[] Basic { get; set; } = BasicStat.FilledArray(1);
        public float[] DamageDealAdds { get; set; } = DamageType.FilledArray(0);
        public float[] DamageDealMults { get; set; } = DamageType.FilledArray(1);
        public float[] DamageTakeAdds { get; set; } = DamageType.FilledArray(0);
        public float[] DamageTakeMults { get; set; } = DamageType.FilledArray(1);
        public float[] StatusEffects { get; set; }

        public Stats GetCopy()
        {
            return new()
            {
                Basic = (float[])Basic.Clone(),
                DamageDealAdds = (float[])DamageDealAdds.Clone(),
                DamageDealMults = (float[])DamageDealMults.Clone(),
                DamageTakeAdds = (float[])DamageTakeAdds.Clone(),
                DamageTakeMults = (float[])DamageTakeMults.Clone(),
            };
        }

        public void Write(NetDataWriter writer)
        {
            for (int i = 0; i < BasicStat.TypeCount; i++)
            {
                writer.Put(Basic[i]);
            }
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                writer.Put(DamageDealAdds[i]);
            }
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                writer.Put(DamageDealMults[i]);
            }
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                writer.Put(DamageTakeAdds[i]);
            }
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                writer.Put(DamageTakeMults[i]);
            }
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                writer.Put(StatusEffects[i]);
            }

        }

        public static Stats FromReader(NetDataReader reader)
        {
            float[] basic = [BasicStat.TypeCount];
            for (int i = 0; i < BasicStat.TypeCount; i++)
            {
                basic[i] = reader.GetFloat();
            }

            float[] damageDealAdds = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageDealAdds[i] = reader.GetFloat();
            }

            float[] damageDealMults = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageDealMults[i] = reader.GetFloat();
            }

            float[] damageTakeAdds = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageTakeAdds[i] = reader.GetFloat();
            }

            float[] damageTakeMults = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageTakeMults[i] = reader.GetFloat();
            }

            float[] statusEffects = [StatusEffect.TypeCount];
            for (int i = 0; i < StatusEffect.TypeCount; i++)
            {
                statusEffects[i] = reader.GetFloat();
            }

            Stats stats = new()
            {
                Basic = basic,
                DamageDealAdds = damageDealAdds,
                DamageDealMults = damageDealMults,
                DamageTakeAdds = damageTakeAdds,
                DamageTakeMults = damageTakeMults,
                StatusEffects = statusEffects,
            };

            return stats;
        }


        public static void CalculateCombatStats(ICombat c)
        {
            if (c == null) return;

            float healthPercentage = c.Stats.CurrentHealth / c.Stats.MaxHealth;
            float manaPercentage = c.Stats.CurrentMana / c.Stats.MaxMana;

            c.Stats = c.BaseStats.GetCopy();

            Stats stats = c.Stats;
            float[] statusEffects = c.Stats.StatusEffects;

            if (statusEffects[StatusEffect.Wet] != 0)
            {
                stats.AttackSpeed += Math.Min(2, 50 / (50 + Math.Max(-49, statusEffects[StatusEffect.Wet])));
            }

            if (statusEffects[StatusEffect.Freezing] != 0)
            {
                float speedMult = 100 / Math.Max(10, (100 + statusEffects[StatusEffect.Freezing]));
            }

            stats.CurrentHealth = stats.MaxHealth * healthPercentage;
            stats.CurrentMana = stats.MaxMana * manaPercentage;
        }
    }

}