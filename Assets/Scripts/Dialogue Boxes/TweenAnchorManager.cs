using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An encapsulation of an anchor, the button childed to it, and the local position the anchor wants to be at if it's sliding.
/// </summary>
public class AnchorSet
{
    public RectTransform anchor;
    public RectTransform button;
    public Vector3 targetLocalPosition;
}

/// <summary>
/// A manager for organising the building button tween start anchors that are grouped together.
/// </summary>
public class TweenAnchorManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Tooltip("The rect transforms that building buttons should child themselves to. If they're sliding anchors, make sure to order them in the list the same as their order in the scene.")]
    [SerializeField] private List<RectTransform> anchors;
    [Tooltip("When an anchor is vacated, should the others slide in to fill it's spot")]
    [SerializeField] private bool slideAnchors;
    [SerializeField] private float slideSpeed;

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<AnchorSet> anchorSets;
    private int index;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        index = 0;
        anchorSets = new List<AnchorSet>();
        
        for (int i = 0; i < anchors.Count; i++)
        {
            anchorSets.Add(new AnchorSet());
            anchorSets[i].anchor = anchors[i];
            anchorSets[i].targetLocalPosition = anchors[i].localPosition;
        }
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        foreach (AnchorSet a in anchorSets)
        {
            if (a.anchor.localPosition != a.targetLocalPosition)
            {
                a.anchor.localPosition = Vector3.MoveTowards(a.anchor.localPosition, a.targetLocalPosition, slideSpeed * Time.deltaTime);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Register a building button and have it paired with an anchor.
    /// </summary>
    /// <param name="button">The RectTransform component of the button being registered.</param>
    public void RegisterButton(RectTransform button)
    {
        if (index < anchorSets.Count)
        {
            anchorSets[index].button = button;
            index++;
        }
        else
        {
            Debug.Log($"{this}.RegisterButton() cannot execute because it does not have any unassigned anchors remaining to assign button {button} to.");
        }
    }

    /// <summary>
    /// De-register a building button paired with an anchor.
    /// </summary>
    /// <param name="button">The RectTransform component of the button being de-registered.</param>
    public void DeRegisterButton(RectTransform button)
    {
        for (int i = 0; i < anchorSets.Count; i++)
        {
            if (anchorSets[i].button = button)
            {
                anchorSets[i].button = null;
                index--;

                if (slideAnchors)
                {
                    //Slide anchors' target positions to the next in line
                    Vector2 lastPos = anchorSets[anchorSets.Count - 1].targetLocalPosition;

                    for (int j = anchorSets.Count - 1; j > i; j--)
                    {                        
                        anchorSets[j].targetLocalPosition = anchorSets[j - 1].targetLocalPosition;
                    }

                    //Put deregistered anchor set at the back of the list
                    AnchorSet a = anchorSets[i];
                    anchorSets.RemoveAt(i);
                    a.targetLocalPosition = lastPos;
                    a.anchor.localPosition = a.targetLocalPosition;
                    anchorSets.Add(a);

                    //Check if any anchors have buttons
                    foreach (AnchorSet set in anchorSets)
                    {
                        if (set.button != null)
                        {
                            //If so, return because they need to visibly slide.
                            return;
                        }
                    }

                    //Else snap anchors to their positions
                    foreach (AnchorSet set in anchorSets)
                    {
                        set.anchor.localPosition = set.targetLocalPosition;
                    }

                    break;
                }
            }
        }
    }
}
