using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z+speed*Time.deltaTime);
        
        if(Input.GetKey(KeyCode.DownArrow))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z-speed*Time.deltaTime);
        
        if(Input.GetKey(KeyCode.LeftArrow))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+speed*Time.deltaTime, transform.eulerAngles.z);
        
        if(Input.GetKey(KeyCode.RightArrow))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y-speed*Time.deltaTime, transform.eulerAngles.z);

        if(Input.GetKey(KeyCode.R))
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
