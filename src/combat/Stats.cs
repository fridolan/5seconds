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

        public static int[] FilledArray(int value)
        {
            int[] array = new int[TypeCount];
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }
    }

    public class Stats
    {
        public int MaxHealth { get => Basic[BasicStat.MaxHealth]; set => Basic[BasicStat.MaxHealth] = value; }
        public int CurrentHealth { get => Basic[BasicStat.CurrentHealth]; set => Basic[BasicStat.CurrentHealth] = value; }
        public int AttackSpeed { get => Basic[BasicStat.AttackSpeed]; set => Basic[BasicStat.AttackSpeed] = value; }
        public int MovementSpeed { get => Basic[BasicStat.MovementSpeed]; set => Basic[BasicStat.MovementSpeed] = value; }
        public int Armor { get => Basic[BasicStat.Armor]; set => Basic[BasicStat.Armor] = value; }
        public int MaxMana { get => Basic[BasicStat.MaxMana]; set => Basic[BasicStat.MaxMana] = value; }
        public int CurrentMana { get => Basic[BasicStat.CurrentMana]; set => Basic[BasicStat.CurrentMana] = value; }

        public float HealthPercentage => CurrentHealth / MaxHealth;
        public float ManaPercentage => CurrentMana / MaxMana;


        public int[] Basic { get; set; } = BasicStat.FilledArray(100);
        public int[] DamageDealAdds { get; set; } = DamageType.FilledArray(0);
        public int[] DamageDealMults { get; set; } = DamageType.FilledArray(100);
        public int[] DamageTakeAdds { get; set; } = DamageType.FilledArray(0);
        public int[] DamageTakeMults { get; set; } = DamageType.FilledArray(100);
        public int[] StatusEffects { get; set; } = StatusEffect.FilledArray(0);

        public Stats GetCopy()
        {
            return new()
            {
                Basic = (int[])Basic.Clone(),
                DamageDealAdds = (int[])DamageDealAdds.Clone(),
                DamageDealMults = (int[])DamageDealMults.Clone(),
                DamageTakeAdds = (int[])DamageTakeAdds.Clone(),
                DamageTakeMults = (int[])DamageTakeMults.Clone(),
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
            int[] basic = [BasicStat.TypeCount];
            for (int i = 0; i < BasicStat.TypeCount; i++)
            {
                basic[i] = reader.GetInt();
            }

            int[] damageDealAdds = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageDealAdds[i] = reader.GetInt();
            }

            int[] damageDealMults = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageDealMults[i] = reader.GetInt();
            }

            int[] damageTakeAdds = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageTakeAdds[i] = reader.GetInt();
            }

            int[] damageTakeMults = [DamageType.TypeCount];
            for (int i = 0; i < DamageType.TypeCount; i++)
            {
                damageTakeMults[i] = reader.GetInt();
            }

            int[] statusEffects = [StatusEffect.TypeCount];
            for (int i = 0; i < StatusEffect.TypeCount; i++)
            {
                statusEffects[i] = reader.GetInt();
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

            int healthPercent = 100 * c.Stats.CurrentHealth / c.Stats.MaxHealth;
            int manaPercent = 100 * c.Stats.CurrentMana / c.Stats.MaxMana;

            c.Stats = c.BaseStats.GetCopy();

            Stats stats = c.Stats;
            int[] statusEffects = c.Stats.StatusEffects;

            if (statusEffects[StatusEffect.Wet] != 0)
            {
                stats.AttackSpeed *= Math.Min(200, 100 * 5000 / (5000 + Math.Max(-4900, statusEffects[StatusEffect.Wet])));
                stats.AttackSpeed /= 100;
            }

            if (statusEffects[StatusEffect.Freezing] != 0)
            {
                int speedMult = 1000000 / Math.Max(1000, (10000 + statusEffects[StatusEffect.Freezing]));
                stats.MovementSpeed *= speedMult;
                stats.MovementSpeed /= 10000;
            }

            stats.CurrentHealth = stats.MaxHealth * healthPercent / 100;
            stats.CurrentMana = stats.MaxMana * manaPercent / 100;
        }
    }

}