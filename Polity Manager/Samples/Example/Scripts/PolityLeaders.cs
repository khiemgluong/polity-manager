using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polity.Example
{
    public class Leaders : MonoBehaviour
    {
        public static Leaders PL { get; private set; }
        public List<Leader> leaders = new();
        [SerializeField] PolityNPC dummy;
        void Awake()
        {
            PolityNPC.OnSpawn += OnLeaderSpawned;
            PolityNPC.OnDespawn += OnLeaderDespawned;
        }

        void OnLeaderSpawned(PolityNPC npc)
        {
            if (npc.TryGetComponent(out Leader leader))
            {
                if (!leaders.Contains(leader))
                    leaders.Add(leader);
                leader.transform.SetParent(transform);
            }
        }

        void OnLeaderDespawned(PolityNPC npc)
        {
            if (npc.TryGetComponent(out Leader leader))
            {
                if (leaders.Contains(leader))
                    leaders.Remove(leader);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}