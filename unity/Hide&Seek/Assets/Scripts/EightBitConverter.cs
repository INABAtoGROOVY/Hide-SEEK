using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EightBitConverter : MonoBehaviour
{
    public bool isConvert = false;

    void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate;
        for (int i = 0; i < _samples; i++)
        {
            _waveTime.Add(0);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isConvert)
            return;

        _bgmSpectrum = SoundManager.Instance.GetBGMSpectrumData(_samples);

        //スペクトル表示
        for (int i = 1; i < _bgmSpectrum.Length - 1; i++)
        {
            Debug.DrawLine(
                new Vector3(Mathf.Log(i - 1) * 3, Mathf.Log(_bgmSpectrum[i - 1]), 3), new Vector3(Mathf.Log(i) * 3, Mathf.Log(_bgmSpectrum[i]), 3), Color.blue
            );
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isConvert)
            return;
        if (!SoundManager.Instance.IsBGMPlaying())
            return;
        if (_bgmSpectrum == null)
            return;

        // 原音と変換後の音の割合を決める
        for (int i = 0; i < _samples; i++)
        {
            // 割合分原音を小さくする
            data[i] *= (1 - _ratio);
        }

        float[] wave = new float[_samples];
        for (int i = 0; i < _samples; i++)
        {
            //Debug.Log(calculateNoteNumberFromFrequency(_bgmSpectrum[i] % 12));
            if (_bgmSpectrum[i] < _threshold) continue;
            if (_bgmSpectrum[i] < _bgmSpectrum.Average())
            {
                //continue;
            }

            //Debug.Log(_bgmSpectrum[i]);
            wave = GetWave(
                _waveType,
                i,
                channels,
                ((float)_sampleRate / (float)_samples) * i * Mathf.Pow(2, (_pitch / 12))
            );

            for (int j = 0; j < data.Length; j++)
            {
                //calculateNoteNumberFromFrequency(wave[j]);
                //Debug.Log(calculateNoteNumberFromFrequency(wave[j] % 12));
                //Debug.Log(noteNames[calculateNoteNumberFromFrequency(wave[j] % 12)]);
                data[j] += wave[j] * _bgmSpectrum[i] * _ratio;
            }

        }
    }

    /// <summary>
    /// 波を取得
    /// </summary>
    private float[] GetWave(WaveType type, int index, int channels, float freqency)
    {
        float[] data = new float[_samples];

        double increment = 2.0 * Math.PI * freqency / _sampleRate;
        for (int i = 0; i < _samples; i += channels)
        {
            data[i] += CalcWave(type, index, increment);

            if (channels == 2) data[i + 1] = data[i];
        }

        return data;
    }

    private float CalcWave(WaveType waveType, int index, double increment)
    {
        float ret = 0;
        switch (waveType)
        {
            case WaveType.Sine:
                _waveTime[index] += increment;
                ret = (float)(_eightBitVolume * 25 * Math.Sin(_waveTime[index]));
                if (_waveTime[index] > 2 * Math.PI) _waveTime[index] = 0;
                return ret;

            case WaveType.Square:
                _waveTime[index] += increment;
                ret = (float)(_eightBitVolume * ((_waveTime[index] % Math.PI * 2) < Math.PI * 2 * 0.5 ? 1.0 : -1.0));
                if (_waveTime[index] > 2 * Math.PI) _waveTime[index] = 0;
                return ret;

            case WaveType.Triangle:
                _waveTime[index] += increment;
                if (_waveTime[index] > 2 * Math.PI) _waveTime[index] = 0;
                double t = (_waveTime[index] + Math.PI * 2) % Math.PI;
                ret = (float)(_eightBitVolume * ((t < Math.PI ? t - Math.PI : Math.PI - t) / Math.PI * 2 + 1.0));
                return ret;

            case WaveType.Sawtooth:
                _waveTime[index] += increment;
                ret = (float)(_eightBitVolume * ((_waveTime[index] + Math.PI) % Math.PI * 2) / Math.PI - 1.0);
                if (_waveTime[index] > 2 * Math.PI) _waveTime[index] = 0;
                return ret;

            default:
                return ret;
        }
    }

    private void TestWave(float[] data, int channels)
    {
        float[] wave = GetWave(
            _waveType,
            0,
            channels,
            440 * Mathf.Pow(2, (_pitch / 12))
        );

        for (int j = 0; j < data.Length; j++)
        {
            data[j] = wave[j] * _ratio;
        }
    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        Debug.Log(freq);
        Debug.Log(Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2)));
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }

    private enum WaveType
    {
        Sine = 0,
        Square = 1,
        Triangle = 2,
        Sawtooth = 3,
    }

    [SerializeField]
    private WaveType _waveType = WaveType.Square;
    [SerializeField, Range(0.0002f, 1)]
    private float _threshold = 0.02f;
    [SerializeField, Range(0, 1)]
    private float _ratio = 0f;
    [SerializeField, Range(0, 1)]
    private double _eightBitVolume = 0.5d;
    [SerializeField]
    private float _pitch = -24;
    [SerializeField]
    private int _samples = 2048;

    private float[] _bgmSpectrum = null;
    List<double> _waveTime = new List<double>();
    private int _sampleRate = 4800; //AudioSettings.outputSampleRate
}
