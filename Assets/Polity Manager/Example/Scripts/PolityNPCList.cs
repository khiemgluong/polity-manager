using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class PolityNPCList : MonoBehaviour
    {
        [System.Serializable]
        public class NPCCategories
        {
            public Faction reader;
            public List<PolityNPC> npcs;
        }
        public List<NPCCategories> npcs;
        void Awake()
        {
            PolityNPC.OnSpawn += OnSpawned;
            PolityNPC.OnDespawn += OnDespawned;
        }

        private void OnSpawned(PolityNPC npc)
        {
            Faction reader = npc.Faction;
            NPCCategories categories = npcs.Find(list => list.reader.name.Equals(reader.name));
            if (categories != null)
            {
                categories.npcs ??= new List<PolityNPC>();
                categories.npcs.Add(npc);
            }
            npc.transform.SetParent(transform);
        }

        private void OnDespawned(PolityNPC npc)
        {
            Faction reader = npc.Faction;
            NPCCategories categories = npcs.Find(list => list.reader.name.Equals(reader.name));
            if (categories != null)
            {
                categories.npcs.Remove(npc);
                // if (categories.npcs.Count == 0)
                // {
                //     npcs.Remove(categories);
                // }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}