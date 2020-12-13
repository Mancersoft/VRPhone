using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using UnityEngine;
using System.Reflection;

   /// <summary>
   /// This class facilitates SQLite operations with a local database. It works with the test data stored
   /// in Data Transfer Objects(DTOs) and an SQLiteConnection object which communicates with the database.
   /// </summary>
public class DBController : MonoBehaviour
{
    public static DBController Instance;

    private SQLiteConnection _connection;

    // Database variables.
    private const string DATABASE_NAME = "TestResults.db";
    private const string FILELOCATION = @"Assets/StreamingAssets/ResultData/" + DATABASE_NAME;

    void Awake() {
        Instance = this;
        // Create database if it doesn't exist.
        if (!File.Exists(FILELOCATION))
        {
            File.Create(FILELOCATION).Dispose();
        }

        // Establish connection to database and create needed tables.
        _connection = new SQLiteConnection(FILELOCATION, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _connection.CreateTable<TestBlockDTO>();
        _connection.CreateTable<TestSequenceDTO>();
        _connection.CreateTable<TestTrialDTO>();
        _connection.CreateTable<DataLogDTO>();
        _connection.CreateTable<TestVerificationDTO>();

    }

    /// <summary>
    /// Insert results into the database.
    /// </summary>
    /// <param name="block"></param>
    public void InsertTestResults(TestBlockDTO block) {
       
        List<TestSequenceDTO> sequences = block.TestSequenceDTOs;
        List<TestTrialDTO> trials = new List<TestTrialDTO>();
        List<DataLogDTO> logs = new List<DataLogDTO>();
        List<TestVerificationDTO> verifications = block.TestVerificationDTOs;
        foreach (TestSequenceDTO sequenceDto in block.TestSequenceDTOs) {
            foreach (TestTrialDTO trialDto in sequenceDto.TestTrialDTOs) {
                trials.Add(trialDto);
                logs.AddRange(trialDto.DataLogDTOs);
            }
        }

        _connection.Insert(block);
        _connection.InsertAll(sequences);
        _connection.InsertAll(trials);
        _connection.InsertAll(logs);
        _connection.InsertAll(verifications);
    }
    
    /// <summary>
    /// </summary>
    /// <returns>Returns the last block, sequence and trial Ids from the database.</returns>
    public LastTableIds GetLastTableIds()
    {
        SQLiteCommand cmd = _connection.CreateCommand("SELECT seq FROM 'sqlite_sequence' WHERE name='TestBlocks'");
        int blockId = cmd.ExecuteScalar<int>();
        cmd = _connection.CreateCommand("SELECT seq FROM 'sqlite_sequence' WHERE name='TestSequences'");
        int sequnceId = cmd.ExecuteScalar<int>();
        cmd = _connection.CreateCommand("SELECT seq FROM 'sqlite_sequence' WHERE name='TestTrials'");
        int trialId = cmd.ExecuteScalar<int>();

        return new LastTableIds {BlockId = blockId, SequenceId = sequnceId, TrialId = trialId};
    }

    public struct LastTableIds
    {
        public int BlockId;
        public int SequenceId;
        public int TrialId;
    }
}
