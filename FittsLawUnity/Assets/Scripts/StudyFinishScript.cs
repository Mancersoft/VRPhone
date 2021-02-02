using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyFinishScript : MonoBehaviour {

	public Text textWait;
	public Text textThanks;
	public Button buttonQuit;

	public static StudyFinishScript Instance;

	void Awake()
    {
		Instance = this;
    }

	void Start () {
		StudyManager.Instance.SendLogData();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	public void EmailSent()
    {
		textWait.gameObject.SetActive(false);
		textThanks.gameObject.SetActive(true);
		buttonQuit.gameObject.SetActive(true);

	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			Quit();
        }
    }

	public void Quit()
    {
		Application.Quit();
    }
}
