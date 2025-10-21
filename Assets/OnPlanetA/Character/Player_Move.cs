using UnityEngine;

public class PlayerControllerRigidbody : MonoBehaviour
{
    [Header("移动设置")]
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

        // 锁定光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 设置 Rigidbody 约束，防止角色倾斜
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // 鼠标视角控制
        HandleMouseLook();

        // 跳跃输入
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // 移动处理在 FixedUpdate 中进行，因为涉及物理
        HandleMovement();
    }

    void HandleMovement()
    {
        // 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        // 应用移动力
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void HandleMouseLook()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 水平旋转
        transform.Rotate(0, mouseX, 0);

        // 垂直旋转
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检测是否在地面上
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 离开地面
        isGrounded = false;
    }
}