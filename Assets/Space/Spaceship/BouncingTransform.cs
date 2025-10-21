using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BouncingTransform : MonoBehaviour
{
    public float powerMultiplier = 0.1f;
    public float maximumForce = 500f;
    public float fixedZ;
    
    private Rigidbody _rb;
    private LineRenderer _lineRenderer;
    
    private bool _isDragging;
    private Vector2 _startDragPos;
    private Vector2 _endDragPos;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _startDragPos = Input.mousePosition;
            _lineRenderer.positionCount = 2;
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            UpdateAimLine();
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            Moving(GetDeltaDrag(_startDragPos));
            
            // 隐藏瞄准线
            _lineRenderer.positionCount = 0;
        }
    }

    void Moving(Vector2 movement)
    {
        if (movement.sqrMagnitude < 0.1f) return;
        float force = Mathf.Min(maximumForce, movement.magnitude * powerMultiplier);
        Vector3 direction = new Vector3(movement.x, movement.y, fixedZ).normalized;
        
        // 重置速度防止多次拖拽导致速度累加
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        // 旋转
        float rotateAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (rotateAngle >= -90 && rotateAngle <= 90)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            rotateAngle += 180;  // 如果飞船默认朝左
        }
        transform.rotation = Quaternion.Euler(0, 0, rotateAngle);
        
        // 移动
        _rb.AddForce(direction * force, ForceMode.Impulse);
    }

    void UpdateAimLine()
    {
        // 起点：弹球当前位置（固定Z轴）
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, fixedZ);
        
        // 终点：鼠标在世界空间的位置（转换到2.5D平面）
        Vector3 endPos = GetMouseWorldPosition();
        
        // 更新瞄准线的两个点
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    Vector2 GetDeltaDrag(Vector2 startDragPos)
    {
        _endDragPos = Input.mousePosition;
        Vector2 deltaDrag = startDragPos - _endDragPos;
        return deltaDrag;
    }
    
    // 辅助方法：将鼠标屏幕坐标转换为世界坐标（适配2.5D侧视图）
    Vector3 GetMouseWorldPosition()
    {
        // 从相机发射射线，计算与Z=fixedZ平面的交点
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 数学计算射线与目标平面的交点（无需碰撞体）
        float t = (fixedZ - ray.origin.z) / ray.direction.z;
        Vector3 worldPos = ray.origin + ray.direction * t;
        // 确保Z轴固定（侧视图平面）
        return new Vector3(worldPos.x, worldPos.y, fixedZ);
    }
}