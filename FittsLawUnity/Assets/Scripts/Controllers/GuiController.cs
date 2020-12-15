using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class provides functionality to the user interface in the TestLoader scene.
/// It contains functions that allow the user to run the experiment using the selected parameters,
/// save and load the current setup for future use, reset or exit the program.
/// </summary>
public class GuiController : MonoBehaviour
{
    [SerializeField] private GameObject _saveStringPanel;
    [SerializeField] private InputField _saveStringInput;
    [SerializeField] private GameObject _errorMessagePanel;
    [SerializeField] private Transform _viveRig;
    [SerializeField] private Transform _foveRig;
    [SerializeField] private Dropdown _vrHmdInput;
    [SerializeField] private InputField _participantCodeInput;
    [SerializeField] private InputField _conditionCodeInput;
    [SerializeField] private InputField _blockCodeInput;
    [SerializeField] private Dropdown _numberOfTargetsInput;
    [SerializeField] private InputField _targetAmplitudesInput;
    [SerializeField] private InputField _targetWidthsInput;
    [SerializeField] private InputField _errorThresholdInput;
    [SerializeField] private InputField _spatialHysteresisInput;
    [SerializeField] private Dropdown _controlMethodInput;
    [SerializeField] private Dropdown _confirmationMethodInput;
    [SerializeField] private InputField _dwellTimeInput;
    [SerializeField] private InputField _timeoutInput;
    [SerializeField] private InputField _eyeSmoothFactorInput;
    [SerializeField] private InputField _mouseSensivityInput;
    [SerializeField] private Toggle _randomizeTargetConiditionsInput;
    [SerializeField] private Toggle _beepOnErrorInput;
    [SerializeField] private Toggle _showCursorInput;
    [SerializeField] private Toggle _hoverHighlightInput;
    [SerializeField] private Toggle _buttonDownHighlightInput;
    [SerializeField] private Toggle _recordGazePositionInput;
    [SerializeField] private ColorPickerButton _backgroundColorPicker;
    [SerializeField] private ColorPickerButton _cursorColorPicker;
    [SerializeField] private ColorPickerButton _targetColorPicker;
    [SerializeField] private ColorPickerButton _buttonDownColorPicker;
    [SerializeField] private ColorPickerButton _hoverColorPicker;
    [SerializeField] private ColorPickerButton _readyForGestureColorPicker;

    private TestBlock.VRHMD _vrHmd;
    private string _participantCode;
    private string _conditionCode;
    private string _blockCode;
    private int _numberOfTargets;
    private List<int> _targetAmplitudes;
    private List<float> _targetWidths;
    private float _errorThreshold;
    private float _spatialHysteresis;
    private TestBlock.ControlMethod _controlMethod;
    private TestBlock.ConfirmationMethod _confirmationMethod;
    private int _dwellTime;
    private int _timeout;
    private int _eyeSmoothFactor;
    private float _mouseSensivity;
    private bool _randomizeTargetConditions;
    private bool _beepOnError;
    private bool _showCursor;
    private bool _hoverHighlight;
    private bool _buttonDownHighlight;
    private bool _recordGazePosition;
    private Color _backgroundColor;
    private Color _cursorColor;
    private Color _targetColor;
    private Color _buttonDownColor;
    private Color _hoverColor;
    private Color _readyForGestureColor;
    private bool _dataValidationSuccessful;
    private string _filePath;

