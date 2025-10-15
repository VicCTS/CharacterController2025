using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float movementSpeed = 5;
    public float attackDamage = 10;

    public void Movement()
    {
        Debug.Log("movimiento base");
    }

    public virtual void Attack()
    {
        Debug.Log("Ataque base");
    }
}
