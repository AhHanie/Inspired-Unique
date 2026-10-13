using Verse;

namespace SK_Inspired_Unique
{
    public class Utils
    {
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
    }
}
