using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float damageCooldown = 1f;


    [Header("Extras")]
    [SerializeField] private DamageUI damageFlash;
    public Transform respawnPoint;

    private float lastDamageTime = -999f;
    private int currentHealth;
    private CharacterController controller;


    private void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
    }


    public bool CanTakeDamage()
    {
        return Time.time >= lastDamageTime + damageCooldown;
    }

    public void TakeDamage(int damage)
    {
        if (!CanTakeDamage()) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;

        if (damageFlash != null)
            damageFlash.Flash();

        Debug.Log("Jugador recibe daño: " + damage + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        Debug.Log("Respawneando jugador...");
        controller.enabled = false;
        currentHealth = maxHealth;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        controller.enabled = true;

    }
}
