using UnityEngine;

public class EnemyHumanoid : EnemyController
{
    [Header("Humanoid Animations")]
    [SerializeField] private string lookState = "Look";
    [SerializeField] private string findState = "Find";
    [SerializeField] private float lookDuration = 3f; // tiempo total del escaneo
    [SerializeField] private float scanAngle = 180f; // amplitud del escaneo
    [SerializeField] private float scanSpeed = 60f;  // grados por segundo

    private int lookHash, findHash;
    private bool isScanning = false;
    private float scanStartTime;
    private float baseYRotation;
    private bool scanningRight = true;

    protected override void Awake()
    {
        base.Awake();
        lookHash = Animator.StringToHash(lookState);
        findHash = Animator.StringToHash(findState);
    }

    protected override void HandlePatrol()
    {

        if (isScanning)
        {
            HandleLook();
            return;
        }

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

            StartLook();
        }
        else
        {
            PlayAnimation(walkHash);
        }
    }

    private void StartLook()
    {
        isScanning = true;
        scanStartTime = Time.time;
        baseYRotation = transform.eulerAngles.y;
        scanningRight = true;
        agent.isStopped = true;
        PlayAnimation(lookHash);
    }

    private void HandleLook()
    {
        float elapsed = Time.time - scanStartTime;


        float halfScan = scanAngle * 0.5f;
        float offset = Mathf.PingPong(elapsed * scanSpeed, scanAngle) - halfScan;

        transform.rotation = Quaternion.Euler(0, baseYRotation + offset, 0);


        if (CanSeePlayer())
        {
            isScanning = false;
            PlayAnimation(findHash);
            Invoke(nameof(StartChaseAfterFind), 1f); 
        }


        if (elapsed >= lookDuration)
        {
            isScanning = false;
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[patrolIndex].position);
        PlayAnimation(walkHash);
    }

    private void StartChaseAfterFind()
    {
        if (player == null) return;
        currentState = State.Chase;
    }

    protected override void HandleChase()
    {
        if (player == null || agent == null) return;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistanceAttack;
        agent.SetDestination(player.position);

        PlayAnimation(runHash);
    }
}
