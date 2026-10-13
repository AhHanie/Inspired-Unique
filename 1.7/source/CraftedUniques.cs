using System.Runtime.CompilerServices;
using Verse;

namespace SK_Inspired_Unique
{
    public static class CraftedUniques
    {
        private static readonly ConditionalWeakTable<Thing, object> pending = new ConditionalWeakTable<Thing, object>();
        private static readonly object marker = new object();

        public static void Mark(Thing thing)
        {
            pending.AddOrUpdate(thing, marker);
        }

        public static bool IsPending(Thing thing)
        {
            return thing != null && pending.TryGetValue(thing, out _);
        }

        public static bool TryConsume(Thing thing)
        {
            return thing != null && pending.Remove(thing);
        }
    }
}
