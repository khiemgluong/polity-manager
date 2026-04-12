using System;
using UnityEngine;

namespace Polities
{
    [Serializable]
    public struct Polity
    {
        public string name;
        public string faction;
        public string group;

        // public string coalitionName;

        // public override readonly bool Equals(object obj)
        // {
        //     if (obj is Polity other)
        //     {
        //         return
        //             // string.Equals(coalitionName ?? string.Empty, 
        //             //             other.coalitionName ?? string.Empty) &&
        //             string.Equals(name ?? string.Empty, other.name ?? string.Empty);
        //     }
        //     return false;
        // }
        // public override readonly int GetHashCode()
        // {
        //     return HashCode.Combine(coalitionName?.ToLowerInvariant(),
        //                             name?.ToLowerInvariant());
        // }

        // public void SetPolity(PolityReader polityReader)
        // {
        //     // _struct = polityReader._struct;
        //     // UpdatePolityIndices();
        // }

        public void SetPolity(Polity polityStruct)
        {
            // _struct = polityStruct;
            // UpdatePolityIndices();
        }
    }
}