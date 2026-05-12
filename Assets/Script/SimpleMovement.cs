using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 8f; 
    public float runSpeed = 15f; 
    public float mouseSensitivity = 2f;
    public Transform playerCamera; 

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 verticalVelocity; 
    private float gravity = -20f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
       
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
      
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f); 
        
        if(playerCamera != null) {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        transform.Rotate(Vector3.up * mouseX);

        float x = Input.GetAxisRaw("Horizontal"); 
        float z = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f) 
        {
            Vector3 move = (transform.right * x + transform.forward * z).normalized;
            
            float speed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }

            controller.Move(move * speed * Time.deltaTime);
        }

       
        if (controller.isGrounded && verticalVelocity.y < 0) 
        {
            verticalVelocity.y = -2f;
        }
        
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}