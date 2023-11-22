using System;
using System.Collections.Generic;
using System.IO;
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
            // Construct Values[Start]
            id = argData.id;
            name = argData.name;
            roleState = argData.roleState;
            actionType = argData.actionType;
            clipPath = argData.clipPath;
            speed = argData.speed;
            startTimeRatio = argData.startTimeRatio;
            endTimeRatio = argData.endTimeRatio;
            damageRatio = argData.damageRatio;
            // Construct Values[End]
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string name { get; set; }
        public string roleState { get; set; }
        public string actionType { get; set; }
        public string clipPath { get; set; }
        public float speed { get; set; }
        public float startTimeRatio { get; set; }
        public float endTimeRatio { get; set; }
        public float damageRatio { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            // ToString Values[Start]
            return $"id({id})name({name})roleState({roleState})actionType({actionType})clipPath({clipPath})speed({speed})startTimeRatio({startTimeRatio})endTimeRatio({endTimeRatio})damageRatio({damageRatio})";
            // ToString Values[End]
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
                // Id Dictionary Values[Start]
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    Debug.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                // Id Dictionary Values[End]
                
                // name Dictionary Values[Start]
                if (nameDictionary.ContainsKey(jsonData.name))
                {
                    Debug.LogError($"{dataName} have same key({jsonData.name})");
                    continue;
                }
                nameDictionary.Add(jsonData.name, jsonData);
                // name Dictionary Values[End]
            }
        }

        public static ActionData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            ActionTable table = DataTable.Tables[tableType] as ActionTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<ActionData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<ActionData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, ActionData argData)
        {
            if (false == _instance.nameDictionary.ContainsKey(argKeyName))
            {
                Debug.LogError($"Don't have string key({argKeyName})");
                return;
            }

            _instance.nameDictionary[argKeyName] = argData;
            _instance.IndexDictionary[argData.id] = argData;
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
            File.WriteAllText(jsonPath, writeText);
        }
    }
}
