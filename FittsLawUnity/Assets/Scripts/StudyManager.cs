using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class StudyManager : MonoBehaviour {

	public enum Conditions
    {
		Direct,
		Indirect,
		UnWarped,
		Warped
    }

	public static StudyManager Instance;

	public string ParticipantId;
	public string BlockId;

	public Conditions Condition;

	public float ScaleFactor;

	void Awake()
    {
		Instance = this;
		ParticipantId = "P" + (PlayerPrefs.GetInt("ParticipantId", 0) + 1);
		BlockId = "B" + (PlayerPrefs.GetInt("BlockId", 0) + 1);
		Condition = Conditions.Direct;
	}

	public void SetParams(string participantId, string blockId, Conditions condition)
    {
		ParticipantId = participantId;
		PlayerPrefs.SetInt("ParticipantId", ParticipantId.Length < 2 ? 0 : int.Parse(ParticipantId.Substring(1)));
		BlockId = blockId;
		PlayerPrefs.SetInt("BlockId", BlockId.Length < 2 ? 0 : int.Parse(BlockId.Substring(1)));
		Condition = condition;
    }

	public void SendData(string data)
    {
		EmailSender.SenderEmail = "fittslaw@yahoo.com";
		EmailSender.SenderPassword = "uissgkvbaoehcxvo";
		EmailSender.SmtpClient = "smtp.mail.yahoo.com";
		EmailSender.SmtpPort = 587;

		string outputTextPath = Application.persistentDataPath + "/" + "VRPhone_P_DATA_" + DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss") + "_" + ParticipantId + "_" + BlockId + ".txt";

		if (!File.Exists(outputTextPath))
		{
			File.WriteAllText(outputTextPath, DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss") + "_" + ParticipantId + "_" + BlockId + " \n\n");
		}

		File.AppendAllText(outputTextPath, data);

		string body = "Email id: " + Guid.NewGuid() +  "\nPlease Find Below the Logs of the player " + ParticipantId + ", There is also a log file attached to this email.";

		EmailSender.SendEmail("mancersoftfiles@gmail.com", "VRPhone PARTICIPANT DATA " + ParticipantId + "_" + BlockId, body, false, new string[] { outputTextPath }, EmailSendCallback);
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
	}
}
