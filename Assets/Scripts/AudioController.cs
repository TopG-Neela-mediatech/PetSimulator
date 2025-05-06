using System.Collections;
using UnityEngine;

enum PlayerState
{
    Idle,
    Listening,
    Talking
}

public static class GameConstants
{
    public const string PlayerTag = "Player";
    public const string GameControllerTag = "AudioController";
    public const string MecanimTalk = "Talk";
    public const string MecanimListen = "Listen";
    public const string MecanimIdle = "Idle";
    public const string MicrophoneDeviceName = null;

    public const int IdleRecordingLength = 1;
    public const int RecordingLength = 10; // Increased to capture more speech
    public const int RecordingFrequency = 48000;

    public const int SampleDataLength = 1024;
    public const float SoundThreshold = 0.025f;
}

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private Animator _playerAnimator;
    private PlayerState _playerState = PlayerState.Idle;
    private AudioSource _audioSource;
    private float[] _clipSampleData;

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _clipSampleData = new float[GameConstants.SampleDataLength];
        Idle();

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Mic: " + device);
        }
    }

    void Update()
    {
        if (_playerState == PlayerState.Idle && IsVolumeAboveThreshold())
        {
            SwitchState();
        }
    }

    private bool IsVolumeAboveThreshold()
    {
        if (_audioSource.clip == null || _audioSource.clip.loadState != AudioDataLoadState.Loaded) return false;

        int position = _audioSource.timeSamples;
        int offset = Mathf.Clamp(position - _clipSampleData.Length, 0, _audioSource.clip.samples - _clipSampleData.Length);

        _audioSource.clip.GetData(_clipSampleData, offset);

        float loudness = 0f;
        foreach (var sample in _clipSampleData)
            loudness += Mathf.Abs(sample);
        loudness /= _clipSampleData.Length;

        Debug.Log("Clip Loudness: " + loudness);
        return loudness > GameConstants.SoundThreshold;
    }

    private void SwitchState()
    {
        switch (_playerState)
        {
            case PlayerState.Idle:
                _playerState = PlayerState.Listening;
                Listen();
                break;
            case PlayerState.Listening:
                _playerState = PlayerState.Talking;
                Talk();
                break;
            case PlayerState.Talking:
                _playerState = PlayerState.Idle;
                Idle();
                break;
        }
    }

    private void Idle()
    {
        if (_audioSource.clip != null)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
        }

        _audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName, true,
                                             GameConstants.IdleRecordingLength, GameConstants.RecordingFrequency);
        Debug.Log("Idle: Recording loop started.");
    }

    private void Listen()
    {
        _audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName, false,
                                             GameConstants.RecordingLength, GameConstants.RecordingFrequency);

        StartCoroutine(CheckForSilenceAndStop());
        Debug.Log("Listening...");
    }

    private void Talk()
    {
        Microphone.End(GameConstants.MicrophoneDeviceName);

        if (_audioSource.clip != null)
        {
            AudioClip trimmedClip = TrimSilenceFromEnd(_audioSource.clip, GameConstants.SoundThreshold);
            _audioSource.clip = trimmedClip;
            _audioSource.Play();

            Debug.Log("Talking...");
            ScheduleStateSwitch(trimmedClip.length);
        }
    }

    private void ScheduleStateSwitch(float delay)
    {
        CancelInvoke(nameof(SwitchState));
        Invoke(nameof(SwitchState), delay);
    }

    private IEnumerator CheckForSilenceAndStop()
    {
        float silenceTimer = 0f;
        float maxSilenceDuration = 1.5f;
        float checkInterval = 0.1f;

        while (_playerState == PlayerState.Listening && Microphone.IsRecording(GameConstants.MicrophoneDeviceName))
        {
            int micPos = Microphone.GetPosition(GameConstants.MicrophoneDeviceName);
            int offset = Mathf.Clamp(micPos - _clipSampleData.Length, 0, _audioSource.clip.samples - _clipSampleData.Length);

            _audioSource.clip.GetData(_clipSampleData, offset);

            float loudness = 0f;
            foreach (var sample in _clipSampleData)
                loudness += Mathf.Abs(sample);
            loudness /= _clipSampleData.Length;

            if (loudness < GameConstants.SoundThreshold)
            {
                silenceTimer += checkInterval;
                if (silenceTimer >= maxSilenceDuration)
                {
                    Debug.Log("Silence detected — ending early.");
                    SwitchState();
                    yield break;
                }
            }
            else
            {
                silenceTimer = 0f;
            }

            yield return new WaitForSeconds(checkInterval);
        }

        SwitchState();
    }

    private AudioClip TrimSilenceFromEnd(AudioClip clip, float threshold)
    {
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        int lastIndex = data.Length - 1;
        while (lastIndex > 0 && Mathf.Abs(data[lastIndex]) < threshold)
        {
            lastIndex--;
        }

        int newLength = lastIndex + 1;
        if (newLength <= 0)
        {
            Debug.LogWarning("Trimmed clip is silent.");
            return clip;
        }

        float[] trimmedData = new float[newLength];
        System.Array.Copy(data, trimmedData, newLength);

        AudioClip newClip = AudioClip.Create("TrimmedClip", newLength / clip.channels, clip.channels, clip.frequency, false);
        newClip.SetData(trimmedData, 0);

        return newClip;
    }
}
