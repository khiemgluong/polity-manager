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
        [SerializeField] string id;
        public string ID { get => id; private set => id = value; }
        public PolityReader reader = new();
        public FamilyStruct family = new();

        /* --------------------------------- EVENTS --------------------------------- */
        public static Action OnLeaderChange;
        public static Action<PolityMember> OnMemberSpawn;
        void Awake()
        {
            if (id == null || id == "") GenerateGUID();
            family.parents ??= new();
            family.partners ??= new();
            family.children ??= new();
        }
        void OnEnable()
        {
            OnMemberSpawn += OnMemberSpawned;
            OnFactionChange += OnFactionChanged;
        }
        void OnDisable()
        {
            OnMemberSpawn -= OnMemberSpawned;
            OnFactionChange -= OnFactionChanged;
        }
        void Start()
        {
            OnMemberSpawn?.Invoke(this);
        }
        [ContextMenu("Generate ID")]
        public void GenerateGUID()
        {
            id = Guid.NewGuid().ToString().Replace("-", "");
        }
        /* --------------------------------- EVENTS --------------------------------- */
        void OnMemberSpawned(PolityMember member)
        {
            if (id == null || id == "") return;
            ReplacePrefabWithInstance(family.parents);
            ReplacePrefabWithInstance(family.partners);
            ReplacePrefabWithInstance(family.children);
            void ReplacePrefabWithInstance(List<PolityMember> members)
            {
                for (int i = 0; i < members.Count; i++)
                    if (members[i].id == member.id && members[i] != member)
                    { members[i] = member; break; }
            }
        }

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
        [ContextMenu("Clear/All")]
        void ClearAll()
        {
            ClearParents();
            ClearPartners();
            ClearChildren();
        }
        [ContextMenu("Clear/Parents")]
        void ClearParents()
        {
            family.parents = family.parents.Where(item => item != null).ToList();
            foreach (var parent in family.parents.ToList())
                parent.family.children.Remove(this);
            family.parents.Clear();
        }
        [ContextMenu("Clear/Partners")]
        void ClearPartners()
        {
            family.partners = family.partners.Where(item => item != null).ToList();
            foreach (var partner in family.partners.ToList())
                partner.family.partners.Remove(this);
            family.partners.Clear();
        }
        [ContextMenu("Clear/Children")]
        void ClearChildren()
        {
            family.children = family.children.Where(item => item != null).ToList();
            foreach (var child in family.children.ToList())
                child.family.parents.Remove(this);
            family.children.Clear();
        }

        [Serializable]
        public struct FamilyStruct
        {
            public List<PolityMember> parents;
            public List<PolityMember> partners;
            public List<PolityMember> children;
        }
    }
}