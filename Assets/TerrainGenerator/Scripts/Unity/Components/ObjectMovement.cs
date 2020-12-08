using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float speed = 50f;

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private float initialZoom;

    private float zoomLimit = 40f;
    private float positionLimitZ = 130f;
    private float positionLimitY = 70f;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
        initialZoom = Camera.main.fieldOfView;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.D))
            if(transform.position.z < initialPosition.z + positionLimitZ)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z+speed*Time.deltaTime);
        
        if(Input.GetKey(KeyCode.A))
            if(transform.position.z > initialPosition.z -positionLimitZ)
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z-speed*Time.deltaTime);

        if(Input.GetKey(KeyCode.W))
            if(transform.position.y < initialPosition.y + positionLimitY)
                transform.position = new Vector3(transform.position.x, transform.position.y+speed*Time.deltaTime, transform.position.z);
        
        if(Input.GetKey(KeyCode.S))
            if(transform.position.y > initialPosition.y - positionLimitY)
                transform.position = new Vector3(transform.position.x, transform.position.y-speed*Time.deltaTime, transform.position.z);
        
        if(Input.GetKey(KeyCode.Q))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+speed*Time.deltaTime, transform.eulerAngles.z);
        
        if(Input.GetKey(KeyCode.E))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y-speed*Time.deltaTime, transform.eulerAngles.z);

        if(Input.GetKey(KeyCode.Z))
            if(Camera.main.fieldOfView < initialZoom+zoomLimit)
                Camera.main.fieldOfView += speed * Time.deltaTime;

        if(Input.GetKey(KeyCode.X))
            if(Camera.main.fieldOfView > initialZoom-zoomLimit)
                Camera.main.fieldOfView -= speed * Time.deltaTime;

        if(Input.GetKey(KeyCode.R))
        {
            transform.position = initialPosition;
            transform.eulerAngles = initialRotation;
            Camera.main.fieldOfView = initialZoom;
        }
    }
}
