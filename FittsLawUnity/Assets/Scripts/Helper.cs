using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper {

    public static List<DataLog> GetDataLogsFromBlock(TestBlock block)
    {
        List<DataLog> dataLogs = new List<DataLog>();
        foreach (TestSequence trial in block.Sequences)
        {
            foreach (TestTrial step in trial.Trials)
            {
                dataLogs.AddRange(step.Logs);
            }
        }
        return dataLogs;
    }

    public static List<TestTrial> GetStepsFromBlock(TestBlock block)
    {
        List<TestTrial> steps = new List<TestTrial>();
        foreach (TestSequence trial in block.Sequences)
        {
            steps.AddRange(trial.Trials);
        }
        return steps;
    }

    private const string ACTION = "com.mancersoft.FITTS_LAW";
    private const string EXTRA_PARAM = "CONDITION";

    public static void SendConditionBroadcast()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[1] { ACTION });
        objIntent.Call<AndroidJavaObject>("putExtra", new object[2] { EXTRA_PARAM, (int)StudyManager.Instance.Condition });
        objActivity.Call("sendBroadcast", objIntent);
        Debug.Log("Condition Broadcast sent, condition: " + StudyManager.Instance.Condition);
#endif
    }
}
