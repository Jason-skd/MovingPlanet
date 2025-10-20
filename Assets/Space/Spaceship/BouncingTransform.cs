using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BouncingTransform : MonoBehaviour
{
    public float powerMultiplier = 0.1f;
    public float maximumForce = 500f;

    private bool _isDragging;
    private Rigidbody _rb;
    private Vector2 _startDragPos;
    private Vector2 _endDragPos;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _startDragPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            _endDragPos = Input.mousePosition;
            Vector2 deltaDrag = _startDragPos - _endDragPos;
            Moving(deltaDrag);
        }
    }

    public void Moving(Vector2 movement)
    {
        if (movement.sqrMagnitude < 0.1f) return;
        float force = Mathf.Min(maximumForce, movement.magnitude * powerMultiplier);
        Vector3 direction = new Vector3(movement.x, movement.y, 0).normalized;
        
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
}
