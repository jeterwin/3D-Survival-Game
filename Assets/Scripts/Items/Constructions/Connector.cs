using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    public ConnectorPosition ConnectorPosition;
    public SelectedBuildType ConnectorParentType;

    [HideInInspector] public bool IsConnectedToFloor = false;
    [HideInInspector] public bool IsConnectedToWall = false;
    [HideInInspector] public bool CanConnectTo = true;

    [SerializeField] private bool canConnectToFloor = true;
    [SerializeField] private bool canConnectToWall = true;

    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void OnValidate()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = IsConnectedToFloor ? (IsConnectedToWall ? Color.red : Color.blue) : (!IsConnectedToWall ? Color.green : Color.yellow);
        Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
    }

    public void UpdateConnectors(bool rootCall = false)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereCollider.radius);
        IsConnectedToFloor = !canConnectToFloor;
        IsConnectedToWall = !canConnectToWall;

        foreach(Collider collider in colliders)
        {
            if(collider.GetInstanceID() == GetComponent<Collider>().GetInstanceID())
            {
                continue;
            }

            if(!collider.gameObject.activeInHierarchy)
            {
                continue;
            }

            if(collider.gameObject.layer == gameObject.layer)
            {
                Connector foundConnector = collider.GetComponent<Connector>();

                if(foundConnector.ConnectorParentType == SelectedBuildType.Floor)
                {
                    IsConnectedToFloor = true;
                }

                if(foundConnector.ConnectorParentType == SelectedBuildType.Wall)
                {
                    IsConnectedToWall = true;
                }

                if(rootCall)
                {
                    foundConnector.UpdateConnectors();
                }
            }
        }

        CanConnectTo = true;

        if(IsConnectedToFloor && IsConnectedToWall)
        {
            CanConnectTo = false;
        }
    }
}
[System.Serializable]
public enum ConnectorPosition
{
    Left,
    Right,
    Top,
    Bottom
}