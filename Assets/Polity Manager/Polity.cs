using System;
using UnityEngine;

namespace Polity
{
    [Serializable]
    public struct Faction : IEquatable<Faction>
    {
        public string name;

        public readonly bool Equals(Faction other)
        {
            return string.Equals(name, other.name, StringComparison.OrdinalIgnoreCase);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Faction other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(name?.ToLowerInvariant());
        }

        public void Set(PolityReader polityReader)
        {
            // _struct = polityReader._struct;
            // UpdatePolityIndices();
        }

        public void Set(Faction polityStruct)
        {
            // _struct = polityStruct;
            // UpdatePolityIndices();
        }
    }
}