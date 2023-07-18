using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    private int sampleNo = 0;
    public float[][] spectrum;
    public int[][] maxBins;
    private int maxAudioLength;
    private int targetFrameRate;
    private AudioSource audioSource;
    bool ended = false;

    [SerializeField]
    private RectTransform progressbar;

    [SerializeField]
    audioComparer ac;
    // Start is called before the first frame update
    void Start()
    {
        
        maxAudioLength= ac.maxAudioLength;
        targetFrameRate= ac.targetFrameRate;
        audioSource = GetComponent<AudioSource>();
        spectrum = new float[maxAudioLength * targetFrameRate][];
        maxBins = new int[maxAudioLength * targetFrameRate][];
    }

    // Update is called once per frame
    void Update()
    {
        updateSpectrumArray();
        if (!ended)
        {
            Vector3 s = progressbar.localScale;
            s.x = (float)sampleNo / (maxAudioLength * targetFrameRate);
            progressbar.localScale = s;
        }
        else
        {
            Vector3 s = progressbar.localScale;
            s.x = 1;
            progressbar.localScale = s;
        }
    }

    private void updateSpectrumArray()
    {
        if (audioSource.isPlaying && sampleNo < (targetFrameRate * maxAudioLength))
        {
            spectrum[sampleNo] = new float[2048];
            maxBins[sampleNo] = new int[3];
            //Debug.Log("playing detected");
            
            audioSource.GetSpectrumData(spectrum[sampleNo], 0, FFTWindow.BlackmanHarris);
            for (int i = 0; i < 3; i++)
            {
                var m = spectrum[sampleNo].Max();
                var binNo = Array.IndexOf(spectrum[sampleNo], m);
                if (m >= .001f)
                {
                    maxBins[sampleNo][i] = binNo;
                    spectrum[sampleNo][binNo] = 0;
                }
                else
                    maxBins[sampleNo][i] = -1;
            }
            sampleNo++;
        }
    }

    public void beginRecording()
    {
        sampleNo= 0;
        Debug.Log(Microphone.devices[0]);
        audioSource.clip = Microphone.Start(Microphone.devices[0], false, maxAudioLength, 44100);
        StartCoroutine(playDelayed());
        StartCoroutine(endRec());
        ended= false;
    }

    public void endRecording()
    {
        if (Microphone.IsRecording(Microphone.devices[0]))
            Microphone.End(Microphone.devices[0]);

    }

    //public void StartSpectrumRecording()
    //{

    //}

    IEnumerator playDelayed()
    {
        yield return new WaitForSeconds(.3f);
        audioSource.Play();
        //yield return new WaitForSeconds(0);
        Debug.Log("record started");
    }

    IEnumerator endRec()
    {
        yield return new WaitForSeconds(maxAudioLength);
        if (Microphone.IsRecording(Microphone.devices[0]))
            Microphone.End(Microphone.devices[0]);
        Debug.Log("record ended");
        ended = true;
    }
}
