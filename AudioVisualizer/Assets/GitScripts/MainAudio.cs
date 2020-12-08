using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Class responsible for the Audio
[RequireComponent (typeof (AudioSource))]
public class MainAudio : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip[] a_clips;
    public static int sampleCount = 512;
    public int inputSampleCount;
    public static int bandCount = 8; //TODO: SHOULD BE ABLE TO CHANGE FROM 8 TO  t.e.x 10
    public static float[] _samples;
    public static float[] _freqBand;
    public static float[] _bandBuffer;
    float[] _bufferDecrease;
    float[] highestBands;
    float[] bufferDecreaseMultipliers;

    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];
    public static float amplitude, amplitudeBuffer;
    public float highestAmplitude;

    public float audioProfile;
    //public Random random = new Random();
    

    // Start is called before the first frame update
    public static int getBandCount(){
        return bandCount;
    }

    //Reset values for song change
    public void resetValues(){
        audioProfiler(audioProfile);
    }

    //Initialize the variables and set some ot their correct start values
    void Start()
    {
        inputSampleCount = sampleCount;
        _samples = new float[sampleCount];
        _freqBand = new float[bandCount];
        _bandBuffer = new float[bandCount];
        _bufferDecrease = new float[bandCount];
        highestBands = new float[bandCount];
        _audioSource = GetComponent<AudioSource>();
        _audioBand = new float[bandCount];
        _audioBandBuffer = new float[bandCount];
        amplitudeBuffer = 0;
        //_audioSource.time = _audioSource.clip.length * 0.3f;
        //_audioSource.volume = 1.0f;

        bufferDecreaseMultipliers = new float[bandCount];
        for(int i = 0; i < bandCount; i++){
            bufferDecreaseMultipliers[i] = 1.2f;
        }
        audioProfiler(audioProfile);
        Random.seed = _audioSource.clip.name.GetHashCode();
    }

    // Update is called once per frame
    void Update()
    {
        getSpectrumAudioSource();
        MakeFreqBands();
        BandBuffer();
        createAudioBands();
        getAmplitude();
    }

    
    //Switch the currently playing song
    public void switchSong(int songNumber){
        if (songNumber == 0){
            _audioSource.clip = a_clips[0];
            _audioSource.Play();
            Random.seed = a_clips[0].name.GetHashCode();
        } else if (songNumber == 1){
            _audioSource.clip = a_clips[1];
            _audioSource.Play();
            Random.seed = a_clips[1].name.GetHashCode();
        } else {
            _audioSource.clip = a_clips[2];
            _audioSource.Play();
            Random.seed = a_clips[2].name.GetHashCode();
        }
    }

    //Get the spectrum data from the audio source via an FFT function
    void getSpectrumAudioSource(){
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    //Change the current value in the frequency band buffers by setting them into the current freqBand value or decreasing the buffer value
    void BandBuffer(){
        for(int i = 0; i < bandCount; i++){
            if(_freqBand[i] > _bandBuffer[i]){
                _bandBuffer[i] = _freqBand[i];
                //delta time ~~ 0.02 borde vara 0.005
                _bufferDecrease[i] = 0.2f * Time.deltaTime;
            }else{
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= bufferDecreaseMultipliers[i];
            }
        }
    }

    //Set the highest value of the bands to profile, this balances the values of the frequency bands
    void audioProfiler(float profile){
        for (int i = 0; i < 8; i++){
            highestBands[i] = profile;
        }
    }

    // Create the final audioBand (and buffer) by using highestBands to balance out the output levels
    // This is the one that will be used by the rest of the scripts
    void createAudioBands(){
        for(int i = 0; i < bandCount; i++){
            if(_freqBand[i] > highestBands[i]){
                highestBands[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / highestBands[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / highestBands[i]);
        }
    }

    // Get the current amplitude of the music and balance it by using the highestAmplitude variable
    void getAmplitude(){
        float currentAmp = 0f;
        float currentAmpBuffer = 0f;
        for(int i = 0; i < bandCount    ; i++){
            currentAmp += _audioBand[i];
            currentAmpBuffer += _audioBandBuffer[i];
        }
        if(currentAmp > highestAmplitude){
            highestAmplitude = currentAmp;
        }
        amplitude = currentAmp / highestAmplitude;
        amplitudeBuffer = currentAmpBuffer / highestAmplitude;
    }

    //Divide the audio to 8 floats
    //Original hertz is 22050, we have 512 samples.
    //43 hertz per sample
    //
    void MakeFreqBands(){
        int count = 0;
        for (int i = 0; i < bandCount; i++){
            float sampleAverage = 0;
            int sampleCount = (int) (Mathf.Pow(2,i) * (inputSampleCount / Mathf.Pow(2,bandCount))); // fucks up if bandcount != 8
            if(i == bandCount-1){
                sampleCount += 2;
            }
            for(int j = 0; j < sampleCount; j++){
                // * 10 to compensate for lowered volume
                sampleAverage += _samples[count] * (count + 1) * 10;
                count++;
            }

            sampleAverage /= count;

            _freqBand[i] = sampleAverage*100;
        }
    }
}
