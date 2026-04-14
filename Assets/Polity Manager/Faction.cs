using System;
using UnityEngine;

namespace Polity
{
    [Serializable]
    public class Faction : IEquatable<Faction>
    {
        [SerializeField] string name;
        public static event Action<string> OnNameChange;
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value; // ← actually assign it
                if (value != null)
                {
                    Debug.LogError($"Faction name changed to '{value}'"); // ← log new value
                    OnNameChange?.Invoke(value); // ← invoke with new value
                }
            }
        }

        public int RandomFactionIndex()
        {
            if (Manager.PM.factions == null || Manager.PM.factions.Length == 0)
            {
                Debug.LogError("No factions available in Manager. Cannot assign random faction.");
                return -1;
            }
            return UnityEngine.Random.Range(0, Manager.PM.factions.Length);
        }
        bool IsManagedFaction()
            => Array.Exists(Manager.PM.factions, f => ReferenceEquals(f, this));

        public void Set(int factionIndex)
        {
            if (IsManagedFaction())
            {
                Debug.LogError($"Cannot call Set() on a managed faction '{name}'. Use the Manager to modify factions.");
                return;
            }
            Faction[] factions = Manager.PM.factions;
            if (factionIndex < 0 || factionIndex >= factions.Length)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. No faction set.");
                return;
            }
            if (factions[factionIndex].name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"Faction index {factionIndex} has the same name '{name}' as the current faction. No change made.");
                return;
            }
            name = factions[factionIndex].name;
        }

        public void Set(string factionName)
        {
            if (IsManagedFaction())
            {
                Debug.LogError($"Cannot call Set() on a managed faction '{name}'. Use the Manager to modify factions.");
                return;
            }
            Faction[] factions = Manager.PM.factions;
            int factionIndex = Array.FindIndex(factions, f =>
                string.Equals(f.name, factionName, StringComparison.OrdinalIgnoreCase));
            Set(factionIndex);
        }

        public void Set(Faction reader)
        {
            name = reader.name;
        }

        /* --------------------------- Equality Operations -------------------------- */
        public bool Equals(Faction other)
        {
            if (other is null) return false; // ← guard here too
            return string.Equals(name, other.name, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Equals(object obj) => obj is Faction f && Equals(f);
        public override int GetHashCode()
            => name?.ToLowerInvariant().GetHashCode() ?? 0;

        public static bool operator ==(Faction a, Faction b)
        {
            if (a is null) return b is null;
            if (b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Faction a, Faction b) => !(a == b);
        /* ------------------------- End Equality Operations ------------------------ */
    }
}