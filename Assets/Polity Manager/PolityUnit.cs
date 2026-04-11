using System.Collections.Generic;

using UnityEngine;

namespace Polities
{
    using System.ComponentModel;
    using Polities;

    [System.Serializable]
    public class Unit
    {
        [ReadOnly(true)]
        public Member leader;
        public List<Member> members = new List<Member>();
    }
}