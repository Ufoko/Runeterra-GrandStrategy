using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Mirror;

public class ArmyMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Army army = default;
    [SerializeField] private Seeker seeker = default;
    [SerializeField] private AILerp aiLerp = default;

    public LineRenderer lineRenderer;

    [Header("Values")]
    [SerializeField] private float lineHeightOffset = 10f;

    public bool isStopped => aiLerp.reachedEndOfPath || !aiLerp.hasPath;
    private bool hasStopped;
    public ProvinceScript province { private set; get; }

    [System.NonSerialized] public List<Vector3> destinations = new List<Vector3>();

    private float originalSpeed;

    private void Awake()
    {
        originalSpeed = aiLerp.speed;
    }

    private void LateUpdate()
    {
        if(isServer)
        {
            ProvinceScript newProvince = ProvinceClicking.singleton.GetNearestProvince(transform.position);
            if(newProvince != province)
            {
                if(province)
                    province.armies.Remove(army);
                newProvince.NewArmy(army);

                province = newProvince;
            }
        }

        if(isStopped)
        {
            if(!hasStopped)
            {
                if(hasAuthority)
                {
                    army.animScript.CmdSetIdleAnim(true);
                    if(destinations.Count > 0 && (!Minions.singleton.minionWalkPhase || Minions.singleton.army != army))
                    {
                        destinations.RemoveAt(0);
                        if(destinations.Count > 0)
                        {
                            SetNewDestination(destinations[0]);
                            return;
                        }
                    }
                }
                aiLerp.SetPath(null);
                lineRenderer.positionCount = 0;
                hasStopped = true;
            }
        }
        else if(army.isSelected)
        {
            List<Vector3> positions = new List<Vector3>();
            aiLerp.GetRemainingPath(positions, out bool _);
            for(int i = 1; i < destinations.Count; ++i)
                positions.Add(destinations[i]);
            lineRenderer.positionCount = positions.Count;
            for(int i = 0; i < positions.Count; ++i)
            {
                Vector3 pos = positions[i];
                pos.y += lineHeightOffset;
                lineRenderer.SetPosition(i, pos);
            }
        }

    }

    public void ClearPath()
    {
        aiLerp.SetPath(null);
    }

    [TargetRpc]
    public void TargetAllowMovement(NetworkConnection ownerConnection, bool value)
    {
        AllowMovement(value);
    }
    public void AllowMovement(bool value)
    {
        aiLerp.speed = value ? originalSpeed : 0f;
    }

    public void SetNewDestination(Vector3 destination)
    {
        seeker.StartPath(transform.position, GetNodePos(destination), OnPathComplete);
        army.animScript.CmdSetWalkingAnim(true);
    }

    private static Vector3 GetNodePos(Vector3 pos)
    {
        NNInfoInternal pathfinderPoint = AstarPath.active.graphs[0].GetNearestForce(pos, NNConstraint.Default);
        if(pathfinderPoint.node == null)
            return Vector3.zero;
        return (Vector3)pathfinderPoint.node.position;
    }

    private void OnPathComplete(Path path)
    {
        if(path.error)
            Debug.LogWarning("[A* Warning]: " + path.error);
        else
            hasStopped = false;
    }

    private static bool IsInNodeRange(Vector3 node, Vector3 otherNode)
    {
        return node == otherNode;
    }

}
