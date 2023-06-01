using System.Collections;

public abstract class EnemyAttack
{
    public float Damage;
    public abstract IEnumerator PerformAttack();
}
