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
    private float GravitySpeed = 4;
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

            transform.position += new Vector3(0, -GravitySpeed * Time.deltaTime, 0);

            //Debug.Log("On the move");
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "colliderObj")
        {
            //First setting up parameters so that the object does no longer move:
            isDown = true;
            gameObject.tag = "colliderObj";

            //Making sure that it doesnt end up in a weird rotation after collision
            transform.position = new Vector3((int) transform.position.x, (int) transform.position.y, 0);
            Debug.Log("Current tag is: " + gameObject.tag.ToString());
        }
        //Debug.Log("Detected collision");
    }
}
