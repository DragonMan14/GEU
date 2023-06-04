using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeShot : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    public float Damage;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCombat"))
        {
            PlayerManager.Instance.PlayerAttributes.DrainHealth(Damage);
            Facing knockbackDirection;
            if (PlayerManager.Instance.PlayerCombat.transform.position.x > this.transform.position.x)
            {
                knockbackDirection = Facing.right;
            }
            else
            {
                knockbackDirection = Facing.left;
            }
            PlayerManager.Instance.PlayerMovementManager.PlayerMovementBattleSystem.ApplyKnockback(knockbackDirection, 1f);
        }
        Destroy(this.gameObject);
    }

    public void Pew(Facing direction)
    {
        if (direction == Facing.right)
        {
            _rigidbody.velocity = new Vector2(5, 0);
        }
        else
        {
            _rigidbody.velocity = new Vector2(-5, 0);
        }
    }
}
