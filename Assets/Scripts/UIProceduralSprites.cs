using UnityEngine;

public static class UIProceduralSprites
{
    private static Sprite primaryButtonNormal;
    private static Sprite primaryButtonHover;
    private static Sprite primaryButtonPressed;
    private static Sprite secondaryButtonNormal;
    private static Sprite secondaryButtonHover;
    private static Sprite secondaryButtonPressed;
    private static Sprite cardBackground;
    private static Sprite modalBackground;
    private static Sprite pillBadge;
    private static Sprite sliderTrack;
    private static Sprite sliderFill;
    private static Sprite sliderKnob;
    private static Sprite snowflakeSprite;
    private static Sprite starFilledSprite;
    private static Sprite starEmptySprite;

    public static Sprite GetPrimaryNormal()
    {
        if (primaryButtonNormal == null)
        {
            primaryButtonNormal = CreateGradientRoundedSprite(
                320, 64, 16,
                new Color(0.08f, 0.55f, 0.88f, 1f), // Top cyan-blue
                new Color(0.02f, 0.40f, 0.72f, 1f), // Bottom deep blue
                new Color(0.40f, 0.85f, 1f, 0.8f),  // Bright cyan border
                2.5f
            );
        }
        return primaryButtonNormal;
    }

    public static Sprite GetPrimaryHover()
    {
        if (primaryButtonHover == null)
        {
            primaryButtonHover = CreateGradientRoundedSprite(
                320, 64, 16,
                new Color(0.18f, 0.70f, 1f, 1f),    // Glowing cyan top
                new Color(0.05f, 0.52f, 0.88f, 1f), // Bright blue bottom
                new Color(0.85f, 0.98f, 1f, 1f),    // High-glow white border
                3.5f
            );
        }
        return primaryButtonHover;
    }

    public static Sprite GetPrimaryPressed()
    {
        if (primaryButtonPressed == null)
        {
            primaryButtonPressed = CreateGradientRoundedSprite(
                320, 64, 16,
                new Color(0.02f, 0.32f, 0.60f, 1f),
                new Color(0.01f, 0.25f, 0.50f, 1f),
                new Color(0.20f, 0.65f, 0.90f, 0.7f),
                2f
            );
        }
        return primaryButtonPressed;
    }

    public static Sprite GetSecondaryNormal()
    {
        if (secondaryButtonNormal == null)
        {
            secondaryButtonNormal = CreateGradientRoundedSprite(
                280, 52, 14,
                new Color(0.10f, 0.16f, 0.28f, 0.88f), // Frosted dark slate
                new Color(0.06f, 0.11f, 0.20f, 0.92f),
                new Color(0.30f, 0.55f, 0.75f, 0.50f), // Soft cyan border
                1.8f
            );
        }
        return secondaryButtonNormal;
    }

    public static Sprite GetSecondaryHover()
    {
        if (secondaryButtonHover == null)
        {
            secondaryButtonHover = CreateGradientRoundedSprite(
                280, 52, 14,
                new Color(0.16f, 0.26f, 0.42f, 0.95f), // Illuminated slate
                new Color(0.10f, 0.18f, 0.30f, 0.95f),
                new Color(0.35f, 0.85f, 1f, 0.95f),    // Vibrant cyan border
                2.5f
            );
        }
        return secondaryButtonHover;
    }

    public static Sprite GetSecondaryPressed()
    {
        if (secondaryButtonPressed == null)
        {
            secondaryButtonPressed = CreateGradientRoundedSprite(
                280, 52, 14,
                new Color(0.04f, 0.08f, 0.15f, 0.95f),
                new Color(0.03f, 0.06f, 0.12f, 0.95f),
                new Color(0.20f, 0.40f, 0.60f, 0.6f),
                1.5f
            );
        }
        return secondaryButtonPressed;
    }

    public static Sprite GetCardBackground()
    {
        if (cardBackground == null)
        {
            cardBackground = CreateGradientRoundedSprite(
                512, 512, 24,
                new Color(0.07f, 0.12f, 0.22f, 0.88f), // Deep alpine midnight
                new Color(0.04f, 0.07f, 0.15f, 0.94f),
                new Color(0.28f, 0.65f, 0.92f, 0.35f), // Subtly glowing rim
                2f
            );
        }
        return cardBackground;
    }

