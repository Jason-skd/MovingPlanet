using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorBuilderBehavier : MonoBehaviour
{
    bool if_put = false;
    private bool is_ground;
    private CharacterController characterController;
    private GameObject[] sensorbuilding;
    private LayerMask layerMask = 1;
    private int energyvalue = 0;
    private float rastcasthitdistance = 0.2f;
   
   public void isPut()
    {
        if_put = true;
    }
   private void put(Vector3 position)
    {
        
        sensorbuilding[0].SetActive(true);
        Instantiate(sensorbuilding[0]);
        sensorbuilding[0].transform.position = position;

       
    }
   private void AlignToGrand(){
        RaycastHit hit;
        
        Vector3 rayStart = sensorbuilding[0].transform.position+Vector3.up*0.1f;
       
        if (Physics.Raycast(rayStart,Vector3.down,out hit,rastcasthitdistance,layerMask))
        {
            is_ground = true;
        }
        sensorbuilding[0].transform.position = hit.point;
        sensorbuilding[0].transform.rotation = Quaternion.FromToRotation(sensorbuilding[0].transform.position,hit.normal)*sensorbuilding[0].transform.rotation;

    }
   private void ApplyGravty()
    {

    }
    //能量获取
   public void SetEnergy(int value)
    {
        energyvalue = value; 
    }
   /**public Vector3[] DetectEnemyPositions()
    {
       
    }
   **/
    // Start is called before the first frame update
    void Start()
    {
        sensorbuilding = GameObject.FindGameObjectsWithTag("SensorBuilder");
        sensorbuilding[0].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
