using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SK_Inspired_Unique
{
    public class Mod: Verse.Mod
    {
        public static Harmony instance;
        public Mod(ModContentPack content)
           : base(content)
        {
            instance = new Harmony("rimworld.sk.inspiredunique");
            LongEventHandler.QueueLongEvent(Init, "Inspired Unique Init", doAsynchronously: true, null);
        }

        public void Init()
        {
            InitializeUniqueVariants();
            instance.PatchAll();
        }

        private void InitializeUniqueVariants()
        {
            List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                ThingDef thingDef = thingDefs[i];
                if (thingDef == null || !thingDef.IsWeapon)
                {
                    continue;
                }

                string uniqueDefName = thingDef.defName + "_Unique";
                ThingDef uniqueDef = DefDatabase<ThingDef>.GetNamedSilentFail(uniqueDefName);
                if (uniqueDef == null || !uniqueDef.HasComp<CompUniqueWeapon>())
                {
                    continue;
                }

                DefModExtension_UniqueVariant extension = thingDef.GetModExtension<DefModExtension_UniqueVariant>();
                if (extension == null)
                {
                    extension = new DefModExtension_UniqueVariant();
                    if (thingDef.modExtensions == null)
                    {
                        thingDef.modExtensions = new List<DefModExtension>();
                    }
                    thingDef.modExtensions.Add(extension);
                }

                extension.uniqueWeapon = uniqueDef;
            }
        }
    }
}
