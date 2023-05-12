using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueAPI : MonoBehaviour
{
    #region TextAPI
    [System.Serializable]
    public class TextAPI
    {
        public string CharacterName;
        public string DisplayName;
        public string Image;
        public float ScrollSpeed;
        public string Audio;
        public string Text;

        public bool HasDisplayName => DisplayName != null;
        public bool HasImage => Image != null;
        public bool HasScrollSpeed => ScrollSpeed != 0;
    }
    #endregion

    #region DialogueClass
    [System.Serializable]
    public class Dialogue
    {
        public int Id { get; }
        public TextAPI[] conversation;
        private int _currentIndex;
        private float _currentScrollSpeed = 1f;

        private string GetTextAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].Text;
        }

        private string GetDisplayNameAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].DisplayName;
        }

        private string GetImageAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].Image;
        }
        private float GetScrollSpeedAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].ScrollSpeed;
        }

        private bool HasDisplayNameChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasDisplayName;
        }

        private bool HasImageChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasImage;
        }

        private bool HasScrollSpeedChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasScrollSpeed;
        }

        private void StartDialogue()
        {
            UIManager.Instance.SetDialogueText(GetTextAtIndex(0));
            UIManager.Instance.EnableDialogueBox();
            UIManager.Instance.EnableDialogueText();

            if (HasImageChangeAtIndex(0))
            {
                UIManager.Instance.EnableDialogueImage();
                UIManager.Instance.LoadImage(GetImageAtIndex(0));
            }
            if (HasDisplayNameChangeAtIndex(0))
            {
                UIManager.Instance.EnableDialogueName();
                UIManager.Instance.SetDialogueNameText(GetDisplayNameAtIndex(0));
            }
            if (HasScrollSpeedChangeAtIndex(_currentIndex))
            {
                _currentScrollSpeed = GetScrollSpeedAtIndex(_currentIndex);
            }
            UIManager.Instance.StartScrollDialogue(_currentScrollSpeed);
            _currentIndex++;
        }

        private void ProgressDialogue()
        {
            if (UIManager.Instance.TextIsScrolling)
            {
                UIManager.Instance.EndScrollDialogue();
                return;
            }

            UIManager.Instance.SetDialogueText(GetTextAtIndex(_currentIndex));
            if (HasImageChangeAtIndex(_currentIndex))
            {
                UIManager.Instance.EnableDialogueImage();
                UIManager.Instance.LoadImage(GetImageAtIndex(_currentIndex));
            }
            if (HasDisplayNameChangeAtIndex(_currentIndex))
            {
                UIManager.Instance.EnableDialogueName();
                UIManager.Instance.SetDialogueNameText(GetDisplayNameAtIndex(_currentIndex));
            }
            if (HasScrollSpeedChangeAtIndex(_currentIndex))
            {
                _currentScrollSpeed = GetScrollSpeedAtIndex(_currentIndex);
            }
            UIManager.Instance.StartScrollDialogue(_currentScrollSpeed);
            _currentIndex++;
        }

        private void EndDialogue()
        {
            if (UIManager.Instance.TextIsScrolling)
            {
                UIManager.Instance.EndScrollDialogue();
                return;
            }
            _currentIndex = 0;
            UIManager.Instance.DisableDialogueUI();
        }

        public void PlayDialogue()
        {
            if (_currentIndex == 0)
            {
                StartDialogue();
            }
            else if (_currentIndex < conversation.Length)
            {
                ProgressDialogue();
            }
            else if (_currentIndex >= conversation.Length)
            {
                EndDialogue();
            }
        }
    }
    #endregion

    #region DialogueGroup
    [System.Serializable]
    public class DialogueGroup
    {
        public Dialogue[] group;

        public static DialogueGroup ReadFromJSON(TextAsset JSON)
        {
            DialogueGroup newGroup = JsonUtility.FromJson<DialogueGroup>(JSON.text);
            return newGroup ?? throw new Exception("Dialogue group is null");
        }
    }

    #endregion
}
