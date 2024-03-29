﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utils;

namespace DataClass
{
    [Serializable]
    public class CharacterStatData
    {
        public CharacterStatData()
        {
            
        }
        public CharacterStatData(CharacterStatData argData)
        {
            // Construct Values[Start]
            id = argData.id;
            name = argData.name;
            health = argData.health;
            mana = argData.mana;
            strength = argData.strength;
            agility = argData.agility;
            intellect = argData.intellect;
            defense = argData.defense;
            // Construct Values[End]
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string name { get; set; }
        public float health { get; set; }
        public float mana { get; set; }
        public float strength { get; set; }
        public float agility { get; set; }
        public float intellect { get; set; }
        public float defense { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            // ToString Values[Start]
            return $"id({id})name({name})health({health})mana({mana})strength({strength})agility({agility})intellect({intellect})defense({defense})";
            // ToString Values[End]
        }
    }

    public class CharacterStatTable : DataTable
    {
        private static Type tableType = typeof(CharacterStatTable);
        private static string dataName = $"{typeof(CharacterStatData)}";
        private static string jsonFileName = "CharacterStat.json";
        private static CharacterStatTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, CharacterStatData> IndexDictionary = new();
        public Dictionary<string, CharacterStatData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<CharacterStatData>(jToken.ToString());
                if (enableLog)
                    Debug.Log($"[testumJsonTable][{tableType}]override.SaveData save obj data({jsonData})");
                // Id Dictionary Values[Start]
                if (IndexDictionary.ContainsKey(jsonData.id))
                {
                    ReleaseLog.LogError($"{dataName} have same id({jsonData.id})");
                    continue;
                }
                IndexDictionary.Add(jsonData.id, jsonData);
                // Id Dictionary Values[End]
                
                // name Dictionary Values[Start]
                if (nameDictionary.ContainsKey(jsonData.name))
                {
                    ReleaseLog.LogError($"{dataName} have same key({jsonData.name})");
                    continue;
                }
                nameDictionary.Add(jsonData.name, jsonData);
                // name Dictionary Values[End]
            }
        }

        public static CharacterStatData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            CharacterStatTable table = DataTable.Tables[tableType] as CharacterStatTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<CharacterStatData> GetList()
        {
            if (null == _instance)
            {
                ReleaseLog.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<CharacterStatData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, CharacterStatData argData)
        {
            if (false == _instance.nameDictionary.ContainsKey(argKeyName))
            {
                ReleaseLog.LogError($"Don't have string key({argKeyName})");
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
