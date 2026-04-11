using System;
using UnityEngine;

namespace Polities
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Polity polity;
        public Unit unit;
        public PolityUnit polity1;

        public Polity[] sharts;

        /* --------------------------------- EVENTS --------------------------------- */
        public static Action<Member> OnMemberSpawn, OnMemberDestroy;
        void Awake()
        {
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

        [System.Serializable]
        public struct Unit
        {
            public string name;
            public bool leader;
        }
    }
}