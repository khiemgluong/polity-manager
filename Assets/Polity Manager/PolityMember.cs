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
        public static Action<PolityMember> OnMemberSpawn, OnMemberDestroy;
        void Awake()
        {
            if (id == null || id == "") GenerateID();
            family.parents ??= new();
            family.partners ??= new();
            family.children ??= new();
        }

        void OnEnable()
        {
            OnMemberSpawn += OnMemberSpawned;
            OnFactionChange += OnFactionChanged;
            OnMemberDestroy += OnMemberDestroyed;
        }
        void OnDisable()
        {
            OnMemberSpawn -= OnMemberSpawned;
            OnFactionChange -= OnFactionChanged;
            OnMemberDestroy -= OnMemberDestroyed;
        }

        void Start()
        {
            OnMemberSpawn?.Invoke(this);
        }

        void OnDestroy()
        {
            OnMemberDestroy?.Invoke(this);
        }
        [ContextMenu("Generate ID")]
        public void GenerateID()
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
        void OnMemberDestroyed(PolityMember member)
        {
            if (id == null || id == "") return;
            RemoveDestroyedMember(family.parents);
            RemoveDestroyedMember(family.partners);
            RemoveDestroyedMember(family.children);
            void RemoveDestroyedMember(List<PolityMember> members)
            {
                for (int i = 0; i < members.Count; i++)
                    if (members[i].id == member.id)
                    { members.Remove(members[i]); break; }
            }
        }
        void OnFactionChanged()
        {

        }
        /* ------------------------------ CONTEXT MENU ------------------------------ */
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