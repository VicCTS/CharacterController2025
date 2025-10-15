using UnityEngine;

public class MeleeEnemy : Enemy, IDamageable
{
    //private float _maxHealth = 50;

    //public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    //public float MaxHealth => _maxHealth;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Attack();

        Debug.Log(MaxHealth);
    }


    public override void Attack()
    {
        base.Attack();
        Debug.Log("Ataque cuerpo a cuerpo");
    }


    public void TakeDamage(float damage)
    {
        Debug.Log("enemigo recibiendo daño");

        MaxHealth -= damage;

        Debug.Log(MaxHealth);
    }
}
