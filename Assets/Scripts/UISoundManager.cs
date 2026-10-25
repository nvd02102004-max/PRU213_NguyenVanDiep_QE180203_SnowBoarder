using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    private static UISoundManager instance;
    public static UISoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("UISoundManager");
                instance = obj.AddComponent<UISoundManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private AudioSource audioSource;
    private AudioClip hoverClip;
    private AudioClip clickClip;
    private AudioClip victoryClip;
    private AudioClip popClip;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D UI sound

        GenerateProceduralClips();
    }

    void GenerateProceduralClips()
    {
        hoverClip = CreateSineChirp(0.045f, 880f, 1320f, 0.22f);
        clickClip = CreateClickImpulse(0.06f, 440f, 180f, 0.45f);
        popClip = CreateSineChirp(0.08f, 520f, 980f, 0.35f);
        victoryClip = CreateVictoryFanfare();
    }

    public void PlayHover()
    {
        if (hoverClip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.98f, 1.02f);
            audioSource.PlayOneShot(hoverClip, 0.25f);
        }
    }

    public void PlayClick()
    {
        if (clickClip != null && audioSource != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(clickClip, 0.5f);
        }
    }

    public void PlayPop()
    {
        if (popClip != null && audioSource != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(popClip, 0.4f);
        }
    }

    public void PlayVictory()
    {
        if (victoryClip != null && audioSource != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(victoryClip, 0.6f);
        }
    }

    private AudioClip CreateSineChirp(float duration, float startFreq, float endFreq, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float phase = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            phase += 2f * Mathf.PI * freq / sampleRate;

            // Soft envelope: attack and smooth release
            float env = Mathf.Sin(t * Mathf.PI);
            samples[i] = Mathf.Sin(phase) * env * volume;
        }

        AudioClip clip = AudioClip.Create("UI_Chirp", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateClickImpulse(float duration, float startFreq, float endFreq, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float phase = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(startFreq, endFreq, t * t);
            phase += 2f * Mathf.PI * freq / sampleRate;

            // Exponential decay
            float env = Mathf.Exp(-t * 9f);
            samples[i] = (Mathf.Sin(phase) + 0.3f * Mathf.Sin(phase * 2.5f)) * env * volume;
        }

        AudioClip clip = AudioClip.Create("UI_Click", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateVictoryFanfare()
    {
        float duration = 0.75f;
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        // Notes: C5 (523Hz), E5 (659Hz), G5 (784Hz), C6 (1046Hz)
        float[] noteFreqs = { 523.25f, 659.25f, 783.99f, 1046.50f };
        float noteLen = duration / 4f;

        for (int n = 0; n < 4; n++)
        {
            float freq = noteFreqs[n];
            int startIdx = Mathf.FloorToInt(n * noteLen * sampleRate);
            int endIdx = Mathf.Min(sampleCount, Mathf.FloorToInt((n + 1) * noteLen * sampleRate));
            float phase = 0f;

            for (int i = startIdx; i < endIdx; i++)
            {
                float localT = (float)(i - startIdx) / (endIdx - startIdx);
                phase += 2f * Mathf.PI * freq / sampleRate;
                float env = Mathf.Sin(localT * Mathf.PI);
                float val = Mathf.Sin(phase) * env * 0.45f;
                // Harmonics
                val += Mathf.Sin(phase * 2f) * env * 0.15f;
                samples[i] += val;
            }
        }

        AudioClip clip = AudioClip.Create("UI_VictoryFanfare", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
