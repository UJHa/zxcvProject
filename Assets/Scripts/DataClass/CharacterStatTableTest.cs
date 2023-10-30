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
    public class TestData
    {
        public TestData()
        {
            
        }
        public TestData(TestData argData)
        {
            // Construct Values
            id = argData.id;
            profileName = argData.profileName;
            health = argData.health;
            mana = argData.mana;
            strength = argData.strength;
            agility = argData.agility;
            intellect = argData.intellect;
            defense = argData.defense;
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string profileName { get; set; }
        public float health { get; set; }
        public float mana { get; set; }
        public float strength { get; set; }
        public float agility { get; set; }
        public float intellect { get; set; }
        public float defense { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            return $"id({id})profileName({profileName})health({health})mana({mana})strength({strength})agility({agility})intellect({intellect})defense({defense})";
        }
    }

    public class TestTable : DataTable
    {
        private static Type tableType = typeof(TestTable);
        private static string dataName = $"{typeof(TestData)}";
        private static string jsonFileName = "Test.json";
        private static TestTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, TestData> IndexDictionary = new();
        public Dictionary<string, TestData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<TestData>(jToken.ToString());
                if (enableLog)
                    Debug.Log($"[testumJsonTable][{tableType}]override.SaveData save obj data({jsonData})");
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    Debug.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                
                if (nameDictionary.ContainsKey(jsonData.profileName))
                {
                    Debug.LogError($"{dataName} have same key({jsonData.profileName})");
                    continue;
                }
                nameDictionary.Add(jsonData.profileName, jsonData);
            }
        }

        public static TestData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            TestTable table = DataTable.Tables[tableType] as TestTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<TestData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<TestData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, TestData argData)
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
