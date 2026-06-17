using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private Transform playerBody;

    private float m_xRotation = 0f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; ;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; ;

        m_xRotation -= mouseY;
        m_xRotation = Mathf.Clamp(m_xRotation, -45f, 45f);

        Camera.main.transform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
