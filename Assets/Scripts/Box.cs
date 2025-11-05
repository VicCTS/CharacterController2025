using UnityEngine;

public class Box : MonoBehaviour, IGrabeable, IDamageable
{
    [SerializeField] private float _health;

    public void Grab()
    {
        Debug.Log("Coger caja");
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if(_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
