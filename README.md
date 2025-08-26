# Laboratorio-2-Escape-Room

## Avances principales

### `EnemyController`

* Script padre.
* Controla navegación, detección de jugador (visión) y estados básicos (Patrol, Chase, Attack).
* Usa animaciones por `Animator.CrossFade`.
* Permite ser extendido para crear diferentes tipos de enemigos.

### `EnemySpider`

* Hereda de `EnemyController`.
* Animaciones: **Idle, Walk, Attack1, Attack2**.
* Patrulla por puntos.
* Si ve al jugador, se queda quieto.
* Si el jugador entra en rango, ataca.

### `EnemyRobot`

* Hereda de `EnemyController`.
* Animaciones: **Idle, Walk, Close, Roll, Open**.
* Patrulla por puntos.
* Si ve al jugador: anima **Close** → **Roll** para perseguir.
* Al llegar al jugador: anima **Open** (en el futuro causará explosión).

## Cómo usar

1. Crear un prefab de enemigo con `NavMeshAgent` + `Animator`.
2. Asignar el script correspondiente (`EnemySpider` o `EnemyRobot`).
3. Configurar puntos de patrulla (`patrolPoints`).
4. Ajustar velocidades, rangos y animaciones desde el inspector.

## Futuro

* Daño real al jugador en ataques.
* Explosión del robot después de animación **Open**.

