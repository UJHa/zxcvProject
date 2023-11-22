using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using Utils;

public struct ConvertInfo
{
    public int startIndex;
    public int endIndex;
    public string blankStr;
    public List<string> lines;
}

public class ConvertCodeClass
{
    private List<string> _templateCodeLines = new();
    private JArray _jsonArray;
    private string _jsonFileName;
    private ConvertInfo _declareInfo;
    private ConvertInfo _contructInfo;
    private ConvertInfo _toStringInfo;
    public void Init()
    {
        ClearConvertInfos();
        LoadTemplateCodeLines();
        for (int i = 0; i < _templateCodeLines.Count; i++)
        {
            var line = _templateCodeLines[i];
            if (line.Contains("Construct Values[Start]"))
            {
                _contructInfo.startIndex = i;
                _contructInfo.blankStr = line.Split("//")[0];
            }
            if (line.Contains("Construct Values[End]"))
            {
                _contructInfo.endIndex = i;
            }
            if (line.Contains("Declaration Values[Start]"))
            {
                _declareInfo.startIndex = i;
                _declareInfo.blankStr = line.Split("//")[0] + "";
            }

            if (line.Contains("Declaration Values[End]"))
            {
                _declareInfo.endIndex = i;
            }
            if (line.Contains("ToString Values[Start]"))
            {
                _toStringInfo.startIndex = i;
                _toStringInfo.blankStr = line.Split("//")[0] + "";
            }

            if (line.Contains("ToString Values[End]"))
            {
                _toStringInfo.endIndex = i;
            }
        }
        AddLog($"[lineCheck]Construct({_contructInfo.startIndex}/{_contructInfo.endIndex})\n" +
               $"_declareInfo({_declareInfo.startIndex}/{_declareInfo.endIndex})\n" +
               $"_toStringInfo({_toStringInfo.startIndex}/{_toStringInfo.endIndex})");
    }

    public void LoadTable(string tableName)
    {
        _jsonFileName = $"{tableName}.json";
        LoadJsonFileLines();
    }

    public void ClearConvertInfos()
    {
        _declareInfo = new() {lines = new()};
        _contructInfo = new() {lines = new()};
        _toStringInfo = new() {lines = new()};
    }

    private void LoadTemplateCodeLines()
    {
        string dataClassPath = UmUtil.GetDataClassPath();
        string sampleCodefilePath = Path.Combine(dataClassPath, "SampleTable.cs");
        _templateCodeLines = GetFileLines(sampleCodefilePath);
        for (int i = 0; i < _templateCodeLines.Count; i++)
        {
            var line = _templateCodeLines[i];
            AddLog($"[codeLine]{line}");
        }
    }
    
    private void LoadJsonFileLines()
    {
        string resourceJsonPath = UmUtil.GetResourceJsonPath();
        string jsonPath = Path.Combine(resourceJsonPath, _jsonFileName);
        var result = File.ReadAllText(jsonPath);
        _jsonArray = JArray.Parse(result);
        if (_jsonArray.First is JObject jObject)
        {
            var toStringText = $"{_toStringInfo.blankStr}return $\"";
            foreach (var property in jObject.Properties())
            {
                var jToken = property.Value;
                string typeStr = GetValueTypeString(jToken);
                if (typeStr.Equals("None"))
                    throw new ArgumentOutOfRangeException();
                var declareText = $"{_declareInfo.blankStr}public {typeStr} {property.Name}" + " { get; set; }";
                _declareInfo.lines.Add(declareText);
                var contructText = $"{_contructInfo.blankStr}{property.Name} = argData.{property.Name};";
                _contructInfo.lines.Add(contructText);
                toStringText += $"{property.Name}({{{property.Name}}})";
                AddLog($"[jsonLine]key({property.Name})type({jToken.Type})typeStr({typeStr})\n{declareText}");
            }

            toStringText += "\";";
            _toStringInfo.lines.Add(toStringText);
        }
    }

    private string GetValueTypeString(JToken jToken)
    {
        string result = "None";
        switch (jToken.Type)
        {
            case JTokenType.None:
                break;
            case JTokenType.Object:
                break;
            case JTokenType.Array:
                // 다차원 배열 필요 시 개발하기(현재 1차원 배열만 처리)
                if (jToken is JArray jArray)
                {
                    result = GetValueTypeString(jArray[0]);
                    result += "[]";
                }
                break;
            case JTokenType.Constructor:
                break;
            case JTokenType.Property:
                break;
            case JTokenType.Comment:
                break;
            case JTokenType.Integer:
                result = "int";
                break;
            case JTokenType.Float:
                result = "float";
                break;
            case JTokenType.String:
                result = "string";
                break;
            case JTokenType.Boolean:
                result = "bool";
                break;
            case JTokenType.Null:
                break;
            case JTokenType.Undefined:
                break;
            case JTokenType.Date:
                break;
            case JTokenType.Raw:
                break;
            case JTokenType.Bytes:
                break;
            case JTokenType.Guid:
                break;
            case JTokenType.Uri:
                break;
            case JTokenType.TimeSpan:
                break;
        }

        return result;
    }

    private List<string> GetFileLines(string path)
    {
        var result = File.ReadAllText(path);
        return result.Split(Environment.NewLine).ToList();
    }
    
    public void ConvertVariable(out List<string> result)
    {
        // 현재 라인 정보는 초기 SampleTable.cs 기준이라서 수정은 역순으로 진행
        List<string> codeLineResult = ConvertLines(_toStringInfo, _templateCodeLines);
        codeLineResult = ConvertLines(_declareInfo, codeLineResult);
        codeLineResult = ConvertLines(_contructInfo, codeLineResult);
        result = codeLineResult;
    }
    
    private List<string> ConvertLines(ConvertInfo cInfo, List<string> fullOriginLines)
    {
        var prevLines = fullOriginLines.GetRange(0, cInfo.startIndex + 1);
        var nextLines = fullOriginLines.GetRange(cInfo.endIndex, fullOriginLines.Count - cInfo.endIndex);
        List<string> codeLineResult = prevLines;
        codeLineResult.AddRange(cInfo.lines);
        codeLineResult.AddRange(nextLines);
        return codeLineResult;
    }

    private void AddLog(string log)
    {
        CustomToolbarManager.AddLog(log);
    }
}
