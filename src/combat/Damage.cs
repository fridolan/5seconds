namespace fiveSeconds
{
    public class Damage
    {
        public int Amount;
        public int ResultingAmount;
        public DamageType Type;

        private static bool debugDamageC = false;
        public static int DamageC(CombatContext combatContext)
        {
            ICombat target = combatContext.Target;
            ICombat source = combatContext.Source;
            Damage damage = combatContext.Damage;
            DamageType type = damage.Type;
            Stats stats = target.Stats;
            int[] status_effects = target.Stats.StatusEffects;

            bool isPhysical = type == DamageType.MELEE || type == DamageType.RANGED;
            bool isMagic = type == DamageType.FIRE || type == DamageType.FROST || type == DamageType.LIGHTNING;
            int resultingAmount = damage.Amount;

            if(debugDamageC) Console.WriteLine($"DamageC: {target} {source}" );
            if(debugDamageC) Console.WriteLine($"Starting Damage {resultingAmount}");

            resultingAmount *= source.Stats.DamageDealMults[type];
            resultingAmount /= 100;
            resultingAmount += source.Stats.DamageDealAdds[type];

            if(debugDamageC) Console.WriteLine($"Damage after mults&adds {resultingAmount}");

            if (isPhysical) // Armor
            {
                int divisor = stats.Armor + 100;
                int dmgPerc = 100 * stats.Armor / divisor;
                resultingAmount *= dmgPerc;
                resultingAmount /= 100;

                if(debugDamageC) Console.WriteLine($"Damage after {stats.Armor} armor {resultingAmount}");
            }

            if (type == DamageType.FIRE && status_effects[StatusEffect.Freezing] != 0)
            { // Extra fire damage on frost
                int ratePerc = 100 / 4;
                int maxBonus = resultingAmount / 2;

                int bonus = Math.Min(status_effects[StatusEffect.Freezing] * ratePerc / 100, maxBonus);

                resultingAmount += bonus;
                status_effects[StatusEffect.Freezing] -= bonus * 100 / ratePerc;

                if(debugDamageC) Console.WriteLine($"Fire Damage after freezing {resultingAmount}");
            }

            if (type == DamageType.FIRE && status_effects[StatusEffect.Wet] != 0)
            { // Lower fire damage on wetness
                int ratePerc = 100 / 4;
                int maxMalus = resultingAmount / 2;

                int malus = Math.Min(status_effects[StatusEffect.Wet] * ratePerc / 100, maxMalus);

                resultingAmount -= malus;
                status_effects[StatusEffect.Wet] -= malus * 100 / ratePerc;

                if(debugDamageC) Console.WriteLine($"Fire Damage after wet {resultingAmount}");
            }

            if (type == DamageType.LIGHTNING && status_effects[StatusEffect.Wet] != 0)
            { // Extra lightning damage on wetness
                int ratePerc = 100 / 3;
                int maxBonus = resultingAmount / 4;

                int bonus = Math.Min(status_effects[StatusEffect.Wet] * ratePerc / 100, maxBonus);

                resultingAmount += bonus;
                status_effects[StatusEffect.Wet] -= bonus * 100 / ratePerc;

                if(debugDamageC) Console.WriteLine($"Lightning Damage after wet {resultingAmount}");
            }

            target.Stats.CurrentHealth -= resultingAmount;
            Console.WriteLine($"Damaged Entity by {resultingAmount} to {target.Stats.CurrentHealth}");

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