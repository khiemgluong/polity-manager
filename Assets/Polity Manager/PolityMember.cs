using System.Collections.Generic;
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

        }
        protected virtual void OnDisable()
        {

        }

        protected virtual void OnFactionChanged()
        {

        }
    }


    public interface IMember
    {
        public Faction Faction { get; }
        public Leader Leader { get; set; }
        public Transform transform { get; }
    }
}