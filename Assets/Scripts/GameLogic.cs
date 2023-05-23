using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    private BlockSpawner blockSpawner;
    void Start()
    {
        Debug.Log("Init of gamelogic");
        blockSpawner = GetComponent<BlockSpawner>();
        initBlockSpawn();
    }

    public void initBlockSpawn()
    {
        Debug.Log("Called spawnBlock inside gamelogic");
        blockSpawner.spawnBlock();
    }
}
