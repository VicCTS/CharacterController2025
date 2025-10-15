using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Caja : MonoBehaviour, IDamageable, IInteratable
{
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
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void IDamageable.TakeDamage(float damage)
    {
        Debug.Log("caja recibiendo daño");
    }

    void IInteratable.Interact()
    {
        Debug.Log("interactuando con caja");
    }
}
