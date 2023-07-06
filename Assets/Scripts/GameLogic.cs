using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    //Main objects
    private BlockSpawner blockSpawner;              //Responsible for handling blocks' spawning
    private Transform currentBlock;                //Block that is currently steered and managed by gamelogic
    
    //Info about current block
    private int leftBound, rightBound, downBound;   //Keeps (left/right/up/down)most coordinate of currentBlock
    private int rotated = 0;                        //Counts how many 90 deegress turns where made - has value from 0-3
    public bool isBlockDown;                        

    //Map size definition
    private static int mapWidth = 11;   //Amount of elements in row
    private static int mapHeight = 20;  //Amount of elements in a column
    private static int offset = 5;      //Offset caused by moving camera :)
    private float colliderMargin = 0.3f;

    //Game parameters
    public KeyCode moveRightKey, moveLeftKey, rotateKey, speedUpKey; 
    private int horizontalOffset = 1;   //How much does block move when on left/right click
    private static int tempBoundary = -9;      //Defines the Y value of the 'lowest' row 
    private float gravitySpeed = 10;    //Defines the speed of blocks falling down
    private float acceleration = 1.5f;  //Defines how many times faster does block move on speedUpKey 
    

    private DataTable blockMap = new DataTable(mapHeight, mapWidth, offset, tempBoundary);

    //Dev options

    public bool DevMode = false;
    public bool gravityOn = true;
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
            else if (Input.GetKeyUp(KeyCode.Space) & DevMode)
            {
                //debugInfo(0);
                isBlockDown = true;
            }
            
            //Imitating gravity
            float new_offset = -gravitySpeed * Time.deltaTime;
            if (Input.GetKey(speedUpKey)) 
            {
                new_offset = new_offset * acceleration;
            }
            
            //colliderMargin >>> new_offset => colliderMargin + new_offset ~ colliderMargin
            float errorMargin = new_offset - colliderMargin;
            if(blockMap.isCollision(currentBlock, errorMargin) == true)               //Checking if block collides with other blocks
            {
                //Debug.Log("Found collision");
                debugInfo(0);
                //If block is already halfway there it should go back 
                isBlockDown = true;
                int code = blockMap.AddData(currentBlock);
                //if (code == -2)
                    //Debug.LogFormat("Parent: {0}; Rotation: {1}; down/left/right {2}, {3}, {4};", currentBlock.name, rotated, downBound, rightBound, leftBound);
            }
            else if (currentBlock.position.y + new_offset - downBound < tempBoundary)  //Checking if block collided with down boundary 
            {
                //Debug.Log("Out of down bound");
                isBlockDown = true; 
                currentBlock.position = new Vector3(Mathf.Round(currentBlock.position.x), Mathf.Round(tempBoundary + downBound), 0);
                int code = blockMap.AddData(currentBlock);
                if (code == -2)
                    debugInfo(0);
            }
            else if(gravityOn) currentBlock.position += new Vector3(0, new_offset, 0);
            //Debug.Log("On the move");
        }
        else
        {
            initBlockSpawn();
        }
    }

    private void debugInfo(int option)
    {
        switch(option)
        {
            case 0:
                Debug.LogFormat("Parent: {0}; Rotation: {1}; down/right/left {2}, {3}, {4};", currentBlock.name, rotated, downBound, rightBound, leftBound); 
                break;

        }
    }

    private void moveHorizontal(int xoffset)
    {
        float xpos = currentBlock.position.x;
        if((xpos + xoffset - leftBound >= 0) & (xpos + xoffset + rightBound < mapWidth))
        {
            if(blockMap.isCollision(currentBlock, 0, xoffset) == false)
            {
                currentBlock.position = new Vector3(Mathf.Round(currentBlock.position.x + xoffset), currentBlock.position.y, 0);
            }
        }
        else
        {
            //Debug.Log("Out of bounds in x axis");
            //debugInfo(0);
        }
    }

    private void blockRotate()
    {
        //Probably this should be changed into temp transform
        currentBlock.Rotate(new Vector3(0, 0, 90), Space.Self);
        Transform currBlock = currentBlock;
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
                currBlock.position += new Vector3(-Mathf.Round(child.position.x), 0, 0);
            }
            else if(child.position.x + rightBound > mapWidth)
            {
                //Debug.Log("Shift from right by: " + (child.position.x + rightBound - mapWidth).ToString());
                //Debug.Log("Child position: " + child.position.x.ToString());
                currBlock.position += new Vector3(-Mathf.Round(child.position.x - mapWidth), 0, 0);
            }
        }
    }

    //Method that spawns new block and resets previous parameters
    private void initBlockSpawn()
    {
        //Debug.Log("Spawn Initialized");
        GameObject new_block =  blockSpawner.spawnBlock();
        currentBlock = new_block.transform;

        //Reseting options
        /*leftBound = 0; 
        rightBound = 0;
        downBound = 0;*/
        rotated = 0;
        updateDimensions();
        isBlockDown = false;
    }

    //Method that finds bounds of building blocks inside parent block
    private void updateDimensions()
    {
        int min_h = 0, max_h = 0, max_v = 0, min_v = 0;     //temp parameters

        foreach(Transform child in currentBlock)
        {
            //Simple translation of local axis to world axis
            int xpos = (rotated % 2 == 1) ? (int) child.localPosition.y : (int) child.localPosition.x;
            int ypos = (rotated % 2 == 1) ? (int) child.localPosition.x : (int) child.localPosition.y;
            
            //Horizontal bounds
            if(xpos > max_h) max_h = xpos;
            if(xpos < min_h) min_h = xpos;
            //Vertical bounds
            if(ypos > max_v) max_v = ypos;
            if(ypos < min_v) min_v = ypos;
        }
        
        Debug.LogFormat("Horizontal: min {0} max {1}, Vertical min {2} max {3}", min_h, max_h, min_v, max_v);
        //Depending on current relation of local axis (in reference to global ones) we find max or min bounds 
        leftBound = (rotated == 0 || rotated == 3) ? min_h : max_h;
        rightBound = (rotated == 0 || rotated == 3) ? max_h : min_h;
        downBound = rotated < 2 ? min_v : max_v;

        //Translating into absolute distance
        leftBound = Mathf.Abs(leftBound);
        rightBound = Mathf.Abs(rightBound);
        downBound = Mathf.Abs(downBound);

        debugInfo(0);

        //Debug.Log("Crucial dimensions: left - " + leftBound.ToString() + " , right - " + rightBound.ToString() + " , down - " + downBound.ToString() + " Current rotation: " + rotated.ToString());
    }

}

