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

        #region Lifecycle
        /* -------------------------------- Lifecycle ------------------------------- */
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
        }

        protected virtual void OnDestroy()
        {
            TransferLeader();
            Faction.OnNameChange -= OnFactionNameChanged;
        }

        protected virtual void Update()
        {
            formation?.Update();
        }
        #endregion

        #region Public APIs
        public void TransferLeader()
        {
            if (members.Count == 0) return;
            IMember closest = members[0];
            float closestDist = Vector3.Distance(transform.position, closest.transform.position);
            foreach (IMember member in members)
            {
                float dist = Vector3.Distance(transform.position, member.transform.position);
                if (dist < closestDist)
                {
                    closest = member;
                    closestDist = dist;
                }
            }
            if (!closest.transform.gameObject.TryGetComponent(out Leader memberLeader))
                memberLeader = closest.transform.gameObject.AddComponent<Leader>();
            memberLeader.Faction = Faction;
            TransferLeader(memberLeader);
        }

        public void TransferLeader(Leader newLeader)
        {
            foreach (IMember member in members)
            {
                member.Faction.Set(newLeader.Faction.Name);
                member.Leader = newLeader;
                newLeader.AddMember(member);
            }
            members.Clear();
            formation = null;
        }

        public void AddMember(IMember member, bool enforceFaction = false)
        {
            if (enforceFaction && !member.Faction.Equals(Faction))
            {
                Debug.LogWarning("Member belongs to another faction and enforceFaction is true.", member.transform);
                return;
            }
            if (member.Leader != null)
                if (member.Leader == this) return;

            member.Faction.Set(Faction.Name);
            member.Leader = this;
            if (!members.Contains(member))
                members.Add(member);
            formation?.Add(member);
        }

        public void RemoveMember(IMember member)
        {
            if (members.Contains(member))
                members.Remove(member);
            formation?.Remove(member);
        }
        #endregion

        #region Callbacks
        void OnFactionNameChanged(string newFactionName)
        {
            foreach (IMember member in members)
                member.Faction.Set(newFactionName);
        }
        #endregion
    }
}