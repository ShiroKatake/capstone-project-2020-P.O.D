using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionHUD : MonoBehaviour
{
//    [Header("Camera")]
//    [SerializeField] private Camera camera;

    [Header("Buttons")]
    [SerializeField] private ButtonClickEventManager[] buttons;

    private ButtonClickEventManager button;

    // Variables for spawning buildings minus the vector movement ones
    private EBuilding selectedBuildingType;
    private Building heldBuilding;
    private Vector3 rawBuildingOffset;
    private Vector3 rawBuildingMovement;
    private bool cycleBuildingSelection = false;
    private bool cyclingBuildingSelection = false;
    private bool spawnBuilding = false;
    private bool placeBuilding = false;
    private bool cancelBuilding = false;

    void Awake(){

        selectedBuildingType = EBuilding.FusionReactor;
    }

    // Update is called once per frame
    //use array for buttons to make their calling simpler
    void Update()
    {
        foreach (ButtonClickEventManager btn in buttons){
            btn.AssociatedKeyPressed();
        }
        //GetInput();
        //CheckBuildingSpawn();
    }

//use array to quickly do every buttons cleanup function
    private void LateUpdate()
    {
        foreach (ButtonClickEventManager btn in buttons){
            btn.AfterUpdateCleanup();
        }
    }

/*
    private void GetInput(){

        rawBuildingMovement = new Vector3(InputController.Instance.GetAxis("LookLeftRight"), 0, InputController.Instance.GetAxis("LookUpDown"));

        //building selection input
        //button.IsClicked is causing errors, need to investigate
        if (!cyclingBuildingSelection && InputController.Instance.ButtonPressed("CycleBuilding")){
            cyclingBuildingSelection = true;
            selectedBuildingType = InputController.Instance.SelectBuilding(selectedBuildingType);
            //print("Alpha Key Pressed: " + selectedBuildingType.ToString());
        } else if (!cyclingBuildingSelection && CheckForUIButtonPress()){
            cyclingBuildingSelection = true;
            selectedBuildingType = button.GetBuildingType;
            //print("Button Pressed: " + selectedBuildingType.ToString());
        } else if (cyclingBuildingSelection && (!InputController.Instance.ButtonPressed("CycleBuilding") || !button.IsClicked)){
            cyclingBuildingSelection = false;
        }

        //building placement input
        //is this redundant?? i need explaination
        //need to put something in here... not sure what
        if (!spawnBuilding){
            if (button.IsClicked){
                spawnBuilding = button.IsClicked;
            } else {
                spawnBuilding = InputController.Instance.ButtonPressed("SpawnBuilding");
            }
            //print("Check Spawn: " + spawnBuilding);
        } else {
            placeBuilding = InputController.Instance.ButtonPressed("PlaceBuilding");
            cancelBuilding = InputController.Instance.ButtonPressed("CancelBuilding");
        }

    }

    private bool CheckForUIButtonPress(){
        foreach (ButtonClickEventManager btn in buttons){
            if (btn.IsClicked){
                button = btn;
                return true;
            }
        }
        return false;
    }

    private void CheckBuildingSpawn(){
        if (spawnBuilding){
            //print("Spawning Building now");
            if (heldBuilding == null){
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard){
                    heldBuilding.transform.position = MousePositionToBuildingPosition(transform.position + heldBuilding.GetOffset(transform.rotation.eulerAngles.y), heldBuilding.XSize, heldBuilding.ZSize);
                } else {
                    rawBuildingOffset = heldBuilding.GetOffset(transform.rotation.eulerAngles.y);
                    heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }
                heldBuilding.Collider.enabled = true;
            } else if (heldBuilding.BuildingType != selectedBuildingType){
                Vector3 pos;
                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard){
                    pos = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.XSize, heldBuilding.ZSize);
                } else {
                    pos = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
                heldBuilding.transform.position = pos;
                heldBuilding.Collider.enabled = true;
            } else {
                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard){
                    heldBuilding.transform.position = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.XSize, heldBuilding.ZSize);
                } else {
                    heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }
            }

            bool collision = heldBuilding.CollisionUpdate();

            if (placeBuilding && ResourceController.Instance.Ore >= heldBuilding.OreCost && !collision){
                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.5f;
                heldBuilding.Place(spawnPos);
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            } else if (cancelBuilding || (placeBuilding && (collision || ResourceController.Instance.Ore < heldBuilding.OreCost))){
                if (placeBuilding){
                    if (ResourceController.Instance.Ore < heldBuilding.OreCost){
                        Debug.Log("You Have Insufficient ore to build this building.");
                    }

                    if (collision){
                        Debug.Log("You cannot place a building there; it would occupy the same area as something else");
                    }
                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            }
        }
    }

    private Vector3 MousePositionToBuildingPosition(Vector3 backup, int xSize, int zSize)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {            
            return SnapBuildingToGrid(hit.point, xSize, zSize);
        }

        return backup;
    }

    private Vector3 RawBuildingPositionToBuildingPosition(int xSize, int zSize)
    {
        Vector3 worldPos = transform.position + rawBuildingOffset;
        Vector3 newOffset = rawBuildingOffset + rawBuildingMovement * Player.Instance.GetMovementSpeed * Time.deltaTime;
        Vector3 newWorldPos = transform.position + newOffset;
        Vector3 newScreenPos = Camera.main.WorldToViewportPoint(newWorldPos);

        if (newScreenPos.x > 0 && newScreenPos.x < 1 && newScreenPos.y > 0 && newScreenPos.y < 1)
        {
            rawBuildingOffset = newOffset;
            worldPos = newWorldPos;
        }

        return SnapBuildingToGrid(worldPos, xSize, zSize);
    }
    

    private Vector3 SnapBuildingToGrid(Vector3 pos, int xSize, int zSize)
    {
        pos.x = Mathf.Round(pos.x) + (xSize == 2 ? 0.5f : 0);
        pos.y = 0.67f;
        pos.z = Mathf.Round(pos.z) + (zSize == 2 ? 0.5f : 0);
        return pos;
    }
*/

}
