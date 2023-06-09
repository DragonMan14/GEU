using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool playerInRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PlayerManager.Instance.CollisionHasTagPlayer(collision)) { return; }
        playerInRange = true;
        PlayerManager.Instance.AddInteractable(this.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PlayerManager.Instance.CollisionHasTagPlayer(collision)) { return; }
        playerInRange = false;
        PlayerManager.Instance.RemoveInteractable(this.gameObject);
    }

    public bool IsInRange() { return playerInRange; }
}
