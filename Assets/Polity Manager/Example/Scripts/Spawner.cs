using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Polity
{
    using static Manager;
    public class Spawner : MonoBehaviour
    {
        [SerializeField] NPC dummy;
        public GameObject cursor;
        public bool spawn = true;
        HashSet<Transform> usedSpawnPoints = new();
        public Dropdown dropdown;
        string dropdownValue;
     
        void Awake()
        {
            foreach (Faction polity in Singleton.factions)
                Debug.Log("Polity: " + polity.name);

            dropdown.ClearOptions();
            List<Dropdown.OptionData> optionList = new();
            foreach (var polity in Singleton.factions)
            {
                optionList.Add(new Dropdown.OptionData(polity.name));
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
                            GameObject npcObj = SpawnNPC(dummy, spawnPoint.position);
                            IMember npc = npcObj.GetComponent<IMember>();
                            int factionIndex = npc.Reader.RandomFactionIndex();
                            int groupIndex = npc.Reader.RandomGroupIndex();
                            npc.Reader.Set(factionIndex, groupIndex);
                            // MeshRenderer meshRenderer = npc.GetComponent<MeshRenderer>();
                            // meshRenderer.material = colors[i];
                            usedSpawnPoints.Add(spawnPoint); break;
                        }
            Time.timeScale = 1;
        }
        GameObject SpawnNPC(NPC dummy, Vector3 position)
        {
            GameObject npcObj = Instantiate(dummy.gameObject, position, Quaternion.Euler(0, 180, 0));
            if (npcObj.TryGetComponent(out NPC npc))
            {
                Debug.Log("Dropdown value: " + dropdown.options[dropdown.value].text);
                npc.Reader.Set(dropdown.value);
            }
            return npcObj;
        }

        void Update()
        {
            if (Time.timeScale == 0)
            {
                cursor.SetActive(false);
                return;
            }
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.collider.TryGetComponent(out Member member))
                {
                    cursor.SetActive(false);
                    // if (Input.GetMouseButtonDown(0))
                    // {
                    //     PolityStruct polityStruct = new()
                    //     {
                    //         polityName = "Orks"
                    //     };
                    //     member.reader.SetPolity(polityStruct);
                    // }
                    return;
                }
                else
                {
                    if (!cursor.activeSelf)
                        cursor.SetActive(true);
                }

                cursor.transform.position = hit.point + Vector3.up * .01f;
                if (Input.GetMouseButtonDown(0))
                    SpawnNPC(dummy, hit.point);

            }
            else
            { if (cursor.activeSelf) cursor.SetActive(false); }
        }
    }
}