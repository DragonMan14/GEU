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
