using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [Header("Enemy Stats")] 
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    //Non-Serialized Fields

    private Health health;
    private Transform target;
    private Vector3 movement;
    private float radius;
    private float targetRadius;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        health = GetComponent<Health>();
        SelectTarget();
        GetRadius();
        CalculateMovement();
    }

    private void SelectTarget()
    {
        if (Planet.Instance.Terraformers.Count == 0)
        {
            target = HumanPod.Instance.transform;
            targetRadius = target.GetComponent<SphereCollider>().radius;
        }
        else if (Planet.Instance.Terraformers.Count == 1)
        {
            target = Planet.Instance.Terraformers[0].transform;
            targetRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        else
        {
            float distance = 0;
            float closestDistance = 99999999;
            Transform closestTarget = null;

            foreach (Terraformer t in Planet.Instance.Terraformers)
            {
                distance = Vector3.Distance(transform.position, t.transform.position);

                if (closestTarget == null)
                {
                    closestTarget = t.transform;
                    closestDistance = distance;
                }
                else
                {
                    if (distance < closestDistance)
                    {
                        closestTarget = t.transform;
                        closestDistance = distance;
                    }
                }
            }

            target = closestTarget;
            targetRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    private void GetRadius()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius /** transform.localScale.magnitude*/;
    }

    private void CalculateMovement()
    {
        movement = target.position - transform.position;
        movement.Normalize();
        movement *= speed;
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        CheckHealth();
        Move();
        CheckTarget();
    }

    private void CheckHealth()
    {
        if (health.IsDead())
        {
            EnemyController.Instance.Enemies.Remove(this);
            health.Die();
        }
    }

    private void Move()
    {
        transform.Translate(movement * Time.deltaTime);
    }

    private void CheckTarget()
    {
        if (target == null)
        {
            SelectTarget();
            CalculateMovement();
        }

        if (Vector3.Distance(transform.position, target.position) < radius + targetRadius)
        {
            target.GetComponent<Health>().Value -= damage;
            EnemyController.Instance.Enemies.Remove(this);
            health.Die();
        }
    }
}
