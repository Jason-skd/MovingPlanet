using UnityEngine;

public class ThirdPersonPlanetController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float jumpForce = 8f;

    [Header("星球设置")]
    public Transform planetCenter;
    public float planetRadius = 10f;
    public float gravityForce = 9.8f;

    [Header("地面检测")]
    public LayerMask groundLayer = 1;
    public float groundCheckDistance = 0.2f;

    // 私有变量
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 gravityDirection;
    private bool isGrounded;
    private float verticalVelocity;
    private float horizontalRotation; // 水平旋转累积量

    // 动画相关（可选）
    private Animator animator;

    void Start()
    {
        // 获取组件
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // 如果没有设置星球中心，默认使用世界原点
        if (planetCenter == null)
        {
            GameObject centerObj = new GameObject("PlanetCenter");
            planetCenter = centerObj.transform;
            planetCenter.position = Vector3.zero;
        }

        // 初始化旋转
        horizontalRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleInput();
        ApplyGravity();
        MoveCharacter();
        AlignToPlanetSurface();
        UpdateAnimations();
    }

    void HandleInput()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向（基于角色的局部坐标系）
        moveDirection = Vector3.zero;

        if (vertical > 0)
        {
            moveDirection += transform.forward; // 向前移动
        }
        else if (vertical < 0)
        {
            moveDirection -= transform.forward; // 向后移动
        }

        // 左右旋转（不移动，只旋转）
        if (horizontal != 0)
        {
            horizontalRotation += horizontal * rotationSpeed * 100 * Time.deltaTime;
        }

        // 跳跃输入
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        // 标准化移动方向
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }
    }

    void ApplyGravity()
    {
        // 计算重力方向（从角色指向星球中心）
        gravityDirection = (planetCenter.position - transform.position).normalized;

        // 地面检测
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, gravityDirection, out hit,
                                   characterController.height / 2 + groundCheckDistance, groundLayer);

        // 应用重力
        if (!isGrounded)
        {
            verticalVelocity -= gravityForce * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = -2f; // 轻微向下的力确保贴地
        }
    }

    void MoveCharacter()
    {
        // 水平移动
        Vector3 horizontalMove = moveDirection * moveSpeed;

        // 垂直移动（重力和跳跃）
        Vector3 verticalMove = gravityDirection * verticalVelocity;

        // 合并移动
        Vector3 totalMove = horizontalMove + verticalMove;

        // 应用移动
        characterController.Move(totalMove * Time.deltaTime);

        // 应用水平旋转
        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentEuler.x, horizontalRotation, currentEuler.z);
    }

    void AlignToPlanetSurface()
    {
        // 计算角色应该朝向的方向（垂直于星球表面）
        Vector3 surfaceNormal = (transform.position - planetCenter.position).normalized;

        // 平滑旋转角色对齐星球表面
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        // 如果有Animator组件，更新动画参数
        if (animator != null)
        {
            float moveMagnitude = moveDirection.magnitude;
            animator.SetFloat("Speed", moveMagnitude);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        if (planetCenter != null)
        {
            // 绘制星球半径
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(planetCenter.position, planetRadius);

            // 绘制重力方向
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, gravityDirection * 2f);

            // 绘制地面检测线
            Gizmos.color = isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawRay(transform.position, gravityDirection * (characterController.height / 2 + groundCheckDistance));

            // 绘制移动方向
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
        }
    }
}