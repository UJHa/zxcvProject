using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Utils;

public class CustomToolbarManager : EditorWindow
{
    private static bool enableLog = true;
    private static CustomToolbarManager _instance;
    
    private static List<string> _logLines = new();
    
    [MenuItem("Json/Execute JsonGenerator")]
    public static void ExecuteButton()
    {
        AddLog($"[test]CustomToolbar");
        _instance = new();
        _instance.ExecuteJsonGenerator();
        _instance = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        CompilationPipeline.compilationFinished -= OnCompilationFinished;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog;
    } 

    private void ExecuteJsonGenerator()
    {
        string jsonPath = UmUtil.GetResourceJsonPath();
        string dataClassPath = UmUtil.GetDataClassPath();
        AddLog($"[testumJson]Application.dataPath({Application.dataPath})");
        ReadFiles(jsonPath);
        ReadFiles(dataClassPath);
        // 샘플 Table.cs 읽어오기
        string sampleCodefilePath = Path.Combine(dataClassPath, "SampleTable.cs");
        List<string> tableCodeLines = GetFileLines(sampleCodefilePath);
        int declareStartIndex = 0;
        int declareEndIndex = 0;
        for (int i = 0; i < tableCodeLines.Count; i++)
        {
            var line = tableCodeLines[i];
            AddLog($"[codeLine]{line}");
            if (line.Contains("Declaration Values[Start]"))
                declareStartIndex = i;
            if (line.Contains("Declaration Values[End]"))
                declareEndIndex = i;
        }

        List<string> codeLineResult = new();

        if (declareStartIndex < declareEndIndex)
        {
            AddLog($"[declare]Success");
            var declarePrevLines = tableCodeLines.GetRange(0, declareStartIndex);
            var declareLines = tableCodeLines.GetRange(declareStartIndex, declareEndIndex - declareStartIndex + 1);
            var declareNextLines = tableCodeLines.GetRange(declareEndIndex + 1, tableCodeLines.Count - (declareEndIndex + 1));
            codeLineResult = tableCodeLines.GetRange(0, declareStartIndex);
            codeLineResult.AddRange(declareLines);
            codeLineResult.AddRange(declareNextLines);
            // PrintLines("declareCodeLinePrev", declarePrevLines);
            // // PrintLines("declareCodeLine", new List<string>(){"string profileName;"});
            // PrintLines("declareCodeLine", declareLines);
            // PrintLines("declareCodeLineNext", declareNextLines);
        }
        else
        {
            AddLog($"[declare]Failed");
        }

        ConvertSample(codeLineResult);

        // 임시 생성
        {
            string filePath = Path.Combine(dataClassPath, "CharacterStatTableTest.cs");
            // File.WriteAllText(filePath, tableCode.Replace("_Sample_", "Test"));
            // todo codeLineResult _sample 변환
            AddLog($"length({codeLineResult.Count})");
            File.WriteAllLines(filePath, codeLineResult.ToArray(), Encoding.UTF8);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
        }
    }

    private void ConvertSample(List<string> codeLineResult)
    {
        for (int i = 0; i < codeLineResult.Count; i++)
        {
            codeLineResult[i] = codeLineResult[i].Replace("_Sample_", "Test").Replace(Environment.NewLine, "");
        }
    }

    private void PrintLines(string checkCategory, List<string> lines)
    {
        foreach (var line in lines)
        {
            AddLog($"[{checkCategory}]{line.Replace("\\", "\\\\")}");

        }
    }

    private void ReadFiles(string path)
    {
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            if (file.Name.Contains(".meta"))
                continue;
            AddLog($"[testFile]fileName({file.Name})");
        }
    }
    
    private List<string> GetFileLines(string path)
    {
        var result = File.ReadAllText(path);
        return result.Split(Environment.NewLine).ToList();
    }

    private static void AddLog(string log)
    {
        if (enableLog)
            _logLines.Add(log);
    }
    
    private static void ExecuteLog()
    {
        foreach (var log in _logLines)
        {
            Debug.Log($"{log}");
        }
        _logLines.Clear();
    }
    
    private static void OnCompilationFinished(object sender)
    {
        Debug.Log("스크립트 컴파일이 완료되었습니다.");
        ExecuteLog();
    }
    
    private static string logFilePath = "CustomToolbarManagerLog.txt";

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 로그 파일에 내용 추가
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logString);
        }
    }
}