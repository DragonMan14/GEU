using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAdvancedTest : Interactable, IInteractable
{
    public TextAsset JSON1;
    private DialogueAPI.DialogueGroup dialogueGroup;
    public int currentGroup = 0;

    private void Awake()
    {
        dialogueGroup = DialogueAPI.DialogueGroup.ReadFromJSON(JSON1);
    }
    public void Interact()
    {
        bool conversationHasEnded = dialogueGroup.group[currentGroup].CurrentIndex >= dialogueGroup.group[currentGroup].conversation.Length;
        if (!conversationHasEnded)
        {
            dialogueGroup.group[currentGroup].PlayDialogue();
        }
        else if (!UIManager.Instance.DialogueOptionsOpen)
        {
            SetOptionTexts(currentGroup);
        }
        else
        {
            PerformOption(currentGroup);
        }
    }

    private void SetOptionTexts(int group)
    {
        switch(group)
        {
            case 0:
                UIManager.Instance.EnableDialogueOptionsMenu(3);
                UIManager.Instance.SetOptionText(0, "Cancel");
                UIManager.Instance.SetOptionText(1, "Group1");
                UIManager.Instance.SetOptionText(2, "Group2");
                break;                
            default:
                dialogueGroup.group[currentGroup].PlayDialogue();
                // When the dialogue menu has closed
                bool dialogueHasEnded = dialogueGroup.group[currentGroup].CurrentIndex == 0;
                if (dialogueHasEnded)
                {
                    currentGroup = 0;
                }
                break;
        }
    }

    private void PerformOption(int group)
    {
        switch (group)
        {
            case 0:
                if (UIManager.Instance.CurrentOption == 0)
                {
                    UIManager.Instance.DisableDialogueUI();
                    dialogueGroup.group[0].CurrentIndex = 0;
                }
                else if (UIManager.Instance.CurrentOption == 1)
                {
                    SetCurrentGroupAndPlay(1);
                }
                else if (UIManager.Instance.CurrentOption == 2)
                {
                    SetCurrentGroupAndPlay(2);
                }
                break;
        }
    }

    private void SetCurrentGroupAndPlay(int group)
    {
        dialogueGroup.group[currentGroup].CurrentIndex = 0;
        currentGroup = group;
        dialogueGroup.group[currentGroup].PlayDialogue();
    }
}
