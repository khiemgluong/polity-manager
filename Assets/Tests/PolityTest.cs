using UnityEngine;
using AYellowpaper.SerializedCollections;
namespace Polity
{
    // using Polity;
    public class PolityTest : MonoBehaviour
    {
        public Reader[] shart;

        public Nest nest;
        public Nest[] nests;

        [System.Serializable]
        public class Nest
        {
            public string name;
            public int value;
            public Reader[] factions;
            public SubNest subNest;

            public SubNest[] subNests;
            [System.Serializable]
            public class SubNest
            {
                public string name;
                public float value;
                public Reader[] factions;
            }
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}