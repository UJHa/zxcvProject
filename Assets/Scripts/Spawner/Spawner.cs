using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private string _resouarcePath;
    [SerializeField] private string _prefabName = "";
    public string temp = "hello";

    public GameObject SpawnObject()
    {
        var loadPrefab = Resources.Load<GameObject>(_resouarcePath);
        if (null == loadPrefab)
        {
            ReleaseLog.LogError($"[testum]SpawnObject({loadPrefab}) fail");
            return null;
        }
        if (false == _prefabName.Equals(""))
            loadPrefab.name = _prefabName;
    
        var resultObj = Instantiate(loadPrefab);
        resultObj.transform.position = transform.position;
        
        return resultObj;
    }
}