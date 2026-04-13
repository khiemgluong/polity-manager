using System;
using UnityEngine;

namespace Polity
{
    [Serializable]
    public class Reader : IEquatable<Reader>
    {
        public string faction;
        public string group;
        public bool Equals(Reader other)
        {
            return string.Equals(faction, other.faction, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(group, other.group, StringComparison.OrdinalIgnoreCase);
        }

        public int RandomFactionIndex()
        {
            if (Manager.Singleton.factions == null || Manager.Singleton.factions.Length == 0)
            {
                Debug.LogError("No factions available in Manager. Cannot assign random faction.");
                return -1;
            }
            return UnityEngine.Random.Range(0, Manager.Singleton.factions.Length);
        }

        public int RandomGroupIndex()
        {
            if (Manager.Singleton.factions == null || Manager.Singleton.factions.Length == 0)
            {
                Debug.LogError("No factions available in Manager. Cannot assign random group.");
                return -1;
            }
            int factionIndex = Array.FindIndex(Manager.Singleton.factions, f =>
                string.Equals(f.name, faction, StringComparison.OrdinalIgnoreCase));
            if (factionIndex < 0 || factionIndex >= Manager.Singleton.factions.Length)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. Cannot assign random group.");
                return -1;
            }
            var groups = Manager.Singleton.factions[factionIndex].groups;
            if (groups == null || groups.Count == 0)
                return 0; // No groups, return default index
            return UnityEngine.Random.Range(-1, groups.Count);
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
            if (groupIndex < 0)
            {
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