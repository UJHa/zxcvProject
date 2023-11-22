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
    public class _Sample_Data
    {
        public _Sample_Data()
        {
            
        }
        public _Sample_Data(_Sample_Data argData)
        {
            // Construct Values[Start]
            id = argData.id;
            name = argData.name;
            // Construct Values[End]
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string name { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            // ToString Values[Start]
            return $"id({id})name({name})";
            // ToString Values[End]
        }
    }

    public class _Sample_Table : DataTable
    {
        private static Type tableType = typeof(_Sample_Table);
        private static string dataName = $"{typeof(_Sample_Data)}";
        private static string jsonFileName = "_Sample_.json";
        private static _Sample_Table _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, _Sample_Data> IndexDictionary = new();
        public Dictionary<string, _Sample_Data> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<_Sample_Data>(jToken.ToString());
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

        public static _Sample_Data GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            _Sample_Table table = DataTable.Tables[tableType] as _Sample_Table;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<_Sample_Data> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<_Sample_Data> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, _Sample_Data argData)
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
            // 파일 생성 및 저장
            File.WriteAllText(jsonPath, writeText);
        }
    }
}