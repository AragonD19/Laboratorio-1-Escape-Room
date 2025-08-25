using UnityEngine;

public class EnemySpider : EnemyController
{
    [Header("Spider Animations")]
    [SerializeField] protected string attack1State = "Attack1";
    [SerializeField] protected string attack2State = "Attack2";
    [SerializeField, Range(0f,1f)] protected float attackAltChance = 0.5f;

    protected int attack1Hash, attack2Hash;

    protected override void Awake()
    {
        base.Awake();
        attack1Hash = Animator.StringToHash(attack1State);
        attack2Hash = Animator.StringToHash(attack2State);
    }

    // Decidir estado: si está en rango -> Attack; si lo ve -> Idle; si no lo ve -> Patrol
    protected override void DecideState(bool seePlayer)
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // Priorizar ataque por distancia (evita perderlo cuando está muy cerca)
        if (dist <= attackRange)
        {
            currentState = State.Attack;
            return;
        }

        if (seePlayer)
        {
            currentState = State.Idle; // la araña se queda quieta mirando
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    protected override void HandleChase()
    {
        // Nunca persigue
        if (agent != null) agent.isStopped = true;
        PlayAnimation(idleHash);
    }

    // Idle: mirar al jugador si lo ve y comprobar distancia continuamente
    protected override void HandleIdle()
    {
        if (agent != null) agent.isStopped = true;
        PlayAnimation(idleHash);

        if (player == null) return;

        // si lo ve, girar hacia él
        if (CanSeePlayer())
        {
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }
        }

        // comprobar distancia sin esperar al tick de detección
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    protected override void HandleAttack()
    {
        if (player == null) return;

        if (agent != null) agent.isStopped = true;

        // mirar al jugador
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }

        // atacar (alternando animaciones)
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            int attackAnim = (Random.value < attackAltChance) ? attack2Hash : attack1Hash;
            PlayAnimation(attackAnim);

            PerformAttack();
        }

        // salir de ataque si se aleja
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange + attackExitBuffer)
        {
            // si lo ve aún, volver a Idle; si no, Patrol
            if (CanSeePlayer()) currentState = State.Idle;
            else currentState = State.Patrol;
        }
    }

    protected override void PerformAttack()
    {
        // Ejemplo: aplicar daño si el player tiene PlayerHealth
        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(10);
        }

        Debug.Log($"{name} (Spider) attacks {(player != null ? player.name : "null")}");
    }
}
