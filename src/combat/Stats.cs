namespace fiveSeconds
{
    public class Stats
    {
        public float MaxHealth;
        public float CurrentHealth;
        public float AttackSpeed;
        public float MovementSpeed;
        public float Armor;
        public float MaxMana;
        public float CurrentMana;

        public float HealthPercentage => CurrentHealth / MaxHealth;
        public float ManaPercentage => CurrentMana / MaxMana;

        public Dictionary<DamageType, float> DamageDealAdds { get; set; } = new()
        {
            { DamageType.FIRE, 0f },
            { DamageType.FROST, 0f },
            { DamageType.LIGHTNING, 0f },
            { DamageType.POISON, 0f },
            { DamageType.MELEE, 0f },
            { DamageType.RANGED, 0f },
            { DamageType.HEAL, 0f },
        };
        public Dictionary<DamageType, float> DamageDealMults { get; set; } = new()
        {
            { DamageType.FIRE, 1f },
            { DamageType.FROST, 1f },
            { DamageType.LIGHTNING, 1f },
            { DamageType.POISON, 1f },
            { DamageType.MELEE, 1f },
            { DamageType.RANGED, 1f },
            { DamageType.HEAL, 1f },
        };
        public Dictionary<DamageType, float> DamageTakeAdds { get; set; } = new()
        {
            { DamageType.FIRE, 0f },
            { DamageType.FROST, 0f },
            { DamageType.LIGHTNING, 0f },
            { DamageType.POISON, 0f },
            { DamageType.MELEE, 0f },
            { DamageType.RANGED, 0f },
            { DamageType.HEAL, 0f },
        };
        public Dictionary<DamageType, float> DamageTakeMults { get; set; } = new()
        {
            { DamageType.FIRE, 1f },
            { DamageType.FROST, 1f },
            { DamageType.LIGHTNING, 1f },
            { DamageType.POISON, 1f },
            { DamageType.MELEE, 1f },
            { DamageType.RANGED, 1f },
            { DamageType.HEAL, 1f },
        };
        public StatusEffects StatusEffects { get; set; }


        public Stats GetCopy()
        {
            return new()
            {
                MaxHealth = MaxHealth,
                CurrentHealth = CurrentHealth,
                AttackSpeed = AttackSpeed,
                MovementSpeed = MovementSpeed,
                Armor = Armor,
                MaxMana = MaxMana,
                CurrentMana = CurrentMana,
                DamageDealAdds = new(DamageDealAdds),
                DamageDealMults = new(DamageDealMults),
                DamageTakeAdds = new(DamageTakeAdds),
                DamageTakeMults = new(DamageTakeMults),
            };
        }


        public static void CalculateCombatStats(ICombat c)
        {
            if (c == null) return;

            float healthPercentage = c.Stats.CurrentHealth / c.Stats.MaxHealth;
            float manaPercentage = c.Stats.CurrentMana / c.Stats.MaxMana;

            c.Stats = c.BaseStats.GetCopy();

            Stats stats = c.Stats;
            StatusEffects statusEffects = c.Stats.StatusEffects;

            if (statusEffects.Wet != 0)
            {
                stats.AttackSpeed += Math.Min(2, 50 / (50 + Math.Max(-49, statusEffects.Wet)));
            }

            if (statusEffects.Freezing != 0)
            {
                float speedMult = 100 / Math.Max(10, (100 + statusEffects.Freezing));
            }

            stats.CurrentHealth = stats.MaxHealth * healthPercentage;
            stats.CurrentMana = stats.MaxMana * manaPercentage;
        }
    }

}