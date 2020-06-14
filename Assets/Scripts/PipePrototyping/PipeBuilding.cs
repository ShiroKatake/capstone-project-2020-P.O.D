using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBuilding : MonoBehaviour
{

    [SerializeField] public Dictionary<EMaterials, float> inputs = new Dictionary<EMaterials, float>();
    [SerializeField] public Dictionary<EMaterials, float> outputs = new Dictionary<EMaterials, float>();

    Transform waterIcon, powerIcon, oreIcon, wasteIcon;

    float scale = 1;

    void Awake() {


        // Get child objects
        for (int i = 0; i < transform.GetChild(0).childCount; i++) {
            if (transform.GetChild(0).GetChild(i).name == "WaterIcon")
                waterIcon = transform.GetChild(0).GetChild(i);
            if (transform.GetChild(0).GetChild(i).name == "PowerIcon")
                powerIcon = transform.GetChild(0).GetChild(i);
            if (transform.GetChild(0).GetChild(i).name == "OreIcon")
                oreIcon = transform.GetChild(0).GetChild(i);
            if (transform.GetChild(0).GetChild(i).name == "WasteIcon")
                wasteIcon = transform.GetChild(0).GetChild(i);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            scale = transform.localScale.x;
            // Production indicators
            {
                Gizmos.color = Color.green;
                if (outputs.ContainsKey(EMaterials.Water)) {
                    Gizmos.DrawSphere(waterIcon.position + new Vector3(0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (outputs.ContainsKey(EMaterials.Ore)) {
                    Gizmos.DrawSphere(oreIcon.position + new Vector3(0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (outputs.ContainsKey(EMaterials.Power)) {
                    Gizmos.DrawSphere(powerIcon.position + new Vector3(0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (outputs.ContainsKey(EMaterials.Waste)) {
                    Gizmos.DrawSphere(wasteIcon.position + new Vector3(0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
            }

            // Consumption indicators
            {
                Gizmos.color = Color.red;
                if (inputs.ContainsKey(EMaterials.Water)) {
                    Gizmos.DrawSphere(waterIcon.position + new Vector3(-0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (inputs.ContainsKey(EMaterials.Ore)) {
                    Gizmos.DrawSphere(oreIcon.position + new Vector3(-0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (inputs.ContainsKey(EMaterials.Power)) {
                    Gizmos.DrawSphere(powerIcon.position + new Vector3(-0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
                if (inputs.ContainsKey(EMaterials.Waste)) {
                    Gizmos.DrawSphere(wasteIcon.position + new Vector3(-0.12f * scale, -0.05f, 0), 0.05f * scale);
                }
            }
        }
    }

}
