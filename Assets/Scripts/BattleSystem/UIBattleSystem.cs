using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleSystem : MonoBehaviour
{

    private RawImage[] _options;
    private TextMeshProUGUI[] _optionsText;
    public int CurrentOption { get; private set; }
    private readonly int _numOfOptions = 5;

    private void Awake()
    {
        _options = new RawImage[_numOfOptions];
        _options[0] = GameObject.FindGameObjectWithTag("ContinueButton").GetComponent<RawImage>();
        _options[1] = GameObject.FindGameObjectWithTag("PhysicalButton").GetComponent<RawImage>();
        _options[2] = GameObject.FindGameObjectWithTag("MagicButton").GetComponent<RawImage>();
        _options[3] = GameObject.FindGameObjectWithTag("ActButton").GetComponent<RawImage>();
        _options[4] = GameObject.FindGameObjectWithTag("FleeButton").GetComponent<RawImage>();
        _optionsText = new TextMeshProUGUI[_numOfOptions];
        _optionsText[0] = GameObject.FindGameObjectWithTag("ContinueButton").GetComponent<TextMeshProUGUI>();
        _optionsText[1] = GameObject.FindGameObjectWithTag("PhysicalButton").GetComponent<TextMeshProUGUI>();
        _optionsText[2] = GameObject.FindGameObjectWithTag("MagicButton").GetComponent<TextMeshProUGUI>();
        _optionsText[3] = GameObject.FindGameObjectWithTag("ActButton").GetComponent<TextMeshProUGUI>();
        _optionsText[4] = GameObject.FindGameObjectWithTag("FleeButton").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (UIManager.Instance.UIBattleSystem != null && UIManager.Instance.UIBattleSystem != this)
        {
            Destroy(this);
        }
        else
        {
            UIManager.Instance.UIBattleSystem = this;
        }
    }

    public void EnableBattleOptionsMenu()
    {
        for (int option = 0; option < _options.Length; option++)
        {
            _options[option].enabled = true;
            _optionsText[option].enabled = true;
        }
    }

    public void DisableBattleOptionsMenu() 
    {
        for (int option = 0; option < _options.Length; option++)
        {
            _options[option].enabled = false;
            _optionsText[option].enabled = false;
        }
    }

    private void SetCurrentOption(int option)
    {
        if (option > _numOfOptions)
        {
            throw new ArgumentOutOfRangeException("Option is greater than the number of options");
        }
        if (option < 0)
        {
            throw new ArgumentOutOfRangeException("Options is less than zero");
        }
        CurrentOption = option;
    }

    public void HandleBattleOptionsInput(float input)
    {
        if (input > 0 && CurrentOption < _numOfOptions - 1)
        {
            SetCurrentOption(++CurrentOption);
        }
        else if (input < 0 && CurrentOption > 0)
        {
            SetCurrentOption(--CurrentOption);
        }
    }

    public void TestInteract()
    {
        switch (CurrentOption)
        {
            case 0:
                print("Continue");
                PlayerManager.Instance.PlayerInputManager.SetInputState(InputState.BattleSystem);
                break;
            case 1:
                print("Physical");
                break;
            case 2:
                print("Magic");
                break;
            case 3:
                print("Act");
                break;
            case 4:
                print("Flee");
                break;
        }
    }
}
