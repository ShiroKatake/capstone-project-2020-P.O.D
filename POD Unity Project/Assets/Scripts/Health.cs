using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private float health;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Property

    public float Value { get => health; set => health = value; }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    public bool IsDead()
    {
        return health <= 0;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
