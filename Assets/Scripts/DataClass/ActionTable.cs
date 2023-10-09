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
        public ActionData(ActionData actionData)
        {
            id = actionData.id;
            actionName = actionData.actionName;
            roleState = actionData.roleState;
            actionType = actionData.actionType;
            clipPath = actionData.clipPath;
            speed = actionData.speed;
            startTimeRatio = actionData.startTimeRatio;
            endTimeRatio = actionData.endTimeRatio;
            damageRatio = actionData.damageRatio;
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
        private static ActionTable _instance = null;
        
        // 런타임 사용 시 값 복사 꼭 고려할 것(id리스트, name리스트 모두)
        public Dictionary<int, ActionData> IndexDictionary = new();
        public Dictionary<string, ActionData> actionNameDictionary = new();
        protected override void Init(JArray dataList)
        {
            base.Init(dataList);
            _instance = this;
            foreach (var jToken in dataList)
            {
                var actionData = JsonConvert.DeserializeObject<ActionData>(jToken.ToString());
                Debug.Log($"[testumJsonTable][{GetType()}]override.SaveData save obj data({actionData})");
                if (IndexDictionary.ContainsKey(actionData.id))
                {
                    Debug.LogError($"ActionData have same id({actionData.id})");
                    continue;
                }
                IndexDictionary.Add(actionData.id, actionData);
                
                if (actionNameDictionary.ContainsKey(actionData.actionName))
                {
                    Debug.LogError($"ActionData have same actionName({actionData.actionName})");
                    continue;
                }
                actionNameDictionary.Add(actionData.actionName, actionData);
            }
        }

        public static ActionData GetActionData(string actionName)
        {
            if (false == DataTable.Tables.ContainsKey(typeof(ActionTable)))
                return null;
            ActionTable actionTable = DataTable.Tables[typeof(ActionTable)] as ActionTable;
            if (null == actionTable)
                return null;
            if (false == actionTable.actionNameDictionary.ContainsKey(actionName))
                return null;
            return new(actionTable.actionNameDictionary[actionName]);
        }

        public static List<ActionData> GetActionList()
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

        public static void SetActionData(string actionName, ActionData action)
        {
            if (false == _instance.actionNameDictionary.ContainsKey(actionName))
            {
                Debug.LogError($"Don't have actionName({actionName})");
                return;
            }

            _instance.actionNameDictionary[actionName] = action;
            _instance.IndexDictionary[action.id] = action;
        }

        public static void Export()
        {
            string writeText = "[\n";
            foreach (var actionData in _instance.IndexDictionary.Values)
            {
                writeText += JsonConvert.SerializeObject(actionData) + ",\n";
                Debug.Log($"[ExportTest]{actionData}\n{JsonConvert.SerializeObject(actionData)}");
            }

            writeText += "]";
            string jsonPath = Path.Combine(UmUtil.GetResourceJsonPath(), "action.json");
            Debug.Log(jsonPath);
            // 파일 생성 및 저장
            File.WriteAllText(jsonPath, writeText);
        }
    }
}