using UnityEngine;

public class PlayerControllerRigidbody : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 7.0f;
    public float mouseSensitivity = 2.0f;

    private Rigidbody rb;
    private Camera playerCamera;
    private float rotationX = 0;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        // �������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ���� Rigidbody Լ������ֹ��ɫ��б
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // ����ӽǿ���
        HandleMouseLook();

        // ��Ծ����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // �ƶ������� FixedUpdate �н��У���Ϊ�漰����
        HandleMovement();
    }

    void HandleMovement()
    {
        // ��ȡ����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ƶ�����
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Ӧ���ƶ���
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void HandleMouseLook()
    {
        // ��ȡ�������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ˮƽ��ת
        transform.Rotate(0, mouseX, 0);

        // ��ֱ��ת
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // ����Ƿ��ڵ�����
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // �뿪����
        isGrounded = false;
    }
}