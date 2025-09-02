using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using static PlatformBehavior;
using static PlayerController;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool addToDontDestroyOnLoad = false;

    private GameObject emptyHolder;

    private static GameObject particleSystemsEmpty;
    private static GameObject gameObjectsEmpty;
    private static GameObject soudFxEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
    private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

    private static PlatformType lastPlatformType = PlatformType.Regular;

    public enum PoolType
    {
        ParticleSystems,
        GameObjects,
        SoundFxs
    }
    public static PoolType PoolingType;

    private void Awake()
    {
        objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    private void SetupEmpties()
    {
        emptyHolder = new GameObject("Object Pools");

        gameObjectsEmpty = new GameObject("Game Objects");
        gameObjectsEmpty.transform.SetParent(emptyHolder.transform);
        //May want to remove this later as im only gonna need object pools !!!
        particleSystemsEmpty = new GameObject("Particle Effects");
        particleSystemsEmpty.transform.SetParent(emptyHolder.transform);
        soudFxEmpty = new GameObject("Sound Effects");
        soudFxEmpty.transform.SetParent(emptyHolder.transform);

        if (addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(particleSystemsEmpty.transform.root);
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(createFunc: () =>
            CreateObject(prefab, pos, rot, poolType), actionOnGet: OnGetObject, actionOnRelease: OnReleaseObject, actionOnDestroy: OnDestroyObject);

        objectPools.Add(prefab, pool);

    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);
        GameObject obj = Instantiate(prefab, pos, rot);
        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {
        PlatformBehavior platformBehavior = obj.GetComponent<PlatformBehavior>();        
        platformBehavior.currentPlatformType = PlatformType.Regular;
        
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(true);
        }
        
        if (currentScore >= 6.0f)
        {
            float randomValue = Random.Range(0f, 1f);

            if (lastPlatformType == PlatformType.Spring)
            {
                if (randomValue < 0.4f)
                {
                    platformBehavior.currentPlatformType = PlatformType.Danger;
                }
            }
            else if (lastPlatformType == PlatformType.Danger)
            {
                if (randomValue < 0.3f)
                {
                    platformBehavior.currentPlatformType = PlatformType.Spring;
                }
            }
            else
            {
                if (randomValue < 0.2f)
                {
                    platformBehavior.currentPlatformType = PlatformType.Spring;
                }
                else if (randomValue < 0.5f)
                {
                    platformBehavior.currentPlatformType = PlatformType.Danger;
                }
            }
        }
        else if (currentScore >= 3.0f)
        {
            if (lastPlatformType != PlatformType.Danger)
            {
                float randomValue = Random.Range(0f, 1f);
                if (randomValue < 0.5f)
                {
                    platformBehavior.currentPlatformType = PlatformType.Danger;
                }
            }
        }
        lastPlatformType = platformBehavior.currentPlatformType;
        
        obj.SetActive(true);
    }

    private static void OnReleaseObject(GameObject obj)
    {
        PlatformBehavior platformBehavior = obj.GetComponent<PlatformBehavior>();        
        platformBehavior.currentPlatformType = PlatformType.Regular;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
        }


        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (cloneToPrefabMap.ContainsKey(obj))
        {
            cloneToPrefabMap.Remove(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType = PoolType.GameObjects)
    {
        switch (poolType)
        {
            case PoolType.ParticleSystems:
                return particleSystemsEmpty;
            case PoolType.GameObjects:
                return gameObjectsEmpty;
            case PoolType.SoundFxs:
                return soudFxEmpty;
            default:
                return null;
        }
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!cloneToPrefabMap.ContainsKey(obj))
            {
                cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;

        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public static void ReturnObjectPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);
            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if (objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }

        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
        }
    }
}
