using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentalController : MonoBehaviour
{
    /// <summary>
    /// Enum contains list of parameters
    /// </summary>
    public enum environmentalParameters {
        Atmosphere,
        Humidity,
        Biodiversity,
    };

    /// <summary>
    /// List of the types of buildings in the game
    /// </summary>
    public enum buildingTypes {
        AtmosphericCondenser,
        WaterDrill,
        Greenhouse
    }

    /// <summary>
    /// Values for each of the parameters.
    /// </summary>
    private Dictionary<environmentalParameters, float> environmentalParameterValues = new Dictionary<environmentalParameters, float>() {
        { environmentalParameters.Atmosphere, 0.0f },
        { environmentalParameters.Humidity, 0.0f },
        { environmentalParameters.Biodiversity, 0.0f }
    };


    /// <summary>
    /// Sets the ideal ratio between buildings
    /// </summary>
    private Dictionary<buildingTypes, int> idealRatio = new Dictionary<buildingTypes, int>() {
        { buildingTypes.AtmosphericCondenser, 3 },
        { buildingTypes.WaterDrill, 2 },
        { buildingTypes.Greenhouse, 1 }
    };

    /// <summary>
    /// Current quantities for buildings
    /// </summary>
    private Dictionary<buildingTypes, int> buildingQuantities = new Dictionary<buildingTypes, int>() {
        { buildingTypes.AtmosphericCondenser, 0 },
        { buildingTypes.WaterDrill, 0 },
        { buildingTypes.Greenhouse, 0 }
    };

    /// <summary>
    /// Total calculated environment progress
    /// </summary>
    private float environmentTotal;




    // Start is called before the first frame update
    void Start()
    {
        PrintEnvironmentValues();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnvironmentTotals();
    }

    public void RegisterBuilding() {

    }

    /// <summary>
    /// Sums up the individual parameters to the total values
    /// </summary>
    public void UpdateEnvironmentTotals() {
        var temp = environmentalParameterValues.Values;

        foreach (buildingTypes firstParam in Enum.GetValues(typeof(buildingTypes))) {
            foreach (buildingTypes secondParam in Enum.GetValues(typeof(buildingTypes))) {
                if (firstParam != secondParam) {
                    int numStructures = buildingQuantities[firstParam];

                    
                }
            }
        }

        float val = temp.Sum();
    }

    /// <summary>
    /// Print the current environment values to the console
    /// </summary>
    public void PrintEnvironmentValues() {

        string debug = "";

        foreach (string name in Enum.GetNames(typeof(environmentalParameters))) {
            debug += name + ": "+ environmentalParameterValues[(environmentalParameters)Enum.Parse(typeof(environmentalParameters), name)] + "\t";
        }

        debug += "\nTotal:\t" + environmentTotal;

        Debug.Log(debug);

        //Enum.GetName
    }

    /// <summary>
    /// Calculated total for the environment calculation.
    /// </summary>
    public float EnvironmentTotal {
        get { return environmentTotal; }
    }

    /// <summary>
    /// Dictionary containing the parameter values.
    /// </summary>
    public Dictionary<environmentalParameters, float> EnvironmentalParameterValues {
        get { return environmentalParameterValues; }
    }

}
