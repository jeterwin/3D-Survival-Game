using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField] private Camera mainCam;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buildingClip;
    [SerializeField] private AudioClip destroyingClip;

    [Header("Build Settings")]
    public GameObject CurrentBuilding;
    public SelectedBuildType CurrentBuildType;
    [SerializeField] private float maxBuildingDistance = 2f;
    [SerializeField] private LayerMask connectorLayer;

    [Header("Destroy Settings")]
    [SerializeField] private KeyCode deleteBuildingsKey;
    [SerializeField] private bool isDestroying = false;
    private Transform lastHitDestroyTransform;
    private List<Material> lastHitMaterials = new();

    [Header("Ghost Settings")]
    [SerializeField] private Material ghostMaterialValid;
    [SerializeField] private Material ghostMaterialInvalid;
    [SerializeField] private float connectorOverlapRadius = 1;
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Internal State")]
    public bool IsBuilding = false;
    private GameObject ghostBuildGameobject;
    private bool isGhostInValidPosition = false;
    private Transform modelParent = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(deleteBuildingsKey))
        {
            isDestroying = !isDestroying;

            resetLastHitDestroyTransform();
        }

        if(IsBuilding && !isDestroying)
        {
            ghostBuild();

            if(Input.GetMouseButtonDown(0))
            {
                placeBuilding();
            }
        }
        else if(ghostBuildGameobject)
        {
            Destroy(ghostBuildGameobject);
            ghostBuildGameobject = null;
        }

        if(isDestroying)
        {
            ghostDestroy();

            if(Input.GetMouseButtonDown(0))
            {
                destroyBuilding();
            }
        }
    }

    private void destroyBuilding()
    {
        if(lastHitDestroyTransform)
        {
            foreach(Connector connector in lastHitDestroyTransform.GetComponentsInChildren<Connector>())
            {
                connector.gameObject.SetActive(false);
                connector.UpdateConnectors(true);
            }

            Destroy(lastHitDestroyTransform.gameObject);

            lastHitDestroyTransform = null;

            audioSource.PlayOneShot(destroyingClip);
        }
    }

    private void ghostDestroy()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxBuildingDistance))
        {
            if(!hit.transform.root.CompareTag("Buildables"))
            {
                resetLastHitDestroyTransform();
                return;
            }
            
            if(!lastHitDestroyTransform)
            {
                lastHitDestroyTransform = hit.transform.root;

                lastHitMaterials.Clear();
                foreach(MeshRenderer lastHitMeshRenderers in lastHitDestroyTransform.GetComponentsInChildren<MeshRenderer>())
                {
                    lastHitMaterials.Add(lastHitMeshRenderers.material);
                }

                ghostifyModel(lastHitDestroyTransform.GetChild(0), ghostMaterialInvalid);
            }
            else if(hit.transform.root != lastHitDestroyTransform)
            {
                resetLastHitDestroyTransform();
            }
        }
    }

    private void resetLastHitDestroyTransform()
    {
        int counter = 0;
        if(lastHitDestroyTransform == null) { return; }

        foreach(MeshRenderer lastHitMeshRenderers in lastHitDestroyTransform.GetComponentsInChildren<MeshRenderer>())
        {
            lastHitMeshRenderers.material = lastHitMaterials[counter];
            counter += 1;
        }

        lastHitDestroyTransform = null;
    }

    private void placeBuilding()
    {
        if(ghostBuildGameobject != null && isGhostInValidPosition)
        {
            GameObject newBuild = Instantiate(CurrentBuilding, ghostBuildGameobject.transform.position,
                ghostBuildGameobject.transform.rotation);

            foreach(Connector connector in newBuild.GetComponentsInChildren<Connector>())
            {
                connector.UpdateConnectors(true);
            }

            //If we're left with items after building, continue the build mode
            InventorySystem.Instance.RemoveItem(CurrentBuilding.name, 1);
            if(InventorySystem.Instance.ReturnMaterialQuantity(CurrentBuilding.name) <= 0)
            {
                IsBuilding = false;
                Destroy(ghostBuildGameobject);
                ghostBuildGameobject = null;
            }

            audioSource.PlayOneShot(buildingClip);
        }
    }

    private void ghostBuild()
    {
        createGhostPrefab(CurrentBuilding);

        moveGhostPrefabToRaycast();
        checkBuildValidity();
    }

    private void checkBuildValidity()
    {
        Collider[] colliders = Physics.OverlapSphere(ghostBuildGameobject.transform.position, 
            connectorOverlapRadius, connectorLayer);

        if(colliders.Length > 0)
        {
            if(CurrentBuildType != SelectedBuildType.Generic)
            {
                ghostConnectBuild(colliders);
            }
            else
            {
                foreach(Connector connector in ghostBuildGameobject.GetComponentsInChildren<Connector>())
                {
                    checkGenericBuildValidity(connector);
                }
            }
        }
        else
        {
            ghostSeparateBuild();

            if(!isGhostInValidPosition) { return; }

            if(CurrentBuildType == SelectedBuildType.Generic)
            {
                foreach(Connector connector in ghostBuildGameobject.GetComponentsInChildren<Connector>())
                {
                    checkGenericBuildValidity(connector);
                }
            }

            Collider[] overlapColliders = Physics.OverlapBox(ghostBuildGameobject.transform.position, 
                new Vector3(2f, 2f, 2f), ghostBuildGameobject.transform.rotation);
            foreach(Collider collider in overlapColliders)
            {
                if(collider.gameObject != ghostBuildGameobject && collider.transform.root.CompareTag("Buildables"))
                {
                    ghostifyModel(modelParent, ghostMaterialInvalid);
                    isGhostInValidPosition = false;
                    print("here4");
                    return;
                }
            }
        }
    }

    private void checkGenericBuildValidity(Connector connector)
    {
        Collider[] _colliders = Physics.OverlapSphere(connector.transform.position, .3f);

        //Means there are no buildables (floors) underneath so we can't place it
        if (_colliders.Length == 0 && connector.ConnectorPosition == ConnectorPosition.Bottom)
        {
            ghostifyModel(modelParent, ghostMaterialInvalid);
            print("here1");
            isGhostInValidPosition = false;
            return;
        }

        if (_colliders.Length >= 1 && connector.ConnectorPosition != ConnectorPosition.Bottom)
        {
            ghostifyModel(modelParent, ghostMaterialInvalid);
            isGhostInValidPosition = false;
            print("here2");
            return;
        }

        foreach(Collider coll in _colliders)
        {
            if(coll.name.Contains("Connector")) { continue; }

            if(!coll.CompareTag("Buildables"))
            {
                ghostifyModel(modelParent, ghostMaterialInvalid);
                isGhostInValidPosition = false;
            }
            else
            {
                ghostifyModel(modelParent, ghostMaterialValid);
                isGhostInValidPosition = true;
            }
        }
    }

    private void ghostSeparateBuild()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxBuildingDistance))
        {
            if(CurrentBuildType == SelectedBuildType.Wall)
            {
                ghostifyModel(modelParent, ghostMaterialInvalid);
                isGhostInValidPosition = false;
                return;
            }

            if(Vector3.Angle(hit.normal, Vector3.up) < maxGroundAngle)
            {
                ghostifyModel(modelParent, ghostMaterialValid);
                isGhostInValidPosition = true;
            }
            else
            {
                ghostifyModel(modelParent, ghostMaterialInvalid);
                            print("here5");
                isGhostInValidPosition = false;
            }
        }
    }

    private void ghostifyModel(Transform modelParent, Material ghostMaterial = null)
    {
        if(ghostMaterial != null)
        {
            foreach(MeshRenderer meshRenderer in modelParent.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = ghostMaterial;
            }
        }
        else
        {
            foreach(Collider modelColliders in modelParent.GetComponentsInChildren<Collider>())
            {
                modelColliders.enabled = false;
            }
        }
    }

    private void ghostConnectBuild(Collider[] colliders)
    {
        Connector bestConnector = null;

        foreach(Collider collider in colliders)
        {
            Connector connector = collider.GetComponent<Connector>();

            if(connector.CanConnectTo)
            {
                bestConnector = connector;
                break;
            }
        }

        if(bestConnector == null || CurrentBuildType == SelectedBuildType.Floor && bestConnector.IsConnectedToFloor
            || CurrentBuildType == SelectedBuildType.Wall && bestConnector.IsConnectedToWall)
        {
            ghostifyModel(modelParent, ghostMaterialInvalid);
            isGhostInValidPosition = false;
            return;
        }

        snapGhostPrefabToConnector(bestConnector);
    }

    private void snapGhostPrefabToConnector(Connector connector)
    {
        Transform ghostConnector = findSnapConnector(connector.transform, ghostBuildGameobject.transform.GetChild(1));
        ghostBuildGameobject.transform.position = connector.transform.position - (ghostConnector.position - ghostBuildGameobject.transform.position);
        if(CurrentBuildType == SelectedBuildType.Wall)
        {
            Quaternion newRotation = ghostBuildGameobject.transform.rotation;
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, connector.transform.rotation.eulerAngles.y, newRotation.eulerAngles.z);
            ghostBuildGameobject.transform.rotation = newRotation;
        }

        ghostifyModel(modelParent, ghostMaterialValid);
        isGhostInValidPosition = true;
    }

    private Transform findSnapConnector(Transform snapConnector, Transform ghostConnectorParent)
    {
        ConnectorPosition OppositeConnectorTag = getOppositePosition(snapConnector.GetComponent<Connector>());

        foreach(Connector connector in ghostConnectorParent.GetComponentsInChildren<Connector>())
        {
            if(connector.ConnectorPosition == OppositeConnectorTag)
            {
                return connector.transform;
            }
        }

        return null;
    }

    private ConnectorPosition getOppositePosition(Connector connector)
    {
        ConnectorPosition position = connector.ConnectorPosition;

        if (CurrentBuildType == SelectedBuildType.Wall && connector.ConnectorParentType == SelectedBuildType.Floor)
        {
            return ConnectorPosition.Bottom;
        }

        if(CurrentBuildType == SelectedBuildType.Floor && connector.ConnectorParentType == SelectedBuildType.Wall
            && connector.ConnectorPosition == ConnectorPosition.Top)
        {
            if(connector.transform.root.rotation.y == 0)
            {
                return getConnectorClosestToPlayer(true);
            }
            else
            {
                return getConnectorClosestToPlayer(false);
            }
        }

        switch(position)
        {
            case ConnectorPosition.Left:
                return ConnectorPosition.Right;
            case ConnectorPosition.Right:
                return ConnectorPosition.Left;
            case ConnectorPosition.Top:
                return ConnectorPosition.Bottom;
            case ConnectorPosition.Bottom:
                return ConnectorPosition.Top;
            default:
                return ConnectorPosition.Bottom;
        }
    }

    private ConnectorPosition getConnectorClosestToPlayer(bool topBottom)
    {
        Transform cameraTransform = mainCam.transform;

        if(topBottom)
        {
            return cameraTransform.position.z >= ghostBuildGameobject.transform.position.z ? ConnectorPosition.Bottom : ConnectorPosition.Top;
        }
        else
        {
            return cameraTransform.position.x >= ghostBuildGameobject.transform.position.x ? ConnectorPosition.Left : ConnectorPosition.Right;
        }
    }

    private void moveGhostPrefabToRaycast()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, maxBuildingDistance))
        {
            ghostBuildGameobject.transform.position = hit.point;
        }
    }

    private void createGhostPrefab(GameObject currentBuild)
    {
        if(ghostBuildGameobject != null)
        {
            Destroy(ghostBuildGameobject);
            ghostBuildGameobject = null;
        }

        if(ghostBuildGameobject == null)
        {
            ghostBuildGameobject = Instantiate(currentBuild);

            modelParent = ghostBuildGameobject.transform.GetChild(0);

            ghostifyModel(modelParent, ghostMaterialValid);
            ghostifyModel(ghostBuildGameobject.transform);
        }
    }
}
[System.Serializable]
public enum SelectedBuildType
{
    Floor,
    Wall,
    Generic
}
