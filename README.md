# Laboratorio-2-Escape-Room

## Scripts Enemigos

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

### `EnemyHumanoid`

* Hereda de `EnemyController`.
* Animaciones: **Idle, Walk, Run, Look, Find**.
* Patrulla por puntos.
* Al llegar a un waypoint: anima **Look** y realiza un escaneo de 180° con su visión.
* Si detecta al jugador: anima **Find** y luego cambia a **Run** para perseguir.

## Futuro

* Daño real al jugador en ataques.
* Explosión del robot después de animación **Open**.
* Posible daño en colisión del humanoide al correr contra el jugador.


