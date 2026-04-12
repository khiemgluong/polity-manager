using System;
using UnityEngine;

namespace Polity
{
    [Serializable]
    public struct Reader : IEquatable<Reader>
    {
        public string faction;
        public string group;

        public readonly bool Equals(Reader reader)
        {
            return string.Equals(faction, reader.faction, StringComparison.OrdinalIgnoreCase)
                && string.Equals(group, reader.group, StringComparison.OrdinalIgnoreCase);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Reader other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(faction?.ToLowerInvariant(), group?.ToLowerInvariant());
        }

        public void Set(int factionIndex, int groupIndex = -1)
        {
            Manager.Faction[] factions = Manager.Singleton.factions;
            if (factionIndex < 0 || factionIndex >= factions.Length)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. No faction set.");
                return;
            }
            faction = factions[factionIndex].name;

            if (groupIndex < 0)
            {
                group = null;
                return;
            }

            if (factions[factionIndex].groups == null || groupIndex >= factions[factionIndex].groups.Count)
            {
                Debug.LogError($"Invalid group index: {groupIndex} for faction '{faction}'. No group set.");
                group = null;
                return;
            }
            group = factions[factionIndex].groups[groupIndex].name;
        }

        public void Set(string factionName, string groupName = null)
        {
            Manager.Faction[] factions = Manager.Singleton.factions;
            int factionIndex = Array.FindIndex(factions, f =>
                string.Equals(f.name, factionName, StringComparison.OrdinalIgnoreCase));
            if (factionIndex == -1)
            {
                Debug.LogError($"Faction '{factionName}' not found. No faction set.");
                return;
            }
            faction = factions[factionIndex].name;

            if (string.IsNullOrEmpty(groupName)) { group = null; return; }
            if (factions[factionIndex].groups == null)
            {
                Debug.LogError($"Faction '{faction}' has no groups. No group set.");
                group = null;
                return;
            }
            int groupIndex = factions[factionIndex].groups.FindIndex(g =>
                string.Equals(g.name, groupName, StringComparison.OrdinalIgnoreCase));
            if (groupIndex == -1)
            {
                Debug.LogError($"Group '{groupName}' not found in faction '{faction}'. No group set.");
                group = null;
                return;
            }
            group = factions[factionIndex].groups[groupIndex].name;
        }

        public void Set(Reader reader)
        {
            faction = reader.faction;
            group = reader.group;
        }
    }
}