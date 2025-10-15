public interface IDamageable
{
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    void TakeDamage(float damage);
}