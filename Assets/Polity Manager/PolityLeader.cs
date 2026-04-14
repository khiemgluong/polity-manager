using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    [DisallowMultipleComponent]
    public class Leader : MonoBehaviour
    {
        public Faction Faction;
        public List<IMember> members = new();
        public static event Action<Leader> OnSpawn, OnDespawn;
        protected virtual void Awake()
        {
            if (TryGetComponent(out IMember member))
            {
                Faction = member.Faction;
                member.Leader = this;
            }
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

        public void AddMember(IMember member)
        {
            // if(member.Leader == this || member.Faction != members[0].Faction)
            // {
            //     Debug.LogError("Member belongs to another leader or faction");
            //     return;
            // }
            if (!members.Contains(member))
                members.Add(member);
        }
        protected virtual void OnDisable()
        {
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