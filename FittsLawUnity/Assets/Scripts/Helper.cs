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
}
