using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBonusType
{
    Damage,
    Speed,
    Armor,
    Sausage
}

public class BonusBehavior : MonoBehaviour
{
    public EBonusType Type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ObjectTags.Player))
        {
            // TODO apply effect based on Type
            //GameManager.GetInstance().Player.ApplyEffect(Type);
            Destroy(gameObject);
        }
    }
}
