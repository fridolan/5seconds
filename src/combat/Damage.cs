namespace fiveSeconds
{
    public class Damage
    {
        public float Amount;
        public float ResultingAmount;
        public DamageType Type;


        public static float DamageC(CombatContext combatContext)
        {
            ICombat target = combatContext.Target;
            ICombat source = combatContext.Source;
            Damage damage = combatContext.Damage;
            DamageType type = damage.Type;
            Stats stats = target.Stats;
            float[] status_effects = target.Stats.StatusEffects;

            bool isPhysical = type == DamageType.MELEE || type == DamageType.RANGED;
            bool isMagic = type == DamageType.FIRE || type == DamageType.FROST || type == DamageType.LIGHTNING;
            float resultingAmount = damage.Amount;

            resultingAmount *= source.Stats.DamageDealMults[type];
            resultingAmount += source.Stats.DamageDealAdds[type];

            if (isPhysical) // Armor
            {
                float armor_mult = 1 - stats.Armor / (stats.Armor + 100);
                resultingAmount *= armor_mult;
            }

            if (type == DamageType.FIRE && status_effects[StatusEffect.Freezing] != 0)
            { // Extra fire damage on frost
                float rate = 1 / 4f;
                float maxBonus = resultingAmount * 0.5f;

                float bonus = Math.Min(status_effects[StatusEffect.Freezing] * rate, maxBonus);

                resultingAmount += bonus;
                status_effects[StatusEffect.Freezing] -= bonus / rate;
            }

            if (type == DamageType.FIRE && status_effects[StatusEffect.Wet] != 0)
            { // Lower fire damage on wetness
                float rate = 1 / 4f;
                float maxMalus = resultingAmount * 0.5f;

                float malus = Math.Min(status_effects[StatusEffect.Wet] * rate, maxMalus);

                resultingAmount -= malus;
                status_effects[StatusEffect.Wet] -= malus / rate;
            }

            if (type == DamageType.LIGHTNING && status_effects[StatusEffect.Wet] != 0)
            { // Extra lightning damage on wetness
                float rate = 1 / 3f;
                float maxBonus = resultingAmount * 0.25f;

                float bonus = Math.Min(status_effects[StatusEffect.Wet] * rate, maxBonus);

                resultingAmount += bonus;
                status_effects[StatusEffect.Wet] -= bonus / rate;
            }

            target.Stats.CurrentHealth -= resultingAmount;

            combatContext.Damage.ResultingAmount = resultingAmount;

            //play_sound(test_audio_buffer);

            if (target.Stats.CurrentHealth <= 0)
            {
                /* entity_on_kill(combat_context);
                entity_on_death(combat_context); */
            }
            //console.log('Damaged entity', target_entity);

            Stats.CalculateCombatStats(combatContext.Source);
            Stats.CalculateCombatStats(combatContext.Target);
            return resultingAmount;
        }
    }
}