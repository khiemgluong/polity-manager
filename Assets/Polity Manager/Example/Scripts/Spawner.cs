using System.Collections.Generic;
using UnityEngine;
using static KL.PolityManager;
namespace KL
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] NPC[] dummies;
        [SerializeField] Material[] colors;
        [SerializeField] GameObject spawnDummy, cursor;
        public bool spawn = true;
        [SerializeField] PolityReader polityReader;
        HashSet<Transform> usedSpawnPoints = new();
        void Awake()
        {
            foreach (Polity polity in PM.polities)
                Debug.Log("Polity: " + polity.name);
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
        }
        GameObject SpawnNPC(GameObject prefab, Vector3 position)
        {
            GameObject npc = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
            if (!npc.TryGetComponent(out PolityMember _))
            {
                PolityMember _member = npc.AddComponent<PolityMember>();
                _member.reader.SetPolity(polityReader);
                Debug.Log("Spawned NPC polity: " + _member.reader.Struct.polityName);
            }
            return npc;
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
                    SpawnNPC(spawnDummy, hit.point);
                }

            }
            else
            { if (cursor.activeSelf) cursor.SetActive(false); }
        }
    }
}