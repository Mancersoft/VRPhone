using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StudyManager : MonoBehaviour {

	public enum Conditions
    {
		Direct,
		Indirect,
		UnWarped,
		Warped
    }

	public enum StudyEnum
    {
		Study1,
		Study2
    }

	public class StudyBlock
    {
		public string Id { get; set; }

		public Conditions Condition { get; set; }

		public float ScaleFactor { get; set; }
    }

	public static StudyManager Instance;

	public string ParticipantId;
	public string BlockId;

	public Conditions Condition;
	public StudyEnum StudyNumber;

	public float ScaleFactor;

	public List<StudyBlock> Blocks;

	public TestBlock initBlock;

	public bool isTest = true;

	private string logDataGeneral;

	private string logDataDetail;

	private string logDataTrials;

	private string studyStartTime = "";

	private void InitLogs()
    {
		logDataGeneral = "ParticipantCode, BlockCode, StudyNumber, ConditionCode, StartTime, VRHMD, ErrorThreshold, SelectedControlMethod, SelectedConfirmationMethod, DwellTime, Timeout, ScaleFactor, MovementTime, ErrorRate, Throughput";
		logDataDetail = "ParticipantCode, BlockCode, StudyNumber, ConditionCode, SequenceNumber, IoD, W, A, IoDe, We, Ae, StartTime, VRHMD, ErrorThreshold, SelectedControlMethod, SelectedConfirmationMethod, DwellTime, Timeout, ScaleFactor, MovementTime, ErrorRate, Throughput";
		logDataTrials = "ParticipantCode, BlockCode, StudyNumber, ConditionCode, ScaleFactor, SequenceNumber, TrialNumber, TargetAngle, StartTime, TotalCursorMovement, TimeToActivate, TimeToFixate, TargetCenterErrorX, TargetCenterErrorY, TimedOut, Error";
		studyStartTime = DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss");
	}

	public void AddLogGeneral(TestBlock testBlock)
    {
		if (isTest)
        {
			return;
        }

		logDataGeneral += "\n" + string.Join(", ", new string[] { testBlock.ParticipantCode, testBlock.BlockCode, StudyNumber.ToString(),
			testBlock.ConditionCode, testBlock.StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), "Cardboard",
			testBlock.ErrorThreshold.ToString(), testBlock.SelectedControlMethod.ToString(),
			testBlock.SelectedConfirmationMethod.ToString(),
			testBlock.DwellTime.ToString(), testBlock.Timeout.ToString(), testBlock.MouseSensivity.ToString(),
			testBlock.MovementTime.ToString(), testBlock.ErrorRate.ToString(), testBlock.Throughput.ToString()});
	}

	public void AddLogDetail(TestBlock testBlock, int sequenceIndex)
	{
		if (isTest)
        {
			return;
        }

		TestSequence testSequence = testBlock.Sequences[sequenceIndex];
		logDataDetail += "\n" + string.Join(", ", new string[] { testBlock.ParticipantCode, testBlock.BlockCode, StudyNumber.ToString(),
			testBlock.ConditionCode, testSequence.SequenceNumber.ToString(),
			testSequence.IndexOfDifficulty.ToString(), testSequence.TargetWidth.ToString(), testSequence.TargetAmplitude.ToString(),
			testSequence.EffecttiveIndexOfDifficulty.ToString(), testSequence.EffectiveTargetWidth.ToString(), testSequence.EffectiveAmplitude.ToString(),
			testSequence.StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), "Cardboard", testBlock.ErrorThreshold.ToString(), 
			testBlock.SelectedControlMethod.ToString(), testBlock.SelectedConfirmationMethod.ToString(),
			testBlock.DwellTime.ToString(), testBlock.Timeout.ToString(), testBlock.MouseSensivity.ToString(),
			testSequence.MovementTime.ToString(), testSequence.ErrorRate.ToString(), testSequence.Throughput.ToString()});
	}

	public void AddLogTrial(TestBlock testBlock, int sequenceIndex, int trialNumber)
    {
		if (isTest)
		{
			return;
		}

		TestSequence testSequence = testBlock.Sequences[sequenceIndex];
		TestTrial trial = testSequence.Trials[trialNumber];
		logDataTrials += "\n" + string.Join(", ", new string[] { testBlock.ParticipantCode, testBlock.BlockCode, StudyNumber.ToString(),
			testBlock.ConditionCode, testBlock.MouseSensivity.ToString(), testSequence.SequenceNumber.ToString(),
			trial.TrialNumber.ToString(), trial.TargetAngle.ToString(),
			trial.StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt"),
			trial.TotalCursorMovement.ToString(), trial.TimeToActivate.ToString(),
			trial.TimeToFixate.ToString(), trial.TargetCenterError.x.ToString(), trial.TargetCenterError.y.ToString(),
			trial.TimedOut.ToString(), trial.Error.ToString()});
	}

	void Awake()
    {
		if (Instance != null)
        {
			Destroy(gameObject);
			return;
        }

		DontDestroyOnLoad(this);
		Instance = this;
		ParticipantId = "P" + (PlayerPrefs.GetInt("ParticipantId", 0) + 1);
		BlockId = "B1";
		Condition = Conditions.Direct;
		StudyNumber = StudyEnum.Study1;

	}

	private const int Study1BlockCount = 6; // 2;

	private readonly float[] ScaleFactors = new float[] { 1f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f, 2.2f, 2.4f };

	public const string TargetAmplitudesStudy1 =	"115 145 295 400 265 345 220 350 440 445";
	public const string TargetWidthsStudy1 =		"100 100 160 160 80 80 40 50 50 40";

	public const string TargetAmplitudesStudy2 =	"115 295 400 290 375 325 410 445";
	public const string TargetWidthsStudy2 =		"100 160 160 80 80 50 50 40";

	public void SetParams(string participantId, string blockId, Conditions condition, StudyEnum studyNumber, float scaleFactor, bool isTest)
    {
		InitLogs();
		this.isTest = isTest;
		Debug.Log("Is test: " + isTest);
		ParticipantId = participantId;
		BlockId = blockId;
		Condition = condition;
		StudyNumber = studyNumber;
		ScaleFactor = scaleFactor;

		int participantIdInt = int.Parse(ParticipantId.Substring(1));
		int blockIdInt = int.Parse(BlockId.Substring(1));
		switch (studyNumber)
        {
			case StudyEnum.Study1:
				Blocks = new List<StudyBlock>();
				int conditionBlockCount = Study1BlockCount / 2;
				var currCondition = Condition;
				for (int i = 0; i < conditionBlockCount; ++i)
                {
					Blocks.Add(new StudyBlock
					{
						Id = "B" + blockIdInt,
						Condition = currCondition,
						ScaleFactor = 1f
					});
					blockIdInt++;
				}
				currCondition = currCondition == Conditions.Direct ? Conditions.Indirect : Conditions.Direct;
				for (int i = 0; i < conditionBlockCount; ++i)
				{
					Blocks.Add(new StudyBlock
					{
						Id = "B" + blockIdInt,
						Condition = currCondition,
						ScaleFactor = 1f
					});
					blockIdInt++;
				}
				break;
			case StudyEnum.Study2:
				Blocks = new List<StudyBlock>();
				var study2BlockCount = ScaleFactors.Length;
				for (int i = 0; i < study2BlockCount; i++)
                {
					Blocks.Add(new StudyBlock
					{
						Id = "B" + blockIdInt,
						Condition = Conditions.Warped,
						ScaleFactor = ScaleFactors[i]
					});
					blockIdInt++;
				}
				break;
        }

		if (!isTest)
        {
			SetBlockValues(1);
		}
	}

	public void StartChangeBlock()
    {
		var blockIdInt = int.Parse(BlockId.Substring(1));
		if (blockIdInt == Blocks.Count)
		{
			SceneManager.LoadScene(2);
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			ToNextBlock();
			TestController.StoreTestData(initBlock);
			SceneManager.LoadScene(1);
		}
	}

	public void SendLogData()
    {
		if (isTest)
        {
			return;
        }

		this.SendData(logDataGeneral);
	}

	public void ToNextBlock()
    {
		if (isTest)
        {
			return;
        }

		var blockIdInt = int.Parse(BlockId.Substring(1)) + 1;
		SetBlockValues(blockIdInt);
	}

	public void SetBlockValues(int blockIdInt)
    {
		BlockId = "B" + blockIdInt;
		Debug.Log("Block id: " + BlockId);
		Condition = Blocks[blockIdInt - 1].Condition;
		ScaleFactor = Blocks[blockIdInt - 1].ScaleFactor;
		Debug.Log(Condition);
		initBlock.ParticipantCode = ParticipantId;
		initBlock.BlockCode = Blocks[blockIdInt - 1].Id;
		initBlock.MouseSensivity = Blocks[blockIdInt - 1].ScaleFactor;
		initBlock.ConditionCode = Blocks[blockIdInt - 1].Condition.ToString();
	}

	private void SendData(string data)
    {
		EmailSender.SenderEmail = "fittslaw@yahoo.com";
		EmailSender.SenderPassword = "uissgkvbaoehcxvo";
		EmailSender.SmtpClient = "smtp.mail.yahoo.com";
		EmailSender.SmtpPort = 587;

		string filePathGeneral = WriteFileGetPath(logDataGeneral, LogType.General);
		string filePathDetail = WriteFileGetPath(logDataDetail, LogType.Detail);

		string body = "Email id: " + Guid.NewGuid() +  "\nPlease Find Below the Logs of the player " + ParticipantId + ", There is also a log file attached to this email.";

		EmailSender.SendEmail("mancersoftfiles@gmail.com", "VRPhone PARTICIPANT DATA " + ParticipantId, body, false, new string[] { filePathGeneral, filePathDetail }, EmailSendCallback);
    }

	public void SaveLogFiles()
    {
		WriteFileGetPath(logDataGeneral, LogType.General);
		WriteFileGetPath(logDataDetail, LogType.Detail);
		WriteFileGetPath(logDataTrials, LogType.Trials);
	}

	private enum LogType
    {
		General,
		Detail,
		Trials
    }

	private string WriteFileGetPath(string data, LogType logType)
    {
		string detail = "";
		switch (logType)
        {
			case LogType.General:
				detail = "_General";
				break;
			case LogType.Detail:
				detail = "_Detail";
				break;
			case LogType.Trials:
				detail = "_Trials";
				break;
        }

		string outputTextPath = Application.persistentDataPath + "/" + "VRPhone_DATA_" + studyStartTime + "_" + ParticipantId + detail + ".csv";

		if (!File.Exists(outputTextPath))
		{
			File.WriteAllText(outputTextPath, studyStartTime + "_" + ParticipantId + " \n\n");
		}

		File.AppendAllText(outputTextPath, data);
		return outputTextPath;
	}

    private void EmailSendCallback(object arg, AsyncCompletedEventArgs e)
    {
		if (e.Cancelled || e.Error != null)
		{
			print("Email not sent: " + e.Error.Message);
		}
		else
		{
			print("Email successfully sent.");
		}

		StudyFinishScript.Instance.EmailSent();
	}
}
