using System;
using UnityEngine;
namespace KL
{
    using static PolityManager;
    [Serializable]
    public class PolityReader
    {
        [SerializeField] PolityStruct _struct = new();
        public PolityStruct Struct => _struct;
        [SerializeField] int polityIndex, classIndex, factionIndex;
        [SerializeField] bool isPolityLeader, isClassLeader, isFactionLeader;

        public void SetPolity(PolityReader polityReader)
        {
            _struct = polityReader._struct;
            UpdatePolityIndices();
        }

        public void SetPolity(PolityStruct polityStruct)
        {
            _struct = polityStruct;
            UpdatePolityIndices();
        }

        void UpdatePolityIndices()
        {
            if (PM == null || PM.polities == null) return;

            // Update polityIndex
            polityIndex = Array.FindIndex(PM.polities, p => p.name == _struct.polityName);
            if (polityIndex >= 0 && polityIndex < PM.polities.Length)
            {
                // Update classIndex
                var classes = PM.polities[polityIndex].classes;
                classIndex = Array.FindIndex(classes, c => c.name == _struct.className) + 1;

                // Update factionIndex
                if (classIndex > 0 && classIndex - 1 < classes.Length)
                {
                    var factions = classes[classIndex - 1].factions;
                    factionIndex = factions.FindIndex(f => f.name == _struct.factionName) + 1;
                }
                else factionIndex = 0;
            }
            else polityIndex = classIndex = factionIndex = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is PolityReader other)
            {
                return string.Equals(_struct.polityName, other._struct.polityName) &&
                    string.Equals(_struct.className
                        ?? string.Empty, other._struct.className ?? string.Empty) &&
                    string.Equals(_struct.factionName
                        ?? string.Empty, other._struct.factionName ?? string.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_struct.polityName?.ToLowerInvariant(),
                                    _struct.className?.ToLowerInvariant(),
                                    _struct.factionName?.ToLowerInvariant());
        }
    }
}