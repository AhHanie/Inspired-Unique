using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace SK_Inspired_Unique
{
    public static class UniqueWeaponAccess
    {
        private static readonly FieldInfo nameField = AccessTools.Field(typeof(CompUniqueWeapon), "name");
        private static readonly FieldInfo colorField = AccessTools.Field(typeof(CompUniqueWeapon), "color");
        private static readonly FieldInfo ignoreAccuracyField = AccessTools.Field(typeof(CompUniqueWeapon), "ignoreAccuracyMaluses");
        private static readonly FieldInfo abilityField = AccessTools.Field(typeof(CompEquippableAbility), "ability");

        public static void SetName(CompUniqueWeapon comp, string name)
        {
            if (nameField == null)
            {
                Log.ErrorOnce("[Inspired Unique] CompUniqueWeapon.name was not found; the custom weapon name cannot be stored.", 1951330171);
                return;
            }
            nameField.SetValue(comp, name);
        }

        public static void ReplaceTraits(CompUniqueWeapon comp, List<WeaponTraitDef> traits)
        {
            comp.TraitsListForReading.Clear();
            foreach (WeaponTraitDef trait in traits)
            {
                comp.AddTrait(trait);
            }

            ignoreAccuracyField?.SetValue(comp, null);

            WeaponTraitDef forcedColorTrait = traits.LastOrDefault(t => t.forcedColor != null);
            if (forcedColorTrait != null && colorField != null)
            {
                colorField.SetValue(comp, forcedColorTrait.forcedColor);
            }

            ResetAbility(comp);
            comp.Setup(fromSave: false);
        }

        private static void ResetAbility(CompUniqueWeapon comp)
        {
            CompEquippableAbilityReloadable abilityComp = comp.parent.TryGetComp<CompEquippableAbilityReloadable>();
            if (abilityComp == null)
            {
                return;
            }

            CompProperties_EquippableAbilityReloadable baseProps = comp.parent.def.comps.OfType<CompProperties_EquippableAbilityReloadable>().FirstOrDefault();
            if (baseProps == null || abilityField == null)
            {
                Log.ErrorOnce("[Inspired Unique] Could not reset the unique weapon ability state; ability traits may not match the customized selection.", 1951330172);
                return;
            }

            abilityComp.props = baseProps;
            abilityField.SetValue(abilityComp, null);
        }
    }
}
