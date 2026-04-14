using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class Leaders : MonoBehaviour
    {
        public static Leaders PL { get; private set; }
        public List<Leader> leaders = new();
        public PolityNPC polityNPCPrefab;
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
            for (int i = 0; i < leader.maxMembers; i++)
            {
                PolityNPC npc = Instantiate(polityNPCPrefab, leader.transform.position, Quaternion.identity);
                npc.Faction.Set(leader.Faction.Name);
                leader.AddMember(npc);
            }
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