    public static Sprite GetModalBackground()
    {
        if (modalBackground == null)
        {
            modalBackground = CreateGradientRoundedSprite(
                512, 512, 28,
                new Color(0.09f, 0.16f, 0.28f, 0.95f),
                new Color(0.04f, 0.08f, 0.17f, 0.98f),
                new Color(0.35f, 0.85f, 1f, 0.75f),
                2.5f
            );
        }
        return modalBackground;
    }

    public static Sprite GetSliderTrack()
    {
        if (sliderTrack == null)
        {
            sliderTrack = CreateGradientRoundedSprite(
                128, 16, 8,
                new Color(0.07f, 0.13f, 0.22f, 0.95f),
                new Color(0.04f, 0.08f, 0.16f, 0.95f),
                new Color(0.25f, 0.55f, 0.80f, 0.50f),
                1.5f
            );
        }
        return sliderTrack;
    }

    public static Sprite GetSliderFill()
    {
        if (sliderFill == null)
        {
            sliderFill = CreateGradientRoundedSprite(
                128, 16, 8,
                new Color(0.20f, 0.85f, 1f, 1f),
                new Color(0.06f, 0.50f, 0.90f, 1f),
                new Color(0.60f, 0.95f, 1f, 0.70f),
                1.2f
            );
        }
        return sliderFill;
    }

    public static Sprite GetSliderKnob()
    {
        if (sliderKnob == null)
        {
            sliderKnob = CreateGradientRoundedSprite(
                48, 48, 24,
                new Color(0.95f, 0.98f, 1f, 1f),
                new Color(0.35f, 0.82f, 1f, 1f),
                new Color(0.15f, 0.65f, 0.95f, 0.95f),
                2.5f
            );
        }
        return sliderKnob;
    }

    public static Sprite GetPillBadge()
    {
        if (pillBadge == null)
        {
            pillBadge = CreateGradientRoundedSprite(
                200, 40, 20,
                new Color(0.12f, 0.28f, 0.45f, 0.75f),
                new Color(0.08f, 0.18f, 0.32f, 0.85f),
                new Color(0.40f, 0.88f, 1f, 0.70f),
                1.5f
            );
        }
        return pillBadge;
    }

    public static Sprite GetStarFilled()
    {
        if (starFilledSprite == null)
        {
            starFilledSprite = CreateStarSprite(64, new Color(1f, 0.82f, 0.15f, 1f), new Color(1f, 0.95f, 0.6f, 1f));
        }
        return starFilledSprite;
    }

    public static Sprite GetStarEmpty()
    {
        if (starEmptySprite == null)
        {
            starEmptySprite = CreateStarSprite(64, new Color(0.25f, 0.32f, 0.45f, 0.5f), new Color(0.4f, 0.5f, 0.65f, 0.6f));
        }
        return starEmptySprite;
    }

    public static Sprite GetSnowflake()
    {
        if (snowflakeSprite == null)
        {
            snowflakeSprite = CreateSnowflakeSprite(48, new Color(0.9f, 0.96f, 1f, 0.9f));
        }
        return snowflakeSprite;
    }

