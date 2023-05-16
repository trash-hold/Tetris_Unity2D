using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D myRigidbody;

    //For future option to change keys in settings menu
    public KeyCode moveRightKey, moveLeftKey, rotateKey; 
    public bool isDown;
    private int Horizontal_offset = 1;
    void Start()
    {
        moveRightKey = KeyCode.D;
        moveLeftKey = KeyCode.A;
        rotateKey = KeyCode.R;
        isDown = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isDown)
        {
            if (Input.GetKeyUp(moveRightKey)) MoveHorizontal(Horizontal_offset);
            else if (Input.GetKeyUp(moveLeftKey)) MoveHorizontal(-Horizontal_offset);
            else if (Input.GetKeyUp(rotateKey)) Rotate();
        }
    }

    private void MoveHorizontal(int offset)
    {
        transform.position = new Vector3((int)transform.position.x + offset, transform.position.y, transform.position.z);
    }

    private void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 90), Space.Self);
    }
}
