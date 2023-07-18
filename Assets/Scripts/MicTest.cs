using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Net;

public class MicTest : MonoBehaviour
{
    private int sampleNo = 0;
    [SerializeField]
    public int maxAudioLength = 5; // in seconds
    [SerializeField]
    public int targetFrameRate = 60;

    private float[][] spectrum;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Log(Microphone.devices[0]);
        spectrum = new float[maxAudioLength * targetFrameRate][];
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, maxAudioLength, 44100);
        StartCoroutine(p());
    }

    IEnumerator p()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        yield return new WaitForSeconds(.3f);
        audioSource.Play();
        //yield return new WaitForSeconds(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            //Debug.Log("recording" + Time.time);

            
        }
        var audsrc = GetComponent<AudioSource>();
        if (audsrc.isPlaying)
        {
            if (sampleNo > (targetFrameRate * maxAudioLength))
                return;
            spectrum[sampleNo] = new float[2048];
            //Debug.Log("playing detected");
            audsrc.GetSpectrumData(spectrum[sampleNo], 0,FFTWindow.BlackmanHarris);
            var m = spectrum[sampleNo].Max();
            var i = Array.IndexOf(spectrum[sampleNo], m);
            if (m >= .001f)
                Debug.Log(m.ToString() + " : " + i);
            else
                Debug.Log("Too quiet");
            sampleNo++;
        }
    }
}
