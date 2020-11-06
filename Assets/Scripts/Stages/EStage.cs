using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enum for identifying each stage of the game.
/// </summary>
public enum EStage
{
    None,
    SkippedTutorial,
    Controls,
    ResourceBuildings,
    Terraforming,
    Combat,
    MainGame,
    Win,
    Lose
}
