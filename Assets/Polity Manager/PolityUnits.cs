using System.Collections.Generic;

using UnityEngine;

namespace Polities
{
    using System.ComponentModel;
    using Polities;
    using static Polities.Manager;
    [System.Serializable]
    public class PolityUnits
    {
        public List<Unit> units = new List<Unit>();
    }

    [System.Serializable]
    public class Unit
    {
        public Member leader;
        public List<Member> members = new List<Member>();
    }
}