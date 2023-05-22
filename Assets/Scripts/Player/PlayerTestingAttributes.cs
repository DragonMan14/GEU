using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestingAttributes : MonoBehaviour
{
    public PlayerAttributes attributes;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PlayerManager.Instance.CollisionHasTagPlayer(collision))
        {
            return;
        }
        attributes.DrainHealth(10);
    }
}
