using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Demo class for enemies.
/// </summary>
public class Alien : MonoBehaviour, IMessenger
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Components")]
    [SerializeField] private List<Collider> bodyColliders;
	[SerializeField] private AlienClaw alienWeapon;

	[Header("Stats")] 
    [SerializeField] private int id;
    [SerializeField] private EAlien type;
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------

    [Header("Testing")]
    
    //Componenets
	private Actor actor;
    private List<Collider> colliders;
    private Health health;
    private NavMeshAgent navMeshAgent;
    private SkinnedMeshRenderer renderer;
    private Rigidbody rigidbody;

	//Movement
	private bool moving;
    private float speed;
    
    //Targeting
    private List<Transform> visibleAliens;
    private List<Transform> visibleTargets;
    [SerializeField] private Transform target;
    [SerializeField] private Health targetHealth;
    [SerializeField] private Size targetSize;
    [SerializeField] private string shotByName;
    [SerializeField] private Transform shotByTransform;
    private float timeOfLastAttack;

    //Public Fields----------------------------------------------------------------------------------------------------------------------------------

    public UnityAction onAttack;
    public UnityAction onDamaged;
    public UnityAction onDie;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------
    
    /// <summary>
    /// The colliders that comprise the alien's body.
    /// </summary>
    public List<Collider> BodyColliders { get => bodyColliders; }

    /// <summary>
    /// Alien's collider components.
    /// </summary>
    public List<Collider> Colliders { get => colliders; }

    /// <summary>
    /// Alien's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// Alien's NavMeshAgent component.
    /// </summary>
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }

    /// <summary>
    /// Alien's MeshRenderer component.
    /// </summary>
    public SkinnedMeshRenderer Renderer { get => renderer; }

    /// <summary>
    /// Alien's EAlien type.
    /// </summary>
    public EAlien Type { get => type; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
		actor = GetComponent<Actor>();
        colliders = new List<Collider>(GetComponents<Collider>());
        health = GetComponent<Health>();
		navMeshAgent = GetComponent<NavMeshAgent>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();

        visibleAliens = new List<Transform>();
        visibleTargets = new List<Transform>();
        moving = false;
        navMeshAgent.enabled = false;
        speed = navMeshAgent.speed;


		alienWeapon = GetComponentInChildren<AlienClaw>();
		alienWeapon.gameObject.SetActive(false);

		health.onDamaged += OnDamaged;
		health.onDie += OnDie;
	}

    /// <summary>
    /// Prepares the Alien to chase its targets when AlienFactory puts it in the world. 
    /// </summary>
    public void Setup(int id)
    {
        this.id = id;
        gameObject.name = $"Alien {id}";
        health.Reset();

        target = CryoEgg.Instance.ColliderTransform;
        targetHealth = CryoEgg.Instance.GetComponent<Health>();
        timeOfLastAttack = attackCooldown * -1;
        MessageDispatcher.Instance.Subscribe("Alien", this);
        renderer.enabled = true;

        //Rotate to face the Cryo egg
        Vector3 targetRotation = CryoEgg.Instance.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetRotation);

        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }

        if (StageManager.Instance.CurrentStage.GetID() == EStage.MainGame)
        {
            navMeshAgent.enabled = true;
            moving = true;
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        if (moving)
        {
            SelectTarget();
            Look();
            Move();            
        }
    }

    //Recurring Methods (FixedUpdate())-------------------------------------------------------------------------------------------------------------  
	
    /// <summary>
    /// Selects the most appropriate target for the alien.
    /// </summary>
    private void SelectTarget()
    {
        switch (visibleTargets.Count)
        {
            case 0:
                //Target Cryo egg
                if (target != CryoEgg.Instance.transform)
                {
                    SetTarget(CryoEgg.Instance.transform);
                }

                break;
            case 1:
                //Get only visible target
                if (target != visibleTargets[0])
                {
                    SetTarget(visibleTargets[0]);
                }

                break;
            default:
                //Prioritise shooter
                if (shotByTransform != null && visibleTargets.Contains(shotByTransform))
                {
                    SetTarget(shotByTransform);
                }
                else
                {
                    //Get closest visible target
                    float distance = 99999999999;
                    float closestDistance = 9999999999999999;
                    Transform closestTarget = null;

                    foreach (Transform t in visibleTargets)
                    {
                        distance = Vector3.Distance(transform.position, t.position);

                        if (closestTarget == null || distance < closestDistance)
                        {
                            closestTarget = t;
                            closestDistance = distance;
                        }
                    }

                    if (target != closestTarget)
                    {
                        SetTarget(closestTarget);
                    }
                }

                break;
        }
    }

    /// <summary>
    /// Sets Alien's target transform, targetHealth and targetSize variables based on the selected target and its components.
    /// </summary>
    /// <param name="selectedTarget">The transform of the selected target.</param>
    private void SetTarget(Transform selectedTarget)
    {
        target = selectedTarget;
        targetHealth = target.GetComponentInParent<Health>();   //Gets Health from target or any of its parents that has it.
        targetSize = target.GetComponentInParent<Size>();   //Gets Radius from target or any of its parents that has it.
    }

    /// <summary>
    /// Alien uses input information to determine which direction it should be facing
    /// </summary>
    private void Look()
    {
        if (navMeshAgent.enabled && target.position != navMeshAgent.destination)
        {
            navMeshAgent.destination = target.position;
        }
    }

    /// <summary>
    /// Gets the position of a target as if it were at the same height as the alien. 
    /// </summary>
    /// <param name="targetPos">The target's position.</param>
    /// <returns>The target's position if it was at the same height as the alien.</returns>
    private Vector3 PositionAtSameHeight(Vector3 targetPos)
    {
        return new Vector3(targetPos.x, transform.position.y, targetPos.z);
    }

    /// <summary>
    /// Moves alien.
    /// </summary>
    private void Move()
    {
        if (Vector3.Distance(transform.position, PositionAtSameHeight(target.position)) > attackRange + targetSize.Radius)
        {
            AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Moves, this.gameObject);

            if (navMeshAgent.speed != speed)
            {
                navMeshAgent.speed = speed;
            }
        }
        else
        {
            if (navMeshAgent.speed != 0)
            {
                navMeshAgent.speed = 0;
            }

            if (Time.time - timeOfLastAttack > attackCooldown)
            {
                timeOfLastAttack = Time.time;
				Attack();
            }
        }
    }

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Send an event message for AlienFX.cs to do attack FX's and deal damage.
	/// If there's no FX script listening to this to call DealDamage(), call it anyway.
	/// </summary>
	private void Attack()
	{
		if (onAttack != null)
		{
			onAttack.Invoke();
            DamagePointer.Jump_Static(transform);
        }
		else
		{
			Debug.Log("No script for Alien FX attached, doing damage without visuals . . .");
			UnsheathClaw();
            DamagePointer.Jump_Static(transform);
        }
	}

	/// <summary>
	/// Send an event message for AlienFX.cs to do damage taken FX's and assign attacker target.
	/// If there's no FX script listening to this, assign attacker target anyway.
	/// </summary>
	private void OnDamaged(float amount, Transform attackerTransform)
	{
		if (onDamaged != null)
		{
			onDamaged.Invoke();
		}
		else
		{
			Debug.Log("No script for Alien FX attached, taking damage without visuals . . .");
		}

		ShotBy(attackerTransform);
	}

	/// <summary>
	/// Send an event message for AlienFX.cs to do death FX's and destroy the alien.
	/// If there's no FX script listening to this to call DestroyAlien(), call it anyway.
	/// </summary>
	public void OnDie()
	{
		if (onDie != null)
		{
			foreach (Collider c in colliders)
			{
				c.enabled = false;
			}

            navMeshAgent.enabled = false;

			onDie.Invoke();
		}
		else
		{
			Debug.Log("No script for Alien FX attached, destroying alien without visuals . . .");
			DestroyAlien();
		}
	}

	/// <summary>
	/// Registers with an alien the name and transform of an entity that shot it.
	/// </summary>
	/// <param name="name">The name of the entity that shot the alien.</param>
	/// <param name="transform">The transform of the entity that shot the alien.</param>
	public void ShotBy(Transform attackerTransform)
	{
		shotByName = attackerTransform.name;
		shotByTransform = attackerTransform;
	}

	/// <summary>
	/// Enables the melee weapon to deal damage.
	/// UnsheathClaw() is intended to be called if there is no AlienFX.cs to trigger attack animation.
	/// </summary>
	public void UnsheathClaw()
	{
		alienWeapon.gameObject.SetActive(true);
	}

	/// <summary>
	/// Disables the melee weapon.
	/// SheathClaw() is intended to be called if there is no AlienFX.cs to trigger attack animation.
	/// </summary>
	public void SheathClaw()
	{
		alienWeapon.gameObject.SetActive(false);
	}

	/// <summary>
	/// Destroy the alien.
	/// DestroyAlien() is intended to be called via an animation clip in AlienFX.cs
	/// </summary>
	public void DestroyAlien()
	{
		AlienFactory.Instance.Destroy(this, type);
	}

	/// <summary>
	/// Allows message-sending classes to deliver a message to this alien.
	/// </summary>
	/// <param name="message">The message to send to this messenger.</param>
	public void Receive(Message message)
    {
        if (message.SenderTag == "Turret" && message.MessageContents == "Dead")
        {
            Transform messenger = message.SenderObject.transform;

            if (shotByTransform == messenger)
            {
                shotByName = "";
                shotByTransform = null;
            }

            if (visibleTargets.Contains(messenger))
            {
                visibleTargets.Remove(messenger);
            }
        }
    }

    /// <summary>
    /// Resets the alien to its inactive state.
    /// </summary>
    public void Reset()
    {
        //navMeshAgent.enabled = false;
        renderer.enabled = false;
        MessageDispatcher.Instance.SendMessage("Turret", new Message(gameObject.name, "Alien", this.gameObject, "Dead"));
        MessageDispatcher.Instance.Unsubscribe("Alien", this);
        moving = false;
        shotByName = "";
        shotByTransform = null;
        visibleTargets.Clear();
        visibleAliens.Clear();
        target = null;

		foreach (Collider c in colliders)
		{
			c.enabled = false;
		}
	}

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (other.CompareTag("Alien"))
            {
                visibleAliens.Add(other.transform);
            }
            else if (other.CompareTag("Building") && !visibleTargets.Contains(other.transform.parent))
            {
                visibleTargets.Add(other.transform.parent);
            }
            else if (other.CompareTag("Player") && !visibleTargets.Contains(other.transform))
            {
                visibleTargets.Add(other.transform);
            }
            else if (other.CompareTag("Projectile"))
            {
                Projectile projectile = other.GetComponent<Projectile>();
                shotByTransform = projectile.Owner.GetComponentInChildren<Collider>().transform;
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            if (visibleAliens.Contains(other.transform))
            {
                visibleAliens.Remove(other.transform);
            }
            else if (visibleTargets.Contains(other.transform))
            {
                visibleTargets.Remove(other.transform);
            }
        }
    }
}