public class BlockData
{
    //Container for info about block with easy access to its variables
    public int x, y;
    private int dataOffsetY;
    private Transform block;
    private GameObject blockObj;

    public BlockData(Transform new_block, int dataOffset)
    {
        blockObj = new_block.gameObject;
        block = new_block;
        x = (int) Mathf.Round(block.position.x);
        y = (int) Mathf.Round(block.position.y) - dataOffset;
        //Debug.Log("My new y is: " + y.ToString());
    }

    public void dropBlock(int dropHeight = 1)
    {
        debugInfo();
        block.position -= new Vector3(0, dropHeight, 0);
        y -= dropHeight;
        //Debug.LogFormat("Moved block: {0} at y: {1}", block.name, y);

    }

    public void DestroyBlock()
    {
        UnityEngine.Object.Destroy(blockObj);
    }
    
    public void debugInfo()
    {
        Debug.LogFormat("Parent: {0}, block: {1}", block.parent.name, block.name);
    }
}

public class DataTable
{
    private int Height, Width;
    private bool[,] isTaken;
    private int dataOffsetX, dataOffsetY;

    private BlockData[,] blockMap;
    private int[] count;

    public DataTable(int height, int width, int offsetx, int offsety)
    {
        if(height >= 0 && width >= 0)
            {
                Height = height;
                Width = width;
                dataOffsetX = offsetx;
                dataOffsetY = offsety;
                blockMap = new BlockData[width, height];
                count = new int[height];
                isTaken = new bool[width, height];
            }
        else Debug.LogError("Placeholder error in Datarow init");
    }

    public int AddData(Transform parentBlock)
    {
        parentBlock.position = new Vector3(Mathf.Round(parentBlock.position.x), Mathf.Round(parentBlock.position.y), 0);

        int code = 0;
        //To avoid bugs first we add all children into the dataTable and THEN clear all the rows :)
        List<int> rowsToClear = new List<int>();
        foreach(Transform child in parentBlock)
        {
            code = addData(child);
            if(code < - 1) 
            {
                Debug.Log("ERROR WITH ADDING DATA");
                return code;
            }
            else if (code >= 0 ) rowsToClear.Add(code);
        }

        foreach(int n in rowsToClear)
        {
            clearData(n);
        }
        return code;
    }
    //Returns: -2 when error occured; -1 when data was added succesfully; int i >= 0 when i-th row was filled and cleared  
    private int addData(Transform blockTransform)
    {   
        BlockData new_block = new BlockData(blockTransform, dataOffsetY); 
        if(new_block.x < Width && new_block.y < Height)
        {
            //Debug.Log("Added block with (x,y): " + new_block.x.ToString() + " " + new_block.y.ToString());

            blockMap[new_block.x, new_block.y] = new_block;
            isTaken[new_block.x, new_block.y] = true;
            count[new_block.y]++;

            if(count[new_block.y] >= Width)
            {
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

    //Ask whether all of the blocks inside parent are on cells that are free
    public bool isCollision(Transform parent, float yoffset, float xoffset = 0)
    {
        bool collided = false;

        //Mock parent to test move and dont loose current position
        foreach(Transform child in parent)
        {
            int x = (int) Mathf.Round(child.position.x + xoffset);
            int y = (int) Mathf.Round(child.position.y + yoffset) - dataOffsetY;

            try
            {
                if(isTaken[x, y] == true) return true;
            } 
            catch
            {
                //Debug.LogFormat("Bad index, block out of bounds: ({0}, {1})", x, y);    
            }
        }

        return collided;
    }

    //Changes data arrays accordingly when a row is cleared
    private void clearData(int rowIndex)
    {   
        //Debug.LogFormat("Deleting row: {0}", rowIndex);
        //First cleaning out deleted row
        for(int i = 0; i < Width; i++)
        {
            blockMap[i, rowIndex].DestroyBlock();
            blockMap[i, rowIndex] = null;
            isTaken[i, rowIndex] = false;
        }
        count[rowIndex] = 0;

        for(int row = rowIndex; row < Height; row++)
        {   
            Debug.Log("Row " + row.ToString());
            //Exchanging data between rows
            for(int column = 0; column < Width; column++)
            {   
                //Changing row indexes of blocks and dropping them on the visible grid
                if(isTaken[column, row + 1] == true)
                {
                    blockMap[column, row] = blockMap[column, row + 1];
                    blockMap[column, row].dropBlock();
                    isTaken[column, row] = true;
                    isTaken[column, row + 1] = false;
                }
            }
            //Exchanging data about rows
            count[row] = count[row + 1];
        }
    }
}
