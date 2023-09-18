using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private string _resouarcePath;

    public GameObject SpawnObject()
    {
        var loadPrefab = Resources.Load<GameObject>(_resouarcePath);
        if (null == loadPrefab)
        {
            Debug.LogError($"[testum]SpawnObject({loadPrefab}) fail");
            return null;
        }
    
        Debug.Log($"[testum]SpawnObject({loadPrefab}) success");
        var resultObj = Instantiate(loadPrefab);
        resultObj.transform.position = transform.position;
        return resultObj;
    }
}