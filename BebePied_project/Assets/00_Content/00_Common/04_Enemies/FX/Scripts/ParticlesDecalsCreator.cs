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
        GameObject decalToCreate = DecalToCreateList[Random.Range(0, DecalToCreateList.Count)];

        Vector3 decalPosition = transform.position;
        decalPosition.y = 0.55f;
        
        Quaternion decalRotation = decalToCreate.transform.rotation;
        decalRotation.eulerAngles = new Vector3(decalRotation.eulerAngles.x, transform.rotation.eulerAngles.y, decalRotation.eulerAngles.z);

        Instantiate(decalToCreate, decalPosition, decalRotation, null);
    }
}
