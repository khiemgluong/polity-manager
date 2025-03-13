using UnityEngine;
using UnityEngine.AI;

namespace KL
{
    using static KL.PolityManager;
    public class NPC : MonoBehaviour
    {
        [SerializeField] Mesh[] npcMeshes = new Mesh[6];
        PolityMember member;
        NavMeshAgent agent;
        Vector3 spawnPos;
        MeshFilter meshFilter;
        readonly float detectionRadius = 8f;
        public PolityMember enemyTarget, allyEnemyTarget;
        /// <summary>
        /// This PolityMember is retrieved from an Ally's NPC_driver enemyTarget.
        /// </summary>
        // public Transform targetArrow;
        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            member = GetComponent<PolityMember>();
            agent = GetComponent<NavMeshAgent>();
            agent.avoidancePriority = Random.Range(1, 99);
            spawnPos = transform.position;
            enemyTarget = null; allyEnemyTarget = null;
            OnRelationChange += OnPolityStateChanged;
        }

        void Start() => SearchForPolityMembers();

        void Update()
        {
            if (allyEnemyTarget != null)
            {
                MoveTowardsPolityMemberTarget(allyEnemyTarget);
                // RotateArrowTowardsTarget(allyEnemyTarget.transform);
            }
            else if (enemyTarget != null)
            {
                MoveTowardsPolityMemberTarget(enemyTarget);
                // RotateArrowTowardsTarget(enemyTarget.transform);
            }
        }
        void SetMesh(int index)
        {
            if (meshFilter.mesh != npcMeshes[index])
            {
                meshFilter.mesh = npcMeshes[index];
            }
        }
        bool beginAttack = false;
        Coroutine attackCoroutine;

        void MoveTowardsPolityMemberTarget(PolityMember polityMember)
        {
            Debug.Log("Remaining distance: " + agent.remainingDistance + "velocity: " + agent.velocity.magnitude);
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                agent.speed = 0;
                agent.velocity = Vector3.zero;
            }
            else
            {
                if (agent.remainingDistance >= agent.stoppingDistance)
                    agent.SetDestination(polityMember.transform.position);
                agent.speed = 2f;
            }
            if (agent.remainingDistance > agent.stoppingDistance + .1f)
            {
                if (agent.velocity.magnitude > 1)
                    SetMesh(1);
                else SetMesh(0);
            }
            else
            {
                if (!beginAttack)
                {
                    attackCoroutine = StartCoroutine(AttackRoutine());
                    beginAttack = true;
                }
            }

            Vector3 direction = (polityMember.transform.position - transform.position).normalized;
            float singleStep = agent.angularSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
        }
        System.Collections.IEnumerator AttackRoutine()
        {
            while (true)
            {
                int randomIndex = Random.Range(2, 5); // Random index between 2 and 4 (inclusive)
                SetMesh(randomIndex);
                if (randomIndex == 3 || randomIndex == 4)
                    yield return new WaitForSeconds(.15f);
                else yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
            }
        }
        void OnPolityStateChanged()
        {
            if (allyEnemyTarget != null)
            {
                PolityRelation relation = PM.CheckPolityRelation(member, allyEnemyTarget);
                switch (relation)
                {
                    case PolityRelation.Allies:
                        enemyTarget = null;
                        agent.SetDestination(spawnPos);
                        // targetArrow.gameObject.SetActive(false);
                        break;
                    case PolityRelation.Neutral:
                        allyEnemyTarget = null;
                        SearchForPolityMembers();
                        // targetArrow.gameObject.SetActive(false);
                        break;
                }
            }
            else if (enemyTarget != null)
            {
                PolityRelation relation = PM.CheckPolityRelation(member, enemyTarget);
                if (relation == PolityRelation.Neutral)
                {
                    enemyTarget = null;
                    agent.SetDestination(spawnPos);
                    // targetArrow.gameObject.SetActive(false);
                }
                else SearchForPolityMembers();
            }
            else SearchForPolityMembers();
        }

        void SearchForPolityMembers()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (var hitCollider in hitColliders)
                if (hitCollider.TryGetComponent<PolityMember>(out var polityMember))
                    if (polityMember != member)
                    {
                        PolityRelation relation = PM.CheckPolityRelation(member, polityMember);
                        switch (relation)
                        {
                            case PolityRelation.Allies:
                                NPC allyNPC = polityMember.GetComponent<NPC>();
                                if (allyNPC.enemyTarget != null)
                                    if (allyNPC.enemyTarget != null)
                                    {
                                        allyEnemyTarget = allyNPC.enemyTarget;
                                        // targetArrow.gameObject.SetActive(true);
                                    }
                                break;
                            case PolityRelation.Enemies:
                                allyEnemyTarget = null;
                                enemyTarget = polityMember;
                                agent.updateRotation = false;
                                agent.SetDestination(enemyTarget.transform.position);
                                // targetArrow.gameObject.SetActive(true);
                                break;
                        }
                    }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        void OnPolityMemberChanged()
        {

        }
    }
}