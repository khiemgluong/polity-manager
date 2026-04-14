using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Polity
{
    using static Manager;
    public class PolitySpawner : MonoBehaviour
    {
        public PolityNPC dummy;
        public bool spawn = true;
        HashSet<Transform> usedSpawnPoints = new();
        public Dropdown dropdown;

        void Awake()
        {
            foreach (Faction polity in PM.factions)
                Debug.Log("Polity: " + polity.Name);

            dropdown.ClearOptions();
            List<Dropdown.OptionData> optionList = new();
            foreach (var polity in PM.factions)
            {
                optionList.Add(new Dropdown.OptionData(polity.Name));
            }
            dropdown.AddOptions(optionList);
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        }
        void OnDropdownValueChanged(int index)
        {
            //    dropdownValue = dropdown.options[index].text;

        }
        void Start()
        {
            List<Transform> spawnPoints = new();
            for (int i = 0; i < transform.childCount; i++)
                spawnPoints.Add(transform.GetChild(i));

            int n = spawnPoints.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                Transform value = spawnPoints[k];
                spawnPoints[k] = spawnPoints[n];
                spawnPoints[n] = value;
            }
            if (spawn)
                for (int i = 0; i < spawnPoints.Count; i++)
                    foreach (var spawnPoint in spawnPoints)
                        if (!usedSpawnPoints.Contains(spawnPoint))
                        {
                            GameObject npcObj = SpawnNPC(spawnPoint.position);
                            IMember npc = npcObj.GetComponent<IMember>();
                            int factionIndex = npc.Faction.RandomFactionIndex();
                            npc.Faction.Set(factionIndex);
                            // MeshRenderer meshRenderer = npc.GetComponent<MeshRenderer>();
                            // meshRenderer.material = colors[i];
                            usedSpawnPoints.Add(spawnPoint); break;
                        }
            Time.timeScale = 1;
        }
        public GameObject SpawnNPC(Vector3 position)
        {
            GameObject npcObj = Instantiate(dummy.gameObject, position, Quaternion.Euler(0, 180, 0));
            if (npcObj.TryGetComponent(out PolityNPC npc))
            {
                Debug.Log("Dropdown value: " + dropdown.options[dropdown.value].text);
                npc.Faction.Set(dropdown.value);
            }
            return npcObj;
        }

        public Leader SpawnLeader(Vector3 position, bool fillMembers = true)
        {
            GameObject leaderObj = Instantiate(dummy.gameObject, position, Quaternion.identity);
            Leader leader = leaderObj.AddComponent<Leader>();
            if (fillMembers)
                for (int i = 0; i < leader.capacity; i++)
                {
                    PolityNPC npc = Instantiate(dummy, leaderObj.transform.position, Quaternion.identity);
                    leader.AddMember(npc);
                }
            return leader;
        }


    }
}