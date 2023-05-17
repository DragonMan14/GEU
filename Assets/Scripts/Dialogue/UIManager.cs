using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private PlayerManager playerManager;

    private RawImage _dialogueBox;
    private TextMeshProUGUI _dialogueText;
    private RectTransform _dialogueTextPosition;
    private RawImage _dialogueImage;
    private RawImage _dialogueName;
    private TextMeshProUGUI _dialogueNameText;
    private RawImage _cg;

    private RawImage _dialogueOptionsMenu;
    private RectTransform _dialogueOptionsMenuPosition;
    public TextMeshProUGUI[] _dialogueOptions;
    private readonly int _maxDialogueOptions = 5;
    private RawImage _dialogueOptionsCursor;
    private RectTransform _dialogueOptionsCursorPosition;
    private int _numOfEnabledOptions = 5;
    public int CurrentOption { get; private set; }

    [HideInInspector] public readonly float BaseTextScrollSpeed = 0.035f;
    [HideInInspector] public readonly float BaseImageAnimationSpeed = 1f;
    [HideInInspector] public readonly float BaseCGAnimationSpeed = 1f;

    public bool TextIsScrolling { get; private set; }
    public bool ImageInAnimation { get; private set; }
    public bool CGInAnimation { get; private set; }
    public bool DialogueOptionsOpen => _dialogueOptionsMenu.enabled == true;
    public bool DialogueOpen => _dialogueBox.enabled == true;
    private Texture2D[] _imageAnimationFrames;
    private int _imageAnimationIndex;
    private Texture2D[] _cgAnimationFrames;
    private int _cgAnimationIndex;

    private readonly float _textWidthWithImage = 530f;
    private readonly float _textWidthNoImage = 650f;
    private readonly float _textHeight = 120f;

    private readonly float _dialogueOptionsMenuWidth = 250f;
    private readonly float _dialogueOptionsCursorXPos = -80f;

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
        _cg = GameObject.FindGameObjectWithTag("CG").GetComponent<RawImage>();

        _dialogueOptionsMenu = GameObject.FindGameObjectWithTag("DialogueOptionsMenu").GetComponent<RawImage>();
        _dialogueOptionsMenuPosition = GameObject.FindGameObjectWithTag("DialogueOptionsMenu").GetComponent<RectTransform>();

        GameObject optionTexts = GameObject.FindGameObjectWithTag("DialogueOptionsText");
        _dialogueOptions = new TextMeshProUGUI[_maxDialogueOptions];
        for (int child = 0; child < _maxDialogueOptions; child++)
        {
            _dialogueOptions[child] = optionTexts.transform.GetChild(child).GetComponent<TextMeshProUGUI>();
        }

        _dialogueOptionsCursor = GameObject.FindGameObjectWithTag("DialogueOptionsCursor").GetComponent<RawImage>();
        _dialogueOptionsCursorPosition = GameObject.FindGameObjectWithTag("DialogueOptionsCursor").GetComponent<RectTransform>();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentOption(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentOption(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCurrentOption(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetCurrentOption(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetCurrentOption(4);
        }
    }

    #region Dialogue
    public void EnableDialogueBox() 
    {
        _dialogueBox.enabled = true;
        playerManager.PlayerInputManager.SetInputState(InputState.Dialogue);
    }
    public void DisableDialogueBox() { _dialogueBox.enabled = false; }
    public void EnableDialogueText() { _dialogueText.enabled = true; }
    public void DisableDialogueText() { _dialogueText.enabled = false; }
    private void IncreaseTextCharacterVisibility() 
    { 
        _dialogueText.maxVisibleCharacters++;
        if (!AudioManager.Instance.IsSFXPlaying("TextScroll"))
        {
            AudioManager.Instance.PlaySFX("TextScroll");
        }
        if (_dialogueText.maxVisibleCharacters >= _dialogueText.GetTextInfo(_dialogueText.text).characterCount)
        {
            AudioManager.Instance.StopSFX("TextScroll");
            TextIsScrolling = false;
            CancelInvoke(nameof(IncreaseTextCharacterVisibility));
        }
    }
    public void StartScrollDialogue(float scrollSpeed)
    {
        TextIsScrolling = true;
        _dialogueText.maxVisibleCharacters = 0;
        //AudioManager.Instance.SetSFXSpeed("TextScroll", scrollSpeed);
        float finalSpeed = BaseTextScrollSpeed / scrollSpeed;
        InvokeRepeating(nameof(IncreaseTextCharacterVisibility), 0, finalSpeed);
    }

    public void EndScrollDialogue()
    {
        AudioManager.Instance.StopSFX("TextScroll");
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
    public void LoadImages(string filepath, float animationSpeed)
    {
        if (filepath == "NONE")
        {
            DisableDialogueImage();
            return;
        }
        _imageAnimationFrames = Resources.LoadAll<Texture2D>(filepath);
        if (_imageAnimationFrames.Length == 1)
        {
            _dialogueImage.texture = _imageAnimationFrames[0];
        }
        else
        {
            StartImageAnimation(animationSpeed);
        }
    }

    private void AdvanceImageAnimationByFrame()
    {
        if (_imageAnimationIndex >= _imageAnimationFrames.Length)
        {
            ImageInAnimation = false;
            _imageAnimationIndex = 0;
            CancelInvoke(nameof(AdvanceImageAnimationByFrame));
        }
        else
        {
            _dialogueImage.texture = _imageAnimationFrames[_imageAnimationIndex++];
        }
    }
    private void StartImageAnimation(float animationSpeed)
    {
        if (_imageAnimationFrames == null)
        {
            throw new Exception("There is no animation loaded");
        }
        ImageInAnimation = true;
        float finalSpeed = BaseImageAnimationSpeed / animationSpeed;
        InvokeRepeating(nameof(AdvanceImageAnimationByFrame), 0, finalSpeed);
    }

    public void EndImageAnimation()
    {
        if (_imageAnimationFrames == null)
        {
            throw new Exception("There is no animation loaded");
        }
        ImageInAnimation = false;
        _imageAnimationIndex = 0;
        CancelInvoke(nameof(AdvanceImageAnimationByFrame));
        _dialogueImage.texture = _imageAnimationFrames[^1];
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

    public void EnableCG()
    {
        _cg.enabled = true;
    }

    public void DisableCG()
    {
        _cg.enabled = false;
    }

    public void LoadCGs(string filepath, float animationSpeed)
    {
        if (filepath == "NONE")
        {
            DisableCG();
            return;
        }
        _cgAnimationFrames = Resources.LoadAll<Texture2D>(filepath);
        if (_cgAnimationFrames.Length == 1)
        {
            _cg.texture = _cgAnimationFrames[0];
        }
        else
        {
            StartCGAnimation(animationSpeed);
        }
    }

    private void AdvanceCGAnimationByFrame()
    {
        if (_cgAnimationIndex >= _cgAnimationFrames.Length)
        {
            CGInAnimation = false;
            _cgAnimationIndex = 0;
            CancelInvoke(nameof(AdvanceCGAnimationByFrame));
        }
        else
        {
            _cg.texture = _cgAnimationFrames[_cgAnimationIndex++];
        }
    }
    private void StartCGAnimation(float animationSpeed)
    {
        if (_cgAnimationFrames == null)
        {
            throw new Exception("There is no animation loaded");
        }
        CGInAnimation = true;
        float finalSpeed = BaseCGAnimationSpeed / animationSpeed;
        InvokeRepeating(nameof(AdvanceCGAnimationByFrame), 0, finalSpeed);
    }

    public void EndCGAnimation()
    {
        if (_cgAnimationFrames == null)
        {
            throw new Exception("There is no animation loaded");
        }
        CGInAnimation = false;
        _cgAnimationIndex = 0;
        CancelInvoke(nameof(AdvanceCGAnimationByFrame));
        _cg.texture = _cgAnimationFrames[^1];
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

    public void EnableDialogueOptionsMenu(int numOfOptions)
    {
        _dialogueOptionsMenu.enabled = true;
        _dialogueOptionsMenuPosition.sizeDelta = new Vector2(_dialogueOptionsMenuWidth, 40 *  numOfOptions);
        for (int option = 0; option < numOfOptions; option++)
        {
            _dialogueOptions[option].enabled = true;
        }
        _numOfEnabledOptions = numOfOptions;
        CurrentOption = _numOfEnabledOptions - 1;
        _dialogueOptionsCursor.enabled = true;
        float newCursorYPos = _dialogueOptions[CurrentOption].GetComponent<RectTransform>().anchoredPosition.y;
        _dialogueOptionsCursorPosition.anchoredPosition = new Vector2(_dialogueOptionsCursorXPos, newCursorYPos);
    }

    public void DisableOptionsMenu()
    {
        _dialogueOptionsMenu.enabled = false;
        for (int option = 0; option < _maxDialogueOptions; option++)
        {
            _dialogueOptions[option].enabled = false;
        }
        _numOfEnabledOptions = 0;
        _dialogueOptionsCursor.enabled = false;
    }

    public void SetOptionText(int index, string text)
    {
        if (index >= _maxDialogueOptions)
        {
            throw new Exception("Index is greater than the max dialogue options");
        }
        if (index >= _numOfEnabledOptions)
        {
            throw new Exception("Index is greater than the number of enabled options");
        }
        _dialogueOptions[index].text = text;
    }

    public void SetCurrentOption(int index)
    {
        if (index >= _maxDialogueOptions)
        {
            throw new Exception("Index is greater than the max dialogue options");
        }
        if (index >= _numOfEnabledOptions)
        {
            throw new Exception("Index is greater than the number of enabled options");
        }
        CurrentOption = index;
        float newYPos = _dialogueOptions[CurrentOption].GetComponent<RectTransform>().anchoredPosition.y;
        _dialogueOptionsCursorPosition.anchoredPosition = new Vector2(_dialogueOptionsCursorXPos, newYPos);
    }

    public void HandleDialogueOptionsInput(float input)
    {
        if (input > 0 && CurrentOption < _numOfEnabledOptions - 1)
        {
            SetCurrentOption(++CurrentOption);
        }
        else if (input < 0 && CurrentOption > 0)
        {
            SetCurrentOption(--CurrentOption);
        }
    }
    public void DisableDialogueUI()
    {
        DisableDialogueBox();
        DisableDialogueText();
        DisableDialogueImage();
        DisableDialogueName();
        DisableCG();
        DisableOptionsMenu();
        InputState state = playerManager.PlayerInputManager.PreviousState;
        playerManager.PlayerInputManager.SetInputState(state);
    }

    #endregion
}
