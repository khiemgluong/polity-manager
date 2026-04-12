using System;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Reader reader;

        /* --------------------------------- EVENTS --------------------------------- */
        // public static Action<Member> OnMemberSpawn, OnMemberDestroy;
        void Awake()
        {
     
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
    }
}