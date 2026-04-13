using System;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Reader reader;

        void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            OnFactionChange += OnFactionChanged;
        }
        protected virtual void OnDisable()
        {
            OnFactionChange -= OnFactionChanged;
        }


        /* --------------------------------- EVENTS --------------------------------- */

        protected virtual void OnFactionChanged()
        {

        }
    }

    public interface IMember
    {
        public Reader Reader { get; set; }
        public void OnFactionChanged();
        public Transform transform { get; }
    }
}