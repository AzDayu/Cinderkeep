using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    private Rigidbody m_rigidbody;
    private Vector3 m_vector;


    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");  

        Vector3 moveDirection = (transform.right * h) + (transform.forward * v);
        m_vector = moveDirection.normalized;
    }

    public void MoveCharacter(Vector3 moveDirection)
    {
        m_vector = moveDirection;
    }
    void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = m_vector.magnitude > 0.01f;
        bool isRunning = isMoving && isShiftPressed;

        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector3 targetVelocity = m_vector * currentSpeed;

        targetVelocity.y = m_rigidbody.linearVelocity.y;
        m_rigidbody.linearVelocity = targetVelocity;


    }
}
