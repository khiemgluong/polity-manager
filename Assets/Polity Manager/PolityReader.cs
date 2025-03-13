using System;
using UnityEngine;
namespace KL
{
    using static PolityManager;
    [Serializable]
    public class PolityReader
    {
        public PolityStruct Struct = new();
        [SerializeField] int polityIndex, classIndex, factionIndex;
        [SerializeField] bool isPolityLeader, isClassLeader, isFactionLeader;
        public System.Collections.Generic.List<PolityMember> parents, partners, children;

        public void SetPolity(PolityReader _polityReader)
        {
            Struct = _polityReader.Struct;
        }
        public override bool Equals(object obj)
        {
            if (obj is PolityReader other)
            {
                return string.Equals(Struct.polityName, other.Struct.polityName) &&
                    string.Equals(Struct.className
                        ?? string.Empty, other.Struct.className ?? string.Empty) &&
                    string.Equals(Struct.factionName
                        ?? string.Empty, other.Struct.factionName ?? string.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // return base.GetHashCode();
            return HashCode.Combine(Struct.polityName?.ToLowerInvariant(),
                                    Struct.className?.ToLowerInvariant(),
                                    Struct.factionName?.ToLowerInvariant());
        }
    }
}