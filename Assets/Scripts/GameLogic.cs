using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    private BlockSpawner blockSpawner;
    private static int mapWidth = 10;
    private static int mapHeight = 15;
    private static int offset = 5;
    private DataTable blockMap = new DataTable(mapHeight, mapWidth, offset);
    void Start()
    {
        Debug.Log("Init of gamelogic");

        blockSpawner = GetComponent<BlockSpawner>();
        initBlockSpawn();
    }

    public void initBlockSpawn(GameObject collidedBlock)
    {
        //Debug.Log("Called spawnBlock inside gamelogic after collision");
        Debug.Log("Collider detected: " + collidedBlock.name + " has children: " + collidedBlock.transform.childCount.ToString());
        Transform[] childrenTransform = collidedBlock.GetComponentsInChildren<Transform>();
        for(int i = 0; i < collidedBlock.transform.childCount; i++)
        {   
            int code = blockMap.AddData(collidedBlock.transform.GetChild(i));
        }
        Debug.Log("Finished adding all children");
        blockSpawner.spawnBlock();
    }

    private void initBlockSpawn()
    {
        Debug.Log("First block spawn called");
        blockSpawner.spawnBlock();
    }
}

public class BlockData
{
    //Container for info about block with easy access to its variables
    public int x, y;
    private int dataOffset;
    private Transform block;
    private GameObject blockObj;

    public BlockData(Transform new_block, int dataOffset)
    {
        blockObj = new_block.gameObject;
        block = new_block;
        x = (int) Mathf.Round(block.position.x) + dataOffset;
        y = (int) -Mathf.Round(block.position.y) - 1;
    }

    public void DestroyBlock()
    {
        UnityEngine.Object.Destroy(blockObj);
    }
}

public class DataTable
{
    public int Height, Width;
    private int dataOffset;

    private BlockData[,] blockMap;
    private int[] count;

    public DataTable(int height, int width, int offset)
    {
        if(height >= 0 && width >= 0)
            {
                Height = height;
                Width = width;
                dataOffset = offset;
                blockMap = new BlockData[width, height];
                count = new int[height];
            }
        else Debug.LogError("Placeholder error in Datarow init");
    }

    //Returns: -2 when error occured; -1 when data was added succesfully; int i >= 0 when i-th row was filled and cleared  
    public int AddData(Transform blockTransform)
    {   
        BlockData new_block = new BlockData(blockTransform, dataOffset); 
        if(new_block.x < Width && new_block.y < Height)
        {
            Debug.Log("Added block with (x,y): " + new_block.x.ToString() + " " + new_block.y.ToString());
            blockMap[new_block.x, new_block.y] = new_block;
            count[new_block.y]++;

            if(count[new_block.y] >= Width)
            {
                ClearData(new_block.y);
                return new_block.y;
            }
            else return -1;
        }
        else 
        {
            Debug.LogError("Placeholder error in AddData func");
            //Error code
            return -2;
        }
    }

    private void ClearData(int rowIndex)
    {
        for(int i = 0; i < Width; i++)
        {
            blockMap[i, rowIndex].DestroyBlock();
            blockMap[i, rowIndex] = null;
        }
        count[rowIndex] = 0;
    }
}
