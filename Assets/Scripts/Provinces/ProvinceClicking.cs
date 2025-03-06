using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceClicking : MonoBehaviour
{
    public static ProvinceClicking singleton;

    Camera cam;
    public GameObject map;
    public float sphereRadius = 10;

    [SerializeField] private LayerMask provinceLayer = default;

    Vector3 hitPont;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        //sorteret array binaray search
        // mindre arealer, hvis den er i den her del så kigger den kun på nogen af dem da den ved den kun er tæt på noget af dem 


        //Det point som man klikker på giver faktisk rigtigt koordinat i world map, så det jo dejeligt!


        //NÅR MAN KLIKKER SKAL DER TJEKKES HVILKEN PRIK DEN ER TÆTTEST PÅ (KAN SES INDE I GroupedSorter, tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);, er på linje 64)!
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitPont = hit.point;

                GetNearestProvince(hit.point).OpenProvinceInterface();
            }

            Debug.DrawRay(ray.origin, ray.direction * 100000, Color.yellow);
        }
       
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitPont, sphereRadius);
    }

    public ProvinceScript GetNearestProvince(Vector3 pos)
    {
        float lowest = 99999;
        int collider = 0;
        Collider[] hitColliders = Physics.OverlapSphere(pos, sphereRadius, provinceLayer);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            float tempDistance = Vector3.Distance(hitColliders[i].transform.position, pos);
            if(tempDistance < lowest)
            {
                lowest = tempDistance;
                collider = i;
            }
        }
        return hitColliders[collider].GetComponent<ProvinceScript>();
    }
}
