using System.Collections.Generic;
using UnityEngine;
using static KL.PolityManager;

namespace KL
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] NPC[] dummies;
        [SerializeField] GameObject dummy, cursor;
        [SerializeField] PolityReader polityReader;
        HashSet<Transform> usedSpawnPoints = new();

        void Awake()
        {
            foreach (Polity polity in PM.polities)
            {
                Debug.LogError(polity.name);
            }
        }
        void Start()
        {
            // return;
            // Collect all child GameObjects as spawn points
            List<Transform> spawnPoints = new();
            for (int i = 0; i < transform.childCount; i++)
                spawnPoints.Add(transform.GetChild(i));

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
            foreach (NPC dummy in dummies)
                foreach (var spawnPoint in spawnPoints)
                {
                    // Check if spawn point has already been used
                    if (!usedSpawnPoints.Contains(spawnPoint))
                    {
                        SpawnNPC(dummy.gameObject, spawnPoint.position);
                        usedSpawnPoints.Add(spawnPoint); break;
                    }
                }
        }
        void SpawnNPC(GameObject prefab, Vector3 position, Quaternion rotation = default)
        {
            GameObject npc = Instantiate(prefab, position, rotation);
            if (!npc.TryGetComponent(out PolityMember _))
            {
                PolityMember _member = npc.AddComponent<PolityMember>();
                _member.reader.SetPolity(polityReader);
                Debug.Log("Spawned NPC polity: " + _member.reader.Struct.polityName);
            }
        }

        void Update()
        {

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            int terrainLayerMask = LayerMask.GetMask("Terrain") | LayerMask.GetMask("NPC");
            if (Physics.Raycast(ray, out RaycastHit hit, 100, terrainLayerMask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
                {
                    cursor.SetActive(false);
                    return;
                }
                else
                {
                    if (!cursor.activeSelf)
                        cursor.SetActive(true);
                }

                cursor.transform.position = hit.point + Vector3.up * .01f;
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(hit.point);
                    SpawnNPC(dummy, hit.point);
                }

            }
            else
            { if (cursor.activeSelf) cursor.SetActive(false); }
        }
    }
}