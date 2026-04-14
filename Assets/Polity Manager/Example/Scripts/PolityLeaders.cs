using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class Leaders : MonoBehaviour
    {
        public static Leaders PL { get; private set; }
        public List<Leader> leaders = new();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            if (leaders.Contains(leader))
                leaders.Remove(leader);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}