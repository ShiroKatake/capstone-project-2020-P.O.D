﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class for enemies.
/// </summary>
public class Enemy : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Enemy Stats")] 
    [SerializeField] private int id;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    //Non-Serialized Fields------------------------------------------------------------------------

    
    private Health health;
    private Transform target;
    private Vector3 movement;
    private float radius;
    private float targetRadius;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Enemy's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// The Enemy's unique ID number. Should only be set in EnemyController.
    /// </summary>
    public int Id { get => id; set => id = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        health = GetComponent<Health>();
        SelectTarget();
        GetRadius();
        CalculateMovement();
    }

    /// <summary>
    /// Chooses a target for Enemy.
    /// </summary>
    private void SelectTarget()
    {
        if (Planet.Instance.Terraformers.Count == 0)
        {
            target = Planet.Instance.CryoEgg.transform;
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

    /// <summary>
    /// Gets Enemy's radius.
    /// </summary>
    private void GetRadius()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        radius = sphereCollider.radius /** transform.localScale.magnitude*/;
    }

    /// <summary>
    /// Calculates the vector for Enemy's movement, multiplies it by its speed.
    /// </summary>
    private void CalculateMovement()
    {
        movement = target.position - transform.position;
        movement.Normalize();
        movement *= speed;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        CheckHealth();
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        Move();
        CheckTarget();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if Enemy has 0 health, destroying it if it has.
    /// </summary>
    private void CheckHealth()
    {
        if (health.IsDead())
        {
            EnemyController.Instance.Enemies.Remove(this);
            health.Die();
        }
    }

    //Recurring Methods (FixedUpdate())-------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Moves Enemy.
    /// </summary>
    private void Move()
    {
        transform.Translate(movement * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Checks if the target has died and reassigns it, and deals damage to the target if touching it.
    /// </summary>
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