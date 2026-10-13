using RimWorld;
using Verse;

namespace SK_Inspired_Unique
{
    public class Utils
    {
        private const int RequiredCraftingLevel = 20;

        public static ThingDef GetUniqueVariant(ThingDef wep)
        {
            if (wep.IsWeapon == true)
            {
                DefModExtension_UniqueVariant extension = wep.GetModExtension<DefModExtension_UniqueVariant>();
                if (extension?.uniqueWeapon != null)
                {
                    return extension.uniqueWeapon;
                }
            }
            return null;
        }

        public static bool MeetsCraftingRequirement(Pawn pawn)
        {
            if (!ModSettings.requireLevel20CrafterForInspiredUniques)
            {
                return true;
            }

            SkillRecord craftingSkill = pawn.skills?.GetSkill(SkillDefOf.Crafting);
            return craftingSkill != null && craftingSkill.Level >= RequiredCraftingLevel;
        }

        public static bool TryGetInspiredUnique(Pawn crafter, ThingDef def, ref ThingDef stuff, out ThingDef uniqueDef)
        {
            uniqueDef = null;
            if (crafter == null || crafter.InspirationDef != InspirationDefOf.Inspired_Creativity)
            {
                return false;
            }

            ThingDef variant = GetUniqueVariant(def);
            if (variant == null || (variant.MadeFromStuff && stuff == null))
            {
                return false;
            }

            if (!MeetsCraftingRequirement(crafter) || !Rand.Chance(ModSettings.craftSuccessChance))
            {
                return false;
            }

            if (!variant.MadeFromStuff)
            {
                stuff = null;
            }
            uniqueDef = variant;
            return true;
        }
    }
}
