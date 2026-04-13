using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class NPCList : MonoBehaviour
    {
        [System.Serializable]
        public class NPCCategories
        {
            public Faction reader;
            public List<NPC> npcs;
        }
        public List<NPCCategories> npcs;
        void Awake()
        {
            NPC.OnSpawn += OnSpawned;
            NPC.OnDespawn += OnDespawned;
        }

        private void OnSpawned(NPC npc)
        {
            Faction reader = npc.Faction;
            NPCCategories categories = npcs.Find(list => list.reader.name.Equals(reader.name));
            if (categories != null)
            {
                categories.npcs ??= new List<NPC>();
                categories.npcs.Add(npc);
            }
            npc.transform.SetParent(transform);
        }

        private void OnDespawned(NPC npc)
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