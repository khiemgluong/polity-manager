using UnityEngine;
using UnityEngine.AI;

namespace Polities
{
    using static Polities.Manager;
    public class PolityNPC : MonoBehaviour
    {
        [SerializeField] Mesh[] npcMeshes = new Mesh[6];
        Member member;
        public Member target, allyTarget;
        int health = 25;
        NavMeshAgent agent;
        Vector3 spawnPos;
        MeshFilter meshFilter;
        readonly float detectionRadius = 8f;
        bool beginAttack = false;
        Coroutine attackCoroutine;

        public static event System.Action<PolityNPC> OnNPCSpawn;
        /// <summary>
        /// This PolityMember is retrieved from an Ally's NPC_driver enemyTarget.
        /// </summary>
        // public Transform targetArrow;
        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            agent = GetComponent<NavMeshAgent>();
            agent.avoidancePriority = Random.Range(1, 99);
            spawnPos = transform.position;
            target = null; allyTarget = null;
            OnRelationChange += OnRelationChanged;
        }

        void Start()
        {
            member = GetComponent<Member>();
        }

        void Update()
        {
            if (!agent.enabled) return;
            SearchForPolityMembers();
            if (allyTarget != null && target != null)
                MoveTowardsTarget(allyTarget);
            else if (target != null)
                MoveTowardsTarget(target);
            else
            {
                target = null;
                allyTarget = null;
                beginAttack = false;
                if (agent.remainingDistance >= agent.stoppingDistance)
                {
                    agent.updateRotation = true;
                    agent.speed = 2;
                }
                else
                {
                    agent.updateRotation = false;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                Quaternion.Euler(0, 180, 0),
                                                agent.angularSpeed * Time.deltaTime);
                }
                agent.SetDestination(spawnPos);
                SetMesh(0);
                if (attackCoroutine != null)
                    StopCoroutine(attackCoroutine);
            }
        }
        void SetMesh(int index)
        {
            if (meshFilter.mesh != npcMeshes[index])
                meshFilter.mesh = npcMeshes[index];
        }


        void MoveTowardsTarget(Member polityMember)
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                agent.speed = 0;
                agent.velocity = Vector3.zero;
            }
            else
            {
                agent.speed = 2f;
            }
            agent.SetDestination(polityMember.transform.position);
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
                    yield return new WaitForSeconds(.1f);
                    if (target != null)
                        target.GetComponent<PolityNPC>().TakeDamage();
                }
                else yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
            }
        }
        void OnRelationChanged()
        {
            if (allyTarget != null)
            {
                PolityRelation relation = Singleton.CheckRelation(member, allyTarget);
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
                PolityRelation relation = Singleton.CheckRelation(member, target);
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
            if (target != null) return;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            Member foundMember = null;
            foreach (var hitCollider in hitColliders)
                if (hitCollider.TryGetComponent<Member>(out var polityMember))
                    if (polityMember.GetComponent<PolityNPC>().health > 0)
                        if (polityMember != member)
                        {
                            foundMember = polityMember;
                            PolityRelation relation = Singleton.CheckRelation(member, polityMember);
                            switch (relation)
                            {
                                case PolityRelation.Allies:
                                    PolityNPC allyNPC = polityMember.GetComponent<PolityNPC>();
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
            if (foundMember == null)
            {
                agent.SetDestination(spawnPos);
                if (attackCoroutine != null)
                    StopCoroutine(attackCoroutine);
                agent.speed = 2f;
                agent.updateRotation = true;
            }
        }
        void TakeDamage()
        {
            health -= 1;
            if (health <= 0)
            {
                StopCoroutine(attackCoroutine);
                target = null;
                transform.rotation = Quaternion.Euler(0, 180, 0);
                SetMesh(5);
                GetComponent<NavMeshAgent>().enabled = false;
                if (target != null)
                {
                    PolityNPC targetNPC = target.GetComponent<PolityNPC>();
                    targetNPC.target = null;
                }
                Destroy(member);
                Destroy(gameObject, 2f);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

    }
}