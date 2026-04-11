using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Polities.Manager;
namespace Polities
{
    public class PolitySpawner : MonoBehaviour
    {
        [SerializeField] PolityNPC[] dummies;
        [SerializeField] Material[] colors;
        [SerializeField] GameObject spawnDummy, cursor;
        public bool spawn = true;
        [SerializeField] PolityReader polityReader = new();
        HashSet<Transform> usedSpawnPoints = new();
        public Dropdown dropdown;
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
            string selectedValue = dropdown.options[index].text;
            Polity polityStruct = new()
            { name = selectedValue };
            polityReader.SetPolity(polityStruct);
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
                for (int i = 0; i < dummies.Length; i++)
                    foreach (var spawnPoint in spawnPoints)
                        if (!usedSpawnPoints.Contains(spawnPoint))
                        {
                            GameObject npc = SpawnNPC(dummies[i].gameObject, spawnPoint.position);
                            MeshRenderer meshRenderer = npc.GetComponent<MeshRenderer>();
                            meshRenderer.material = colors[i];
                            usedSpawnPoints.Add(spawnPoint); break;
                        }
            Time.timeScale = 1;
        }
        GameObject SpawnNPC(GameObject prefab, Vector3 position)
        {
            GameObject npc = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
            if (!npc.TryGetComponent(out Member _))
            {
                Member _member = npc.AddComponent<Member>();
                _member.reader.SetPolity(polityReader);
            }
            return npc;
        }

        void Update()
        {
            if (Time.timeScale == 0)
            {
                cursor.SetActive(false);
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                    SpawnNPC(spawnDummy, hit.point);

            }
            else
            { if (cursor.activeSelf) cursor.SetActive(false); }
        }
    }
}