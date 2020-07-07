using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Stage currentStage;
    private Locatable target;
    private MeshRenderer renderer;
    private DamageIndicator arrowToTargetPrefab;
    private DamageIndicator arrowToTarget;
    private float decalMinLerp;
    private float decalMaxLerp;
    private float largeLerpMultiplier;
    private float lerpMultiplier;
    private float lerpProgress;
    private bool lerpForward;
    private List<Locatable> upcomingTargets;

    public Target Instance { get; protected set; }

    public Stage CurrentStage { get => currentStage; set => currentStage = value; }
    public List<Locatable> UpcomingTargets { get => upcomingTargets; }

    void Awake()
    {

    }

    private void Configure()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Activate the building target at the locatable's location
    private void ActivateTarget(Locatable l)
    {
        if (buildMenuCanvasGroup.alpha == 0)
        {
            lerpTargetLock = true;
            GetLocationOf(l);

            if (targetTile.Building != null && stage != TutorialStage.BuildHarvesters && stage != TutorialStage.BuildExtenderInFog)
            {
                l = GetBackupTarget(l);
            }

            buildingTarget.Location = targetTile;
            buildingTarget.transform.position = l.transform.position;
            targetRenderer.enabled = true;

            tileTargetLerpProgress = 0f;
            tileTargetLerpForward = true;

            arrowToTarget.On = true;

            ActivateMouse();
        }
    }

    //Get location of a locatable object
    private void GetLocationOf(Locatable l)
    {
        if (l != null)
        {
            targetTile = l.Location;
        }
        else
        {
            Debug.Log("Locatable l in TutorialController.GetLocationOf(Locatable l) is null");
        }

        if (targetTile == null)
        {
            Debug.Log("TutorialController.CurrentTile is null");
        }
    }

    //Compensates for if the player speedruns and builds on the tile the target is trying to be activated on
    Locatable GetBackupTarget(Locatable l)
    {
        List<TileData> alternatives = new List<TileData>();
        List<TileData> invalidTiles = new List<TileData>();

        foreach (Locatable t in lerpTargetsRemaining)
        {
            invalidTiles.Add(t.Location);
        }

        foreach (TileData t in targetTile.AllAdjacentTiles)
        {
            if (!invalidTiles.Contains(t) && t.Building == null && t.Resource == null && t.PowerSource != null && !t.FogUnitActive && !t.buildingChecks.obstacle)
            {
                alternatives.Add(t);
            }
        }

        if (alternatives.Count == 0)
        {
            foreach (TileData t in WorldController.Instance.ActiveTiles)
            {
                if (!invalidTiles.Contains(t) && t.Building == null && t.Resource == null && t.PowerSource != null && !t.FogUnitActive && !t.buildingChecks.obstacle)
                {
                    if (alternatives.Count == 0)
                    {
                        alternatives.Add(t);
                    }
                    else
                    {
                        float dist = Vector3.Distance(targetTile.Position, t.Position);
                        float bestDist = Vector3.Distance(targetTile.Position, alternatives[0].Position);

                        if (dist < bestDist)
                        {
                            alternatives.Clear();
                            alternatives.Add(t);
                        }
                        else if (dist == bestDist)
                        {
                            alternatives.Add(t);
                        }
                    }
                }
            }
        }

        if (alternatives.Count > 0)
        {
            targetTile = alternatives[UnityEngine.Random.Range(0, alternatives.Count)];
            l.Location = targetTile;
            l.transform.position = targetTile.Position;
        }

        return l;
    }

    //Lerp the target decal
    private void LerpDecal()
    {
        float lerped;

        if (stage != TutorialStage.ActivateSonar)
        {
            lerped = Mathf.Lerp(decalMinLerp, decalMaxLerp, tileTargetLerpProgress);
        }
        else
        {
            lerped = Mathf.Lerp(decalMinLerp * largeLerpMultiplier, decalMaxLerp * largeLerpMultiplier, tileTargetLerpProgress);
        }

        buildingTarget.transform.localScale = new Vector3(lerped, 1, lerped);

        UpdateTileTargetLerpValues();
    }

    //Update lerp progress
    private void UpdateTileTargetLerpValues()
    {
        if (tileTargetLerpForward)
        {
            tileTargetLerpProgress += Time.deltaTime * lerpMultiplier;
        }
        else
        {
            tileTargetLerpProgress -= Time.deltaTime * lerpMultiplier;
        }

        if (tileTargetLerpProgress > 1)
        {
            tileTargetLerpProgress = 1;
            tileTargetLerpForward = false;
        }
        else if (tileTargetLerpProgress < 0)
        {
            tileTargetLerpProgress = 0;
            tileTargetLerpForward = true;
        }
    }

    //Deactivates the building target
    private void DeactivateTarget()
    {
        targetRenderer.enabled = false;
        arrowToTarget.On = false;
    }
}
