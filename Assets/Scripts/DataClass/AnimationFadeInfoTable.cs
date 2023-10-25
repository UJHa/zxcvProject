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
    public class AnimationFadeInfoData
    {
        public AnimationFadeInfoData()
        {
            
        }
        public AnimationFadeInfoData(AnimationFadeInfoData argData)
        {
            id = argData.id;
            playerType = argData.playerType;
            idleStart = argData.idleStart;
            walkStart = argData.walkStart;
            runStart = argData.runStart;
            jumpUpStart = argData.jumpUpStart;
            jumpEnd = argData.jumpEnd;
            damageLandingStart = argData.damageLandingStart;
        }

        public int id { get; set; }
        public string playerType { get; set; }
        public float idleStart { get; set; }
        public float walkStart { get; set; }
        public float runStart { get; set; }
        public float jumpUpStart { get; set; }
        public float jumpEnd { get; set; }
        public float damageLandingStart { get; set; }
        public override string ToString()
        {
            return $"id({id})playerType({playerType})idleStart({idleStart})walkStart({walkStart})runStart({runStart})jumpUpStart({jumpUpStart})jumpEnd({jumpEnd})damageLandingStart({damageLandingStart})";
        }
    }

    public class AnimationFadeInfoTable : DataTable
    {
        private static Type tableType = typeof(AnimationFadeInfoTable);
        private static string dataName = $"{typeof(AnimationFadeInfoData)}";
        private static string jsonFileName = "AnimationFadeInfo.json";
        private static AnimationFadeInfoTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, AnimationFadeInfoData> IndexDictionary = new();
        public Dictionary<string, AnimationFadeInfoData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<AnimationFadeInfoData>(jToken.ToString());
                if (enableLog)
                    Debug.Log($"[testumJsonTable][{tableType}]override.SaveData save obj data({jsonData})");
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    Debug.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                
                if (nameDictionary.ContainsKey(jsonData.playerType))
                {
                    Debug.LogError($"{dataName} have same key({jsonData.playerType})");
                    continue;
                }
                nameDictionary.Add(jsonData.playerType, jsonData);
            }
        }

        public static AnimationFadeInfoData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            AnimationFadeInfoTable table = DataTable.Tables[tableType] as AnimationFadeInfoTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<AnimationFadeInfoData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<AnimationFadeInfoData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, AnimationFadeInfoData argData)
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