using System;
using UnityEngine;

namespace Polity
{
  
    [Serializable]
    public struct Faction : IEquatable<Faction>
    {
        public string name;
        public string group;

        public readonly bool Equals(Faction faction)
        {
            return string.Equals(name, faction.name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(group, faction.group, StringComparison.OrdinalIgnoreCase);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Faction other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(name?.ToLowerInvariant(), group?.ToLowerInvariant());
        }


        public void Set(Faction polityStruct)
        {
            // _struct = polityStruct;
            // UpdatePolityIndices();
        }
    }
}