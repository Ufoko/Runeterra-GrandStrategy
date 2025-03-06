using UnityEngine;

public class EntityStats : MonoBehaviour
{
    public HealthBarScript healthBarScript;
    public float attackDamage = 1;
    [System.NonSerialized] public float currentHealth;
    public float maxHealth = 10;
    [Tooltip("How many attacks in a second")]
    public float attackSpeed = 0.5f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start() {
        Invoke(nameof(LateStart), 0.5f);
    }

    void LateStart() {
        if(healthBarScript)
            healthBarScript.SetMaxHealth(maxHealth);
    }

    /// <summary>
    /// Server-only!
    /// </summary>
    /// <param name="damage">Returns false if died, and true if survives.</param>
    /// <returns></returns>
    public bool TakeDamage(float damage)
    {
        currentHealth -= damage;
        return currentHealth > 0f;
    }
}