    private static Material particleMaterial;
    public static Material GetParticleMaterial()
    {
        if (particleMaterial == null)
        {
            Shader s = Shader.Find("Particles/Standard Unlit");
            if (s == null) s = Shader.Find("Sprites/Default");
            particleMaterial = new Material(s);
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float center = (size - 1) * 0.5f;
            Color32[] px = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                    float alpha = Mathf.Clamp01(1f - d);
                    alpha = alpha * alpha;
                    byte a = (byte)(alpha * 255);
                    px[y * size + x] = new Color32(255, 255, 255, a);
                }
            }
            tex.SetPixels32(px);
            tex.Apply();
            particleMaterial.mainTexture = tex;
        }
        return particleMaterial;
    }

    public static Sprite CreateGradientRoundedSprite(
        int width, int height, float cornerRadius,
        Color topColor, Color bottomColor,
        Color borderColor, float borderWidth)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color32[] pixels = new Color32[width * height];

        float halfW = width * 0.5f;
        float halfH = height * 0.5f;
        float innerRadius = Mathf.Max(0.5f, cornerRadius - borderWidth);

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            Color rowBaseColor = Color.Lerp(bottomColor, topColor, t);

            for (int x = 0; x < width; x++)
            {
                // Calculate signed distance to rounded box
                float dx = Mathf.Abs(x - halfW) - (halfW - cornerRadius);
                float dy = Mathf.Abs(y - halfH) - (halfH - cornerRadius);
                dx = Mathf.Max(0, dx);
                dy = Mathf.Max(0, dy);
                float distFromCorner = Mathf.Sqrt(dx * dx + dy * dy);

                // Anti-aliased outer edge
                float outerDist = distFromCorner - cornerRadius;
                float alpha = Mathf.Clamp01(0.5f - outerDist);

                if (alpha <= 0.001f)
                {
                    pixels[y * width + x] = new Color32(0, 0, 0, 0);
                    continue;
                }

                // Check border
                float borderDist = distFromCorner - innerRadius;
                Color pixelColor;

                if (borderWidth > 0 && borderDist > 0)
                {
                    float borderBlend = Mathf.Clamp01(borderDist / borderWidth);
                    pixelColor = Color.Lerp(rowBaseColor, borderColor, borderBlend);
                }
                else
                {
                    pixelColor = rowBaseColor;
                }

                pixelColor.a *= alpha;
                pixels[y * width + x] = pixelColor;
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        float b = Mathf.Min(cornerRadius, Mathf.Min(width * 0.45f, height * 0.45f));
        Vector4 border = b > 0 ? new Vector4(b, b, b, b) : Vector4.zero;
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }

    private static Sprite CreateStarSprite(int size, Color fillColor, Color edgeColor)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color32[] pixels = new Color32[size * size];

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float outerR = size * 0.44f;
        float innerR = size * 0.19f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x, y) - center;
                float angle = Mathf.Atan2(p.y, p.x);
                if (angle < 0) angle += Mathf.PI * 2f;

                // 5-point star
                float angleStep = Mathf.PI / 5f;
                float r = p.magnitude;
                float segment = (angle + Mathf.PI * 0.5f) % (angleStep * 2f);
                if (segment > angleStep) segment = (angleStep * 2f) - segment;

                float maxR = Mathf.Lerp(outerR, innerR, segment / angleStep);
                float dist = r - maxR;
                float alpha = Mathf.Clamp01(0.5f - dist);

                if (alpha > 0.01f)
                {
                    Color c = Color.Lerp(fillColor, edgeColor, Mathf.Clamp01(r / outerR));
                    c.a *= alpha;
                    pixels[y * size + x] = c;
                }
                else
                {
                    pixels[y * size + x] = new Color32(0, 0, 0, 0);
                }
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private static Sprite CreateSnowflakeSprite(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color32[] pixels = new Color32[size * size];

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.42f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x, y) - center;
                float r = p.magnitude;
                float angle = Mathf.Atan2(p.y, p.x);
                if (angle < 0) angle += Mathf.PI * 2f;

                // 6-arm snowflake
                float armAngle = (Mathf.PI * 2f) / 6f;
                float modAngle = angle % armAngle;
                if (modAngle > armAngle * 0.5f) modAngle = armAngle - modAngle;

                float distToArm = r * Mathf.Sin(modAngle);
                bool onArm = distToArm < 1.6f && r <= radius;
                bool centerDot = r < radius * 0.28f;

                // Little diagonal branches
                bool onBranch = false;
                if (r > radius * 0.35f && r < radius * 0.85f)
                {
                    float branchDist = Mathf.Abs(distToArm - (r - radius * 0.35f) * 0.6f);
                    if (branchDist < 1.2f) onBranch = true;
                }

                if (centerDot || onArm || onBranch)
                {
                    pixels[y * size + x] = color;
                }
                else
                {
                    pixels[y * size + x] = new Color32(0, 0, 0, 0);
                }
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
