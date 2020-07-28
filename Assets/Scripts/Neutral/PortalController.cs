using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject portalExit;

    [SerializeField]
    private float teleportAfter;
    [SerializeField]
    private LayerMask acceptEntitiesFromThisLayer;
    [SerializeField]
    private GameObject teleportParticleFeedback;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coll");
        if(acceptEntitiesFromThisLayer == (acceptEntitiesFromThisLayer | (1 << other.gameObject.layer))) //if the layer of the object is withing the specified layerMask
        {
            StartCoroutine(Teleport(other.gameObject.transform));
        }
    }

    private IEnumerator Teleport(Transform objectToTeleport)
    {     
        yield return new WaitForSeconds(teleportAfter);
        objectToTeleport.gameObject.transform.position = portalExit.transform.position;
        Instantiate(teleportParticleFeedback, objectToTeleport.position, teleportParticleFeedback.transform.rotation);
    }
}
