using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBasic : Interactable, IInteractable
{
    // Dialogue that implements only one static conversation
    [SerializeField] private TextAsset _JSON;
    private DialogueAPI.DialogueGroup dialogueGroup;

    private void Awake()
    {
        dialogueGroup = DialogueAPI.DialogueGroup.ReadFromJSON(_JSON);
    }

    public void Interact()
    {
        dialogueGroup.group[0].PlayDialogue();
    }
}
