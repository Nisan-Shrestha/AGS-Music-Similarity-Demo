using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class audioComparer : MonoBehaviour
{

    [SerializeField]
    GameObject audioholder1;

    [SerializeField]
    GameObject audioholder2;

    [SerializeField]
    TMPro.TextMeshProUGUI resultText;
    [SerializeField]
    public int maxAudioLength = 2; // in seconds
    [SerializeField]
    public int targetFrameRate = 60;


    private float[][] spectrum1;
    private float[][] spectrum2;
    private int[][] bin1;
    private int[][] bin2;

    private int sample1 = 0;
    private int sample2 = 0;

    [SerializeField]
    private int lookForwardSamples = 30;
    // Start is called before the first frame update
    void Start()
    {
        resultText.text= string.Empty;
        Application.targetFrameRate= targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void compareAudios()
    {
        float comparisionScore = 0;
        int validSamples = 0;
        sample1= 0;
        sample2= 0;
        spectrum1 = audioholder1.GetComponent<AudioRecorder>().spectrum;
        spectrum2 = audioholder2.GetComponent<AudioRecorder>().spectrum;
        bin1 = audioholder1.GetComponent<AudioRecorder>().maxBins;
        bin2 = audioholder2.GetComponent<AudioRecorder>().maxBins;
        int sampleCount = maxAudioLength * targetFrameRate;
        for (int i = 0; i < sampleCount  && sample1< sampleCount&& sample2<sampleCount; i++)
        {
            if (bin1[sample1] == null || bin2[sample2] == null)
            {
                continue;
            }
            if (bin1[sample1][0]!=-1)
            {
                validSamples++;
                for (int j = 0; j < lookForwardSamples && (sample2+j<sampleCount); j++)
                {
                    if (bin2[sample2+j] == null)
                    {
                        continue;
                    }
                    if (bin1[sample1][0] == bin2[sample2+j][0])
                    {
                        float temp = 0;
                        temp += 1 - ((1.0f / (float)lookForwardSamples)*j);
                        if (bin1[sample1][1] == bin2[sample2 + j][1] && bin1[sample2 + j][1] != -1 && j != 0)
                            temp += .3f - ((.5f / (float)lookForwardSamples) * j);
                        if (bin1[sample1][2] == bin2[sample2 + j][2] && bin1[sample2 + j][2] != -1 && j != 0)
                            temp += .2f - ((.5f / (float)lookForwardSamples) * j);
                        comparisionScore+=  Mathf.Clamp(temp, 0.0f, 1.0f);
                        break; 
                    }
                }
            }
            sample1++;
            sample2++;
        }
        float similarPercent = comparisionScore/ validSamples * 100.0f;
        Debug.Log("score: " + similarPercent + " " + validSamples + " valid samples");
        resultText.text = "Similarity: " + similarPercent.ToString();
        return;
    }
}
