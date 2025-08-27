using UnityEngine;

public class EnemyRobot : EnemyController
{
    [Header("Robot Animations")]
    [SerializeField] protected string closeState = "Close"; 
    [SerializeField] protected string rollState = "Roll";   
    [SerializeField] protected string openState = "Open";   

    [Header("Robot Behavior")]
    [SerializeField] protected float closeDuration = 0.6f;
    [SerializeField] protected float rollSpeed = 6f;

    protected int closeHash, rollHash, openHash;


    protected bool isClosing = false;   
    protected bool isRolling = false;   
    protected float closeEndTime = 0f;
    protected bool hasOpened = false;  

    protected override void Awake()
    {
        base.Awake();
        closeHash = Animator.StringToHash(closeState);
        rollHash = Animator.StringToHash(rollState);
        openHash = Animator.StringToHash(openState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        ResetRobotStateFlags();
    }

    void ResetRobotStateFlags()
    {
        isClosing = false;
        isRolling = false;
        hasOpened = false;
        closeEndTime = 0f;
    }


    protected override void DecideState(bool seePlayer)
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

            if (isClosing || isRolling)
            {
                currentState = State.Chase;
            }
            else
            {

                isClosing = true;
                closeEndTime = Time.time + Mathf.Max(0.01f, closeDuration);
                hasOpened = false;

                if (agent != null) agent.isStopped = true;
                PlayAnimation(closeHash);
                currentState = State.Chase;
            }
        }
        else
        {

            if (isClosing || isRolling)
            {
                ResetRobotStateFlags();
                if (agent != null)
                {
                    agent.isStopped = false;
                    agent.speed = patrolSpeed;
                }
            }
            currentState = State.Patrol;
        }
    }


    protected override void HandleChase()
    {
        if (player == null || agent == null)
        {
            // fallback a patrulla
            ResetRobotStateFlags();
            currentState = State.Patrol;
            return;
        }


        if (isClosing)
        {

            PlayAnimation(closeHash);


            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }


            if (Time.time >= closeEndTime)
            {
                isClosing = false;
                StartRolling();
            }

            return;
        }


        if (isRolling)
        {

            agent.isStopped = false;
            agent.speed = rollSpeed;
            PlayAnimation(rollHash);


            if (Time.time - lastPathUpdateTime > pathUpdateRate)
            {
                lastPathUpdateTime = Time.time;
                agent.SetDestination(player.position);
            }


            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= attackRange)
            {

                isRolling = false;
                hasOpened = true;
                if (agent != null) agent.isStopped = true;
                currentState = State.Attack;
            }

            return;
        }


        base.HandleChase();
    }

    void StartRolling()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = rollSpeed;
            lastPathUpdateTime = Time.time;
            agent.SetDestination(player != null ? player.position : transform.position);
        }

        isRolling = true;
        PlayAnimation(rollHash);
    }


    protected override void HandleAttack()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }


        if (!hasOpened)
        {

            if (agent != null) agent.isStopped = true;
            PlayAnimation(openHash);
            hasOpened = true;

            PerformOpen();
        }
        else
        {

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > attackRange + attackExitBuffer)
            {

                if (CanSeePlayer())
                {

                    isClosing = true;
                    closeEndTime = Time.time + Mathf.Max(0.01f, closeDuration);
                    if (agent != null) agent.isStopped = true;
                    PlayAnimation(closeHash);
                    currentState = State.Chase;
                }
                else
                {

                    ResetRobotStateFlags();
                    currentState = State.Patrol;
                    if (agent != null)
                    {
                        agent.isStopped = false;
                        agent.speed = patrolSpeed;

                        if (patrolPoints != null && patrolPoints.Length > 0)
                            agent.SetDestination(patrolPoints[patrolIndex].position);
                    }
                }
            }
        }
    }


    protected virtual void PerformOpen()
    {
        Debug.Log($"{name} performed OPEN (no damage implemented yet).");
    }


    protected override void OnDisable()
    {
        ResetRobotStateFlags();
    }



}
