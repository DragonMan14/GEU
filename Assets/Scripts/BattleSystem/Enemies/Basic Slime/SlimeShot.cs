using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeShot : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private Animator _animator;

    public float Damage;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetTrigger("Kaboom");
        if (other.CompareTag("PlayerCombat"))
        {
            PlayerManager.Instance.PlayerAttributes.DrainHealth(Damage);

            Facing knockbackDirection = PlayerManager.Instance.PlayerCombat.transform.position.x > this.transform.position.x ? Facing.right : Facing.left;

            PlayerManager.Instance.PlayerMovementManager.PlayerMovementBattleSystem.ApplyKnockback(knockbackDirection, 1f);
        }
    }

    private void DestroySlimeball()
    {
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

            Vector3 flippedScale = this.transform.localScale;
            flippedScale.x *= -1;
            this.transform.localScale = flippedScale;
        }
    }
}
