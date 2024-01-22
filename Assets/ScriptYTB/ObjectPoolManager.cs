using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace objectPoolController
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        private GameObject _objectPoolEmptyHolder;

        private static GameObject _particleSystemsEmpty;
        private static GameObject _gameObjectsEmpty;

        public enum PoolType
        {
            ParticleSystem,
            GameObject,
            None
        }
        public static PoolType PoolingType;

        private void Awake()
        {
            SetupEmpties();
        }

        private void SetupEmpties()
        {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");
           // _objectPoolEmptyHolder.AddComponent<PhotonView>();

            _particleSystemsEmpty = new GameObject("Particle Effects");
            _particleSystemsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

            _gameObjectsEmpty = new GameObject("GameObjects");
            _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }



        public static GameObject SpawnObject(GameObject objectTospawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
        {
            //Find or Create Pool
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectTospawn.name);

            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectTospawn.name };
                ObjectPools.Add(pool);
            }

            //Check if there are any iactive objects in the pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            //if there are no iactive objects in the pool
            if (spawnableObj == null)
            {
                Debug.LogWarning("sth created");
                //Find the parent of the empty object
                GameObject parentObject = SetParentObject(poolType);

                //spawnableObj = PhotonNetwork.Instantiate(objectTospawn.name, spawnPosition, spawnRotation);
                spawnableObj=Instantiate(objectTospawn, spawnPosition, spawnRotation);
                //spawnableObj.AddComponent<PhotonView>();

                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }
            }
            //if there are an iactive objects in the pool
            //change position and rotation of the gameobject
            else
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }

        //if needed,there is a override
        public static GameObject SpawnObject(GameObject objectTospawn, Transform parentTransform)
        {
            //Find or Create Pool
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectTospawn.name);

            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectTospawn.name };
                ObjectPools.Add(pool);
            }

            //Check if there are any iactive objects in the pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            //if there are no iactive objects in the pool
            if (spawnableObj == null)
            {
                

                spawnableObj = PhotonNetwork.Instantiate(objectTospawn.name, parentTransform.position,parentTransform.rotation);

                
            }
            //if there are an iactive objects in the pool
            //change position and rotation of the gameobject
            else
            {
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }

        public static void ReturnObjectToPool(GameObject obj)
        {
            string goName = obj.name.Substring(0, obj.name.Length - 7);//by takeing off 7 characters,we are romoving the (Clone) from it

            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object that is not pooled: " + goName);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.GameObject:
                    return _gameObjectsEmpty;
                case PoolType.ParticleSystem:
                    return _particleSystemsEmpty;
                case PoolType.None:
                    return null;
                default:
                    return null;
            }
        }
    }

    

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
}
