using Verse;
using UnityEngine;

namespace SK_Inspired_Unique
{
    public static class ModSettingsWindow
    {
        public static void Draw(Rect parent)
        {
            Rect innerRect = new Rect(parent);
            innerRect.x += 5;
            innerRect.width -= 35;
            innerRect.y += 10;
            innerRect.height -= 20;
            Listing_Standard list = new Listing_Standard(GameFont.Small);
            Widgets.DrawMenuSection(parent);
            list.Begin(innerRect);
            list.Label("InspiredUnique.Settings.CraftChance.Label".Translate());
            ModSettings.craftSuccessChance = Widgets.HorizontalSlider(list.GetRect(22f), ModSettings.craftSuccessChance, 0, 1, false, ModSettings.craftSuccessChance.ToStringPercent(), null, null, 0.01f);
            list.End();
        }
    }
}
