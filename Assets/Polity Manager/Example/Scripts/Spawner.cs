using System.Collections.Generic;
using UnityEngine;

namespace KL
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] GameObject[] npcPrefabs;
        [SerializeField] GameObject cursor;
        [SerializeField] PolityReader polityReader;
        private HashSet<Transform> usedSpawnPoints = new HashSet<Transform>(); // Hash set to track used spawn points

        void Start()
        {
            // Collect all child GameObjects as spawn points
            List<Transform> spawnPoints = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                spawnPoints.Add(transform.GetChild(i));
            }

            // Shuffle the list of spawn points to randomize order
            int n = spawnPoints.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                Transform value = spawnPoints[k];
                spawnPoints[k] = spawnPoints[n];
                spawnPoints[n] = value;
            }

            // Spawn each NPC prefab at a random spawn point from the collected spawn points
            foreach (var prefab in npcPrefabs)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    // Check if spawn point has already been used
                    if (!usedSpawnPoints.Contains(spawnPoint))
                    {
                        // Instantiate the prefab at the spawn point
                        SpawnNPC(prefab, spawnPoint.position);
                        // Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

                        // Add the spawn point to the used set to avoid reusing it
                        usedSpawnPoints.Add(spawnPoint);
                        break; // Break out of the inner loop once a spawn point is used
                    }
                }
            }
        }
        void SpawnNPC(GameObject prefab, Vector3 position, Quaternion rotation = default)
        {
            GameObject npc = Instantiate(prefab, position, rotation);
            // NPC _npc = npc.GetComponent<NPC>();
            if (npc.TryGetComponent(out PolityMember member))
            {
                member.reader.SetPolity(polityReader);
            }
            else
            {
                PolityMember _member = npc.AddComponent<PolityMember>();
                _member.reader.SetPolity(polityReader);
            }
        }

        void Update()
        {

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            int terrainLayerMask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(ray, out RaycastHit hit, 100, terrainLayerMask))
            {
                if (!cursor.activeSelf) cursor.SetActive(true);
                cursor.transform.position = hit.point + Vector3.up * .01f;
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(hit.point);
                    SpawnNPC(npcPrefabs[Random.Range(0, npcPrefabs.Length)], hit.point);
                }

            }
            else
            { if (cursor.activeSelf) cursor.SetActive(false); }
        }
    }
}