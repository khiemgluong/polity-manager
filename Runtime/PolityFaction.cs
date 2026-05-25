using System;
using UnityEngine;

namespace Polity
{
    [Serializable]
    public class Faction : IEquatable<Faction>
    {
        [SerializeField] string name;
        [SerializeField] int hash;
        public int Hash => hash;

        public static event Action<string> OnNameChange;

        public string Name
        {
            get => name;
            set
            {
                // ReferenceEquals(name, value) will be false if Awake uses: Name = new(Name.Trim())
                // This forces a recalculation even if the name content is the same.
                bool isForced = !ReferenceEquals(name, value);
                bool isNameDifferent = name != value;

                name = value;

                if (value != null && (isNameDifferent || hash == 0 || isForced))
                {
                    UpdateHash();
                    OnNameChange?.Invoke(value);
                }
            }
        }

        bool IsManagedFaction()
            => Manager.PM.factions.Exists(f => ReferenceEquals(f, this));

        void UpdateHash()
        {
            if (string.IsNullOrEmpty(name))
            { hash = 0; return; }

            hash = GetHashCode();

            Debug.Log($"[Faction ID:{GetHashCode()}] '{name}' hash updated to {hash}");
        }

        public void Set(int factionIndex)
        {
            if (IsManagedFaction())
            {
                Debug.LogError($"Cannot call Set() on a managed faction '{name}'.");
                return;
            }
            var factions = Manager.PM.factions;
            if (factionIndex < 0 || factionIndex >= factions.Count)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. No faction set.");
                return;
            }
            if (factions[factionIndex].name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return;

            Name = factions[factionIndex].name;
        }

        public void Set(string factionName)
        {
            if (IsManagedFaction())
            {
                Debug.LogError($"Cannot call Set() on a managed faction '{name}'.");
                return;
            }
            var factions = Manager.PM.factions;
            int factionIndex = factions.FindIndex(f =>
                string.Equals(f.name, factionName, StringComparison.OrdinalIgnoreCase));
            Set(factionIndex);
        }

        public void Set(Faction reader)
        {
            Name = reader.name;
            UpdateHash();
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