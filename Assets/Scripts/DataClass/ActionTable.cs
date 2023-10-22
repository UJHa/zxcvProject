using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utils;

namespace DataClass
{
    [Serializable]
    public class ActionData
    {
        public ActionData()
        {
            
        }
        public ActionData(ActionData argData)
        {
            id = argData.id;
            actionName = argData.actionName;
            roleState = argData.roleState;
            actionType = argData.actionType;
            clipPath = argData.clipPath;
            speed = argData.speed;
            startTimeRatio = argData.startTimeRatio;
            endTimeRatio = argData.endTimeRatio;
            damageRatio = argData.damageRatio;
        }

        public int id { get; set; }
        public string actionName { get; set; }
        public string roleState { get; set; }
        public string actionType { get; set; }
        public string clipPath { get; set; }
        public float speed { get; set; }
        public float startTimeRatio { get; set; }
        public float endTimeRatio { get; set; }
        public float damageRatio { get; set; }
        public override string ToString()
        {
            return $"id({id})actionName({actionName})roleState({roleState})action({actionType})speed({speed})clipPath({clipPath})startTimeRatio({startTimeRatio})endTimeRatio({endTimeRatio})damageRatio({damageRatio})";
        }
    }

    public class ActionTable : DataTable
    {
        private static Type tableType = typeof(ActionTable);
        private static string dataName = $"{typeof(ActionData)}";
        private static string jsonFileName = "Action.json";
        private static ActionTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, ActionData> IndexDictionary = new();
        public Dictionary<string, ActionData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<ActionData>(jToken.ToString());
                if (enableLog)
                    Debug.Log($"[testumJsonTable][{tableType}]override.SaveData save obj data({jsonData})");
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    Debug.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                
                if (nameDictionary.ContainsKey(jsonData.actionName))
                {
                    Debug.LogError($"dataName have same key({jsonData.actionName})");
                    continue;
                }
                nameDictionary.Add(jsonData.actionName, jsonData);
            }
        }

        public static ActionData GetData(string argData)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            ActionTable actionTable = DataTable.Tables[tableType] as ActionTable;
            if (null == actionTable)
                return null;
            if (false == actionTable.nameDictionary.ContainsKey(argData))
                return null;
            return new(actionTable.nameDictionary[argData]);
        }

        public static List<ActionData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"ActionTable is not instantiate!");
                return null;
            }

            List<ActionData> result = new();

            // 값 복사를 통해서 ActionTable 값과 분리하여 전달
            foreach (var actionData in _instance.IndexDictionary.Values)
            {
                result.Add(new (actionData));
            }
            return result;
        }

        public static void SetData(string actionName, ActionData action)
        {
            if (false == _instance.nameDictionary.ContainsKey(actionName))
            {
                Debug.LogError($"Don't have actionName({actionName})");
                return;
            }

            _instance.nameDictionary[actionName] = action;
            _instance.IndexDictionary[action.id] = action;
        }

        public static void Export()
        {
            string writeText = "[\n";
            foreach (var data in _instance.IndexDictionary.Values)
            {
                writeText += JsonConvert.SerializeObject(data) + ",\n";
                Debug.Log($"[ExportTest]{data}\n{JsonConvert.SerializeObject(data)}");
            }

            writeText += "]";
            string jsonPath = Path.Combine(UmUtil.GetResourceJsonPath(), jsonFileName);
            Debug.Log(jsonPath);
            // 파일 생성 및 저장
            File.WriteAllText(jsonPath, writeText);
        }
    }
}