using HarmonyLib;
using RimWorld;
using System;
using Verse.AI;
using Verse;

namespace SK_Inspired_Unique.Patches
{
    public static class Toils_RecipePatches
    {
        public static bool FINISHING_PRODUCT_FLAG = false;
        public static Thing uniqueWeapon = null;
        public static ThingDef uniqueWeaponDef = null;
        [HarmonyPatch(typeof(Toils_Recipe), "FinishRecipeAndStartStoringProduct")]
        public static class FinishRecipeAndStartStoringProduct
        {
            public static void Postfix(ref Toil __result)
            {
                Toil originalToil = __result;
                Action originalInitAction = __result.initAction;
                __result.initAction = delegate
                {
                    if (originalToil.actor.InspirationDef != InspirationDefOf.Inspired_Creativity || !Rand.Chance(ModSettings.craftSuccessChance)) 
                    {
                        originalInitAction();
                        return;
                    }
                    FINISHING_PRODUCT_FLAG = true;
                    originalInitAction();
                    FINISHING_PRODUCT_FLAG = false;

                    if (uniqueWeapon != null && ModSettings.showCustomizationMenu)
                    {
                        CompUniqueWeapon comp = uniqueWeapon.TryGetComp<CompUniqueWeapon>();
                        comp.TraitsListForReading.Clear();
                        Find.WindowStack.Add(new Dialog_UniqueWeaponCustomization(uniqueWeapon));
                        uniqueWeapon = null;
                        uniqueWeaponDef = null;
                    }
                };
            }
        }
    }
}
