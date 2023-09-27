using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DataClass
{
    //엄 todo : json 데이터 클래스를 다른 네임스페이스로 이동하기
    [Serializable]
    public class ActionData
    {
        public int id { get; set; }
        public string actionName { get; set; }
        public string actionType { get; set; }
        public string clipPath { get; set; }
        public float startTimeRatio { get; set; }
        public float endTimeRatio { get; set; }
        public float damageRatio { get; set; }
        public override string ToString()
        {
            return $"id({id})actionName({actionName})action({actionType})clipPath({clipPath})startTimeRatio({startTimeRatio})endTimeRatio({endTimeRatio})damageRatio({damageRatio})";
        }
    }

    public class ActionTable : DataTable
    {
        public Dictionary<int, ActionData> IndexDictionary = new();
        public Dictionary<string, ActionData> actionNameDictionary = new();
        protected override void SaveData(JArray dataList)
        {
            base.SaveData(dataList);
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
            return actionTable.actionNameDictionary[actionName];
        }
    }
}