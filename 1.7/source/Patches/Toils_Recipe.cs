using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace SK_Inspired_Unique.Patches
{
    [HarmonyPatch]
    public static class Toils_Recipe_FinishRecipeAndStartStoringProduct
    {
        public static MethodBase TargetMethod()
        {
            MethodInfo notifyBillDone = AccessTools.Method(typeof(RecordsUtility), nameof(RecordsUtility.Notify_BillDone));
            MethodInfo factory = AccessTools.Method(typeof(Toils_Recipe), nameof(Toils_Recipe.FinishRecipeAndStartStoringProduct));
            return AccessTools.Lambdas(factory).FirstOrDefault(lambda => PatchProcessor.GetOriginalInstructions(lambda).Any(instruction => instruction.Calls(notifyBillDone)));
        }

        [HarmonyPrefix]
        [HarmonyInfix(typeof(RecordsUtility), nameof(RecordsUtility.Notify_BillDone))]
        public static void Prefix(List<Thing> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                Thing product = products[i];
                if (!CraftedUniques.TryConsume(product) || !ModSettings.showCustomizationMenu)
                {
                    continue;
                }

                Dialog_UniqueWeaponCustomization dialog = new Dialog_UniqueWeaponCustomization(product);
                if (dialog.HasSelectableTraits)
                {
                    Find.WindowStack.Add(dialog);
                }
            }
        }
    }
}
