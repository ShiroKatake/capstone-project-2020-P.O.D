using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A controller class for tracking which parts of the map have buildings, can be spawned to by aliens, etc.
/// </summary>
[ExecuteInEditMode]
public class PathCalculationsManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Map Stats")]
    [SerializeField] private int xMax;
    [SerializeField] private int zMax;
    [SerializeField] private Transform cryoEggColliderTransform;

    [Header("Pathfinders")]
    [SerializeField] private Alien[] pathfinders;

    [Header("Pathfinding Settings")]
    [SerializeField] private float pathfinderSpawnHeight;
    [SerializeField] private bool pauseLoop;
    [SerializeField] private float pathfindingFrameTimeLimit;
    [SerializeField] private bool calculatePaths;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private NavMeshPath[,] positions;
    private LayerMask groundLayerMask;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// The functionality of MapController's Start() method.
    /// </summary>
    public void Update()
    {
        if (calculatePaths)
        {
            calculatePaths = false;
            StartCoroutine(CalculatePaths());
        }
    }

    /// <summary>
    /// Calculates paths from every map position to the cryo egg.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CalculatePaths()
    {
        System.Diagnostics.Stopwatch totalStopwatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch loopStopwatch = new System.Diagnostics.Stopwatch();
        totalStopwatch.Restart();
        loopStopwatch.Restart();

        positions = new NavMeshPath[xMax + 1, zMax + 1];

        Debug.Log($"MapController.CalculatePaths(), starting");
        NavMeshPath calculatedPath = null;

        for (int i = 0; i < pathfinders.Length; i++)
        {
            GameObject pathfinderGO = Instantiate(pathfinders[i].gameObject);
            NavMeshAgent pathfinder = pathfinderGO.GetComponent<NavMeshAgent>();
            EAlien pathfinderType = pathfinderGO.GetComponent<Alien>().Type;
            DestroyImmediate(pathfinder.GetComponent<Actor>());
            DestroyImmediate(pathfinder.GetComponent<Alien>());
            DestroyImmediate(pathfinder.GetComponent<AlienFX>());
            DestroyImmediate(pathfinder.GetComponent<Health>());

            //Component[] children = pathfinder.transform.GetComponentsInChildren<Transform>();

            //for (int i = 0l i < children.Length; i+)

            for (int j = 0; j < xMax; j++)
            {
                for (int k = 0; k < zMax; k++)
                {
                    Vector3 pos = new Vector3(j, pathfinderSpawnHeight, k);
                    RaycastHit hit;
                    positions[j, k] = null;

                    if (Physics.Raycast(pos, Vector3.down, out hit, 20, groundLayerMask))
                    {
                        pathfinder.transform.position = new Vector3(pos.x, hit.point.y, pos.z);
                        NavMeshHit navHit;

                        if (NavMesh.SamplePosition(pathfinder.transform.position, out navHit, 1, NavMesh.AllAreas))
                        {
                            pathfinder.enabled = true;
                            calculatedPath = new NavMeshPath();

                            Debug.Log($"pathfinder: {pathfinder}, NavMeshAgent: {pathfinder}, cryoEggColliderTransform: {cryoEggColliderTransform}, calculatedPath: {calculatedPath}, x: {j}/{xMax}, z: {k}/{zMax}, type: {pathfinderType}");

                            if (pathfinder.CalculatePath(cryoEggColliderTransform.position, calculatedPath))
                            {
                                positions[j,k] = calculatedPath;
                                Debug.Log($"CalculatePath() returned true, path is {calculatedPath.ToString()}.");
                            }

                            pathfinder.enabled = false;
                        }
                    }

                    if (pauseLoop && loopStopwatch.ElapsedMilliseconds >= pathfindingFrameTimeLimit)
                    {
                        Debug.Log($"MapController.CalculatePaths(), yielding in loop, x: {j}/{xMax}, z: {k}/{zMax}, milliseconds elapsed: {loopStopwatch.ElapsedMilliseconds}/{pathfindingFrameTimeLimit}");
                        yield return null;
                        loopStopwatch.Restart();
                    }
                }
            }

            DestroyImmediate(pathfinder.gameObject);
            pathfinders[i] = null;

            Debug.Log($"MapController.CalculatePaths(), has finished, time elapsed is {totalStopwatch.ElapsedMilliseconds} ms, or {totalStopwatch.ElapsedMilliseconds / 1000} s.");

            yield return null;
        }
    }
}

    
