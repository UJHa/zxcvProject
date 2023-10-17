using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanBoneInfo
{
    Dictionary<string, HumanBodyBones> _bodyBoneTypeMap = new();
    Dictionary<HumanBodyBones, Transform> _bodyBoneMap = new();
    public void Init(Animator animator)
    {
        foreach (HumanBodyBones boneType in Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (boneType == HumanBodyBones.LastBone)
                continue;
            var boneTfm = animator.GetBoneTransform(boneType);
            if (null == boneTfm)
            {
                // Debug.LogError("boneTfm is NULL({boneType})");
                continue;
            }
            _bodyBoneTypeMap.Add(boneTfm.name, boneType);
            _bodyBoneMap.Add(boneType, boneTfm);
        }
    }

    public string GetBoneType(string argName)
    {
        if (_bodyBoneTypeMap.ContainsKey(argName))
            return _bodyBoneTypeMap[argName].ToString();
        return "NONE";
    }
    
    public Transform GetBoneTransform(HumanBodyBones boneType)
    {
        if (_bodyBoneMap.ContainsKey(boneType))
            return _bodyBoneMap[boneType];
        return null;
    }
}