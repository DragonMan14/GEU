using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private RawImage _dialogueBox;
    private TextMeshProUGUI _dialogueText;
    private RectTransform _dialogueTextPosition;
    private RawImage _dialogueImage;
    private RawImage _dialogueName;
    private TextMeshProUGUI _dialogueNameText;
    private readonly float _baseSpeed = 0.035f;
    public bool TextIsScrolling { get; private set; }

    private readonly float _textWidthWithImage = 530f;
    private readonly float _textWidthNoImage = 650f;
    private readonly float _textHeight = 120f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<RawImage>();
        _dialogueText = GameObject.FindGameObjectWithTag("DialogueText").GetComponent<TextMeshProUGUI>();
        _dialogueTextPosition = GameObject.FindGameObjectWithTag("DialogueText").GetComponent<RectTransform>();
        _dialogueImage = GameObject.FindGameObjectWithTag("DialogueImage").GetComponent<RawImage>();
        _dialogueName = GameObject.FindGameObjectWithTag("DialogueName").GetComponent<RawImage>();
        _dialogueNameText = GameObject.FindGameObjectWithTag("DialogueNameText").GetComponent<TextMeshProUGUI>();
    }

    #region Dialogue
    public void EnableDialogueBox() { _dialogueBox.enabled = true; }
    public void DisableDialogueBox() { _dialogueBox.enabled = false; }
    public void EnableDialogueText() { _dialogueText.enabled = true; }
    public void DisableDialogueText() { _dialogueText.enabled = false; }
    private void IncreaseTextCharacterVisibility() 
    { 
        _dialogueText.maxVisibleCharacters++;
        if (_dialogueText.maxVisibleCharacters >= _dialogueText.GetTextInfo(_dialogueText.text).characterCount)
        {
            TextIsScrolling = false;
            CancelInvoke(nameof(IncreaseTextCharacterVisibility));
        }
    }
    public void StartScrollDialogue(float scrollSpeed)
    {
        TextIsScrolling = true;
        _dialogueText.maxVisibleCharacters = 0;
        float finalSpeed = _baseSpeed / scrollSpeed;
        InvokeRepeating(nameof(IncreaseTextCharacterVisibility), 0, finalSpeed);
    }

    public void EndScrollDialogue()
    {
        TextIsScrolling = false;
        _dialogueText.maxVisibleCharacters = _dialogueText.GetTextInfo(_dialogueText.text).characterCount;
        CancelInvoke(nameof(IncreaseTextCharacterVisibility));
    }

    public void EnableDialogueImage() 
    { 
        _dialogueImage.enabled = true;
        _dialogueTextPosition.sizeDelta = new Vector2(_textWidthWithImage, _textHeight);
    }
    public void DisableDialogueImage() 
    { 
        _dialogueImage.enabled = false;
        _dialogueTextPosition.sizeDelta = new Vector2(_textWidthNoImage, _textHeight);
    }
    public void LoadImage(string filepath)
    {
        if (filepath == "NONE")
        {
            DisableDialogueImage();
            return;
        }
        _dialogueImage.texture = Resources.Load<Texture>(filepath);
    }
    public void LoadImage(Texture2D texture)
    {
        _dialogueImage.texture = texture;
    }

    public void EnableDialogueName()
    {
        _dialogueName.enabled = true;
        _dialogueNameText.enabled = true;
    }

    public void DisableDialogueName()
    {
        _dialogueName.enabled = false;
        _dialogueNameText.enabled = false;
    }

    public void DisableDialogueUI()
    {
        DisableDialogueBox();
        DisableDialogueText();
        DisableDialogueImage();
        DisableDialogueName();
    }

    public void SetDialogueNameText(string text)
    {
        if (text == "NONE")
        {
            DisableDialogueName();
            return;
        }
        _dialogueNameText.text = text;
    }

    public void SetDialogueText(string text)
    {
        _dialogueText.text = text;
    }
    #endregion
}
