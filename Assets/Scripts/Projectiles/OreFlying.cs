using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreFlying : MonoBehaviour
{
	float t;

	public Transform p0;
	public Transform p1;
	public Transform p2;
	public float speed;

    // Update is called once per frame
    void Update()
    {
		t += Time.deltaTime;
		transform.position = CurveTowards(p0.position, p1.position, p2.position, t, speed);
	}

	public Vector3 CurveTowards(Vector3 p0, Vector3 p1, Vector3 p2, float t, float speed)
	{
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t * speed), Vector3.Lerp(p1, p2, t * speed), t * speed);
	}
}
