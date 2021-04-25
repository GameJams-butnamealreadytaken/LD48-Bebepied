using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLogic : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<LevelLogic>().TriggerEndChainCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<LevelLogic>().TriggerEndChainCollision();
    }
}
