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
        protected static bool enableLog = false;
        public static Dictionary<Type, DataTable> Tables = new();
        public static void LoadJsonData()
        {
            string jsonPath = UmUtil.GetResourceJsonPath();
            if (enableLog)
                Debug.Log($"[testumJson]Application.dataPath({Application.dataPath})");
            var info = new DirectoryInfo(jsonPath);
            var fileInfo = info.GetFiles();
            foreach(var file in fileInfo)
            {
                if (file.Name.Contains(".meta"))
                    continue;
                if (enableLog)
                    Debug.Log($"[testumJson]fileName({file.Name})");
                string fileClassName = UmUtil.ConvertJsonToTableClassName(file.Name);
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
                
                string path = Path.Combine(jsonPath, file.Name);
                var result = File.ReadAllText(path);
                var dataList = JArray.Parse(result);
                dataTable.Init(dataList);
            }
        }

        protected virtual void Init(JArray dataList)
        {
        }
    }
}