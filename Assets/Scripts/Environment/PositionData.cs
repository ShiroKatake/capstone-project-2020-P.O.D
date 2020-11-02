using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// A container class for all the data on a given map position.
/// </summary>
public class PositionData
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private int x;
    private int z;
    private float angle;
    private EResource resource;
    private bool hasBuilding;
    private bool hasMineral;
    private bool aliensBanned;
    private bool isInTutorialAlienSpawnArea;

    private Dictionary<EAlien, NavMeshPath> paths;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Simple Public Properties---------------------------------------------------------------------

    /// <summary>
    /// Is this position too close to the cryo egg or a cliff for alien spawning to be allowed, or outside the bounds of the nav mesh?
    /// </summary>
    public bool AliensBanned { get => aliensBanned; set => aliensBanned = value; }

    /// <summary>
    /// The angle from the centre of the map to this position.
    /// </summary>
    public float Angle { get => angle; }

    /// <summary>
    /// Is this position occupied by a building?
    /// </summary>
    public bool HasBuilding { set => hasBuilding = value; }

    /// <summary>
    /// Is this position occupied by a mineral?
    /// </summary>
    public bool HasMineral { set => hasMineral = value; }

    /// <summary>
    /// Is this position close enough to the cryo egg for alien spawning to be allowed during the tutorial?
    /// </summary>
    public bool IsInTutorialAlienSpawnArea { get => isInTutorialAlienSpawnArea; }

    /// <summary>
    /// The resource this tile has, if any.
    /// </summary>
    public EResource Resource { get => resource; }

    /// <summary>
    /// The nav mesh paths from this position to the cryo egg for each type of alien.
    /// </summary>
    public Dictionary<EAlien, NavMeshPath> Paths { get => paths; }

    /// <summary>
    /// This position's x coordinate.
    /// </summary>
    public int X { get => x; }

    /// <summary>
    /// This position's z coordinate.
    /// </summary>
    public int Z { get => z; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// Can a building be built at this position?
    /// </summary>
    public bool IsBuildable
    {
        get
        {
            return !hasBuilding && !hasMineral;
        }
    }

    /// <summary>
    /// Can an alien be spawned at this position during non-tutorial gameplay?
    /// </summary>
    public bool IsAlienSpawnableDuringCombatTutorial
    {
        get
        {
            return IsBuildable && isInTutorialAlienSpawnArea;
        }
    }

    /// <summary>
    /// Can an alien be spawned at this position during the combat tutorial?
    /// </summary>
    public bool IsAlienSpawnableDuringGameplay
    {
        get
        {
            return IsBuildable && !aliensBanned;
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// PositionData's constructor.
    /// </summary>
    /// <param name="x">This position's x coordinate.</param>
    /// <param name="z">This position's z coordinate.</param>
    /// <param name="angle">The angle of this position from the centre of the map.</param>
    /// <param name="resource">What resource, if any, does this tile have?</param>
    /// <param name="isInTutorialAlienSpawnArea">Is this position inside the tutorial combat stage-only alien spawnable area around the cryo egg?</param>
    /// <param name="isInAlienExclusionArea">Is this position inside the non-tutorial alien exclusion area around the cryo egg?</param>
    public PositionData(int x, int z, float angle, EResource resource, bool isInTutorialAlienSpawnArea, bool isInAlienExclusionArea)
    {
        this.x = x;
        this.z = z;
        this.angle = angle;
        this.resource = resource;
        this.isInTutorialAlienSpawnArea = isInTutorialAlienSpawnArea;
        this.aliensBanned = isInAlienExclusionArea;
        this.hasBuilding = false;
        this.hasMineral = false;
        paths = new Dictionary<EAlien, NavMeshPath>();
    }
}