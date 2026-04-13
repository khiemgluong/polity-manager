using System;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [DisallowMultipleComponent]
    public class Member : MonoBehaviour
    {
        public Faction faction;
        protected virtual void OnEnable()
        {
            OnFactionChange += OnFactionChanged;
        }
        protected virtual void OnDisable()
        {
            OnFactionChange -= OnFactionChanged;
        }



        protected virtual void OnFactionChanged()
        {

        }
    }

    public interface IMember
    {
        public Faction Faction { get; set; }
        public void OnFactionChanged();
        public Transform transform { get; }
    }
}