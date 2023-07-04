using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    //Main objects
    private BlockSpawner blockSpawner;              //Responsible for handling blocks' spawning
    private GameObject currentBlock;                //Block that is currently steered and managed by gamelogic
    
    //Info about current block
    private int leftBound, rightBound, downBound;   //Keeps (left/right/up/down)most coordinate of currentBlock
    private int rotated = 0;                        //Counts how many 90 deegress turns where made - has value from 0-3
    public bool isBlockDown;                        

    //Map size definition
    private static int mapWidth = 10;   //Amount of elements in row
    private static int mapHeight = 15;  //Amount of elements in a column
    private static int offset = 5;      //Offset caused by moving camera :)
    private DataTable blockMap = new DataTable(mapHeight, mapWidth, offset);

    //Game parameters
    public KeyCode moveRightKey, moveLeftKey, rotateKey, speedUpKey; 
    private int horizontalOffset = 1;   //How much does block move when on left/right click
    private int tempBoundary = -9;      //Defines the Y value of the 'lowest' row 
    private float gravitySpeed = 10;    //Defines the speed of blocks falling down
    private float acceleration = 1.5f;  //Defines how many times faster does block move on speedUpKey 
    
    void Start()
    {
        Debug.Log("Init of gamelogic");

        //Getting block spawner
        blockSpawner = GetComponent<BlockSpawner>();
        initBlockSpawn();

        //Setting default settings 
        moveRightKey = KeyCode.D;
        moveLeftKey = KeyCode.A;
        rotateKey = KeyCode.R;
        speedUpKey = KeyCode.S;
        rotated = 0;
        isBlockDown = false;
    }

    void Update()
    {   

        //Managing player's inputs
        if (!isBlockDown)
        {
            if (Input.GetKeyUp(moveRightKey)) moveHorizontal(horizontalOffset);
            else if (Input.GetKeyUp(moveLeftKey)) moveHorizontal(-horizontalOffset);
            else if (Input.GetKeyUp(rotateKey)) blockRotate();
            
            //Imitating gravity
            float new_offset = -gravitySpeed * Time.deltaTime;
            if (Input.GetKey(speedUpKey)) 
            {
                new_offset = new_offset * acceleration;
            }
            
            //Checking if block collided
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

    private void moveHorizontal(int offset)
    {
        Transform currBlock = currentBlock.transform;
        if((currBlock.position.x + offset - leftBound >= 0) & (currBlock.position.x + offset + rightBound <= mapWidth))
            currBlock.position = new Vector3((int)currBlock.position.x + offset, currBlock.position.y, 0);
        else Debug.Log("Out of bounds in x axis");
    }

    private void blockRotate()
    {
        //Probably this should be changed into temp transform
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
                //Debug.Log("Shift from right by: " + (child.position.x + rightBound - mapWidth).ToString());
                //Debug.Log("Child position: " + child.position.x.ToString());
                currBlock.position += new Vector3(-(child.position.x - mapWidth), 0, 0);
            }
        }
    }

    //Method that spawns new block
    private void initBlockSpawn()
    {
        //Debug.Log("Spawn Initialized");
        currentBlock =  blockSpawner.spawnBlock();
        //Reseting options
        updateDimensions();
        rotated = 0;
        isBlockDown = false;
    }

    //Method that finds bounds of building blocks inside parent block
    private void updateDimensions()
    {
        int min_h = 0, max_h = 0, max_v = 0, min_v = 0;     //temp parameters

        foreach(Transform child in currentBlock.transform)
        {
            //Simple translation of local axis to world axis
            int xpos = rotated % 2 == 1 ? (int) child.localPosition.y : (int) child.localPosition.x;
            int ypos = rotated % 2 == 1 ? (int) child.localPosition.x : (int) child.localPosition.y;
            
            //Horizontal bounds
            if(xpos > max_h) max_h = xpos;
            else if(xpos < min_h) min_h = xpos;
            //Vertical bounds
            if(ypos > max_v) max_v = ypos;
            else if(ypos < min_v) min_v = ypos;
        }
        
        //Depending on current relation of local axis (in reference to global ones) we find max or min bounds 
        leftBound = (rotated == 0 || rotated == 3) ? min_h : max_h;
        rightBound = (rotated == 0 || rotated == 3) ? max_h : min_h;
        downBound = rotated < 2 ? min_v : max_v;

        //Translating into absolute distance
        leftBound = Mathf.Abs(leftBound);
        rightBound = Mathf.Abs(rightBound);
        downBound = Mathf.Abs(downBound);

        Debug.Log("Crucial dimensions: left - " + leftBound.ToString() + " , right - " + rightBound.ToString() + " , down - " + downBound.ToString() + " Current rotation: " + rotated.ToString());
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
