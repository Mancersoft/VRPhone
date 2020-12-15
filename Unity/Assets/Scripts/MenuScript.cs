using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Toggle[] cond1Toggles;

    public Toggle[] cond2Toggles;

    public Toggle debugToggle;

    public TMP_InputField[] inputs;

    public GameObject menuPanel;

    private bool runAutoToggle = false;

    public bool isDebugMode;

    public int cond1Id;

    public int cond2Id;

    private void Start()
    {
        cond1Toggles[PlayerPrefs.GetInt("cond1Toggle", 0)].isOn = true;
        cond2Toggles[PlayerPrefs.GetInt("cond2Toggle", 0)].isOn = true;
        debugToggle.isOn = PlayerPrefs.GetInt("debugToggle", 0) != 0;

        foreach (var inputField in inputs)
        {
            string storedValue = PlayerPrefs.GetString(inputField.name, string.Empty);
            if (!string.IsNullOrWhiteSpace(storedValue))
            {
                inputField.text = storedValue;
            }
        }
    }

    public void ToggleValueChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            runAutoToggle = true;
            bool isCond1 = changedToggle.CompareTag("cond1Toggle");
            var toggles = isCond1 ? cond1Toggles : cond2Toggles;
            for (int i = 0; i < toggles.Length; ++i)
            {
                if (toggles[i] != changedToggle)
                {
                    toggles[i].isOn = false;
                }
                else if (isCond1)
                {
                    cond1Id = i;
                    PlayerPrefs.SetInt("cond1Toggle", cond1Id);
                }
                else
                {
                    cond2Id = i;
                    PlayerPrefs.SetInt("cond2Toggle", cond2Id);
                }
            }

            PlayerPrefs.Save();
            runAutoToggle = false;
        }
        else if (!runAutoToggle)
        {
            changedToggle.isOn = true;
        }
    }

    public void ToggleDebugChanged(Toggle debugToggle)
    {
        isDebugMode = debugToggle.isOn;
        PlayerPrefs.SetInt("debugToggle", isDebugMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleMenu()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleMenu();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }



    ////private void Update()
    ////{
    ////    if (gameObject.activeSelf && OVRInput.GetDown(OVRInput.Button.One))
    ////    {
    ////        StartButtonClick();
    ////    }
    ////}

    public void StartButtonClick()
    {
        gameObject.SetActive(false);
    }

    public void ButtonIncrement(TMP_InputField input)
    {
        int newVal = Convert.ToInt32(input.text.Substring(1)) + 1;
        SetInputValue(input, newVal);
    }

    public void ButtonDecrement(TMP_InputField input)
    {
        int newVal = Convert.ToInt32(input.text.Substring(1)) - 1;
        SetInputValue(input, newVal);
    }

    private void SetInputValue(TMP_InputField input, int value)
    {
        input.text = input.text[0].ToString() + value;
        PlayerPrefs.SetString(input.name, input.text);
        PlayerPrefs.Save();
    }
}
