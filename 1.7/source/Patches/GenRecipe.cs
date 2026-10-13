using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace SK_Inspired_Unique.Patches
{
    [HarmonyPatch]
    public static class GenRecipe_MakeRecipeProducts
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.StateMachineMoveNext(AccessTools.Method(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts)));
        }

        [HarmonyPrefix]
        [HarmonyInfix(typeof(ThingMaker), nameof(ThingMaker.MakeThing))]
        public static void Prefix(ref ThingDef def, ref ThingDef stuff, [HarmonyOuter][HarmonyArgument("worker", ArgumentMode.Captured)] Pawn crafter, out bool __state)
        {
            __state = Utils.TryGetInspiredUnique(crafter, def, ref stuff, out ThingDef uniqueDef);
            if (__state)
            {
                def = uniqueDef;
            }
        }

        [HarmonyPostfix]
        [HarmonyInfix(typeof(ThingMaker), nameof(ThingMaker.MakeThing))]
        public static Thing Postfix(Thing __result, bool __state)
        {
            if (__state && __result != null)
            {
                CraftedUniques.Mark(__result);
            }
            return __result;
        }
    }

    [HarmonyPatch(typeof(GenRecipe), "PostProcessProduct")]
    public static class GenRecipe_PostProcessProduct
    {
        [HarmonyPrefix]
        [HarmonyInfix(typeof(CompQuality), nameof(CompQuality.SetQuality))]
        public static bool Prefix(CompQuality __instance)
        {
            return !CraftedUniques.IsPending(__instance.parent);
        }
    }
}
