using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SK_Inspired_Unique
{
    public class Dialog_UniqueWeaponCustomization : Window
    {
        private Thing weapon;
        private CompUniqueWeapon compUniqueWeapon;
        private QualityCategory quality;
        private string customName;
        private List<WeaponTraitDef> availableTraits = new List<WeaponTraitDef>();
        private List<WeaponTraitDef> selectedTraits = new List<WeaponTraitDef>();
        private int maxTraitSelections;
        private Vector2 scrollPosition;
        private float scrollHeight = 999f;
        private bool wasPaused;

        private const float WindowWidth = 600f;
        private const float WindowHeight = 700f;
        private const float WeaponIconSize = 64f;

        public override Vector2 InitialSize => new Vector2(WindowWidth, WindowHeight);

        public Dialog_UniqueWeaponCustomization(Thing weapon)
        {
            this.weapon = weapon;
            this.compUniqueWeapon = weapon.TryGetComp<CompUniqueWeapon>();
            this.customName = compUniqueWeapon.TransformLabel(weapon.def.label);

            CompQuality compQuality = weapon.TryGetComp<CompQuality>();
            this.quality = compQuality?.Quality ?? QualityCategory.Normal;

            this.maxTraitSelections = GetMaxTraitSelections(quality);

            GenerateAvailableTraits();

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = false;

            wasPaused = Find.TickManager.Paused;
            if (!wasPaused)
            {
                Find.TickManager.TogglePaused();
            }
        }

        private int GetMaxTraitSelections(QualityCategory quality)
        {
            switch (quality)
            {
                case QualityCategory.Awful:
                case QualityCategory.Poor:
                case QualityCategory.Normal:
                case QualityCategory.Good:
                    return 1;
                case QualityCategory.Excellent:
                case QualityCategory.Masterwork:
                    return 2;
                case QualityCategory.Legendary:
                    return 3;
                default:
                    return 1;
            }
        }

        private void GenerateAvailableTraits()
        {
            availableTraits.Clear();

            CompProperties_UniqueWeapon props = compUniqueWeapon.Props;

            List<WeaponTraitDef> allTraits = DefDatabase<WeaponTraitDef>.AllDefsListForReading
                .Where(t => props.weaponCategories.Contains(t.weaponCategory))
                .ToList();

            allTraits.Shuffle();

            foreach (WeaponTraitDef trait in allTraits)
            {
                if (availableTraits.Count >= 6)
                    break;

                bool conflicts = false;
                foreach (WeaponTraitDef existingTrait in availableTraits)
                {
                    if (trait.Overlaps(existingTrait))
                    {
                        conflicts = true;
                        break;
                    }
                }

                if (!conflicts)
                {
                    availableTraits.Add(trait);
                }
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(0f, 0f, inRect.width, 35f);
            Widgets.Label(titleRect, "InspiredUnique.WeaponCustomizationWindow.Title".Translate());

            Text.Font = GameFont.Small;
            float curY = titleRect.yMax + 10f;

            // Weapon icon and name
            Rect iconRect = new Rect(inRect.x, curY, WeaponIconSize, WeaponIconSize);
            Widgets.ThingIcon(iconRect, weapon);

            Rect nameRect = new Rect(iconRect.xMax + 10f, curY, inRect.width - WeaponIconSize - 10f, 30f);
            Widgets.Label(nameRect, "InspiredUnique.WeaponCustomizationWindow.Label.WeaponName".Translate());

            Rect nameFieldRect = new Rect(nameRect.x, nameRect.yMax + 2f, nameRect.width, 30f);
            customName = Widgets.TextField(nameFieldRect, customName);

            curY = iconRect.yMax + 10f;

            Rect qualityRect = new Rect(0f, curY, inRect.width, 30f);
            string qualityText = "InspiredUnique.WeaponCustomizationWindow.Label.SelectTraits".Translate(quality.GetLabel().CapitalizeFirst(), maxTraitSelections);
            Widgets.Label(qualityRect, qualityText);
            curY = qualityRect.yMax + 10f;

            Rect outRect = new Rect(0f, curY, inRect.width, inRect.height - curY - 50f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, Mathf.Max(scrollHeight, outRect.height));

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            foreach (WeaponTraitDef trait in availableTraits)
            {
                bool isSelected = selectedTraits.Contains(trait);
                bool wasSelected = isSelected;

                Rect traitRect = listing.GetRect(30f);

                Rect checkRect = new Rect(traitRect.x, traitRect.y, 24f, 24f);
                Widgets.Checkbox(checkRect.x, checkRect.y, ref isSelected, 24f, disabled: false, paintable: false);

                Rect labelRect = new Rect(checkRect.xMax + 5f, traitRect.y, traitRect.width - checkRect.width - 5f, traitRect.height);
                Widgets.Label(labelRect, trait.LabelCap);

                if (Mouse.IsOver(traitRect))
                {
                    Widgets.DrawHighlight(traitRect);
                    TooltipHandler.TipRegion(traitRect, trait.description);
                }

                if (isSelected != wasSelected)
                {
                    if (isSelected)
                    {
                        if (selectedTraits.Count < maxTraitSelections)
                        {
                            selectedTraits.Add(trait);
                        }
                    }
                    else
                    {
                        selectedTraits.Remove(trait);
                    }
                }

                listing.Gap(2f);
            }

            listing.End();

            if (Event.current.type == EventType.Layout)
            {
                scrollHeight = listing.CurHeight;
            }

            Widgets.EndScrollView();

            Rect bottomRect = new Rect(0f, inRect.height - 40f, inRect.width, 40f);

            Rect confirmRect = new Rect(bottomRect.xMax - 120f, bottomRect.y, 120f, 35f);

            if (selectedTraits.Count != maxTraitSelections)
            {
                GUI.color = Color.gray;
            }

            if (Widgets.ButtonText(confirmRect, "InspiredUnique.WeaponCustomizationWindow.Button.Confirm".Translate()))
            {
                if (selectedTraits.Count == maxTraitSelections)
                {
                    ApplyCustomization();
                    Close(false);
                }
                else
                {
                    Messages.Message("InspiredUnique.WeaponCustomizationWindow.Messages.InvalidTraitNumber".Translate(maxTraitSelections), MessageTypeDefOf.RejectInput, historical: false);
                }
            }

            GUI.color = Color.white;

            if (selectedTraits.Count != maxTraitSelections)
            {
                Rect countRect = new Rect(0f, bottomRect.y + 5f, bottomRect.width - 250f, 30f);
                string countText = "InspiredUnique.WeaponCustomizationWindow.Label.SelectTraitsCounter".Translate(selectedTraits.Count, maxTraitSelections);
                Widgets.Label(countRect, countText);
            }
        }

        private void ApplyCustomization()
        {
            if (compUniqueWeapon == null)
                return;

            foreach (WeaponTraitDef trait in selectedTraits)
            {
                compUniqueWeapon.AddTrait(trait);
            }

            compUniqueWeapon.Setup(fromSave: false);

            if (!customName.NullOrEmpty())
            {
                typeof(CompUniqueWeapon)
                    .GetField("name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(compUniqueWeapon, customName);

                CompArt compArt = weapon.TryGetComp<CompArt>();
                if (compArt != null)
                {
                    compArt.Title = customName;
                }
            }

            Messages.Message("InspiredUnique.WeaponCustomizationWindow.Messages.SuccessMessage".Translate(customName), MessageTypeDefOf.PositiveEvent, historical: false);
        }

        public override void Close(bool doCloseSound = true)
        {
            if (!wasPaused && Find.TickManager.Paused)
            {
                Find.TickManager.TogglePaused();
            }

            base.Close(doCloseSound);
        }
    }
}