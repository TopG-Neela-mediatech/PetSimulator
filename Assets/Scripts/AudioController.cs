using System.Collections;
using UnityEngine;

enum PlayerState
{
    Idle,
    Listening,
    Talking
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

        // Debug microphone info
        string micName = "";
        foreach (var device in Microphone.devices)
        {
            micName = device;
            Debug.Log("Mic: " + micName);
        }
        Microphone.GetDeviceCaps(micName, out int min, out int max);
        Debug.Log($"Mic Frequency Range: {min} - {max}");
    }

    void Update()
    {
        if (_playerState == PlayerState.Idle && IsVolumeAboveThreshold())
        {
            SwitchState(); // To Listening
        }
    }

    private bool IsVolumeAboveThreshold()
    {
        if (_audioSource.clip == null) return false;

        _audioSource.clip.GetData(_clipSampleData, _audioSource.timeSamples);
        float clipLoudness = 0f;
        foreach (var sample in _clipSampleData)
        {
            clipLoudness += Mathf.Abs(sample);
        }
        clipLoudness /= GameConstants.SampleDataLength;

        Debug.Log("Clip Loudness: " + clipLoudness);
        return clipLoudness > GameConstants.SoundThreshold;
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
        if (_playerAnimator != null)
        {
            //_playerAnimator.SetTrigger(GameConstants.MecanimIdle);

            if (_audioSource.clip != null)
            {
                _audioSource.Stop();
                _audioSource.clip = null;
            }

            // Record loop to detect speaking
            _audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName, true,
                                                 GameConstants.IdleRecordingLength, GameConstants.RecordingFrequency);
            Debug.Log("Idle: Recording loop started.");
        }
    }

    private void Listen()
    {
        if (_playerAnimator != null)
        {
            //_playerAnimator.SetTrigger(GameConstants.MecanimListen);

            // Record fixed length audio
            _audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName, false,
                                                 GameConstants.RecordingLength, GameConstants.RecordingFrequency);

            StartCoroutine(CheckForSilenceAndStop());
            Debug.Log("Listening...");
        }
    }

    private void Talk()
    {
        if (_playerAnimator != null)
        {
            //_playerAnimator.SetTrigger(GameConstants.MecanimTalk);

            Microphone.End(GameConstants.MicrophoneDeviceName);

            if (_audioSource.clip != null)
            {
                // Trim trailing silence before playback
                AudioClip trimmedClip = TrimSilenceFromEnd(_audioSource.clip, GameConstants.SoundThreshold);
                _audioSource.clip = trimmedClip;
                _audioSource.Play();

                Debug.Log("Talking...");
                ScheduleStateSwitch(trimmedClip.length); // Go back to Idle after playback
            }
        }
    }

    private void ScheduleStateSwitch(float delay)
    {
        CancelInvoke(nameof(SwitchState));
        Invoke(nameof(SwitchState), delay);
    }

    /// <summary>
    /// Checks during Listening if a long silence is heard, and transitions to Talking early.
    /// </summary>
    private IEnumerator CheckForSilenceAndStop()
    {
        float silenceTimer = 0f;
        float maxSilenceDuration = 1.5f;
        float checkInterval = 0.1f;

        while (_playerState == PlayerState.Listening && Microphone.IsRecording(GameConstants.MicrophoneDeviceName))
        {
            int micPosition = Microphone.GetPosition(GameConstants.MicrophoneDeviceName);
            int offset = micPosition - _clipSampleData.Length;
            if (offset < 0) offset = 0;

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
                    SwitchState(); // Jump to Talking
                    yield break;
                }
            }
            else
            {
                silenceTimer = 0f; // Reset timer if user is speaking
            }

            yield return new WaitForSeconds(checkInterval);
        }

        // Fallback in case silence wasn’t long enough
        SwitchState();
    }

    /// <summary>
    /// Trims trailing silence from an audio clip.
    /// </summary>
    private AudioClip TrimSilenceFromEnd(AudioClip clip, float threshold)
    {
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        int lastSampleIndex = data.Length - 1;
        while (lastSampleIndex > 0 && Mathf.Abs(data[lastSampleIndex]) < threshold)
        {
            lastSampleIndex--;
        }

        int newLength = lastSampleIndex + 1;
        if (newLength <= 0)
        {
            Debug.LogWarning("Trimmed clip is silent.");
            return clip;
        }

        float[] trimmedData = new float[newLength];
        System.Array.Copy(data, trimmedData, newLength);

        AudioClip newClip = AudioClip.Create("TrimmedClip", newLength / clip.channels,
                                             clip.channels, clip.frequency, false);
        newClip.SetData(trimmedData, 0);

        return newClip;
    }
}
