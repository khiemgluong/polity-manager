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
        [SerializeField] Leader leaderDummy;
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

        public void SpawnLeader(bool fillMembers = true)
        {
            Leader leaderObj = Instantiate(leaderDummy, transform.position, Quaternion.identity);
            if (fillMembers)
                for (int i = 0; i < leaderObj.capacity; i++)
                {
                    PolityNPC npc = Instantiate(dummy, leaderObj.transform.position, Quaternion.identity);
                    leaderObj.AddMember(npc);
                }
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