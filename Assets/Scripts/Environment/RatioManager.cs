using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatioManager : MonoBehaviour
{

    [SerializeField] private int[] currentRatio = { 0, 0, 0 };
    [SerializeField] private int[] targetRatio = { 0, 0, 0 };

    [SerializeField] private float pointsPerRatio;
    [SerializeField] private float multiplierIncreasePerWave;
    [SerializeField] private int maxRatioValue;

    private int[] waveStartRatio;

    float currentMultiplier = 1;
    float storedPoints = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float ScoreRatioAlignment() {
        int gatingIndex = 0;
        int maxIndex = 0;
        float[] factors = new float[3];
        factors[0] = (float)currentRatio[0] / (float)targetRatio[0];
        factors[1] = (float)currentRatio[1] / (float)targetRatio[1];
        factors[2] = (float)currentRatio[2] / (float)targetRatio[2];

        for (int i = 0; i < factors.Length; i ++) {
            if (factors[i] < factors[gatingIndex])
                gatingIndex = i;
            if (factors[i] > factors[maxIndex])
                maxIndex = i;
        }

        int multiplier = (int)Mathf.Floor(factors[gatingIndex]);
        int maxTier = (int)Mathf.Ceil(factors[maxIndex]);
        int leftoverTier = maxTier - multiplier;

        int[] leftoverRatio = new int[3];
        leftoverRatio[0] = currentRatio[0] - multiplier * targetRatio[0];
        leftoverRatio[1] = currentRatio[1] - multiplier * targetRatio[1];
        leftoverRatio[2] = currentRatio[2] - multiplier * targetRatio[2];

        float[] ratioAccuracy = new float[3];
        ratioAccuracy[0] = (float)leftoverRatio[0] / (float)(targetRatio[0] * leftoverTier);
        ratioAccuracy[1] = (float)leftoverRatio[1] / (float)(targetRatio[1] * leftoverTier);
        ratioAccuracy[2] = (float)leftoverRatio[2] / (float)(targetRatio[2] * leftoverTier);

        float averageAccuracy = (ratioAccuracy[0] + ratioAccuracy[1] + ratioAccuracy[2])/3;
        float maxPoints = pointsPerRatio * maxTier;

        float scoredPoints = multiplier * pointsPerRatio + averageAccuracy * pointsPerRatio;

        return scoredPoints;
    }

    private void IncreaseMultiplier() {
        currentMultiplier += multiplierIncreasePerWave;
    }

    private void ClearMultiplier() {
        currentMultiplier = 1;
    }

    /// <summary>
    /// Updates the target ratio for the player
    /// </summary>
    public void UpdateTargetRatio() {
        targetRatio[0] = Random.Range(1, maxRatioValue);
        targetRatio[1] = Random.Range(1, maxRatioValue);
        targetRatio[2] = Random.Range(1, maxRatioValue);

        if (targetRatio[0] == targetRatio[1] && targetRatio[1] == targetRatio[2]) {
            targetRatio[0] = 1;
            targetRatio[1] = 1;
            targetRatio[2] = 1;
        }
    }

    /// <summary>
    /// Stores the ratio at the start of the wave for comparison at the end
    /// </summary>
    /// <param name="ratios"></param>
    public void StartWave(int[] ratios) {
        waveStartRatio = ratios;
    }

    /// <summary>
    /// Calculates changes in the multiplier based on wave performance
    /// </summary>
    /// <param name="ratios"></param>
    public void EndWave(int[] ratios) {

        // Are any building counts lower than at the start of the wave
        bool isCountLower = (waveStartRatio[0] > ratios[0] ||
                             waveStartRatio[1] > ratios[1] ||
                             waveStartRatio[2] > ratios[2]);
        if (isCountLower)
            ClearMultiplier();
        else
            IncreaseMultiplier();
    }

    /// <summary>
    /// Returns the total number of points the player receives for dispersing on this wave.
    /// Must be called after EndWave
    /// </summary>
    /// <returns></returns>
    public float DispersePoints() {
        float pointsThisWave = ScoreRatioAlignment();

        float returnVal = (pointsThisWave + storedPoints) * currentMultiplier;
        storedPoints = 0;
        ClearMultiplier();

        return returnVal;
    }

    /// <summary>
    /// Stores the points earned this wave in the ratio manager
    /// </summary>
    public void StorePoints() {
        storedPoints += ScoreRatioAlignment();
    }

}
