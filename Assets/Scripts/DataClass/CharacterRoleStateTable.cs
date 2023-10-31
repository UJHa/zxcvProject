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
    public class CharacterRoleStateData
    {
        public CharacterRoleStateData()
        {
            
        }
        public CharacterRoleStateData(CharacterRoleStateData argData)
        {
            // Construct Values[Start]
            id = argData.id;
            name = argData.name;
            idle = argData.idle;
            walk = argData.walk;
            run = argData.run;
            runStop = argData.runStop;
            jumpUp = argData.jumpUp;
            jumpDown = argData.jumpDown;
            landing = argData.landing;
            normalDamaged = argData.normalDamaged;
            airborneDamaged = argData.airborneDamaged;
            airbornePowerDownDamaged = argData.airbornePowerDownDamaged;
            knockBackDamaged = argData.knockBackDamaged;
            flyAwayDamaged = argData.flyAwayDamaged;
            damagedAirborneLoop = argData.damagedAirborneLoop;
            damagedLanding = argData.damagedLanding;
            wakeUp = argData.wakeUp;
            dead = argData.dead;
            getItem = argData.getItem;
            weekAttack1 = argData.weekAttack1;
            weekAttack2 = argData.weekAttack2;
            weekAttack3 = argData.weekAttack3;
            airWeekAttack1 = argData.airWeekAttack1;
            airWeekAttack2 = argData.airWeekAttack2;
            airWeekAttack3 = argData.airWeekAttack3;
            strongAttack1 = argData.strongAttack1;
            strongAttack2 = argData.strongAttack2;
            // Construct Values[End]
        }

        // Declaration Values[Start]
        public int id { get; set; }
        public string name { get; set; }
        public string[] idle { get; set; }
        public string[] walk { get; set; }
        public string[] run { get; set; }
        public string[] runStop { get; set; }
        public string[] jumpUp { get; set; }
        public string[] jumpDown { get; set; }
        public string[] landing { get; set; }
        public string[] normalDamaged { get; set; }
        public string[] airborneDamaged { get; set; }
        public string[] airbornePowerDownDamaged { get; set; }
        public string[] knockBackDamaged { get; set; }
        public string[] flyAwayDamaged { get; set; }
        public string[] damagedAirborneLoop { get; set; }
        public string[] damagedLanding { get; set; }
        public string[] wakeUp { get; set; }
        public string[] dead { get; set; }
        public string[] getItem { get; set; }
        public string[] weekAttack1 { get; set; }
        public string[] weekAttack2 { get; set; }
        public string[] weekAttack3 { get; set; }
        public string[] airWeekAttack1 { get; set; }
        public string[] airWeekAttack2 { get; set; }
        public string[] airWeekAttack3 { get; set; }
        public string[] strongAttack1 { get; set; }
        public string[] strongAttack2 { get; set; }
        // Declaration Values[End]
        public override string ToString()
        {
            // ToString Values[Start]
            return $"id({id})name({name})idle({idle})walk({walk})run({run})runStop({runStop})jumpUp({jumpUp})jumpDown({jumpDown})landing({landing})normalDamaged({normalDamaged})airborneDamaged({airborneDamaged})airbornePowerDownDamaged({airbornePowerDownDamaged})knockBackDamaged({knockBackDamaged})flyAwayDamaged({flyAwayDamaged})damagedAirborneLoop({damagedAirborneLoop})damagedLanding({damagedLanding})wakeUp({wakeUp})dead({dead})getItem({getItem})weekAttack1({weekAttack1})weekAttack2({weekAttack2})weekAttack3({weekAttack3})airWeekAttack1({airWeekAttack1})airWeekAttack2({airWeekAttack2})airWeekAttack3({airWeekAttack3})strongAttack1({strongAttack1})strongAttack2({strongAttack2})";
            // ToString Values[End]
        }
    }

    public class CharacterRoleStateTable : DataTable
    {
        private static Type tableType = typeof(CharacterRoleStateTable);
        private static string dataName = $"{typeof(CharacterRoleStateData)}";
        private static string jsonFileName = "CharacterRoleState.json";
        private static CharacterRoleStateTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, CharacterRoleStateData> IndexDictionary = new();
        public Dictionary<string, CharacterRoleStateData> nameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var jsonData = JsonConvert.DeserializeObject<CharacterRoleStateData>(jToken.ToString());
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

        public static CharacterRoleStateData GetData(string argKeyName)
        {
            if (false == DataTable.Tables.ContainsKey(tableType))
                return null;
            CharacterRoleStateTable table = DataTable.Tables[tableType] as CharacterRoleStateTable;
            if (null == table)
                return null;
            if (false == table.nameDictionary.ContainsKey(argKeyName))
                return null;
            return new(table.nameDictionary[argKeyName]);
        }

        public static List<CharacterRoleStateData> GetList()
        {
            if (null == _instance)
            {
                Debug.LogError($"{tableType} is not instantiate!");
                return null;
            }

            List<CharacterRoleStateData> result = new();

            // 값 복사를 통해서 Table 값과 분리하여 전달
            foreach (var data in _instance.IndexDictionary.Values)
            {
                result.Add(new (data));
            }
            return result;
        }

        public static void SetData(string argKeyName, CharacterRoleStateData argData)
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
