using UnityEngine;

public class EnemyRobot : EnemyController
{
    [Header("Robot Animations")]
    [SerializeField] protected string closeState = "Close"; // animación de "cerrarse" antes de rodar
    [SerializeField] protected string rollState = "Roll";   // animación de rodar (persecución)
    [SerializeField] protected string openState = "Open";   // animación de abrirse al llegar

    [Header("Robot Behavior")]
    [Tooltip("Tiempo (s) que tarda la animación Close antes de empezar a rodar. Ajusta al length real de la anim.")]
    [SerializeField] protected float closeDuration = 0.6f;
    [Tooltip("Velocidad de rodar (usa agent.speed cuando está rodando).")]
    [SerializeField] protected float rollSpeed = 6f;

    protected int closeHash, rollHash, openHash;

    // Flags / timers
    protected bool isClosing = false;   // está reproduciendo Close y aún no empieza a rodar
    protected bool isRolling = false;   // está en modo rodar (persecución)
    protected float closeEndTime = 0f;
    protected bool hasOpened = false;   // evitando reabrir repetidamente

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
        // Reset flags por si fue desactivado/activado
        ResetRobotStateFlags();
    }

    void ResetRobotStateFlags()
    {
        isClosing = false;
        isRolling = false;
        hasOpened = false;
        closeEndTime = 0f;
    }

    // DecideState: si está en rango -> Attack (Open); si lo ve -> iniciar Close -> Roll; si no lo ve -> Patrol
    protected override void DecideState(bool seePlayer)
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // Si está dentro del rango de "apertura", ir a Attack (Open)
        if (dist <= attackRange)
        {
            currentState = State.Attack;
            return;
        }

        // Si lo ve (pero no en rango): iniciar secuencia Close->Roll
        if (seePlayer)
        {
            // Si ya está rodando o cerrando, simplemente mantener Chase (para que HandleChase gestione)
            if (isClosing || isRolling)
            {
                currentState = State.Chase;
            }
            else
            {
                // iniciar close: mantener en Chase para que HandleChase lo procese
                isClosing = true;
                closeEndTime = Time.time + Mathf.Max(0.01f, closeDuration);
                hasOpened = false;
                // aseguramos que el agente esté detenido mientras "close" se reproduce
                if (agent != null) agent.isStopped = true;
                PlayAnimation(closeHash);
                currentState = State.Chase;
            }
        }
        else
        {
            // Si perdemos al jugador, cancelar close/roll y volver a patrullar
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

    // HandleChase: durante close -> esperar y girar si hace falta; después close -> pasar a rodar (isRolling)
    protected override void HandleChase()
    {
        if (player == null || agent == null)
        {
            // fallback a patrulla
            ResetRobotStateFlags();
            currentState = State.Patrol;
            return;
        }

        // Si estamos en la fase de "close" (cerrado, sin mover)
        if (isClosing)
        {
            // reproducir close mientras dura
            PlayAnimation(closeHash);

            // opcional: girar ligeramente hacia el jugador mientras se cierra (da sensación de "apuntar")
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }

            // Al terminar close -> empezar a rodar
            if (Time.time >= closeEndTime)
            {
                isClosing = false;
                StartRolling();
            }

            return;
        }

        // Si estamos en rolling (persecución)
        if (isRolling)
        {
            // asegurarnos agente activo y velocidad de roll
            agent.isStopped = false;
            agent.speed = rollSpeed;
            PlayAnimation(rollHash);

            // actualizar destino a intervalos para no spamear pathfinding
            if (Time.time - lastPathUpdateTime > pathUpdateRate)
            {
                lastPathUpdateTime = Time.time;
                agent.SetDestination(player.position);
            }

            // si llega al rango de apertura -> pasar a Attack (Open)
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= attackRange)
            {
                // parar rodar y abrirse
                isRolling = false;
                hasOpened = true;
                if (agent != null) agent.isStopped = true;
                currentState = State.Attack;
            }

            return;
        }

        // Si por alguna razón no está cerrando ni rodando, reproducir run/walk del padre
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

    // HandleAttack se usará para la animación "Open" cuando llegue al jugador
    protected override void HandleAttack()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        // Ejecutar Open (una vez por entrada a Attack)
        if (!hasOpened)
        {
            // detener agent y reproducir Open
            if (agent != null) agent.isStopped = true;
            PlayAnimation(openHash);
            hasOpened = true;

            // Aquí no hacemos daño todavía. Se puede programar el efecto futuro en PerformAttack/PerformOpen
            PerformOpen();
        }
        else
        {
            // Mantenerse en Open mientras el jugador esté en rango; si se aleja, volver a Chase/Patrol
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > attackRange + attackExitBuffer)
            {
                // si aún lo ve, comenzar la secuencia Close->Roll otra vez
                if (CanSeePlayer())
                {
                    // forzar re-inicio de close->roll
                    isClosing = true;
                    closeEndTime = Time.time + Mathf.Max(0.01f, closeDuration);
                    if (agent != null) agent.isStopped = true;
                    PlayAnimation(closeHash);
                    currentState = State.Chase;
                }
                else
                {
                    // perder al jugador -> volver a patrullar
                    ResetRobotStateFlags();
                    currentState = State.Patrol;
                    if (agent != null)
                    {
                        agent.isStopped = false;
                        agent.speed = patrolSpeed;
                        // reestablecer destino de patrulla si hay puntos
                        if (patrolPoints != null && patrolPoints.Length > 0)
                            agent.SetDestination(patrolPoints[patrolIndex].position);
                    }
                }
            }
        }
    }

    // Placeholder: en el futuro aquí haremos la explosión / daño. Por ahora solo debug.
    protected virtual void PerformOpen()
    {
        Debug.Log($"{name} performed OPEN (no damage implemented yet).");
    }

    // Cuando se desactiva/activa el objeto, limpiamos banderas
    protected virtual void OnDisable()
    {
        ResetRobotStateFlags();
    }

    // Opcional: puedes sobrescribir PerformAttack si deseas que Open sea tratado como attack en el padre.
    protected override void PerformAttack()
    {
        // No usamos el PerformAttack tradicional aquí; Open usa PerformOpen.
    }
}
