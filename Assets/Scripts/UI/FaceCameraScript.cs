using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    public GameObject general;

    // Update is called once per frame
    void Update()
    {
        if (Player.general.isLux) {
            transform.position = general.transform.position + new Vector3(21f,22.8f,29.1f);
        } else {
            transform.position = general.transform.position + new Vector3(0.21f, 1.08f, 0.41f);
        }
        
    }
}
