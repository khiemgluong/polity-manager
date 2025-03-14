using System;
using UnityEngine;
namespace KL
{
    using static PolityManager;
    [Serializable]
    public class PolityReader
    {
        public PolityStruct polity = new();
        [SerializeField] int polityIndex, classIndex, factionIndex;
        [SerializeField] bool isPolityLeader, isClassLeader, isFactionLeader;
        public System.Collections.Generic.List<PolityMember> parents, partners, children;

        public void SetPolity(PolityReader polityReader)
        {
            polity = polityReader.polity;
        }
        public override bool Equals(object obj)
        {
            if (obj is PolityReader other)
            {
                return string.Equals(polity.polityName, other.polity.polityName) &&
                    string.Equals(polity.className
                        ?? string.Empty, other.polity.className ?? string.Empty) &&
                    string.Equals(polity.factionName
                        ?? string.Empty, other.polity.factionName ?? string.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // return base.GetHashCode();
            return HashCode.Combine(polity.polityName?.ToLowerInvariant(),
                                    polity.className?.ToLowerInvariant(),
                                    polity.factionName?.ToLowerInvariant());
        }
    }
}