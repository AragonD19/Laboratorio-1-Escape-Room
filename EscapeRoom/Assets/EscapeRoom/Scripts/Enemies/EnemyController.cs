using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Idle }

    [Header("References")]
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected Transform player;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected string playerTag = "Player";

    [Header("Nav & Movement")]
    [SerializeField] protected float patrolSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 4f;
    [SerializeField] protected float stoppingDistanceAttack = 1.8f;
    [SerializeField] protected float pathUpdateRate = 0.25f;

    [Header("Vision")]
    [SerializeField] protected float viewDistance = 10f;
    [SerializeField, Range(0, 360)] protected float viewAngle = 60f;
    [SerializeField] protected float detectionCooldown = 0.2f;

    [Header("Attack")]
    [SerializeField] protected float attackCooldown = 1.2f;
    [SerializeField] protected float attackRange = 1.8f;
    [SerializeField] protected float attackExitBuffer = 0.5f;

    [Header("Animation (CrossFade)")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string idleState = "Idle";
    [SerializeField] protected string walkState = "Walk";
    [SerializeField] protected string runState = "Run";
    [SerializeField] protected string attackState = "Attack";
    [SerializeField] protected float crossFadeDuration = 0.15f;

    [Header("Rotation")]
    [SerializeField] protected float rotationSpeed = 8f;

    // internals
    protected NavMeshAgent agent;
    protected State currentState = State.Patrol;
    protected int patrolIndex = 0;
    protected float lastDetectionTime = 0f;
    protected float lastAttackTime = -999f;
    protected float defaultStoppingDistance = 0f;
    protected float lastPathUpdateTime = 0f;

    // animation hashes
    protected int idleHash, walkHash, runHash, attackHash;
    protected int currentPlayingAnimHash = -1;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        defaultStoppingDistance = agent != null ? agent.stoppingDistance : 0f;

        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go) player = go.transform;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();

        // compute hashes
        idleHash = Animator.StringToHash(idleState);
        walkHash = Animator.StringToHash(walkState);
        runHash = Animator.StringToHash(runState);
        attackHash = Animator.StringToHash(attackState);
    }

    protected virtual void Start()
    {
        if (agent != null)
        {
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.speed = patrolSpeed;
        }

        if (patrolPoints != null && patrolPoints.Length > 0 && agent != null)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            PlayAnimation(walkHash);
        }
        else
        {
            if (animator != null) PlayAnimation(idleHash);
        }
    }

    protected virtual void Update()
    {
        if (Time.time - lastDetectionTime > detectionCooldown)
        {
            lastDetectionTime = Time.time;
            bool seePlayer = CanSeePlayer();
            DecideState(seePlayer);
        }

        switch (currentState)
        {
            case State.Patrol: HandlePatrol(); break;
            case State.Chase: HandleChase(); break;
            case State.Attack: HandleAttack(); break;
            case State.Idle: HandleIdle(); break;
        }
    }
    
    protected virtual void OnEnable()
    {
        // por defecto no hace nada.
    }

    protected virtual void OnDisable()
    {
        // por defecto no hace nada.
    }


    #region State handlers
    protected virtual void DecideState(bool seePlayer)
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);


        if (dist <= attackRange)
        {
            currentState = State.Attack;
            return;
        }


        if (seePlayer)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    protected virtual void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || agent == null)
        {
            if (agent != null) agent.isStopped = true;
            PlayAnimation(idleHash);
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = defaultStoppingDistance;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        PlayAnimation(walkHash);
    }

    protected virtual void HandleChase()
    {
        if (player == null || agent == null) return;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistanceAttack;

        if (Time.time - lastPathUpdateTime > pathUpdateRate)
        {
            lastPathUpdateTime = Time.time;
            agent.SetDestination(player.position);
        }

        PlayAnimation(runHash);
    }

    protected virtual void HandleAttack()
    {
        if (player == null) return;

        if (agent != null) agent.isStopped = true;

        // face player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }

        PlayAnimation(attackHash);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack();
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange + attackExitBuffer)
        {
            currentState = State.Chase;
            if (agent != null) agent.isStopped = false;
        }
    }

    protected virtual void HandleIdle()
    {
        if (agent != null) agent.isStopped = true;
        PlayAnimation(idleHash);
    }

    protected virtual void PerformAttack()
    {
        Debug.Log($"{name} performs base attack on {(player != null ? player.name : "null")}");
    }
    #endregion

    #region Detection & animation
    protected virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;
        if (dist > viewDistance) return false;

        
        float cosAngle = Vector3.Dot(transform.forward.normalized, dirToPlayer.normalized);
        if (cosAngle < Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad)) return false;

        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 target = player.position + Vector3.up * 1.0f;
        Vector3 dir = (target - origin).normalized;

        int mask = obstacleMask.value == 0 ? Physics.AllLayers : obstacleMask.value;

        RaycastHit[] hits = Physics.RaycastAll(origin, dir, viewDistance, mask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform)
                continue;

        
            if (hit.collider.transform.IsChildOf(player) || hit.collider.transform == player)
                return true;

            
            return false;
        }

        return false;
    }

    protected virtual void PlayAnimation(int stateHash)
    {
        if (animator == null) return;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.shortNameHash == stateHash || currentPlayingAnimHash == stateHash) return;

        currentPlayingAnimHash = stateHash;
        animator.CrossFade(stateHash, crossFadeDuration, 0, 0f);
    }
    #endregion

    #region Gizmos
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward;
        Quaternion leftQ = Quaternion.Euler(0, -viewAngle * 0.5f, 0);
        Quaternion rightQ = Quaternion.Euler(0, viewAngle * 0.5f, 0);
        Vector3 leftDir = leftQ * forward;
        Vector3 rightDir = rightQ * forward;

        Gizmos.color = Color.red;
        Vector3 eye = transform.position + Vector3.up * 1.0f;
        Gizmos.DrawLine(eye, eye + leftDir * viewDistance);
        Gizmos.DrawLine(eye, eye + rightDir * viewDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.12f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }

            if (patrolPoints.Length > 1 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
        }
    }
    #endregion
}
