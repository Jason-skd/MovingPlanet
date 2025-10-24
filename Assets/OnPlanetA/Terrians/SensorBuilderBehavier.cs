using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorBuilderBehavier : MonoBehaviour
{
    bool if_put = false;
    private bool is_ground;
    private GameObject[] sensorbuilding;
    private LayerMask layerMask = 1;
    private int energyvalue = 0;
    private float rastcasthitdistance = 0.2f;
    private FixedJoint fixedJoint;
    private Rigidbody rigidbody;
    private float breakforce = Mathf.Infinity;
    private float breaktorque = Mathf.Infinity;
    public void isPut()
    {
        if_put = true;
    }
   public void put(Vector3 position)
    {
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        Vector3 rayStart1 = position + Vector3.right * 0.1f;
        Vector3 rayStart3 = position + Vector3.forward * 0.1f;
        Vector3 rayStart4 = position + Vector3.left * 0.1f;
        GameObject onesensor = new GameObject("SensorBuilder");
        Physics.SphereCast(rayStart1, 1f, Vector3.right, out hit1);
        Physics.SphereCast(rayStart3, 1f, Vector3.forward, out hit2);
        Physics.SphereCast(rayStart4, 1f, Vector3.left, out hit3);
            if (if_put) {
                if (hit1.collider.CompareTag("Player")||hit2.collider.CompareTag("Player")||hit3.collider.CompareTag("Player")){
                    onesensor.SetActive(true);
                    Instantiate(onesensor);
                    onesensor.transform.position = position;
                }
            }
        
    }
   private void AlignToGrand(){
        foreach (GameObject sensor in sensorbuilding)
        {

            RaycastHit hit;

            Vector3 rayStart = sensor.transform.position + Vector3.up * 0.1f;

            if (Physics.SphereCast(rayStart, 0.5f, Vector3.down, out hit))
            {
                is_ground = true;
                rigidbody = sensor.GetComponent<Rigidbody>();
                fixedJoint = sensor.GetComponent<FixedJoint>();
                if (rigidbody == null || fixedJoint == null)
                {
                    rigidbody = sensor.AddComponent<Rigidbody>();
                    fixedJoint = sensor.AddComponent<FixedJoint>();


                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        fixedJoint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody>();
                    }
                    else
                    {
                        hit.collider.gameObject.AddComponent<Rigidbody>();
                        fixedJoint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody>();
                        fixedJoint.breakForce = breakforce;
                        fixedJoint.breakTorque = breaktorque;
                    }
                }
            }
            sensor.transform.position = hit.point;
            sensor.transform.rotation = Quaternion.FromToRotation(sensorbuilding[0].transform.position, hit.normal) * sensorbuilding[0].transform.rotation;
        }
    }

    private void ApplyGravty()
    {
        foreach (GameObject sensor in sensorbuilding)
        {
            rigidbody = sensor.GetComponent<Rigidbody>();

            if (!is_ground)
            {
                if (rigidbody != null)
                {
                    rigidbody.AddForce(Physics.gravity * rigidbody.mass);
                }
                else
                {
                    sensor.AddComponent<Rigidbody>();
                }
            }
        }
    }
    //能量获取
   public void SetEnergy(int value)
    {
        energyvalue = value; 
    }
    public List<Vector3> DetectEnemyPositions()
    {
        if (energyvalue != 0)
        {
            float detectRadius = energyvalue / 100f;
            string str = "Enermy";
            int count3 = 0;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius);
            List<Vector3> enemyPositions = new List<Vector3>();
            foreach (var hitCollider in hitColliders)
            {
                for (int count = 0; count < hitCollider.gameObject.tag.Length - str.Length; count++)
                {
                    for (int count2 = count; count2 < count + str.Length; count2++)
                    {
                        if (hitCollider.gameObject.tag[count2] == str[count2 - count] && count3 == str.Length)
                        {
                            enemyPositions.Add(hitCollider.gameObject.transform.position);
                            count3 = 0;
                        }
                        count3++;
                    }
                }
            }
            return enemyPositions;
        }
        else
        {
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sensorbuilding = GameObject.FindGameObjectsWithTag("SensorBuilder");
        foreach (GameObject sensor in sensorbuilding)
        {
            sensor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AlignToGrand();
        ApplyGravty();
       
    }
    private void Awake()
    {
        
    }
}
