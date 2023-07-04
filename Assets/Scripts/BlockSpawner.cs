using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Parent;
    private GameObject[] prefabsList;
    private Color32[] colorPallette;
    private GameObject currentClone;
    // Update is called once per frame
    private int numGenerated = 0;
    private int startY = 10;
    private int offsetX = 5;

    void Start()
    {
        //Loading prefabs
        //prefabsList = Resources.LoadAll<GameObject>("Blocks");
        prefabsList = Resources.LoadAll<GameObject>("Blocks");
        //Creating colour pallette: red, yellow, green, blue
        colorPallette = new Color32[]{
            new Color32(219, 48, 105, 255), 
            new Color32(245, 213, 71, 255), 
            new Color32(112, 163, 127, 255), 
            new Color32(20, 70, 160, 255)};

    }
    public GameObject spawnBlock()
    {
        GameObject newPrefab = getPrefab();

        //Obj init
        GameObject newObject =  Instantiate(newPrefab, new Vector3(offsetX, startY, 0), new Quaternion(0,0,0,0), Parent.transform);

        //Changing name and block colours
        newObject.name = newPrefab.name.ToString() + numGenerated.ToString();
        Color NewColor = colorPallette[numGenerated % colorPallette.Length];
        Component[] childrenSprites = newObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer childSprite in childrenSprites)
        {
            //Debug.Log("Current child: " + childSprite.gameObject.name.ToString());
            childSprite.color = NewColor;
            //Debug.Log("Current: " + childSprite.gameObject.name.ToString());
        }

        //Debug.Log("Spawn block inside blockspawner");
        numGenerated++;
        return newObject;
    }

    private GameObject getPrefab()
    {
        //getting 
        return prefabsList[Random.Range(0, prefabsList.Length)];
    }
}
