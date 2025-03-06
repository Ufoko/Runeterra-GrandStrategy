using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class PlayerCommanding : NetworkBehaviour
{
    private List<Army> selectedArmies = new List<Army>();
    private bool selectedThisTick;

    RenderTexture cameraIcon;

    private void Start() {
        cameraIcon = new RenderTexture(256, 256, -100);
    }

    private void LateUpdate()
    {
        if(!hasAuthority)
            return;

        /*if(!EventSystem.current.IsPointerOverGameObject())
        {*/
            if(!selectedThisTick && Input.GetMouseButtonDown(0))
            {
                DeselectAll();
            }
            else if(Input.GetMouseButtonDown(1) && !Minions.singleton.minionWalkPhase)
            {
                for(int i = 0; i < selectedArmies.Count; ++i)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    const float dist = 1000f;
                    if(Physics.Raycast(ray, out RaycastHit hit, dist))
                    {
                        selectedArmies[i].movement.SetNewDestination(hit.point);
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 5f);
                    }
                    else
                        Debug.DrawRay(ray.origin, ray.direction * dist, Color.red, 5f);
                }
        //    }
        }
        selectedThisTick = false;
    }

    [Client]
    public void DeselectAll()
    {
        for(int i = 0; i < selectedArmies.Count; i++)
        {
            selectedArmies[i].cameraIcon.SetActive(false);
        }
        selectedArmies.Clear();
    }

    [Client]
    public void SelectArmy(Army army)
    {
        selectedArmies.Add(army);
        selectedThisTick = true;
        army.isSelected = true;
        army.cameraIcon.SetActive(true);
    }
}
