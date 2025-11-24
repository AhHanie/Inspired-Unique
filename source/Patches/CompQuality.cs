using HarmonyLib;
using RimWorld;
using Verse;

namespace SK_Inspired_Unique.Patches
{
    // Unique weapons generate their own quality
    public static class CompQualityPatches
    {
        [HarmonyPatch(typeof(CompQuality), "SetQuality")]
        public static class SetQuality
        {
            public static bool Prefix(CompQuality __instance)
            {
                if (!Toils_RecipePatches.FINISHING_PRODUCT_FLAG)
                {
                    return true;
                }

                ThingDef uniqueVariant = Utils.GetUniqueVariant(__instance.parent.def);
                if (uniqueVariant == null)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
