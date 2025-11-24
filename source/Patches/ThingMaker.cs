using HarmonyLib;
using Verse;

namespace SK_Inspired_Unique.Patches
{
    public static class ThingMakerPatches
    {
        [HarmonyPatch(typeof(ThingMaker), "MakeThing")]
        public static class MakeThing
        {
            public static void Prefix(ref ThingDef def)
            {
                if (!Toils_RecipePatches.FINISHING_PRODUCT_FLAG)
                {
                    return;
                }
                ThingDef uniqueVariant = Utils.GetUniqueVariant(def);
                if (uniqueVariant != null)
                {
                    def = uniqueVariant;
                }
            }
        }
    }
}
