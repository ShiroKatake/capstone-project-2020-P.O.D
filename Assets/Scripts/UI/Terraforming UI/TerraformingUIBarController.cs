using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for controlling a set of TerraformingUIBars.
/// </summary>
class TerraformingUIBarController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private TerraformingUIBar[] bars;
    [SerializeField] private BuildingStats[] buildingStats;
    [SerializeField] private TerraformingUI_TodaysRatio[] dailyRatios;

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Updates the target ratio bars for all TerraformingUIBars in this controller's set.
    /// </summary>
    /// <param name="ratioArray">The current target ratio of terraforming buildings.</param>
    public void UpdateTargetRatio(int[] ratioArray)
    {
        foreach (TerraformingUIBar bar in bars)
        {
            bar.UpdateTargetRatio(ratioArray);
        }

        foreach (BuildingStats stats in buildingStats)
        {
            stats.DisplayBuildingInfo();
        }

        foreach (TerraformingUI_TodaysRatio dailyRatio in dailyRatios)
        {
            dailyRatio.ChangeTexts(ratioArray);
        }
    }

    /// <summary>
    /// Updates the current ratio bars for all TerraformingUIBars in this controller's set.
    /// </summary>
    /// <param name="ratioArray">The current ratio of terraforming buildings.</param>
    public void UpdateCurrentRatio(int[] ratioArray)
    {
        foreach (TerraformingUIBar bar in bars)
        {
            bar.UpdateCurrentRatio(ratioArray);
        }

        foreach (BuildingStats stats in buildingStats)
        {
            stats.DisplayBuildingInfo();
        }
    }
}