    [SerializeField] private Canvas _vrCanvas;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        //If a saved string exists, load GUI values from it
        _filePath = Application.persistentDataPath + @"\SavedSetup.txt";
        if (File.Exists(_filePath))
        {
            LoadDataFromString(System.IO.File.ReadAllText(_filePath));
            return;
        }
        //If no saved string exists, initiate GUI with the default values below
        _vrHmdInput.value = 2; //1;
        _participantCodeInput.text = "";
        _conditionCodeInput.text = "";
        _blockCodeInput.text = "";
        _numberOfTargetsInput.value = 1;
        _targetAmplitudesInput.text = "265 326 163 224 203 409 458 467";
        _targetWidthsInput.text = "200 200 80 80 40 60 60 40";
        _errorThresholdInput.text = "15";
        _spatialHysteresisInput.text = "2.0";
        _controlMethodInput.value = 7; //1;
        _confirmationMethodInput.value = 0;
        _dwellTimeInput.text = "300";
        _timeoutInput.text = "10000";
        _eyeSmoothFactorInput.text = "5";
        _mouseSensivityInput.text = "10";
        _backgroundColorPicker.CurrentColour = Color.white; //new Color32(35, 23, 10, 255);
        _targetColorPicker.CurrentColour = Color.black; //new Color32(29, 11, 40, 255);
        _cursorColorPicker.CurrentColour = Color.black; //new Color32(52, 0, 0, 255);
        _hoverColorPicker.CurrentColour = Color.gray; //new Color32(0, 30, 36, 255);
        _buttonDownColorPicker.CurrentColour = Color.gray; //new Color32(100, 100, 100, 255);
        _readyForGestureColorPicker.CurrentColour = Color.red;
    }

    /// <summary>
    /// Parse the user selections into the appropriate format.
    /// If all selections are valid, create a TestBlock and pass the data to TestController.cs.
    /// </summary>
    private void LoadData()
    {
        _dataValidationSuccessful = true;
        _vrHmd = (TestBlock.VRHMD) _vrHmdInput.value;
        _participantCode = _participantCodeInput.text;
        _conditionCode = _conditionCodeInput.text;
        _blockCode = _blockCodeInput.text;
        _randomizeTargetConditions = _randomizeTargetConiditionsInput.isOn;

        //Parse target amplitude field. If format is incorrect, paint the field in red and pop error out message.
        //Correct format is number, followed by one empty space, followed by another number (optionally).
        Int32.TryParse(_numberOfTargetsInput.options[_numberOfTargetsInput.value].text, out _numberOfTargets);
        try
        {
            Debug.Log(_targetAmplitudesInput.text);
            _targetAmplitudes = Array.ConvertAll(_targetAmplitudesInput.text.Split(' '), int.Parse).ToList();
        }
        catch (FormatException)
        {
            ColorBlock cb = _targetAmplitudesInput.GetComponentInChildren<InputField>().colors;
            cb.normalColor = Color.red;
            _targetAmplitudesInput.GetComponentInChildren<InputField>().colors = cb;
            _dataValidationSuccessful = false;
            _errorMessagePanel.SetActive(true);
        }
        //If _randomizeTargetConditions checkbox is ticked, randomize order of amplitudes.
        if (_randomizeTargetConditions)
            _targetAmplitudes = _targetAmplitudes.OrderBy(x => UnityEngine.Random.Range(0, int.MaxValue)).ToList();

        //Parse target width field. If format is incorrect, paint the field in red and pop error out message.
        //Correct format is number, followed by one empty space, followed by another number (optionally).
        try
        {
            _targetWidths = Array.ConvertAll(_targetWidthsInput.text.Split(' '), float.Parse).ToList();
        }
        catch (FormatException)
        {
            ColorBlock cb = _targetWidthsInput.GetComponentInChildren<InputField>().colors;
            cb.normalColor = Color.red;
            _targetWidthsInput.GetComponentInChildren<InputField>().colors = cb;
            _dataValidationSuccessful = false;
            _errorMessagePanel.SetActive(true);
        }
        //If _randomizeTargetConditions checkbox is ticked, randomize order of widths.
        if (_randomizeTargetConditions)
            _targetWidths = _targetWidths.OrderBy(x => UnityEngine.Random.Range(0, int.MaxValue)).ToList();

        float.TryParse(_errorThresholdInput.text, out _errorThreshold);
        float.TryParse(_spatialHysteresisInput.text, out _spatialHysteresis);
        _controlMethod = (TestBlock.ControlMethod) _controlMethodInput.value;
        _confirmationMethod = (TestBlock.ConfirmationMethod) _confirmationMethodInput.value;
        Int32.TryParse(_dwellTimeInput.text, out _dwellTime);
        Int32.TryParse(_timeoutInput.text, out _timeout);
        Int32.TryParse(_eyeSmoothFactorInput.text, out _eyeSmoothFactor);
        float.TryParse(_mouseSensivityInput.text, out _mouseSensivity);
        _beepOnError = _beepOnErrorInput.isOn;
        _showCursor = _showCursorInput.isOn;
        _hoverHighlight = _hoverHighlightInput.isOn;
        _buttonDownHighlight = _buttonDownHighlightInput.isOn;
        _recordGazePosition = _recordGazePositionInput.isOn;
        _backgroundColor = _backgroundColorPicker.CurrentColour;
        _cursorColor = _cursorColorPicker.CurrentColour;
        _targetColor = _targetColorPicker.CurrentColour;
        _buttonDownColor = _buttonDownColorPicker.CurrentColour;
        _hoverColor = _hoverColorPicker.CurrentColour;
        _readyForGestureColor = _readyForGestureColorPicker.CurrentColour;
        
        //Perform further validation, and if successful, create a TestBlock from the data and pass it to TestController.
        DataValidationCheck();
        if(_dataValidationSuccessful)
            TestController.StoreTestData(CreateTestData());
    }

    /// <summary>
    /// Create a TestBlock from existing data.
    /// </summary>
    /// <returns>Return the TestBlock.</returns>
    private TestBlock CreateTestData()
    {
        return new TestBlock(_vrHmd, _participantCode, _conditionCode, _blockCode, _numberOfTargets, _targetAmplitudes, _targetWidths, _errorThreshold, 
            _spatialHysteresis, _controlMethod, _confirmationMethod, _dwellTime, _timeout, _eyeSmoothFactor, _mouseSensivity, _randomizeTargetConditions, _beepOnError, _showCursor, _hoverHighlight, 
            _buttonDownHighlight, _recordGazePosition, _backgroundColor, _cursorColor, _targetColor, _buttonDownColor, _hoverColor, _readyForGestureColor);
    }

    /// <summary>
    /// Generates a string out of the selected values in the GUI and adds separators.
    /// </summary>
    /// <returns>Returns the complete string.</returns>
    private string GenerateSaveString()
    {
        string saveString =  "" + _conditionCodeInput.text + ";" + _blockCodeInput.text + ";" + _numberOfTargetsInput.value + ";";
        for (int i = 0; i < _targetAmplitudes.Count; i++)
        {
            saveString += _targetAmplitudes[i];
            if (i < _targetAmplitudes.Count - 1)
                saveString += "-";
            else
                saveString += ";";
        }
        for (int i = 0; i < _targetWidths.Count; i++) {
            saveString += _targetWidths[i];
            if (i < _targetWidths.Count - 1)
                saveString += "-";
            else
                saveString += ";";
        }
        saveString += _errorThreshold + ";" + _spatialHysteresis + ";" + (int) _controlMethod + ";" +
                      (int) _confirmationMethod + ";" + _dwellTime + ";" + _timeout + ";" + _eyeSmoothFactor + ";" +_mouseSensivity + ";" + _randomizeTargetConditions +
                      ";" + _beepOnError + ";" + _showCursor + ";" + _hoverHighlight + ";" + _buttonDownHighlight + ";" + _recordGazePosition + ";" +
                      _backgroundColor.r + "-" + _backgroundColor.g + "-" + _backgroundColor.b + "-" + _backgroundColor.a + ";" +
                      _cursorColor.r + "-" + _cursorColor.g + "-" + _cursorColor.b + "-" + _cursorColor.a + ";" +
                      _targetColor.r + "-" + _targetColor.g + "-" + _targetColor.b + "-" + _targetColor.a + ";" +
                      _buttonDownColor.r + "-" + _buttonDownColor.g + "-" + _buttonDownColor.b + "-" +
                      _buttonDownColor.a + ";" + _hoverColor.r + "-" + _hoverColor.g + "-" + _hoverColor.b + "-" +
                      _hoverColor.a + ";" + _readyForGestureColor.r + "-" + _readyForGestureColor.g + "-" +
                      _readyForGestureColor.b + "-" + _readyForGestureColor.a + ";";
        return saveString;
    }

    /// <summary>
    /// Parses a string parameter and sets the values in the GUI based on the parsed values.
    /// </summary>
    /// <param name="loadString">The string parameter to be parsed.</param>
    private void LoadDataFromString(string loadString)
    {
        string[] data = loadString.Split(';');

        _conditionCodeInput.text = data[0];
        _blockCodeInput.text = data[1];
        _numberOfTargetsInput.value = int.Parse(data[2]);

        string inputString = "";
        string[] dataString = data[3].Split('-');
        for (int i = 0; i < dataString.Length-1; i++)
        {
            inputString += dataString[i] + " ";
        }
        inputString += dataString[dataString.Length - 1];
        _targetAmplitudesInput.text = inputString;

        inputString = "";
        dataString = data[4].Split('-');
        for (int i = 0; i < dataString.Length - 1; i++) {
            inputString += dataString[i] + " ";
        }
        inputString += dataString[dataString.Length - 1];
        _targetWidthsInput.text = inputString;
        _errorThresholdInput.text = data[5];
        _spatialHysteresisInput.text = data[6];
        _controlMethodInput.value = int.Parse(data[7]);
        _confirmationMethodInput.value = int.Parse(data[8]);
        _dwellTimeInput.text = data[9];
        _timeoutInput.text = data[10];
        _eyeSmoothFactorInput.text = data[11];
        _mouseSensivityInput.text = data[12];
        _randomizeTargetConiditionsInput.isOn = bool.Parse(data[13]);
        _beepOnErrorInput.isOn = bool.Parse(data[14]);
        _showCursorInput.isOn = bool.Parse(data[15]);
        _hoverHighlightInput.isOn = bool.Parse(data[16]);
        _buttonDownHighlightInput.isOn = bool.Parse(data[17]);
        _recordGazePositionInput.isOn = bool.Parse(data[18]);
        string colorstring = data[19];
        string[] colorvalues = colorstring.Split('-');
        _backgroundColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

        colorstring = data[20];
        colorvalues = colorstring.Split('-');
        _cursorColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

        colorstring = data[21];
        colorvalues = colorstring.Split('-');
        _targetColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

        colorstring = data[22];
        colorvalues = colorstring.Split('-');
        _buttonDownColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

        colorstring = data[23];
        colorvalues = colorstring.Split('-');
        _hoverColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

        colorstring = data[24];
        colorvalues = colorstring.Split('-');
        _readyForGestureColorPicker.CurrentColour = new Color(float.Parse(colorvalues[0]), float.Parse(colorvalues[1]), float.Parse(colorvalues[2]), float.Parse(colorvalues[3]));

    }

    /// <summary>
    /// If data selected in GUI is validated successfully through LoadData(), load the main experiment scene.
    /// </summary>
    public void RunExperiment()
    {
        LoadData();
        if(_dataValidationSuccessful)
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// This function handles the UI pop-up functionality when the user clicks the Save Setup button.
    /// It displays the relevant UI element with a string made up of all UI field selections.
    /// Also saves the string to disk for future use. 
    /// </summary>
    public void SaveButtonClicked()
    {
        LoadData();
        _saveStringPanel.SetActive(true);
        _saveStringInput.text = GenerateSaveString();
        _saveStringInput.readOnly = true;

        //Save file to disk as well
        System.IO.File.WriteAllText(_filePath, _saveStringInput.text);
    }

    /// <summary>
    /// This function handles the UI pop-up functionality when the user clicks the Load Setup button.
    /// It displays the relevant UI element with a field that takes a string, formatted in the same way as strings 
    /// generated by the GenerateSaveString() function.
    /// </summary>
    public void LoadButtonClicked()
    {
        _saveStringPanel.SetActive(true);
        _saveStringInput.readOnly = false;
        _saveStringInput.text = "";
    }

    /// <summary>
    /// This function holds the relevant functionality for the Reset Setup button.
    /// When clicked, it deletes any existing saved string setups and resets the UI element values
    /// to the defaults set here.
    /// </summary>
    public void ResetButtonClicked()
    {
        //Delete save file .txt
        if (File.Exists(_filePath))
            System.IO.File.Delete(_filePath);
        //Reset UI values to defaults
        _vrHmdInput.value = 1;
        _participantCodeInput.text = "";
        _conditionCodeInput.text = "";
        _blockCodeInput.text = "";
        _numberOfTargetsInput.value = 1;
        _targetAmplitudesInput.text = "80 120";
        _targetWidthsInput.text = "50 75";
        _errorThresholdInput.text = "20";
        _spatialHysteresisInput.text = "2.0";
        _controlMethodInput.value = 1;
        _confirmationMethodInput.value = 0;
        _dwellTimeInput.text = "300";
        _timeoutInput.text = "10000";
        _eyeSmoothFactorInput.text = "5";
        _mouseSensivityInput.text = "10";
        _backgroundColorPicker.CurrentColour = new Color32(35, 23, 10, 255);
        _targetColorPicker.CurrentColour = new Color32(29, 11, 40, 255);
        _cursorColorPicker.CurrentColour = new Color32(52, 0, 0, 255);
        _hoverColorPicker.CurrentColour = new Color32(0, 30, 36, 255);
        _buttonDownColorPicker.CurrentColour = new Color32(100, 100, 100, 255);
        _readyForGestureColorPicker.CurrentColour = Color.red;
    }

    /// <summary>
    /// This function exits the application if the user clicks the Exit button.
    /// </summary>
    public void ExitButtonClicked()
    {
        Application.Quit();
    }

    /// <summary>
    /// When the user clicks the OK button, this function deactivates the Save Setup pop-up.
    /// </summary>
    public void SaveStringOKButtonClicked()
    {
        LoadDataFromString(_saveStringInput.text);
        _saveStringPanel.SetActive(false);
    }

    /// <summary>
    /// When the user clicks the cancel button, this function deactivates the Save Setup pop-up.
    /// </summary>
    public void SaveStringCancelButtonClicked()
    {
        _saveStringPanel.SetActive(false);
    }

    /// <summary>
    /// When the user clicks the OK button, this function deactivates the error message pop-up.
    /// </summary>
    public void ErrorMessageOKButtonClicked()
    {
        _errorMessagePanel.SetActive(false);
    }

    public void VRHMDChanged()
    {
        switch (_vrHmdInput.value)
        {
        //FOVE
            case 0:
                _foveRig.gameObject.SetActive(true);
                _viveRig.gameObject.SetActive(false);
                _vrCanvas.worldCamera = _foveRig.transform.Find("Fove Interface").GetComponent<Camera>();
                break;
        //Vive
            case 1:
                _foveRig.gameObject.SetActive(false);
                _viveRig.gameObject.SetActive(true);
                _vrCanvas.worldCamera = _viveRig.transform.Find("Camera (eye)").GetComponent<Camera>();
                break;
        //No VRHMD
            case 2:
                _foveRig.gameObject.SetActive(false);
                _viveRig.gameObject.SetActive(false);
                _vrCanvas.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// This function performs validation on the user selections in the UI.
    /// It'll change the _dataValidationSuccessful flag to false if a discrepancy is detected,
    /// activate the error message pop-up and highlight the problem fields in red.
    /// </summary>
    public void DataValidationCheck()
    {
        //Hand pointer with a vrHmd that isn't VIVE
        if (_controlMethod == TestBlock.ControlMethod.HandPointer && _vrHmd != TestBlock.VRHMD.VIVE) {
            ColorBlock ca = _controlMethodInput.GetComponentInChildren<Dropdown>().colors;
            ca.normalColor = Color.red;
            _controlMethodInput.GetComponentInChildren<Dropdown>().colors = ca;
            ColorBlock cb = _vrHmdInput.GetComponentInChildren<Dropdown>().colors;
            cb.normalColor = Color.red;
            _vrHmdInput.GetComponentInChildren<Dropdown>().colors = cb;
            _dataValidationSuccessful = false;
            _errorMessagePanel.SetActive(true);
        }

        //Eyetracking and Headtracking with NoHMD
        if ((_controlMethod == TestBlock.ControlMethod.Eyetracking || _controlMethod == TestBlock.ControlMethod.Headtracking)
            && _vrHmd == TestBlock.VRHMD.NoHMD)
        {
            ColorBlock ca = _controlMethodInput.GetComponentInChildren<Dropdown>().colors;
            ca.normalColor = Color.red;
            _controlMethodInput.GetComponentInChildren<Dropdown>().colors = ca;
            ColorBlock cb = _vrHmdInput.GetComponentInChildren<Dropdown>().colors;
            cb.normalColor = Color.red;
            _vrHmdInput.GetComponentInChildren<Dropdown>().colors = cb;
            _dataValidationSuccessful = false;
            _errorMessagePanel.SetActive(true);
        }

        //HeadGesture with NoHMD
        if (_confirmationMethod == TestBlock.ConfirmationMethod.HeadGesture && _vrHmd == TestBlock.VRHMD.NoHMD)
        {
            ColorBlock ca = _confirmationMethodInput.GetComponentInChildren<Dropdown>().colors;
            ca.normalColor = Color.red;
            _confirmationMethodInput.GetComponentInChildren<Dropdown>().colors = ca;
            ColorBlock cb = _vrHmdInput.GetComponentInChildren<Dropdown>().colors;
            cb.normalColor = Color.red;
            _vrHmdInput.GetComponentInChildren<Dropdown>().colors = cb;
            _dataValidationSuccessful = false;
            _errorMessagePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Resets the color of the control method field to white.
    /// </summary>
    public void ResetControlMethodColor()
    {
        ColorBlock cb = _controlMethodInput.GetComponentInChildren<Dropdown>().colors;
        cb.normalColor = Color.white;
        _controlMethodInput.GetComponentInChildren<Dropdown>().colors = cb;
        ResetVrhmdColor();
    }

    /// <summary>
    /// Resets the color of the confirmation method field to white.
    /// </summary>
    public void ResetConfirmationMethodColor()
    {
        ColorBlock cb = _confirmationMethodInput.GetComponentInChildren<Dropdown>().colors;
        cb.normalColor = Color.white;
        _confirmationMethodInput.GetComponentInChildren<Dropdown>().colors = cb;
        ResetVrhmdColor();
    }

    /// <summary>
    /// Resets the color of the Vrhmd field to white.
    /// </summary>
    public void ResetVrhmdColor()
    {
        ColorBlock cb = _vrHmdInput.GetComponentInChildren<Dropdown>().colors;
        cb.normalColor = Color.white;
        _vrHmdInput.GetComponentInChildren<Dropdown>().colors = cb;
    }

    /// <summary>
    /// Resets the color of the target amplitudes field to white.
    /// </summary>
    public void ResetAmplitudesColor()
    {
        ColorBlock cb = _targetAmplitudesInput.GetComponentInChildren<InputField>().colors;
        cb.normalColor = Color.white;
        _targetAmplitudesInput.GetComponentInChildren<InputField>().colors = cb;
    }

    /// <summary>
    /// Resets the color of the target widths field to white.
    /// </summary>
    public void ResetWidthsColor()
    {
        ColorBlock cb = _targetWidthsInput.GetComponentInChildren<InputField>().colors;
        cb.normalColor = Color.white;
        _targetWidthsInput.GetComponentInChildren<InputField>().colors = cb;
    }
}
