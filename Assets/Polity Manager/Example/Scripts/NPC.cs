using UnityEngine;
using UnityEngine.AI;

namespace KL
{
    using static KL.PolityManager;
    public class NPC : MonoBehaviour
    {
        [SerializeField] Mesh[] npcMeshes = new Mesh[6];
        PolityMember member;
        public PolityMember target, allyTarget;
        public int health = 10;
        NavMeshAgent agent;
        Vector3 spawnPos;
        MeshFilter meshFilter;
        readonly float detectionRadius = 8f;
        bool beginAttack = false;
        Coroutine attackCoroutine;
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
            target = null; allyTarget = null;
            OnRelationChange += OnPolityStateChanged;
        }

        void Start() => SearchForPolityMembers();

        void Update()
        {
            if (allyTarget != null)
                MoveTowardsPolityMemberTarget(allyTarget);
            else if (target != null)
                MoveTowardsPolityMemberTarget(target);
        }
        void SetMesh(int index)
        {
            if (meshFilter.mesh != npcMeshes[index])
                meshFilter.mesh = npcMeshes[index];
        }


        void MoveTowardsPolityMemberTarget(PolityMember polityMember)
        {
            // Debug.Log("Remaining distance: " + agent.remainingDistance + "velocity: " + agent.velocity.magnitude);
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
                {
                    yield return new WaitForSeconds(.125f);
                    if (target != null)
                        target.GetComponent<NPC>().TakeDamage();
                }
                else yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
            }
        }
        void OnPolityStateChanged()
        {
            if (allyTarget != null)
            {
                PolityRelation relation = PM.CheckPolityRelation(member, allyTarget);
                switch (relation)
                {
                    case PolityRelation.Allies:
                        target = null;
                        agent.SetDestination(spawnPos);
                        break;
                    case PolityRelation.Neutral:
                        allyTarget = null;
                        SearchForPolityMembers();
                        break;
                }
            }
            else if (target != null)
            {
                PolityRelation relation = PM.CheckPolityRelation(member, target);
                if (relation == PolityRelation.Neutral)
                {
                    target = null;
                    agent.SetDestination(spawnPos);
                }
                else SearchForPolityMembers();
            }
            else SearchForPolityMembers();
        }

        public void SearchForPolityMembers()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (var hitCollider in hitColliders)
                if (hitCollider.TryGetComponent<PolityMember>(out var polityMember))
                    if (polityMember.GetComponent<NPC>().health > 0)
                        if (polityMember != member)
                        {
                            PolityRelation relation = PM.CheckPolityRelation(member, polityMember);
                            switch (relation)
                            {
                                case PolityRelation.Allies:
                                    NPC allyNPC = polityMember.GetComponent<NPC>();
                                    if (allyNPC.target != null)
                                        if (allyNPC.target != null)
                                            allyTarget = allyNPC.target;
                                    break;
                                case PolityRelation.Enemies:
                                    allyTarget = null;
                                    target = polityMember;
                                    agent.updateRotation = false;
                                    agent.SetDestination(target.transform.position);
                                    break;
                            }
                        }
        }
        void TakeDamage()
        {
            health -= 1;
            if (health <= 0)
            {
                StopCoroutine(attackCoroutine);
                NPC targetNPC = target.GetComponent<NPC>();
                target = null;
                transform.rotation = Quaternion.identity;
                SetMesh(5);
                Destroy(GetComponent<NavMeshAgent>());
                targetNPC.StopCoroutine(targetNPC.attackCoroutine);
                targetNPC.SetMesh(0);
                targetNPC.target = null;
                targetNPC.SearchForPolityMembers();
                Destroy(gameObject, 5f);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

    }
}