using System;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Reader reader;

        void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            OnFactionChange += OnFactionChanged;
        }
        protected virtual void OnDisable()
        {
            OnFactionChange -= OnFactionChanged;
        }

        // public void ChangeFaction(int factionIndex)
        // {
        //     if (factionIndex < 0 || factionIndex >= Manager.Singleton.factions.Length)
        //     {
        //         Debug.LogError($"Invalid faction index: {factionIndex}. No faction change applied.");
        //         return;
        //     }
        //     reader.Set(factionIndex);
        //     Manager.OnFactionChange?.Invoke();
        // }

        // public void ChangeFaction(string newFactionName)
        // {
        //     int factionIndex = Array.FindIndex(Manager.Singleton.factions, 
        //         f => string.Equals(f.name, newFactionName, StringComparison.OrdinalIgnoreCase));
        //     if (factionIndex == -1)
        //     {
        //         Debug.LogError($"Faction '{newFactionName}' not found. No faction change applied.");
        //         return;
        //     }
        //     reader.Set(factionIndex);
        //     Manager.OnFactionChange?.Invoke();
        // }

        /* --------------------------------- EVENTS --------------------------------- */

        void OnFactionChanged()
        {

        }
    }
}