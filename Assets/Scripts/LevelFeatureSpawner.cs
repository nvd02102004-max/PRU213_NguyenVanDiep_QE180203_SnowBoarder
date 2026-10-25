using UnityEngine;

public class LevelFeatureSpawner : MonoBehaviour
{
    [Header("Snowflake Settings")]
    [SerializeField] int pointsPerSnowflake = 50;

    void Start()
    {
        EnsureFinishLineAtBase();
        SpawnSnowflakes();
        SpawnFallingSnow();
        SpawnParallaxBackground();
        SpawnTrackDecorations();
        SanitizeSceneDecorations();
    }

    private Sprite whiteBoxSprite = null;

    private Sprite GetWhiteBoxSprite()
    {
        if (whiteBoxSprite == null)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            whiteBoxSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
        }
        return whiteBoxSprite;
    }

    private (Sprite tree1, Sprite tree2) FindTreeSprites()
    {
        Sprite t1 = null, t2 = null;
        SpriteRenderer[] srs = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in srs)
        {
            if (sr.sprite == null) continue;
            if (sr.sprite.name.Contains("Tree-1") && t1 == null) t1 = sr.sprite;
            if (sr.sprite.name.Contains("Tree-2") && t2 == null) t2 = sr.sprite;
        }
        if (t1 == null && t2 != null) t1 = t2;
        if (t2 == null && t1 != null) t2 = t1;
        return (t1, t2);
    }

    void EnsureFinishLineAtBase()
    {
        GameObject finishLine = GameObject.Find("Finish Line");
        if (finishLine == null)
        {
            finishLine = new GameObject("Finish Line");
            finishLine.AddComponent<FinishLine>();
        }

        // Position at the flat landing at the bottom of the downhill mountain
        finishLine.transform.position = new Vector3(152f, -31.5f, 0f);

        // Hide old thin stick posts if present
        Transform legacyPost = finishLine.transform.Find("Post");
        if (legacyPost != null) legacyPost.gameObject.SetActive(false);
        Transform legacyTop = finishLine.transform.Find("Top");
        if (legacyTop != null) legacyTop.gameObject.SetActive(false);
        Transform legacyRight = finishLine.transform.Find("RightPost");
        if (legacyRight != null) legacyRight.gameObject.SetActive(false);
        Transform legacySign = finishLine.transform.Find("FinishBannerSign");
        if (legacySign != null) legacySign.gameObject.SetActive(false);

        var (treeSprite1, treeSprite2) = FindTreeSprites();
        Sprite boxSprite = GetWhiteBoxSprite();

        // 1. LEFT TREE (Snow-Tree-1) - Frame behind the snow track (Order -2, Z = 1.0)
        Transform leftTree = finishLine.transform.Find("FinishTree_Left");
        if (leftTree == null)
        {
            GameObject obj = new GameObject("FinishTree_Left");
            obj.transform.SetParent(finishLine.transform, false);
            leftTree = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = treeSprite1 ?? treeSprite2;
            sr.sortingOrder = -2;
        }
        else
        {
            SpriteRenderer sr = leftTree.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = -2;
        }
        leftTree.localPosition = new Vector3(-3.2f, 1.8f, 1.0f);
        leftTree.localScale = new Vector3(1.35f, 1.35f, 1f);

        // 2. RIGHT TREE (Snow-Tree-2 mirrored) - Frame behind the snow track (Order -2, Z = 1.0)
        Transform rightTree = finishLine.transform.Find("FinishTree_Right");
        if (rightTree == null)
        {
            GameObject obj = new GameObject("FinishTree_Right");
            obj.transform.SetParent(finishLine.transform, false);
            rightTree = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = treeSprite2 ?? treeSprite1;
            sr.flipX = true; // Natural mirror variation
            sr.sortingOrder = -2;
        }
        else
        {
            SpriteRenderer sr = rightTree.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = -2;
        }
        rightTree.localPosition = new Vector3(7.2f, 1.8f, 1.0f);
        rightTree.localScale = new Vector3(1.4f, 1.4f, 1f);

        // 3. GROUND FINISH RIBBON (Checkered line on snow between the 2 trees)
        Transform groundLine = finishLine.transform.Find("GroundFinishRibbon");
        if (groundLine == null)
        {
            GameObject obj = new GameObject("GroundFinishRibbon");
            obj.transform.SetParent(finishLine.transform, false);
            groundLine = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.95f, 0.25f, 0.35f, 0.9f); // Bold racing red/orange finish line
            sr.sortingOrder = 1;
        }
        groundLine.localPosition = new Vector3(2.0f, -0.1f, 0f);
        groundLine.localScale = new Vector3(8.5f, 0.35f, 1f);

        // 4. LEFT SUSPENSION ROPE (Tied from left tree to banner)
        Transform leftRope = finishLine.transform.Find("LeftHangingRope");
        if (leftRope == null)
        {
            GameObject obj = new GameObject("LeftHangingRope");
            obj.transform.SetParent(finishLine.transform, false);
            leftRope = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.38f, 0.26f, 0.16f, 1f); // Dark rustic rope
            sr.sortingOrder = 5;
        }
        leftRope.localPosition = new Vector3(-0.6f, 4.8f, -0.05f);
        leftRope.localRotation = Quaternion.Euler(0, 0, -28f);
        leftRope.localScale = new Vector3(0.09f, 2.2f, 1f);

        // 5. RIGHT SUSPENSION ROPE (Tied from right tree to banner)
        Transform rightRope = finishLine.transform.Find("RightHangingRope");
        if (rightRope == null)
        {
            GameObject obj = new GameObject("RightHangingRope");
            obj.transform.SetParent(finishLine.transform, false);
            rightRope = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.38f, 0.26f, 0.16f, 1f); // Dark rustic rope
            sr.sortingOrder = 5;
        }
        rightRope.localPosition = new Vector3(4.6f, 4.8f, -0.05f);
        rightRope.localRotation = Quaternion.Euler(0, 0, 28f);
        rightRope.localScale = new Vector3(0.09f, 2.2f, 1f);

        // 6. HORIZONTAL BANNER ROOT (Suspended between the two trees)
        Transform bannerRoot = finishLine.transform.Find("HorizontalFinishBanner");
        if (bannerRoot == null)
        {
            GameObject obj = new GameObject("HorizontalFinishBanner");
            obj.transform.SetParent(finishLine.transform, false);
            bannerRoot = obj.transform;
        }
        bannerRoot.localPosition = new Vector3(2.0f, 4.3f, -0.15f);

        // 6.1 Wooden Banner Board Frame
        Transform boardFrame = bannerRoot.Find("BoardFrame");
        if (boardFrame == null)
        {
            GameObject obj = new GameObject("BoardFrame");
            obj.transform.SetParent(bannerRoot, false);
            boardFrame = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.24f, 0.16f, 0.10f, 1f); // Timber border
            sr.sortingOrder = 6;
        }
        boardFrame.localPosition = Vector3.zero;
        boardFrame.localScale = new Vector3(6.4f, 1.45f, 1f);

        // 6.2 Inner Plaque (Alpine Indigo card)
        Transform innerPlaque = bannerRoot.Find("InnerPlaque");
        if (innerPlaque == null)
        {
            GameObject obj = new GameObject("InnerPlaque");
            obj.transform.SetParent(bannerRoot, false);
            innerPlaque = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.08f, 0.16f, 0.28f, 1f); // Deep Alpine Indigo
            sr.sortingOrder = 7;
        }
        innerPlaque.localPosition = new Vector3(0f, 0f, -0.01f);
        innerPlaque.localScale = new Vector3(6.1f, 1.20f, 1f);

        // 6.3 Snow Pile On Top of Banner
        Transform snowCap = bannerRoot.Find("SnowCap");
        if (snowCap == null)
        {
            GameObject obj = new GameObject("SnowCap");
            obj.transform.SetParent(bannerRoot, false);
            snowCap = obj.transform;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = boxSprite;
            sr.color = new Color(0.95f, 0.98f, 1f, 1f); // Fresh white snow
            sr.sortingOrder = 8;
        }
        snowCap.localPosition = new Vector3(0f, 0.68f, -0.02f);
        snowCap.localScale = new Vector3(6.5f, 0.22f, 1f);

        // 6.4 Text "FINISH" (Drop shadow + main bold text)
        Transform textShadow = bannerRoot.Find("TextShadow");
        if (textShadow == null)
        {
            GameObject obj = new GameObject("TextShadow");
            obj.transform.SetParent(bannerRoot, false);
            textShadow = obj.transform;
            TextMesh tm = obj.AddComponent<TextMesh>();
            tm.text = "★  FINISH  ★";
            tm.fontSize = 44;
            tm.characterSize = 0.085f;
            tm.fontStyle = FontStyle.Bold;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = new Color(0f, 0f, 0f, 0.9f);
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = 9;
        }
        textShadow.localPosition = new Vector3(0.04f, -0.04f, -0.03f);

        Transform mainText = bannerRoot.Find("MainText");
        if (mainText == null)
        {
            GameObject obj = new GameObject("MainText");
            obj.transform.SetParent(bannerRoot, false);
            mainText = obj.transform;
            TextMesh tm = obj.AddComponent<TextMesh>();
            tm.text = "★  FINISH  ★";
            tm.fontSize = 44;
            tm.characterSize = 0.085f;
            tm.fontStyle = FontStyle.Bold;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = new Color(1f, 0.88f, 0.20f, 1f); // Radiant Gold
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = 10;
        }
        mainText.localPosition = new Vector3(0f, 0f, -0.04f);

        // 7. FINISH TRIGGER BOX COLLIDER (Tall barrier spanning world height to catch high air jumps)
        BoxCollider2D bc = finishLine.GetComponent<BoxCollider2D>();
        if (bc == null) bc = finishLine.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.offset = new Vector2(2.0f, 25.0f);
        bc.size = new Vector2(12.0f, 70.0f);

        // 8. VICTORY FIREWORKS & CONFETTI PARTICLES
        Transform fwTrans = finishLine.transform.Find("FinishFireworks");
        ParticleSystem ps = null;
        if (fwTrans == null)
        {
            GameObject fwObj = new GameObject("FinishFireworks");
            fwObj.transform.SetParent(finishLine.transform, false);
            fwObj.transform.localPosition = new Vector3(2.0f, 4.5f, 0f);

            ps = fwObj.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr = fwObj.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.material = UIProceduralSprites.GetParticleMaterial();
            }
            var main = ps.main;
            main.playOnAwake = false;
            main.startLifetime = 2.0f;
            main.startSpeed = 8.5f;
            main.startSize = 0.28f;
            main.maxParticles = 150;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0.05f, 60),
                new ParticleSystem.Burst(0.55f, 60)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 45f;
            shape.radius = 1.2f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                    new GradientColorKey(new Color(0.2f, 0.9f, 1f), 0.5f),
                    new GradientColorKey(new Color(1f, 0.3f, 0.4f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.85f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;
        }
        else
        {
            ps = fwTrans.GetComponent<ParticleSystem>();
        }

        // Link particle to FinishLine component if finishEffect is empty
        FinishLine fl = finishLine.GetComponent<FinishLine>();
        if (fl != null && ps != null)
        {
            var field = typeof(FinishLine).GetField("finishEffect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.GetValue(fl) == null)
            {
                field.SetValue(fl, ps);
            }
        }
    }

    void SpawnSnowflakes()
    {
        if (GameObject.Find("Snowflake_Collectibles") != null) return;

        GameObject root = new GameObject("Snowflake_Collectibles");

        // Try to load any snowflake/cloud sprite
        Sprite snowSprite = Resources.Load<Sprite>("Sprites/Sprite_Cloud_5");
        if (snowSprite == null)
        {
            SpriteRenderer[] anySr = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            foreach (var sr in anySr)
            {
                if (sr.sprite != null && sr.sprite.name.Contains("Cloud"))
                {
                    snowSprite = sr.sprite;
                    break;
                }
            }
        }

        Vector3[] points = new Vector3[]
        {
            // First jump arc
            new Vector3(-28f, 7.5f, 0f),
            new Vector3(-25f, 9.2f, 0f),
            new Vector3(-22f, 9.2f, 0f),
            new Vector3(-19f, 7.5f, 0f),

            // Mid-mountain crest jump
            new Vector3(18f, 4.0f, 0f),
            new Vector3(22f, 6.2f, 0f),
            new Vector3(26f, 7.0f, 0f),
            new Vector3(30f, 6.0f, 0f),

            // Big downhill drop jump (Massive Air!)
            new Vector3(48f, 3.2f, 0f),
            new Vector3(52f, 6.0f, 0f),
            new Vector3(56f, 6.0f, 0f),
            new Vector3(60f, 2.5f, 0f),

            // Steep slope chute
            new Vector3(72f, -15.5f, 0f),
            new Vector3(76f, -21.5f, 0f),
            new Vector3(80f, -27.5f, 0f),

            // Final sprint to the finish gate
            new Vector3(100f, -31.5f, 0f),
            new Vector3(115f, -26.5f, 0f),
            new Vector3(125f, -27.5f, 0f),
            new Vector3(135f, -29.5f, 0f),
            new Vector3(145f, -31.0f, 0f)
        };

        for (int i = 0; i < points.Length; i++)
        {
            GameObject sf = new GameObject($"Snowflake_{i + 1}");
            sf.transform.parent = root.transform;
            sf.transform.position = points[i];
            sf.transform.localScale = new Vector3(0.20f, 0.20f, 1f);

            SpriteRenderer sr = sf.AddComponent<SpriteRenderer>();
            sr.sprite = snowSprite;
            sr.color = new Color(0.85f, 0.95f, 1f, 0.95f);
            sr.sortingOrder = 5;

            CircleCollider2D cc = sf.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            cc.radius = 1.6f;

            SnowflakeItem item = sf.AddComponent<SnowflakeItem>();
            item.SetPointValue(pointsPerSnowflake);
        }
    }

    void SpawnFallingSnow()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        if (cam.transform.Find("FallingSnowFX") != null) return;

        GameObject snowObj = new GameObject("FallingSnowFX");
        snowObj.transform.parent = cam.transform;
        snowObj.transform.localPosition = new Vector3(0, 8f, 10f);

        ParticleSystem ps = snowObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = snowObj.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            psr.material = UIProceduralSprites.GetParticleMaterial();
        }

        var main = ps.main;
        main.startLifetime = 3.8f;
        main.startSpeed = 3.8f;
        main.startSize = 0.16f;
        main.startColor = new Color(1f, 1f, 1f, 0.85f);
        main.maxParticles = 500;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 80;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(36, 1, 1);

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.x = -1.5f;
        vel.y = -3.5f;
    }

    void SpawnParallaxBackground()
    {
        if (GameObject.Find("Parallax_Skyline") != null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();

        GameObject skylineRoot = new GameObject("Parallax_Skyline");
        ParallaxCameraFollower follower = skylineRoot.AddComponent<ParallaxCameraFollower>();
        // Vertical factor reduced to 0.20f so sky/clouds remain high above and do not sink into the ground
        follower.Initialize(mainCam != null ? mainCam.transform : null, 0.40f, 0.20f);

        // Find sprites from scene
        Sprite cloudSprite = null;
        Sprite mountainSprite = null;
        Sprite treeSprite = null;

        SpriteRenderer[] anySr = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in anySr)
        {
            if (sr.sprite == null) continue;
            string sName = sr.sprite.name;
            if (cloudSprite == null && sName.Contains("Cloud")) cloudSprite = sr.sprite;
            if (mountainSprite == null && sName.Contains("Rock")) mountainSprite = sr.sprite;
            if (treeSprite == null && sName.Contains("Tree")) treeSprite = sr.sprite;
        }

        // 1. Layer: Distant Snow Mountain Horizon (SortingOrder = -30, Z = 8f)
        if (mountainSprite != null)
        {
            float[] peakX = new float[] { -60f, -25f, 10f, 45f, 80f, 115f, 150f, 185f, 220f };
            float[] peakY = new float[] { 22f, 16f, 20f, 12f, -5f, -15f, -18f, -20f, -22f };
            float[] peakScale = new float[] { 12f, 10f, 14f, 11f, 13f, 12f, 14f, 11f, 12f };

            for (int i = 0; i < peakX.Length; i++)
            {
                GameObject peak = new GameObject($"DistantMountain_{i}");
                peak.transform.parent = skylineRoot.transform;
                peak.transform.position = new Vector3(peakX[i], peakY[i], 8f);
                peak.transform.localScale = new Vector3(peakScale[i] * (i % 2 == 0 ? 1 : -1), peakScale[i] * 1.15f, 1f);

                SpriteRenderer sr = peak.AddComponent<SpriteRenderer>();
                sr.sprite = mountainSprite;
                sr.color = new Color(0.68f, 0.82f, 0.95f, 0.65f); // Atmospheric winter mountain fog
                sr.sortingOrder = -30;
            }
        }

        // 2. Layer: Distant Alpine Pine Forest Ridges (SortingOrder = -15 & -18, Z = 6f)
        if (treeSprite != null)
        {
            float[] ridgeX = new float[] {
                -50f, -38f, -26f, -14f, -2f, 10f, 22f, 34f, 46f, 58f,
                70f, 82f, 94f, 106f, 118f, 130f, 142f, 154f, 166f, 178f
            };

            for (int i = 0; i < ridgeX.Length; i++)
            {
                float t = (float)i / (ridgeX.Length - 1);
                float baseY = Mathf.Lerp(14f, -28f, t);

                // Lower ridge tree
                GameObject t1 = new GameObject($"RidgeTree_A_{i}");
                t1.transform.parent = skylineRoot.transform;
                t1.transform.position = new Vector3(ridgeX[i], baseY + Random.Range(3f, 8f), 6f);
                t1.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

                SpriteRenderer sr1 = t1.AddComponent<SpriteRenderer>();
                sr1.sprite = treeSprite;
                sr1.color = new Color(0.48f, 0.65f, 0.82f, 0.70f);
                sr1.sortingOrder = -15;

                // Upper high-altitude tree for high jump vertical coverage
                GameObject t2 = new GameObject($"RidgeTree_B_{i}");
                t2.transform.parent = skylineRoot.transform;
                t2.transform.position = new Vector3(ridgeX[i] + 6f, baseY + Random.Range(16f, 26f), 6.5f);
                t2.transform.localScale = new Vector3(1.15f, 1.15f, 1f);

                SpriteRenderer sr2 = t2.AddComponent<SpriteRenderer>();
                sr2.sprite = treeSprite;
                sr2.color = new Color(0.55f, 0.72f, 0.88f, 0.55f);
                sr2.sortingOrder = -18;
            }
        }

        // 3. Layer: High Altitude Atmosphere Clouds (SortingOrder = -20, strictly upper sky, soft mist)
        if (cloudSprite != null)
        {
            // Positioned exclusively in the high sky (Y: 28f - 44f) so they never cover slope or landing spots
            float[] cloudX = new float[] { -55f, -25f, 8f, 40f, 75f, 110f, 145f, 180f, -10f, 60f, 130f, 175f };
            float[] cloudY = new float[] { 34f, 38f, 32f, 42f, 34f, 28f, 32f, 28f, 44f, 42f, 36f, 30f };
            float[] cloudScale = new float[] { 2.2f, 1.9f, 2.4f, 2.0f, 1.9f, 2.2f, 2.0f, 2.3f, 2.6f, 2.2f, 2.4f, 2.1f };

            for (int i = 0; i < cloudX.Length; i++)
            {
                GameObject cloud = new GameObject($"AtmosphereCloud_{i}");
                cloud.transform.parent = skylineRoot.transform;
                cloud.transform.position = new Vector3(cloudX[i], cloudY[i], 5f);
                cloud.transform.localScale = Vector3.one * cloudScale[i];

                SpriteRenderer sr = cloud.AddComponent<SpriteRenderer>();
                sr.sprite = cloudSprite;
                sr.color = new Color(0.92f, 0.96f, 1f, 0.25f); // Translucent mist
                sr.sortingOrder = -20;
            }
        }
    }

    void SpawnTrackDecorations()
    {
        if (GameObject.Find("Track_Decorations") != null) return;

        GameObject decorRoot = new GameObject("Track_Decorations");

        // Find tree and rock sprites
        Sprite treeSprite = null;
        Sprite rockSprite = null;

        SpriteRenderer[] allSrs = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in allSrs)
        {
            if (sr.sprite == null) continue;
            if (sr.sprite.name.Contains("Tree") && treeSprite == null) treeSprite = sr.sprite;
            if (sr.sprite.name.Contains("Rock") && rockSprite == null) rockSprite = sr.sprite;
        }

        // Add background pine trees for depth
        if (treeSprite != null)
        {
            Vector3[] treePositions = new Vector3[]
            {
                new Vector3(-35f, 8.5f, 2f),
                new Vector3(-12f, 5.0f, 2f),
                new Vector3(5f, 0.5f, 2f),
                new Vector3(38f, 3.0f, 2f),
                new Vector3(68f, -12.5f, 2f),
                new Vector3(92f, -27.0f, 2f),
                new Vector3(110f, -29.5f, 2f),
                new Vector3(140f, -31.0f, 2f)
            };

            for (int i = 0; i < treePositions.Length; i++)
            {
                GameObject treeObj = new GameObject($"BgTree_{i}");
                treeObj.transform.parent = decorRoot.transform;
                treeObj.transform.position = treePositions[i];
                treeObj.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

                SpriteRenderer sr = treeObj.AddComponent<SpriteRenderer>();
                sr.sprite = treeSprite;
                sr.color = new Color(0.7f, 0.82f, 0.95f, 0.65f); // Atmospheric distant tint
                sr.sortingOrder = -6; // Behind snow track surface (order 0)
            }
        }
    }

    /// <summary>
    /// Runtime safety sanitizer to ensure all decorative scene objects (trees, rocks, clouds)
    /// remain strictly behind the track surface and player, and never obscure landing spots or gameplay.
    /// </summary>
    void SanitizeSceneDecorations()
    {
        SpriteRenderer[] srs = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in srs)
        {
            if (sr == null || sr.gameObject == null) continue;
            string objName = sr.gameObject.name;

            // Preserve player components, finish line banners, and collectibles
            if (objName.Contains("Boarder") || objName.Contains("Barry")) continue;
            if (objName.Contains("Banner") || objName.Contains("Plaque") || objName.Contains("SnowCap") || objName.Contains("Rope") || objName.Contains("Ribbon")) continue;
            if (objName.Contains("Snowflake_") || objName.Contains("Collectibles")) continue;

            // 1. Scene Clouds: Send to distant sky layer, make mist translucent, prevent slope clipping
            if (objName.Contains("Cloud"))
            {
                sr.sortingOrder = -20;
                Color c = sr.color;
                if (c.a > 0.35f)
                {
                    c.a = 0.30f;
                    sr.color = c;
                }
                Vector3 p = sr.transform.position;
                if (p.z < 3f) p.z = 5f;
                // If cloud is down in the terrain area, lift it up to the high sky
                if (p.y < 25f) p.y = Mathf.Max(p.y + 18f, 30f);
                sr.transform.position = p;
            }
            // 2. Scene Trees: Keep behind snow track surface (Order -6, Z >= 1.5f)
            else if (objName.Contains("Tree") && !objName.StartsWith("FinishTree"))
            {
                sr.sortingOrder = -6;
                Vector3 p = sr.transform.position;
                if (p.z < 1.0f) { p.z = 1.5f; sr.transform.position = p; }
            }
            // 3. Scene Rocks: Keep behind snow track surface (Order -4, Z >= 1.2f)
            else if (objName.Contains("Rock") && !objName.StartsWith("DistantMountain"))
            {
                sr.sortingOrder = -4;
                Vector3 p = sr.transform.position;
                if (p.z < 1.0f) { p.z = 1.2f; sr.transform.position = p; }
            }

            // 4. Ensure no decorative props have blocking colliders that impede Barry
            if (objName.Contains("Tree") || objName.Contains("Rock") || objName.Contains("Cloud"))
            {
                Collider2D col = sr.GetComponent<Collider2D>();
                if (col != null && !col.isTrigger)
                {
                    col.enabled = false;
                }
            }
        }
    }
}

public class ParallaxCameraFollower : MonoBehaviour
{
    private Transform targetCamera;
    private float factorX = 0.40f;
    private float factorY = 0.70f;
    private Vector3 initialCamPos;
    private Vector3 initialPos;

    public void Initialize(Transform cam, float fx, float fy)
    {
        targetCamera = cam;
        factorX = fx;
        factorY = fy;
        if (targetCamera != null) initialCamPos = targetCamera.position;
        initialPos = transform.position;
    }

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            Camera c = Camera.main;
            if (c != null)
            {
                targetCamera = c.transform;
                initialCamPos = targetCamera.position;
            }
            return;
        }

        Vector3 camDelta = targetCamera.position - initialCamPos;
        transform.position = initialPos + new Vector3(camDelta.x * factorX, camDelta.y * factorY, 0f);
    }
}
