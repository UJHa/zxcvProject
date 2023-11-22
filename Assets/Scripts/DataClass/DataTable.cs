using System;
using System.Collections.Generic;
using System.IO;
using DataClass;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utils;

namespace DataClass
{
    public class DataTable
    {
        protected static bool enableLog = true;
        public static Dictionary<Type, DataTable> Tables = new();
        public static void LoadJsonData()
        {
            var jsonFiles = Resources.LoadAll("Json/", typeof(TextAsset));
            foreach (var obj in jsonFiles)
            {
                if (obj is not TextAsset textAsset)
                    continue;
                var fileClassName = UmUtil.ConvertJsonToTableClassName(textAsset.name);
                if (enableLog)
                {
                    Debug.Log($"[loadjson]name({fileClassName})jsonContent({textAsset.text})");
                    Debug.Log($"[testumJson]fileName({fileClassName})");
                }
                fileClassName = $"DataClass.{fileClassName}";
                Type type = Type.GetType(fileClassName);
                if (type is null)
                {
                    Debug.LogError($"Fail to load fileClassName({fileClassName})");
                    continue;
                }
                
                if (typeof(DataTable) != type.BaseType)
                {
                    Debug.LogError($"type class don't inherit DataTable : type({type}) type.baseType({type.BaseType})");
                    continue;
                }

                if (Tables.ContainsKey(type))
                {
                    Debug.LogError($"Contain Key type({type})");
                    continue;
                }
                DataTable dataTable = Activator.CreateInstance(type) as DataTable;
                if (dataTable is null)
                {
                    Debug.LogError($"Failed to create dataTable instance : type({type})");
                    continue;
                }
                Tables.Add(type, dataTable);
                
                var dataList = JArray.Parse(textAsset.text);
                dataTable.Init(dataList);
            }
        }

        protected virtual void Init(JArray dataList)
        {
        }
    }
}