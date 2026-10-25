using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float torqueAmount = 10f;
    [SerializeField] float boostSpeed = 45f;
    [SerializeField] float baseSpeed = 22f;
    [SerializeField] float slowSpeed = 10f;

    [Header("Nitro Settings")]
    [SerializeField] float maxNitro = 100f;
    [SerializeField] float nitroConsumeRate = 28f; // % per second
    [SerializeField] float nitroPassiveRecharge = 3f; // % per second

    private Rigidbody2D rb2d;
    private SurfaceEffector2D surfaceEffector2D;
    private bool canMove = true;
    private float horizontalInput = 0f;
    private float currentNitro = 100f;
    private bool isNitroActive = false;

    public bool CanMove => canMove;
    public float Nitro => currentNitro;
    public float MaxNitro => maxNitro;
    public bool IsNitroActive => isNitroActive;

    public float CurrentSpeed
    {
        get
        {
            if (rb2d == null) return 0f;
#if UNITY_6000_0_OR_NEWER
            return rb2d.linearVelocity.magnitude;
#else
            return rb2d.velocity.magnitude;
#endif
        }
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentNitro = maxNitro;

#if UNITY_2023_1_OR_NEWER
        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
#else
        surfaceEffector2D = FindObjectOfType<SurfaceEffector2D>();
#endif

        if (surfaceEffector2D != null)
        {
            surfaceEffector2D.speed = baseSpeed;
        }

        // Auto-attach TrickDetector to player if missing
        if (GetComponent<TrickDetector>() == null)
        {
            gameObject.AddComponent<TrickDetector>();
        }

        // Auto-attach FallDetector to player if missing
        if (GetComponent<FallDetector>() == null)
        {
            gameObject.AddComponent<FallDetector>();
        }

        // Auto-attach GameHUD to scene if missing
#if UNITY_2023_1_OR_NEWER
        if (FindAnyObjectByType<GameHUD>() == null)
#else
        if (FindObjectOfType<GameHUD>() == null)
#endif
        {
            GameObject hudObj = new GameObject("GameHUD");
            hudObj.AddComponent<GameHUD>();
        }

        // Auto-attach LevelFeatureSpawner to populate snowflakes, weather FX and finish gate
#if UNITY_2023_1_OR_NEWER
        if (FindAnyObjectByType<LevelFeatureSpawner>() == null)
#else
        if (FindObjectOfType<LevelFeatureSpawner>() == null)
#endif
        {
            GameObject spawnerObj = new GameObject("LevelFeatureSpawner");
            spawnerObj.AddComponent<LevelFeatureSpawner>();
        }
    }

    void Update()
    {
        if (canMove)
        {
            CaptureInput();
            HandleNitroAndSpeed();
        }
        else
        {
            horizontalInput = 0f;
            isNitroActive = false;
        }
    }

    void FixedUpdate()
    {
        if (canMove && rb2d != null && horizontalInput != 0f)
        {
            // Consistent physics torque applied in FixedUpdate
            rb2d.AddTorque(-horizontalInput * torqueAmount);
        }
    }

    public void DisableControls()
    {
        canMove = false;
        horizontalInput = 0f;
        isNitroActive = false;
    }

    public void EnableControls()
    {
        canMove = true;
    }

    public void AddNitro(float amount)
    {
        currentNitro = Mathf.Clamp(currentNitro + amount, 0f, maxNitro);
    }

    private void CaptureInput()
    {
        horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
    }

    private void HandleNitroAndSpeed()
    {
        if (surfaceEffector2D == null) return;

        bool wantsBoost = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ||
                          Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ||
                          Input.GetKey(KeyCode.Space);

        bool wantsBrake = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (wantsBoost && currentNitro > 0f)
        {
            isNitroActive = true;
            currentNitro = Mathf.Max(0f, currentNitro - (nitroConsumeRate * Time.deltaTime));
            surfaceEffector2D.speed = boostSpeed;
        }
        else if (wantsBrake)
        {
            isNitroActive = false;
            surfaceEffector2D.speed = slowSpeed;
            // Slowly recharge nitro while braking
            currentNitro = Mathf.Min(maxNitro, currentNitro + (nitroPassiveRecharge * 1.5f * Time.deltaTime));
        }
        else
        {
            isNitroActive = false;
            surfaceEffector2D.speed = baseSpeed;
            // Passive recharge
            currentNitro = Mathf.Min(maxNitro, currentNitro + (nitroPassiveRecharge * Time.deltaTime));
        }
    }

    public void TemporarySpeedBoost(float boostValue, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(boostValue, duration));
    }

    System.Collections.IEnumerator SpeedBoostRoutine(float boostValue, float duration)
    {
        float originalBoost = boostSpeed;
        boostSpeed = boostValue;
        if (surfaceEffector2D != null) surfaceEffector2D.speed = boostValue;
        yield return new WaitForSeconds(duration);
        boostSpeed = originalBoost;
        if (surfaceEffector2D != null) surfaceEffector2D.speed = baseSpeed;
    }
}
