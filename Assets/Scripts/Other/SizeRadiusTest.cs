using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeRadiusTest : MonoBehaviour
{
    [SerializeField] private Building building;
    [SerializeField] private float distance;
    [SerializeField] private float angle;
    [SerializeField] private float deltaX;
    [SerializeField] private float deltaZ;
    [SerializeField] private float retrievedRadius;

    // Update is called once per frame
    void Update()
    {
        if (building != null)
        {
            distance = Vector2.Distance(new Vector2(building.transform.position.x, building.transform.position.z), new Vector2(transform.position.x, transform.position.z));
            angle = MathUtility.Instance.Angle(building.transform.position, transform.position);
            deltaX = MathUtility.Instance.FloatMagnitude(building.transform.position.x - transform.position.x);
            deltaZ = MathUtility.Instance.FloatMagnitude(building.transform.position.z - transform.position.z);
            retrievedRadius = building.Size.Radius(transform.position);
        }
        else
        {
            distance = 0;
            angle = 0;
            deltaX = 0;
            deltaZ = 0;
            retrievedRadius = 0;
        }
    }
}
