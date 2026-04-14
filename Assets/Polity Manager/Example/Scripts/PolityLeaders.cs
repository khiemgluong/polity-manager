using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class Leaders : MonoBehaviour
    {
        public static Leaders PL { get; private set; }
        public List<Leader> leaders = new();
        [SerializeField] PolityNPC dummy;
        void Awake()
        {
            Leader.OnSpawn += OnLeaderSpawned;
            Leader.OnDespawn += OnLeaderDespawned;
        }

        void OnLeaderSpawned(Leader leader)
        {
            if (!leaders.Contains(leader))
                leaders.Add(leader);
            leader.transform.SetParent(transform);

        }

        void OnLeaderDespawned(Leader leader)
        {
            if (leaders.Contains(leader)) leaders.Remove(leader);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}