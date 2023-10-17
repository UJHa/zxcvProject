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
    public class AttackInfoData
    {
        public AttackInfoData()
        {
            
        }
        public AttackInfoData(AttackInfoData argData)
        {
            id = argData.id;
            stateName = argData.stateName;
            hitboxType = argData.hitboxType;
            damageRatio = argData.damageRatio;
            startRatio = argData.startRatio;
            endRatio = argData.endRatio;
            attackType = argData.attackType;
            airborneHeight = argData.airborneHeight;
            airborneTime = argData.airborneTime;
        }

        public int id { get; set; }
        public string stateName { get; set; }
        public string hitboxType { get; set; }
        public float damageRatio { get; set; }
        public float startRatio { get; set; }
        public float endRatio { get; set; }
        public string attackType { get; set; }
        public float airborneHeight { get; set; }
        public float airborneTime { get; set; }
        public override string ToString()
        {
            return $"id({id})stateName({stateName})rangeType({hitboxType})damageRatio({damageRatio})startRatio({startRatio})endRatio({endRatio})attackType({attackType})airborneHeight({airborneHeight})airborneTime({airborneTime})";
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
            var dataType = typeof(AttackInfoData).Assembly;
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<AttackInfoData>(jToken.ToString());
                Debug.Log($"[testumJsonTable][{tableType}]override.SaveData save obj data({jsonData})");
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    Debug.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                
                if (nameDictionary.ContainsKey(jsonData.stateName))
                {
                    Debug.LogError($"{dataName} have same key({jsonData.stateName})");
                    continue;
                }
                nameDictionary.Add(jsonData.stateName, jsonData);
            }
        }

        public static AttackInfoData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            AttackInfoTable attackInfoTable = DataTable.Tables[tableType] as AttackInfoTable;
            if (null == attackInfoTable)
                return null;
            if (false == attackInfoTable.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(attackInfoTable.nameDictionary[argKeyName]);
        }

        public static List<AttackInfoData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"AttackInfoTable is not instantiate!");
                return null;
            }

            List<AttackInfoData> result = new();

            // 값 복사를 통해서 AttackInfoTable 값과 분리하여 전달
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