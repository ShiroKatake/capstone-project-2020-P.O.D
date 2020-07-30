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
    [SerializeField] private float winAmount;
    [SerializeField] List<Terraformer> terraformers = new List<Terraformer>();

    public bool Win {get; private set;} = false;

	private float maxRenderBarValue;

    [SerializeField] private float AtmosphereVal = 0f;
    [SerializeField] private float HumidityVal = 0f;
    [SerializeField] private float BiodiversityVal = 0f;

    [SerializeField] private float TotalVal = 0.0f;

    private float atmoMalice = 1.0f;
    private float humMalice = 1.0f;
    private float bioMalice = 1.0f;

	private float atmosRatio = 0f;
	private float humRatio = 0f;
	private float bioRatio = 0f;

    private float progressProportion = 0;

    public static EnvironmentalController Instance { get; protected set; }

    public List<Terraformer> Terraformers { get => terraformers; }
    public float ProgressProportion { get => progressProportion; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one EnvironmentalController.");
        }

        Instance = this;

        progress.SetMax(winAmount);
        progress.SetBarValue(TotalVal);
        bars[0].SetMaxBarValue(1);
        bars[0].SetBarValue(bioRatio);
        bars[1].SetMaxBarValue(1);
        bars[1].SetBarValue(humRatio);
        bars[2].SetMaxBarValue(1);
        bars[2].SetBarValue(atmosRatio);

		maxRenderBarValue = 1f / bars.Count * 2f;
	}

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            float tpf = Time.deltaTime;
            UpdateParameters();
            CalculateBuildingDeltas(tpf);

            UpdateTotalValue();

            progress.SetBarValue(TotalVal);
            progressProportion = TotalVal / winAmount;
            if (TotalVal >= winAmount)
            {
                Win = true;
            }
            bars[0].SetBarValue(bioRatio);
            bars[1].SetBarValue(humRatio);
            bars[2].SetBarValue(atmosRatio);

            float x = 0;
            foreach (RatioBars r in bars)
            {
                if (r.CurrentValue > x)
                {
                    x = r.CurrentValue;
                }
            }
            foreach (RatioBars r in bars)
            {
                r.SetMaxRenderBarValue(maxRenderBarValue);
            }
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

        AtmosphereVal = Mathf.Min(AtmosphereVal + atmoDelta * atmoMalice * tpf, winAmount);
        HumidityVal = Mathf.Min(HumidityVal + humDelta * humMalice * tpf, winAmount);
        BiodiversityVal = Mathf.Min(BiodiversityVal + bioDelta * bioMalice * tpf, winAmount);

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
        outputText += "; Humidity Ratio: " + humRatio;
        outputText += "; Biodiversity Ratio: " + bioRatio;

        float minthresh = 4f;

        if (AtmosphereVal > minthresh || HumidityVal > minthresh || BiodiversityVal > minthresh) {
            outputText += "\n";

            float atmoMaliceT = Mathf.Abs( 1 - Math.Max(0, (atmosRatio - 0.3333f)));
            float humMaliceT = Mathf.Abs(1 - Math.Max(0, (humRatio - 0.3333f)));
            float bioMaliceT = Mathf.Abs(1 - Math.Max(0, (bioRatio - 0.3333f)));

            atmoMalice = MaliceFunction(atmoMaliceT);
            humMalice = MaliceFunction(humMaliceT);
            bioMalice = MaliceFunction(bioMaliceT);

            outputText += "; Atmos Malice: " +atmoMaliceT +"    :    " + atmoMalice;
            outputText += "; Humidity Malice: " + MaliceFunction(humMaliceT);
            outputText += "; Biodiversity Malice: " + MaliceFunction(bioMaliceT);
            outputText += "Atmosphere: " + AtmosphereVal + "; Humidity: " + HumidityVal + "; Biodiversity: " + BiodiversityVal;

            //Debug.Log(outputText);
        }
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
        string debug = "Atmosphere: " + AtmosphereVal + "; Humidity: " + HumidityVal + "; Biodiversity: " + BiodiversityVal;
        Debug.Log(debug);
    }
}
