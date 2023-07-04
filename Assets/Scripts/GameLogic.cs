using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update

    //Main objects
    private BlockSpawner blockSpawner;
    private GameObject currentBlock;
    //Informs about dimensions of currentBlock
    private int leftBound, rightBound, downBound;
    private int rotated = 0;

    //Map size definition
    private static int mapWidth = 10;
    private static int mapHeight = 15;
    private static int offset = 5;
    private DataTable blockMap = new DataTable(mapHeight, mapWidth, offset);

    //Game parameters
    public KeyCode moveRightKey, moveLeftKey, rotateKey, speedUpKey; 
    public bool isBlockDown;
    private int Horizontal_offset = 1;
    private int tempBoundary = -8;
    private float GravitySpeed = 10;
    private float Acceleration = 1.5f;
    
    void Start()
    {
        Debug.Log("Init of gamelogic");

        blockSpawner = GetComponent<BlockSpawner>();
        initBlockSpawn();

        //Setting default settings 
        moveRightKey = KeyCode.D;
        moveLeftKey = KeyCode.A;
        rotateKey = KeyCode.R;
        speedUpKey = KeyCode.S;
        isBlockDown = false;
    }

    void Update()
    {   
        if (!isBlockDown)
        {
            if (Input.GetKeyUp(moveRightKey)) MoveHorizontal(Horizontal_offset);
            else if (Input.GetKeyUp(moveLeftKey)) MoveHorizontal(-Horizontal_offset);
            else if (Input.GetKeyUp(rotateKey)) Rotate();
            
            float new_offset = -GravitySpeed * Time.deltaTime;
            if (Input.GetKey(speedUpKey)) 
            {
                new_offset = new_offset * Acceleration;
            }
            
            if (currentBlock.transform.position.y + new_offset - downBound < tempBoundary)
            {
                currentBlock.transform.position = new Vector3Int((int) currentBlock.transform.position.x, tempBoundary + downBound, 0);
                isBlockDown = true;   
            }
            else currentBlock.transform.position += new Vector3(0, new_offset, 0);
            //Debug.Log("On the move");
        }
        else
        {
            initBlockSpawn();
        }
    }

    private void MoveHorizontal(int offset)
    {
        Transform currBlock = currentBlock.transform;
        if((currBlock.position.x + offset - leftBound >= 0) & (currBlock.position.x + offset + rightBound <= mapWidth))
            currBlock.position = new Vector3((int)currBlock.position.x + offset, currBlock.position.y, 0);
        else Debug.Log("Out of bounds in x axis");
    }

    private void Rotate()
    {
        currentBlock.transform.Rotate(new Vector3(0, 0, 90), Space.World);
        Transform currBlock = currentBlock.transform;
        rotated = (rotated + 1) % 4;
        updateDimensions();

        foreach(Transform child in currBlock)
        {
            if(child.position.y + downBound < tempBoundary)
            {
                currBlock.position += new Vector3(0, child.position.y + downBound, 0);
            }

            if(child.position.x - leftBound < 0)
            {
                //Debug.Log("Shift from left by: " + (child.position.x - leftBound).ToString());
                //Debug.Log("Child position: " + child.position.x.ToString());
                currBlock.position += new Vector3(-child.position.x, 0, 0);
            }
            else if(child.position.x + rightBound > mapWidth)
            {
                currBlock.position += new Vector3(-(child.position.x - mapWidth), 0, 0);
                //Debug.Log("Shift from right by: " + (child.position.x + rightBound - mapWidth).ToString());
                //Debug.Log("Child position: " + child.position.x.ToString());
            }
        }
    }

    private void initBlockSpawn()
    {
        Debug.Log("First block spawn called");
        currentBlock =  blockSpawner.spawnBlock();
        updateDimensions();
        isBlockDown = false;
        rotated = 0;

    }

    private void updateDimensions()
    {
        int min_h = 0, max_h = 0, max_v = 0, min_v = 0;
        foreach(Transform child in currentBlock.transform)
        {
            int xpos = rotated % 2 == 1 ? (int) child.localPosition.y : (int) child.localPosition.x;
            int ypos = rotated % 2 == 1 ? (int) child.localPosition.x : (int) child.localPosition.y;

            if(xpos > max_h) max_h = xpos;
            else if(xpos < min_h) min_h = xpos;

            if(ypos > max_v) max_v = ypos;
            else if(ypos < min_v) min_v = ypos;
        }

        leftBound = (rotated == 0 || rotated == 3) ? min_h : max_h;
        rightBound = (rotated == 0 || rotated == 3) ? max_h : min_h;

        downBound = rotated < 2 ? min_v : max_v;

        leftBound = Mathf.Abs(leftBound);
        rightBound = Mathf.Abs(rightBound);
        downBound = Mathf.Abs(downBound);
        Debug.Log("Crucial dimensions: left - " + leftBound.ToString() + " , right - " + rightBound.ToString() + " , down - " + downBound.ToString() + " Current rotation: " + rotated.ToString());
    }

    public bool DataValidation(Transform blockTransform, bool rotation = false)
    {
        // this is literally shit XDDD
        if (rotation) blockTransform.Rotate(new Vector3(0, 0, 90), Space.Self);
        Vector3 blockPosition = blockTransform.position;
        if(blockPosition.y >= mapHeight - 1)
        {
            if((blockPosition.x >= mapWidth - 1) & (blockPosition.x < 0)) return true;
            else return false;
        }
        else return false;
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
        Debug.Log("My new y is: " + y.ToString());
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
