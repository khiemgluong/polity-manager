using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    [DisallowMultipleComponent]
    public class Leader : MonoBehaviour
    {
        public Faction Faction;
        [Range(1, 100)]
        public int capacity = 10;
        public List<IMember> members = new();
        public Formation formation;
        public static event Action<Leader> OnSpawn, OnDespawn;
        protected virtual void Awake()
        {
            if (TryGetComponent(out IMember member))
            {
                Faction = member.Faction;
                member.Leader = this;
            }
            formation = new Formation(this);
            Faction.OnNameChange += OnFactionNameChanged;
        }

        protected virtual void Start()
        {
            OnSpawn?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            OnDespawn?.Invoke(this);
            Faction.OnNameChange -= OnFactionNameChanged;
        }

        public void AddMember(IMember member, bool enforceFaction = false)
        {
            if (enforceFaction && !member.Faction.Equals(Faction))
            {
                Debug.Log("Member belongs to another faction", member.transform);
                return;
            }
            member.Faction.Set(Faction.Name);
            member.Leader = this;
            if (!members.Contains(member))
                members.Add(member);
            formation.Add(member);
        }

        public void RemoveMember(IMember member)
        {
            if (members.Contains(member))
                members.Remove(member);
            formation.Remove(member);
        }

        void OnFactionNameChanged(string newFactionName)
        {
            foreach (IMember member in members)
            {
                member.Faction.Set(newFactionName);
            }
        }
    }
}