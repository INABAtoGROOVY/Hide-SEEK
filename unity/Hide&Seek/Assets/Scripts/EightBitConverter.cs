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

        // TODO 最終的に使わなかったら消す
        // 音程に対応した周波数を計算する
        // for(int i = 0; i < _pickUpNotesNum; i++)
        // {
        //     _pickUpNotesFrequencies.Add(
        //         ConvertNoteNumberToFrequency(i + _pickUpNotesTop)
        //     );
        // }
    }

    void Update()
    {
        if (!isConvert)
            return;

        _bgmSpectrum = SoundManager.Instance.GetBGMSpectrumData(_samples);

        //�X�y�N�g���\��
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
        if (_bgmSpectrum == null)
            return;

        // �����ƕϊ���̉��̊��������߂�
        for (int i = 0; i < _samples; i++)
        {
            // ����������������������
            data[i] *= (1 - _ratio);
        }

        float[] wave = new float[_samples];
        for (int i = 0; i < _samples; i++)
        {
            if (_bgmSpectrum[i] < _threshold) continue;
            if (_bgmSpectrum[i] < _bgmSpectrum.Average()) continue;

            // 音程下限と上限でフィルタリングする
            var freqency = CalculateFrequency(i) * Mathf.Pow(2, _pitch / 12);
            var noteNubmer = ConvertFrequencyToNoteNumber(freqency);
            if (noteNubmer < _pickUpNotesTop || _pickUpNotesTop + _pickUpNotesNum <= noteNubmer)
            {
                continue;
            }

            wave = GetWave(
                _waveType,
                i,
                channels,
                freqency
            );

            for (int j = 0; j < data.Length; j++)
            {
                data[j] += wave[j] * _bgmSpectrum[i] * _ratio;
            }
        }
    }

    /// <summary>
    /// �g���擾
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

    /// <summary>
    /// 周波数から音程の近似値に変換する
    /// https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int ConvertFrequencyToNoteNumber(float freq)
    {
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440));
    }

    /// <summary>
    /// 音程から周波数の近似値に変換する
    /// https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private float ConvertNoteNumberToFrequency(int noteNubmer)
    {
        return (float)(440 * Math.Exp(Math.Log(2) / 12 * (noteNubmer - 69)));
    }

    /// <summary>
    /// サンプル数に応じた周波数を計算する
    /// </summary>
    private float CalculateFrequency(int index)
    {
        return (float)_sampleRate / (float)_samples * index;
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

    private int _pickUpNotesTop = 48;
    private int _pickUpNotesNum = 48;

    // TODO 最終的に使わなかったら消す
    //private List<float> _pickUpNotesFrequencies = new List<float>(); 
}
