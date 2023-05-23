using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public GameObject parent;
    //public GameObject parent;
    private GameObject CurrentClone;
    // Update is called once per frame
    private int i = 0;

    public GameObject spawnBlock()
    {
        GameObject newObject =  Instantiate(prefab, new Vector3(0, 15, 0), new Quaternion(0,0,0,0), parent.transform);

        newObject.name = "block" + i.ToString();
        //Debug.Log("Spawn block inside blockspawner");
        i++;
        return newObject;
    }
}
