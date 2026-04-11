using System;
using UnityEngine;
namespace Polities
{
    using static Manager;
    [Serializable]
    public class PolityReader
    {
        [SerializeField] Polity _struct = new();
        public Polity Struct => _struct;
        [SerializeField] int polityIndex, classIndex, factionIndex;
        [SerializeField] bool isPolityLeader, isClassLeader, isFactionLeader;

        public void SetPolity(PolityReader polityReader)
        {
            _struct = polityReader._struct;
            UpdatePolityIndices();
        }

        public void SetPolity(Polity polityStruct)
        {
            _struct = polityStruct;
            UpdatePolityIndices();
        }

        void UpdatePolityIndices()
        {
            if (Singleton == null || Singleton.factions == null) return;

            // Update polityIndex
            polityIndex = Array.FindIndex(Singleton.factions, p => p.name == _struct.name);
            if (polityIndex >= 0 && polityIndex < Singleton.factions.Length)
            {
                // Update classIndex
                // var classes = PM.polities[polityIndex].classes;
                // classIndex = Array.FindIndex(classes, c => c.name == _struct.className) + 1;

                // Update factionIndex
                // if (classIndex > 0 && classIndex - 1 < classes.Length)
                // {
                //     var factions = classes[classIndex - 1].factions;
                //     factionIndex = factions.FindIndex(f => f.name == _struct.factionName) + 1;
                // }
                // else factionIndex = 0;
            }
            else polityIndex = classIndex = factionIndex = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is PolityReader other)
            {
                return string.Equals(_struct.name, other._struct.name) &&
                    string.Equals(_struct.coalitionName
                        ?? string.Empty, other._struct.coalitionName ?? string.Empty) &&
                    string.Equals(_struct.name
                        ?? string.Empty, other._struct.name ?? string.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_struct.name?.ToLowerInvariant(),
                                    _struct.coalitionName?.ToLowerInvariant());
        }
    }
}