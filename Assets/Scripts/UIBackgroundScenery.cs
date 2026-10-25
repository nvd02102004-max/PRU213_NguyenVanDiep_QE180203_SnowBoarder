using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackgroundScenery : MonoBehaviour
{
    private class FlakeData
    {
        public RectTransform rect;
        public float speedY;
        public float swaySpeed;
        public float swayAmount;
        public float startX;
    }

    private class CloudData
    {
        public RectTransform rect;
        public float speedX;
    }

    private List<FlakeData> flakes = new List<FlakeData>();
    private List<CloudData> clouds = new List<CloudData>();

    private RectTransform canvasRect;

    public void Initialize(RectTransform parentCanvas)
    {
        canvasRect = parentCanvas;
        CreateClouds();
        CreateSnowflakes();
    }

    void CreateClouds()
    {
        Sprite[] cloudSprites = new Sprite[3];
        cloudSprites[0] = Resources.Load<Sprite>("Sprites/Sprite_Cloud_2");
        cloudSprites[1] = Resources.Load<Sprite>("Sprites/Sprite_Cloud_5");
        cloudSprites[2] = Resources.Load<Sprite>("Sprites/Sprite_Cloud_6");

        // If not in Resources, load from AssetDatabase or create procedural soft cloud blobs
        for (int i = 0; i < 4; i++)
        {
            GameObject cloudObj = new GameObject($"Cloud_{i}");
            cloudObj.transform.SetParent(transform, false);

            Image img = cloudObj.AddComponent<Image>();
            img.color = new Color(0.85f, 0.95f, 1f, Random.Range(0.08f, 0.16f)); // Translucent icy clouds
            img.raycastTarget = false;

            RectTransform rt = cloudObj.GetComponent<RectTransform>();
            float scale = Random.Range(180f, 320f);
            rt.sizeDelta = new Vector2(scale * 1.6f, scale * 0.8f);

            float posX = Random.Range(-960f, 960f);
            float posY = Random.Range(150f, 480f);
            rt.anchoredPosition = new Vector2(posX, posY);

            clouds.Add(new CloudData
            {
                rect = rt,
                speedX = Random.Range(8f, 22f)
            });
        }
    }

    void CreateSnowflakes()
    {
        Sprite flakeSprite = UIProceduralSprites.GetSnowflake();

        for (int i = 0; i < 35; i++)
        {
            GameObject flakeObj = new GameObject($"Flake_{i}");
            flakeObj.transform.SetParent(transform, false);

            Image img = flakeObj.AddComponent<Image>();
            img.sprite = flakeSprite;
            img.color = new Color(0.9f, 0.96f, 1f, Random.Range(0.25f, 0.75f));
            img.raycastTarget = false;

            RectTransform rt = flakeObj.GetComponent<RectTransform>();
            float size = Random.Range(10f, 26f);
            rt.sizeDelta = new Vector2(size, size);

            float posX = Random.Range(-960f, 960f);
            float posY = Random.Range(-540f, 540f);
            rt.anchoredPosition = new Vector2(posX, posY);

            flakes.Add(new FlakeData
            {
                rect = rt,
                speedY = Random.Range(35f, 95f),
                swaySpeed = Random.Range(1f, 3f),
                swayAmount = Random.Range(15f, 40f),
                startX = posX
            });
        }
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        // Update clouds
        for (int i = 0; i < clouds.Count; i++)
        {
            Vector2 pos = clouds[i].rect.anchoredPosition;
            pos.x += clouds[i].speedX * dt;
            if (pos.x > 1200f)
            {
                pos.x = -1200f;
                pos.y = Random.Range(150f, 480f);
            }
            clouds[i].rect.anchoredPosition = pos;
        }

        // Update snowflakes
        for (int i = 0; i < flakes.Count; i++)
        {
            FlakeData f = flakes[i];
            Vector2 pos = f.rect.anchoredPosition;
            pos.y -= f.speedY * dt;

            float sway = Mathf.Sin(Time.unscaledTime * f.swaySpeed + i) * f.swayAmount;
            pos.x = f.startX + sway;

            if (pos.y < -580f)
            {
                pos.y = 580f;
                f.startX = Random.Range(-960f, 960f);
            }
            f.rect.anchoredPosition = pos;
        }
    }
}
