using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KL
{
    using static KL.PolityManager;
    [DisallowMultipleComponent]
    public class PolityMember : MonoBehaviour
    {
        public PolityReader reader;
        public List<PolityMember> parents, partners, children;

        /* --------------------------------- EVENTS --------------------------------- */
        public static Action OnLeaderChange;

        void OnEnable() => OnFactionChange += OnFactionChanged;
        void OnDisable() => OnFactionChange -= OnFactionChanged;
        void Awake() => CleanupFamily();

        void OnFactionChanged()
        {
            // bool isCurrentFactionStillAvailable = false;
            // foreach (Faction faction in PM.polities[selectedPolityIndex].classes[selectedClassIndex - 1].factions)
            // {
            //     if (faction.name.Equals(factionName))
            //     { isCurrentFactionStillAvailable = true; break; }
            // }
            // if (!isCurrentFactionStillAvailable)
            // {
            //     Debug.Log(factionName + " is removed from factions list");
            //     selectedFactionIndex = 0; factionName = "";
            // }
        }

        [ContextMenu("Check Family")]
        void ResetRelationships()
        {
            CleanupFamily();
            CheckRelationship(parents, member => member.children, "parent");
            CheckRelationship(partners, member => member.partners, "partner");
            CheckRelationship(children, member => member.parents, "child");
        }

        [ContextMenu("Delete Family")]
        void DeleteFamily()
        {
            // Remove this member from all partners' lists and vice versa
            foreach (PolityMember partner in new List<PolityMember>(partners))
                partner.partners.Remove(this);
            partners.Clear();
            foreach (PolityMember parent in new List<PolityMember>(parents))
                parent.children.Remove(this);
            parents.Clear();
            foreach (PolityMember child in new List<PolityMember>(children))
                child.parents.Remove(this);
            children.Clear();
        }
        [ContextMenu("Cleanup Family")]
        void CleanupFamily()
        {
            parents = parents.Where(item => item != null).ToList();
            partners = partners.Where(item => item != null).ToList();
            children = children.Where(item => item != null).ToList();
        }
        void CheckRelationship(List<PolityMember> yourFamily, Func<PolityMember, List<PolityMember>> theirFamily, string relationshipType)
        {
            if (yourFamily.Any())
            {
                List<PolityMember> toRemove = new List<PolityMember>();
                foreach (PolityMember member in yourFamily)
                    if (!theirFamily(member).Contains(this))
                        toRemove.Add(member);

                foreach (PolityMember nonReciprocal in toRemove)
                {
                    yourFamily.Remove(nonReciprocal);
                    Debug.Log($"Removed non-reciprocal {relationshipType}: {nonReciprocal} from {this}'s {relationshipType} list.");
                }
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                             PUBLIC API METHODS                             */
        /* -------------------------------------------------------------------------- */

        public FamilyStruct GetMemberFamily()
        {
            FamilyStruct familyStruct = new()
            {
                parents = parents.ToArray(),
                partners = partners.ToArray(),
                children = children.ToArray(),
            }; return familyStruct;
        }
     

        /* -------------------------------------------------------------------------- */
        /*                                FAMILYSTRUCT                                */
        /* -------------------------------------------------------------------------- */
        /// <summary>
        /// This struct declares a PolityMember's current parents, partners and children array.
        /// </summary>
        public struct FamilyStruct
        {
            public PolityMember[] parents;
            public PolityMember[] partners;
            public PolityMember[] children;
        }
    }
}