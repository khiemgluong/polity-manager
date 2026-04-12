using System;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Faction faction;
        public Group group;

        /* --------------------------------- EVENTS --------------------------------- */
        // public static Action<Member> OnMemberSpawn, OnMemberDestroy;
        void Awake()
        {
            Debug.LogError($"Member '{name}' Awake. Polity: '{faction.name}'");
        }

        void OnEnable()
        {
            OnFactionChange += OnFactionChanged;
        }
        void OnDisable()
        {
            OnFactionChange -= OnFactionChanged;
        }

        void Start()
        {
            Shart();
        }

        public void Shart()
        {
          
        }

        void OnDestroy()
        {
        }
        [ContextMenu("Generate ID")]
        public void GenerateID()
        {
        }
        /* --------------------------------- EVENTS --------------------------------- */
        void OnMemberSpawned(Member member)
        {

        }
        void OnMemberDestroyed(Member member)
        {
        }
        void OnFactionChanged()
        {

        }

        [Serializable]
        public struct Group
        {
            public string name;
            public bool leader;
        }
    }
}