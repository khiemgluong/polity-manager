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
            public List<PolityMember> parents;
            public List<PartnerStruct> partners;
            [Serializable]
            public struct PartnerStruct
            {
                public PolityMember partner;
                public List<PolityMember> children;
            }
        }

    }
}