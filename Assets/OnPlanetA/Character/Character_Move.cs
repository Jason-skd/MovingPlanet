using UnityEngine;

public class ThirdPersonPlanetController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float jumpForce = 8f;

    [Header("��������")]
    public Transform planetCenter;
    public float planetRadius = 10f;
    public float gravityForce = 9.8f;

    [Header("������")]
    public LayerMask groundLayer = 1;
    public float groundCheckDistance = 0.2f;

    // ˽�б���
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 gravityDirection;
    private bool isGrounded;
    private float verticalVelocity;
    private float horizontalRotation; // ˮƽ��ת�ۻ���

    // ������أ���ѡ��
    private Animator animator;

    void Start()
    {
        // ��ȡ���
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // ���û�������������ģ�Ĭ��ʹ������ԭ��
        if (planetCenter == null)
        {
            GameObject centerObj = new GameObject("PlanetCenter");
            planetCenter = centerObj.transform;
            planetCenter.position = Vector3.zero;
        }

        // ��ʼ����ת
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
        // ��ȡWASD����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ƶ����򣨻��ڽ�ɫ�ľֲ�����ϵ��
        moveDirection = Vector3.zero;

        if (vertical > 0)
        {
            moveDirection += transform.forward; // ��ǰ�ƶ�
        }
        else if (vertical < 0)
        {
            moveDirection -= transform.forward; // ����ƶ�
        }

        // ������ת�����ƶ���ֻ��ת��
        if (horizontal != 0)
        {
            horizontalRotation += horizontal * rotationSpeed * 100 * Time.deltaTime;
        }

        // ��Ծ����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        // ��׼���ƶ�����
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }
    }

    void ApplyGravity()
    {
        // �����������򣨴ӽ�ɫָ���������ģ�
        gravityDirection = (planetCenter.position - transform.position).normalized;

        // ������
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, gravityDirection, out hit,
                                   characterController.height / 2 + groundCheckDistance, groundLayer);

        // Ӧ������
        if (!isGrounded)
        {
            verticalVelocity -= gravityForce * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = -2f; // ��΢���µ���ȷ������
        }
    }

    void MoveCharacter()
    {
        // ˮƽ�ƶ�
        Vector3 horizontalMove = moveDirection * moveSpeed;

        // ��ֱ�ƶ�����������Ծ��
        Vector3 verticalMove = gravityDirection * verticalVelocity;

        // �ϲ��ƶ�
        Vector3 totalMove = horizontalMove + verticalMove;

        // Ӧ���ƶ�
        characterController.Move(totalMove * Time.deltaTime);

        // Ӧ��ˮƽ��ת
        Vector3 currentEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentEuler.x, horizontalRotation, currentEuler.z);
    }

    void AlignToPlanetSurface()
    {
        // �����ɫӦ�ó���ķ��򣨴�ֱ��������棩
        Vector3 surfaceNormal = (transform.position - planetCenter.position).normalized;

        // ƽ����ת��ɫ�����������
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        // �����Animator��������¶�������
        if (animator != null)
        {
            float moveMagnitude = moveDirection.magnitude;
            animator.SetFloat("Speed", moveMagnitude);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
    }

    // ���ӻ�����
    void OnDrawGizmos()
    {
        if (planetCenter != null)
        {
            // ��������뾶
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(planetCenter.position, planetRadius);

            // ������������
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, gravityDirection * 2f);

            // ���Ƶ�������
            Gizmos.color = isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawRay(transform.position, gravityDirection * (characterController.height / 2 + groundCheckDistance));

            // �����ƶ�����
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
        }
    }
}