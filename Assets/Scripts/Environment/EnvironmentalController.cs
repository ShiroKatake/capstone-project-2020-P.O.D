using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalController : MonoBehaviour {
    //testing variable
    //[SerializeField] private Text var;
    [SerializeField] private ProgressBar progress;
    [SerializeField] private List<RatioBars> bars = new List<RatioBars>();
    //[SerializeField] private RatioBars Bio;
    //[SerializeField] private RatioBars O2;
    //[SerializeField] private RatioBars GHG;
    [SerializeField] private float WinAmount;
    [SerializeField] List<Terraformer> terraformers = new List<Terraformer>();

    public float AtmosphereVal { get; private set; } = 0f;
    public float HumidityVal { get; private set; } = 0f;
    public float BiodiversityVal { get; private set; } = 0f;

    public float TotalVal { get; private set; } = 0.0f;

    private float atmoMalice = 1.0f;
    private float humMalice = 1.0f;
    private float bioMalice = 1.0f;

    private float atmosRatio = 0f;
    private float humRatio = 0f;
    private float bioRatio = 0f;

    public static EnvironmentalController Instance { get; protected set; }

    public List<Terraformer> Terraformers { get => terraformers; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one EnvironmentalController.");
        }

        Instance = this;

        progress.SetMax(WinAmount);
        progress.SetBarValue(TotalVal);
        bars[0].SetMaxBarValue(1);
        bars[0].SetBarValue(atmosRatio);
        bars[1].SetMaxBarValue(1);
        bars[1].SetBarValue(humRatio);
        bars[2].SetMaxBarValue(1);
        bars[2].SetBarValue(bioRatio);
    }

    // Update is called once per frame
    void Update()
    {
        float tpf = Time.deltaTime;
        UpdateParameters();
        CalculateBuildingDeltas(tpf);

        UpdateTotalValue();

        progress.SetBarValue(TotalVal);
        bars[0].SetBarValue(atmosRatio);
        bars[1].SetBarValue(humRatio);
        bars[2].SetBarValue(bioRatio);
        
        float x = 0;
        foreach (RatioBars r in bars){
            print("Value: " + r.CurrentValue);
            if (r.CurrentValue > x){
                x = r.CurrentValue;
            }
        }
        print("New Max: " + x);
        foreach(RatioBars r in bars){
            r.SetMaxRenderBarValue(x);
        }


    }

    private void UpdateTotalValue() {
        TotalVal = (AtmosphereVal / 3f) +
                   (HumidityVal / 3f) +
                   (BiodiversityVal / 3f);
    }

    public void CalculateBuildingDeltas(float tpf) {

        float atmoDelta = 0;
        float humDelta = 0;
        float bioDelta = 0;

        foreach (Terraformer t in terraformers) {
            if (t.Building.Operational) {
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

        AtmosphereVal = Mathf.Min(AtmosphereVal + atmoDelta * atmoMalice * tpf, 100);
        HumidityVal = Mathf.Min(HumidityVal + humDelta * humMalice * tpf, 100);
        BiodiversityVal = Mathf.Min(BiodiversityVal + bioDelta * bioMalice * tpf,100);

    }


    public void UpdateParameters() {

        float baseSum = AtmosphereVal + HumidityVal + BiodiversityVal;

        string outputText = "";


        if (baseSum != 0){
            atmosRatio = AtmosphereVal / baseSum;
            humRatio = HumidityVal / baseSum;
            bioRatio = BiodiversityVal / baseSum;
        }
        //Debug.Log("AtmosphereRatio: " + atmosRatio);

        outputText += "Atmosphere Ratio: " + atmosRatio;
        outputText += "\nHumidity Ratio: " + humRatio;
        outputText += "\nBiodiversity Ratio: " + bioRatio;

        float minthresh = 4f;

        if (AtmosphereVal > minthresh || HumidityVal > minthresh || BiodiversityVal > minthresh) {
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

        //var.text = outputText;

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
            if (terraformers[i].Building.Id == id) {
                terraformers.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Print current values to console
    /// </summary>
    public void PrintEnvironmentValues() {
        string debug = "Atmosphere: " + AtmosphereVal + "\tHumidity: " + HumidityVal + "\tBiodiversity: " + BiodiversityVal;
        Debug.Log(debug);
    }
}
