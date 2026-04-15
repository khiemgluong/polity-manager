using UnityEngine;
using AYellowpaper.SerializedCollections;
namespace Polity
{
    // using Polity;
    public class PolityTest : MonoBehaviour
    {
        public Faction[] shart;

        public Nest nest;
        public Nest[] nests;

        [System.Serializable]
        public class Nest
        {
            public string name;
            public int value;
            public Faction[] factions;
            public SubNest subNest;

            public SubNest[] subNests;
            [System.Serializable]
            public class SubNest
            {
                public string name;
                public float value;
                public Faction[] factions;
            }

        }
        public SerializedDictionary<Faction, string> dict0;

        public SerializedDictionary<Faction[], string> dict;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}