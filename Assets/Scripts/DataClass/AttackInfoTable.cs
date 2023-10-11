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
        public AttackInfoData(AttackInfoData attackInfoData)
        {
            id = attackInfoData.id;
            stateName = attackInfoData.stateName;
            hitboxType = attackInfoData.hitboxType;
            damageRatio = attackInfoData.damageRatio;
            startRatio = attackInfoData.startRatio;
            endRatio = attackInfoData.endRatio;
            attackType = attackInfoData.attackType;
            airborneHeight = attackInfoData.airborneHeight;
            airborneTime = attackInfoData.airborneTime;
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
        private static AttackInfoTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, AttackInfoData> IndexDictionary = new();
        public Dictionary<string, AttackInfoData> StateNameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var attackInfoData = JsonConvert.DeserializeObject<AttackInfoData>(jToken.ToString());
                Debug.Log($"[testumJsonTable][{GetType()}]override.SaveData save obj data({attackInfoData})");
                if (IndexDictionary.ContainsKey(attackInfoData.id))
                {
                    Debug.LogError($"AttackInfoData have same id({attackInfoData.id})");
                    continue;
                }
                IndexDictionary.Add(attackInfoData.id, attackInfoData);
                
                if (StateNameDictionary.ContainsKey(attackInfoData.stateName))
                {
                    Debug.LogError($"AttackInfoData have same attackInfoName({attackInfoData.stateName})");
                    continue;
                }
                StateNameDictionary.Add(attackInfoData.stateName, attackInfoData);
            }
        }

        public static AttackInfoData GetAttackInfoData(string attackInfoName)
        {
            if (false == DataTable.Tables.ContainsKey(typeof(AttackInfoTable)))
                return null;
            AttackInfoTable attackInfoTable = DataTable.Tables[typeof(AttackInfoTable)] as AttackInfoTable;
            if (null == attackInfoTable)
                return null;
            if (false == attackInfoTable.StateNameDictionary.ContainsKey(attackInfoName))
                return null;
            return new(attackInfoTable.StateNameDictionary[attackInfoName]);
        }

        public static List<AttackInfoData> GetAttackInfoList()
        {
            if (null == _instance)
            {
                Debug.LogError($"AttackInfoTable is not instantiate!");
                return null;
            }

            List<AttackInfoData> result = new();

            // 값 복사를 통해서 AttackInfoTable 값과 분리하여 전달
            foreach (var attackInfoData in _instance.IndexDictionary.Values)
            {
                result.Add(new (attackInfoData));
            }
            return result;
        }

        public static void SetAttackInfoData(string attackInfoName, AttackInfoData attackInfo)
        {
            if (false == _instance.StateNameDictionary.ContainsKey(attackInfoName))
            {
                Debug.LogError($"Don't have attackInfoName({attackInfoName})");
                return;
            }

            _instance.StateNameDictionary[attackInfoName] = attackInfo;
            _instance.IndexDictionary[attackInfo.id] = attackInfo;
        }

        public static void Export()
        {
            string writeText = "[\n";
            foreach (var attackInfoData in _instance.IndexDictionary.Values)
            {
                writeText += JsonConvert.SerializeObject(attackInfoData) + ",\n";
                Debug.Log($"[ExportTest]{attackInfoData}\n{JsonConvert.SerializeObject(attackInfoData)}");
            }

            writeText += "]";
            string jsonPath = Path.Combine(UmUtil.GetResourceJsonPath(), "AttackInfo.json");
            Debug.Log(jsonPath);
            // 파일 생성 및 저장
            File.WriteAllText(jsonPath, writeText);
        }
    }
}