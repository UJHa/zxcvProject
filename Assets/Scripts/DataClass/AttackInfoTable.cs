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
    public class AttackInfoData
    {
        public AttackInfoData()
        {
            
        }
        public AttackInfoData(AttackInfoData argData)
        {
            // Construct Values[Start]
            id = argData.id;
            name = argData.name;
            hitboxType = argData.hitboxType;
            damageRatio = argData.damageRatio;
            startRatio = argData.startRatio;
            endRatio = argData.endRatio;
            attackType = argData.attackType;
            offset = argData.offset;
            size = argData.size;
            airborneHeight = argData.airborneHeight;
            airborneTime = argData.airborneTime;
            // Construct Values[End]
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string name { get; set; }
        public string hitboxType { get; set; }
        public float damageRatio { get; set; }
        public float startRatio { get; set; }
        public float endRatio { get; set; }
        public string attackType { get; set; }
        public float[] offset { get; set; }
        public float[] size { get; set; }
        public float airborneHeight { get; set; }
        public float airborneTime { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            // ToString Values[Start]
            return $"id({id})name({name})hitboxType({hitboxType})damageRatio({damageRatio})startRatio({startRatio})endRatio({endRatio})attackType({attackType})offset({offset})size({size})airborneHeight({airborneHeight})airborneTime({airborneTime})";
            // ToString Values[End]
        }
    }

    public class AttackInfoTable : DataTable
    {
        private static Type tableType = typeof(AttackInfoTable);
        private static string dataName = $"{typeof(AttackInfoData)}";
        private static string jsonFileName = "AttackInfo.json";
        private static AttackInfoTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, AttackInfoData> IndexDictionary = new();
        public Dictionary<string, AttackInfoData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<AttackInfoData>(jToken.ToString());
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

        public static AttackInfoData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            AttackInfoTable table = DataTable.Tables[tableType] as AttackInfoTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<AttackInfoData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<AttackInfoData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, AttackInfoData argData)
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
            Debug.Log(jsonPath);
            // 파일 생성 및 저장
            File.WriteAllText(jsonPath, writeText);
        }
    }
}
