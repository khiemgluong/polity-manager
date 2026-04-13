using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public class NPCList : MonoBehaviour
    {
        [System.Serializable]
        public class NPCCategories
        {
            public Reader reader;
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
            Reader reader = npc.Reader;
            NPCCategories categories = npcs.Find(list => list.reader.faction.Equals(reader.faction));
            if (categories != null)
            {
                categories.npcs ??= new List<NPC>();
                categories.npcs.Add(npc);
            }
            npc.transform.SetParent(transform);
        }

        private void OnDespawned(NPC npc)
        {
            Reader reader = npc.Reader;
            NPCCategories categories = npcs.Find(list => list.reader.faction.Equals(reader.faction));
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