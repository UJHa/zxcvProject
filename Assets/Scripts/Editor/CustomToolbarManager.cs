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

    private ConvertCodeClass _convertCodeClass;

    private void ExecuteJsonGenerator()
    {
        string jsonPath = UmUtil.GetResourceJsonPath();
        string dataClassPath = UmUtil.GetDataClassPath();
        AddLog($"[testumJson]Application.dataPath({Application.dataPath})");
        ReadFiles(jsonPath);
        ReadFiles(dataClassPath);

        // 샘플 Table.cs 읽어오기
        _convertCodeClass = new();
        string tableName = "CharacterRoleState";
        _convertCodeClass.Init($"{tableName}.json");
        _convertCodeClass.ConvertVariable(out var codeLineResult);

        ConvertClassName(codeLineResult, tableName);

        // 임시 생성
        {
            string filePath = Path.Combine(dataClassPath, $"{tableName}Table.cs");
            // File.WriteAllText(filePath, tableCode.Replace("_Sample_", "Test"));
            // todo codeLineResult _sample 변환
            AddLog($"length({codeLineResult.Count})");
            File.WriteAllLines(filePath, codeLineResult.ToArray(), Encoding.UTF8);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
        }
    }

    private void ConvertClassName(List<string> codeLineResult, string tableName)
    {
        for (int i = 0; i < codeLineResult.Count; i++)
        {
            codeLineResult[i] = codeLineResult[i].Replace("_Sample_", tableName).Replace(Environment.NewLine, "");
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

    public static void AddLog(string log)
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