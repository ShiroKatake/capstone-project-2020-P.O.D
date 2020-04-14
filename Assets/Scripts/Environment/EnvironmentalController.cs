using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalController : MonoBehaviour
{
    [SerializeField] private Text var;
    [SerializeField] List<Terraformer> terraformers = new List<Terraformer>();

    float atmosphereVal = 0.00001f;
    float humidityVal = 0.00001f;
    float biodiversityVal = 0.00001f;

    float atmoMalice = 1.0f;
    float humMalice = 1.0f;
    float bioMalice = 1.0f;

    float totalVal = 0.0f;    

    public static EnvironmentalController Instance { get; protected set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one EnvironmentalController.");
        }

        Instance = this;
    }

    float counter = 0;

    // Update is called once per frame
    void Update()
    {
        float tpf = Time.deltaTime;
        UpdateParameters();
        //TempBuildingConstruction();
        CalculateBuildingDeltas(tpf);
        counter += tpf;

        if (counter > 1) {

            //atmosphereVal += 0.5f * atmoMalice;
            //PrintEnvironmentValues();
            counter = 0;
        }
    }

    public void CalculateBuildingDeltas(float tpf) {

        float atmoDelta = 0;
        float humDelta = 0;
        float bioDelta = 0;

        foreach (Terraformer t in terraformers) {
            if (t.Operational) {
                switch (t.EnvironmentParameter) {
                    case EEnvironmentParameter.Atmosphere:
                        atmoDelta += t.EnvironmentalAffect;
                        break;
                    case EEnvironmentParameter.Humidity:
                        humDelta += t.EnvironmentalAffect;
                        break;
                    case EEnvironmentParameter.Biodiversity:
                        bioDelta += t.EnvironmentalAffect;
                        break;
                    default:
                        break;
                }
            }
        }

        atmosphereVal = Mathf.Min(atmosphereVal + atmoDelta * atmoMalice * tpf, 100);
        humidityVal = Mathf.Min(humidityVal + humDelta * humMalice * tpf, 100);
        biodiversityVal = Mathf.Min(biodiversityVal + bioDelta * bioMalice * tpf,100);

    }

    //public void TempBuildingConstruction() {
    //    if (Input.GetKeyDown(KeyCode.Keypad1)) {
    //        constructedBuildings.Add(("Building",EEnvironmentParameter.Atmosphere, 0.5f));
    //    }
    //    if (Input.GetKeyDown(KeyCode.Keypad2)) {
    //        constructedBuildings.Add(("Building",EEnvironmentParameter.Humidity, 0.5f));
    //    }
    //    if (Input.GetKeyDown(KeyCode.Keypad3)) {
    //        constructedBuildings.Add(("Building", EEnvironmentParameter.Biodiversity, 0.5f));
    //    }
    //}

    public void UpdateParameters() {

        float baseSum = atmosphereVal + humidityVal + biodiversityVal;

        string outputText = "";

        float atmosRatio = atmosphereVal / baseSum;
        float humRatio = humidityVal / baseSum;
        float bioRatio = biodiversityVal / baseSum;
        //Debug.Log("AtmosphereRatio: " + atmosRatio);

        outputText += "Atmosphere Ratio: " + atmosRatio;
        outputText += "\nHumidity Ratio: " + humRatio;
        outputText += "\nBiodiversity Ratio: " + bioRatio;

        float minthresh = 4f;

        if (atmosphereVal > minthresh || humidityVal > minthresh || biodiversityVal > minthresh) {
            outputText += "\n";

            float atmoMaliceT = Mathf.Abs( 1 - Math.Max(0, (atmosRatio - 0.3333f)));
            float humMaliceT = Mathf.Abs(1 - Math.Max(0, (humRatio - 0.3333f)));
            float bioMaliceT = Mathf.Abs(1 - Math.Max(0, (bioRatio - 0.3333f)));

            atmoMalice = MaliceFunction(atmoMaliceT);
            humMalice = MaliceFunction(humMaliceT);
            bioMalice = MaliceFunction(bioMaliceT);

            outputText += "\nAtmos Malice: " +atmoMaliceT +"    :    " + atmoMalice;
            outputText += "\nHumidity Malice: " + MaliceFunction(humMaliceT);
            outputText += "\nBiodiversity Malice: " + MaliceFunction(bioMaliceT);

        }


        var.text = outputText;

    }

    private float MaliceFunction(float input) {
        float normalised = 1 / 0.66666f * input - 0.5f;

        if (normalised > 0.8)
            return normalised;

        float output = Mathf.Max(0, 10 * normalised - 7.25f);
        return output;
    }

    public void RegisterBuilding(Terraformer terraformer) {
        terraformers.Add(terraformer);
    }

    public void RemoveBuilding(int id) {
        for (int i = 0; i < terraformers.Count; i++) {
            if (terraformers[i].BuildingId == id) {
                terraformers.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Print current values to console
    /// </summary>
    public void PrintEnvironmentValues() {
        string debug = "Atmosphere: " + atmosphereVal + "\tHumidity: " + humidityVal + "\tBiodiversity: " + biodiversityVal;
        Debug.Log(debug);
    }
}
