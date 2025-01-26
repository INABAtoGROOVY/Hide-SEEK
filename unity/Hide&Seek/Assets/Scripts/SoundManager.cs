using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public enum SoundType
    {
        BGM,
        SE,
        Voice
    } 

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayBGM(BGMData.BGMType type)
    {
        BGMData data = GetBGMClip(type);

        _bgmSource.clip = data.clip;
        _bgmSource.loop = data.isLoop;
        _bgmSource.Play();

        _converter.isConvert = true;
    }

    public void StopBGM()
    {
        _converter.isConvert = false;
        _bgmSource.Stop();
    }

    public void PlaySE(SEData.SEType type, AudioSource source = null)
    {
        if (source != null)
        {
            source = _systemSESource;
        }

        if (source == null)
            return;

        SEData data = GetSEClip(type);
        source.PlayOneShot(data.clip);

    }

    public void StopSEAll()
    {
        _systemSESource.Stop();
    }

    public void SetVolume(SoundType type, float volume)
    {
        switch(type)
        {
            case SoundType.BGM:
                _bgmSource.volume = volume;
                break;
            case SoundType.SE:
                _systemSESource.volume = volume;
                break;
            case SoundType.Voice:
                break;
        }
    }

    public float[] GetBGMSpectrumData(int numSamples)
    {
        if (!_bgmSource.isPlaying)
            return null;

        float[] spectrum = new float[numSamples];
        _bgmSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        return spectrum;
    }

    public bool IsBGMPlaying()
    {
        return _bgmSource.isPlaying;
    }

    public void Initialized()
    {

    }

    private BGMData GetBGMClip(BGMData.BGMType type)
    {
        for(int idx = 0; idx < _bgmDataList.Count; idx++)
        {
            if(_bgmDataList[idx].type == type)
            {
                return _bgmDataList[idx];
            }
        }

        return null;
    }

    private SEData GetSEClip(SEData.SEType type)
    {
        for (int idx = 0; idx < _seDataList.Count; idx++)
        {
            if (_seDataList[idx].type == type)
            {
                return _seDataList[idx];
            }
        }

        return null;
    }

    [SerializeField]
    private AudioSource _bgmSource;
    [SerializeField]
    private AudioSource _systemSESource;

    [SerializeField]
    private List<BGMData> _bgmDataList = new List<BGMData>();
    [SerializeField]
    private List<SEData> _seDataList = new List<SEData>();

    [SerializeField]
    private EightBitConverter _converter;
}

[System.Serializable]
public class BGMData
{
    public enum BGMType
    {
        None,
        Title,
        InGame,
    }

    [SerializeField]
    public BGMType type;
    [SerializeField]
    public AudioClip clip;
    [SerializeField]
    public bool isLoop;
}

[System.Serializable]
public class SEData
{
    public enum SEType
    {
        None,
        
    }

    [SerializeField]
    public SEType type;
    [SerializeField]
    public AudioClip clip;
}
