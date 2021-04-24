using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ParticlesDecalsCreator : MonoBehaviour
{
    [Header("General")]
    public float TimeBeforeDecalApplication;
    
    [Header("Decal")] 
    public List<GameObject> DecalToCreateList = new List<GameObject>();


    private void Start()
    {
        Invoke(nameof(CreateDecal), TimeBeforeDecalApplication);
    }

    private void CreateDecal()
    {
        if(NavMesh.SamplePosition(transform.position, out var navHit, 100 , -1))
        {
            GameObject decalToCreate = DecalToCreateList[Random.Range(0, DecalToCreateList.Count)];
            
            Quaternion decalRotation = decalToCreate.transform.rotation;
            decalRotation.eulerAngles = new Vector3(decalRotation.eulerAngles.x, transform.rotation.eulerAngles.y, decalRotation.eulerAngles.z);

            Instantiate(decalToCreate, navHit.position, decalRotation, null);
        }
    }
}
