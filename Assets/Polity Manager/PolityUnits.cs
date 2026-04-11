using System.Collections.Generic;

using UnityEngine;

namespace Polities
{
    using System.ComponentModel;
    using Polities;
    using static Polities.Manager;
    [System.Serializable]
    public struct PolityUnit
    {
        public Polity polity;
        public Member.Unit unit;
    }

    [System.Serializable]
    public struct Unit
    {
        public string name;
        public Member leader;
        public List<Member> members;
    }
}