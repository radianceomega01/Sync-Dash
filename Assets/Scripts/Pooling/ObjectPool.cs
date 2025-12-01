using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour {
    public GameObject prefab;
    public int initialSize = 5;


    Queue<GameObject> pool = new Queue<GameObject>();


    void Start(){
        for (int i=0;i<initialSize;i++) {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }


    public GameObject Get(){
        if (pool.Count == 0) {
            var go = Instantiate(prefab);
            go.SetActive(false);
            pool.Enqueue(go);
        }
        var item = pool.Dequeue();
        item.SetActive(true);
        return item;
    }


    public void Return(GameObject go){
        go.SetActive(false);
        pool.Enqueue(go);
        go.transform.SetParent(transform);
    }
}