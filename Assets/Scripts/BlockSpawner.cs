using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parent;
    private GameObject[] PrefabsList;
    private Color32[] ColorPallette;
    private GameObject CurrentClone;
    // Update is called once per frame
    private int NumGenerated = 0;
    private int StartY = 20;

    void Start()
    {
        //Loading prefabs
        PrefabsList = Resources.LoadAll<GameObject>("Blocks");
        //Creating colour pallette: red, yellow, green, blue
        ColorPallette = new Color32[]{new Color32(219, 48, 105, 255), new Color32(245, 213, 71, 255), new Color32(112, 163, 127, 255), new Color32(20, 70, 160, 255)};

    }
    public GameObject spawnBlock()
    {
        GameObject newPrefab = getPrefab();

        //Obj init
        GameObject newObject =  Instantiate(newPrefab, new Vector3(0, StartY, 0), new Quaternion(0,0,0,0), parent.transform);

        //Changing name and block colours
        newObject.name = newPrefab.name.ToString() + NumGenerated.ToString();
        Color NewColor = ColorPallette[NumGenerated % ColorPallette.Length];
        Component[] childrenSprites = newObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer childSprite in childrenSprites)
        {
            //Debug.Log("Current child: " + childSprite.gameObject.name.ToString());
            childSprite.color = NewColor;
            Debug.Log("Current: " + childSprite.gameObject.name.ToString());
        }

        //Debug.Log("Spawn block inside blockspawner");
        NumGenerated++;
        return newObject;
    }

    private GameObject getPrefab()
    {
        //getting 
        return PrefabsList[Random.Range(0, PrefabsList.Length)];
    }
}
