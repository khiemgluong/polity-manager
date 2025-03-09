using System;
using System.Collections.Generic;
using UnityEngine;
namespace KL
{
    public class PolityFamily : ScriptableObject
    {
        public FamilyStruct family = new();

        /* -------------------------------------------------------------------------- */
        /*                                FAMILYSTRUCT                                */
        /* -------------------------------------------------------------------------- */
        /// <summary>
        /// This struct declares a PolityMember's current parents, partners and children array.
        /// </summary>
        [Serializable]
        public struct FamilyStruct
        {
            public PolityMember member;
            public PolityMember[] parents;
            public PartnerStruct partners;
            // public Dictionary<PolityMember, PolityMember[]> partners2;
            [Serializable]
            public struct PartnerStruct
            {
                public PolityMember partner;
                public PolityMember[] children;
            }
        }

    }
}