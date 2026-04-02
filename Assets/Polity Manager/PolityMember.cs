using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public PolityReader reader = new();
        public Group group;

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
    }
}