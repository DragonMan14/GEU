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
        public float ImageAnimationSpeed;
        public string CG;
        public float CGAnimationSpeed;
        public string Audio;
        public string Text;
        public float TextScrollSpeed;

        public bool HasDisplayName => DisplayName != null;
        public bool HasImage => Image != null;
        public bool HasImageAnimationSpeed => ImageAnimationSpeed != 0;
        public bool HasCG => CG != null;
        public bool HasCGAnimationSpeed => CGAnimationSpeed != 0;
        public bool HasTextScrollSpeed => TextScrollSpeed != 0;
    }
    #endregion

    #region DialogueClass
    [System.Serializable]
    public class Dialogue
    {
        public int Id { get; }
        public TextAPI[] conversation;
        public int CurrentIndex; 
        private float _currentTextScrollSpeed = 1f;
        private float _currentImageAnimationSpeed = 1f;
        private float _currentCGAnimationSpeed = 1f;

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
        private float GetImageAnimationSpeedAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].ImageAnimationSpeed;
        }
        private string GetCGAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].CG;
        }
        private float GetCGAnimationSpeedAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].CGAnimationSpeed;
        }
        private float GetTextScrollSpeedAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].TextScrollSpeed;
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
        private bool HasImageAnimationSpeedChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasImageAnimationSpeed;
        }
        private bool HasCGChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasCG;
        }
        private bool HasCGAnimationSpeedChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasCGAnimationSpeed;
        }
        private bool HasTextScrollSpeedChangeAtIndex(int index)
        {
            if (index >= conversation.Length)
            {
                throw new Exception("Index is greater than the length of conversation");
            }
            return conversation[index].HasTextScrollSpeed;
        }

        private void StartDialogue()
        {
            UIManager.Instance.DialogueManager.SetDialogueText(GetTextAtIndex(0));
            UIManager.Instance.DialogueManager.EnableDialogueBox();
            UIManager.Instance.DialogueManager.EnableDialogueText();

            if (HasDisplayNameChangeAtIndex(0))
            {
                UIManager.Instance.DialogueManager.EnableDialogueName();
                UIManager.Instance.DialogueManager.SetDialogueNameText(GetDisplayNameAtIndex(0));
            }
            if (HasImageChangeAtIndex(0))
            {
                UIManager.Instance.DialogueManager.EnableDialogueImage();
                UIManager.Instance.DialogueManager.LoadImages(GetImageAtIndex(0), _currentImageAnimationSpeed);
            }
            if (HasImageAnimationSpeedChangeAtIndex(0))
            {
                _currentImageAnimationSpeed = GetImageAnimationSpeedAtIndex(0);
            }
            if (HasCGChangeAtIndex(0))
            {
                UIManager.Instance.DialogueManager.EnableCG();
                UIManager.Instance.DialogueManager.LoadCGs(GetCGAtIndex(0), _currentCGAnimationSpeed);
            }
            if (HasCGAnimationSpeedChangeAtIndex(0))
            {
                _currentCGAnimationSpeed = GetCGAnimationSpeedAtIndex(0);
            }
            if (HasTextScrollSpeedChangeAtIndex(0))
            {
                _currentTextScrollSpeed = GetTextScrollSpeedAtIndex(0);
            }
            UIManager.Instance.DialogueManager.StartScrollDialogue(_currentTextScrollSpeed);
            UIManager.Instance.DialogueManager.DisableOptionsMenu();
            CurrentIndex++;
        }

        private void ProgressDialogue()
        {
            if (UIManager.Instance.DialogueManager.TextIsScrolling || UIManager.Instance.DialogueManager.ImageInAnimation)
            {
                if (UIManager.Instance.DialogueManager.TextIsScrolling)
                {
                    UIManager.Instance.DialogueManager.EndScrollDialogue();
                }
                if (UIManager.Instance.DialogueManager.ImageInAnimation)
                {
                    UIManager.Instance.DialogueManager.EndImageAnimation();
                }
                return;
            }
            UIManager.Instance.DialogueManager.SetDialogueText(GetTextAtIndex(CurrentIndex));
            if (HasDisplayNameChangeAtIndex(CurrentIndex))
            {
                UIManager.Instance.DialogueManager.EnableDialogueName();
                UIManager.Instance.DialogueManager.SetDialogueNameText(GetDisplayNameAtIndex(CurrentIndex));
            }
            if (HasImageChangeAtIndex(CurrentIndex))
            {
                UIManager.Instance.DialogueManager.EnableDialogueImage();
                UIManager.Instance.DialogueManager.LoadImages(GetImageAtIndex(CurrentIndex), _currentImageAnimationSpeed);
            }
            if (HasImageAnimationSpeedChangeAtIndex(CurrentIndex))
            {
                _currentImageAnimationSpeed = GetImageAnimationSpeedAtIndex(CurrentIndex);
            }
            if (HasCGChangeAtIndex(CurrentIndex))
            {
                UIManager.Instance.DialogueManager.EnableCG();
                UIManager.Instance.DialogueManager.LoadCGs(GetCGAtIndex(CurrentIndex), _currentCGAnimationSpeed);
            }
            if (HasCGAnimationSpeedChangeAtIndex(CurrentIndex))
            {
                _currentCGAnimationSpeed = GetCGAnimationSpeedAtIndex(CurrentIndex);
            }
            if (HasTextScrollSpeedChangeAtIndex(CurrentIndex))
            {
                _currentTextScrollSpeed = GetTextScrollSpeedAtIndex(CurrentIndex);
            }
          
            UIManager.Instance.DialogueManager.StartScrollDialogue(_currentTextScrollSpeed);
            CurrentIndex++;
        }

        private void EndDialogue()
        {
            if (UIManager.Instance.DialogueManager.TextIsScrolling || UIManager.Instance.DialogueManager.ImageInAnimation)
            {
                if (UIManager.Instance.DialogueManager.TextIsScrolling)
                {
                    UIManager.Instance.DialogueManager.EndScrollDialogue();
                }
                if (UIManager.Instance.DialogueManager.ImageInAnimation)
                {
                    UIManager.Instance.DialogueManager.EndImageAnimation();
                }
                return;
            }
            CurrentIndex = 0;
            UIManager.Instance.DialogueManager.DisableDialogueUI();
        }

        public void PlayDialogue()
        {
            if (CurrentIndex == 0)
            {
                StartDialogue();
            }
            else if (CurrentIndex < conversation.Length)
            {
                ProgressDialogue();
            }
            else if (CurrentIndex >= conversation.Length)
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
