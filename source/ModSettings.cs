using Verse;

namespace SK_Inspired_Unique
{
    public class ModSettings : Verse.ModSettings
    {
        public static float craftSuccessChance = 1f;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref craftSuccessChance, "craftSuccessChance", 1f);
        }
    }
}
