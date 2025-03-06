using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionProvinceText : MonoBehaviour
{

    public LayerMask mask;

    public void SetPosition()
    {
    /*    RaycastHit hit;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.cyan, 6000);
        if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit, Mathf.Infinity,mask))
        {




            Vector3 tempPos = transform.localToWorldMatrix.GetPosition();
            
            Debug.Log(hit.point.z + " pos: " + tempPos.z );
            GetComponent<RectTransform>().position = new Vector3(tempPos.x,tempPos.y,hit.point.z); 

        }*/


    }



}
