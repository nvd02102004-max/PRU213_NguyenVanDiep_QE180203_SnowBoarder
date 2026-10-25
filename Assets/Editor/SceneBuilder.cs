using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class SceneBuilder
{
    [MenuItem("SnowBoarder/Generate Scenes")]
    public static void GenerateScenes()
    {
        BuildMainMenuScene();
        BuildEndScene();
        UpdateBuildSettings();
        Debug.Log("SnowBoarder: Scenes successfully generated and registered to Build Settings!");
    }

    [MenuItem("SnowBoarder/Setup Finish Gate (2 Trees & Banner)")]
    public static void SetupFinishGateInLevel1()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        bool wasDifferentScene = activeScene.name != "Level1";
        if (wasDifferentScene)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);
        }

        GameObject finishLine = GameObject.Find("Finish Line");
        if (finishLine == null)
        {
            finishLine = new GameObject("Finish Line");
            finishLine.AddComponent<FinishLine>();
        }

        finishLine.transform.position = new Vector3(152f, -31.5f, 0f);

        // Hide legacy thin stick posts if present
        Transform legacyPost = finishLine.transform.Find("Post");
        if (legacyPost != null) legacyPost.gameObject.SetActive(false);
        Transform legacyTop = finishLine.transform.Find("Top");
        if (legacyTop != null) legacyTop.gameObject.SetActive(false);
        Transform legacyRight = finishLine.transform.Find("RightPost");
        if (legacyRight != null) legacyRight.gameObject.SetActive(false);
        Transform legacySign = finishLine.transform.Find("FinishBannerSign");
        if (legacySign != null) legacySign.gameObject.SetActive(false);

        Sprite tree1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Snow-Tree-1.png");
        Sprite tree2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Snow-Tree-2.png");

        Texture2D whiteTex = new Texture2D(2, 2);
        whiteTex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        whiteTex.Apply();
        Sprite boxSprite = Sprite.Create(whiteTex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);

        // 1. Left Tree
        Transform leftTree = finishLine.transform.Find("FinishTree_Left");
        if (leftTree == null)
        {
            GameObject obj = new GameObject("FinishTree_Left");
            obj.transform.SetParent(finishLine.transform, false);
            leftTree = obj.transform;
        }
        leftTree.localPosition = new Vector3(-2.8f, 1.8f, 0.2f);
        leftTree.localScale = new Vector3(1.35f, 1.35f, 1f);
        SpriteRenderer leftSr = leftTree.GetComponent<SpriteRenderer>() ?? leftTree.gameObject.AddComponent<SpriteRenderer>();
        leftSr.sprite = tree1;
        leftSr.sortingOrder = 3;

        // 2. Right Tree
        Transform rightTree = finishLine.transform.Find("FinishTree_Right");
        if (rightTree == null)
        {
            GameObject obj = new GameObject("FinishTree_Right");
            obj.transform.SetParent(finishLine.transform, false);
            rightTree = obj.transform;
        }
        rightTree.localPosition = new Vector3(6.8f, 1.8f, 0.2f);
        rightTree.localScale = new Vector3(1.4f, 1.4f, 1f);
        SpriteRenderer rightSr = rightTree.GetComponent<SpriteRenderer>() ?? rightTree.gameObject.AddComponent<SpriteRenderer>();
        rightSr.sprite = tree2 ?? tree1;
        rightSr.flipX = true;
        rightSr.sortingOrder = 3;

        // 3. Ground Finish Ribbon
        Transform groundLine = finishLine.transform.Find("GroundFinishRibbon");
        if (groundLine == null)
        {
            GameObject obj = new GameObject("GroundFinishRibbon");
            obj.transform.SetParent(finishLine.transform, false);
            groundLine = obj.transform;
        }
        groundLine.localPosition = new Vector3(2.0f, -0.1f, 0f);
        groundLine.localScale = new Vector3(8.5f, 0.35f, 1f);
        SpriteRenderer grSr = groundLine.GetComponent<SpriteRenderer>() ?? groundLine.gameObject.AddComponent<SpriteRenderer>();
        grSr.sprite = boxSprite;
        grSr.color = new Color(0.95f, 0.25f, 0.35f, 0.9f);
        grSr.sortingOrder = 1;

        // 4. Left Rope
        Transform leftRope = finishLine.transform.Find("LeftHangingRope");
        if (leftRope == null)
        {
            GameObject obj = new GameObject("LeftHangingRope");
            obj.transform.SetParent(finishLine.transform, false);
            leftRope = obj.transform;
        }
        leftRope.localPosition = new Vector3(-0.6f, 4.8f, -0.05f);
        leftRope.localRotation = Quaternion.Euler(0, 0, -28f);
        leftRope.localScale = new Vector3(0.09f, 2.2f, 1f);
        SpriteRenderer lrSr = leftRope.GetComponent<SpriteRenderer>() ?? leftRope.gameObject.AddComponent<SpriteRenderer>();
        lrSr.sprite = boxSprite;
        lrSr.color = new Color(0.38f, 0.26f, 0.16f, 1f);
        lrSr.sortingOrder = 5;

        // 5. Right Rope
        Transform rightRope = finishLine.transform.Find("RightHangingRope");
        if (rightRope == null)
        {
            GameObject obj = new GameObject("RightHangingRope");
            obj.transform.SetParent(finishLine.transform, false);
            rightRope = obj.transform;
        }
        rightRope.localPosition = new Vector3(4.6f, 4.8f, -0.05f);
        rightRope.localRotation = Quaternion.Euler(0, 0, 28f);
        rightRope.localScale = new Vector3(0.09f, 2.2f, 1f);
        SpriteRenderer rrSr = rightRope.GetComponent<SpriteRenderer>() ?? rightRope.gameObject.AddComponent<SpriteRenderer>();
        rrSr.sprite = boxSprite;
        rrSr.color = new Color(0.38f, 0.26f, 0.16f, 1f);
        rrSr.sortingOrder = 5;

        // 6. Horizontal Banner Root
        Transform bannerRoot = finishLine.transform.Find("HorizontalFinishBanner");
        if (bannerRoot == null)
        {
            GameObject obj = new GameObject("HorizontalFinishBanner");
            obj.transform.SetParent(finishLine.transform, false);
            bannerRoot = obj.transform;
        }
        bannerRoot.localPosition = new Vector3(2.0f, 4.3f, -0.15f);

        // Frame
        Transform boardFrame = bannerRoot.Find("BoardFrame");
        if (boardFrame == null)
        {
            GameObject obj = new GameObject("BoardFrame");
            obj.transform.SetParent(bannerRoot, false);
            boardFrame = obj.transform;
        }
        boardFrame.localPosition = Vector3.zero;
        boardFrame.localScale = new Vector3(6.4f, 1.45f, 1f);
        SpriteRenderer bfSr = boardFrame.GetComponent<SpriteRenderer>() ?? boardFrame.gameObject.AddComponent<SpriteRenderer>();
        bfSr.sprite = boxSprite;
        bfSr.color = new Color(0.24f, 0.16f, 0.10f, 1f);
        bfSr.sortingOrder = 6;

        // Plaque
        Transform innerPlaque = bannerRoot.Find("InnerPlaque");
        if (innerPlaque == null)
        {
            GameObject obj = new GameObject("InnerPlaque");
            obj.transform.SetParent(bannerRoot, false);
            innerPlaque = obj.transform;
        }
        innerPlaque.localPosition = new Vector3(0f, 0f, -0.01f);
        innerPlaque.localScale = new Vector3(6.1f, 1.20f, 1f);
        SpriteRenderer ipSr = innerPlaque.GetComponent<SpriteRenderer>() ?? innerPlaque.gameObject.AddComponent<SpriteRenderer>();
        ipSr.sprite = boxSprite;
        ipSr.color = new Color(0.08f, 0.16f, 0.28f, 1f);
        ipSr.sortingOrder = 7;

        // Snow Cap
        Transform snowCap = bannerRoot.Find("SnowCap");
        if (snowCap == null)
        {
            GameObject obj = new GameObject("SnowCap");
            obj.transform.SetParent(bannerRoot, false);
            snowCap = obj.transform;
        }
        snowCap.localPosition = new Vector3(0f, 0.68f, -0.02f);
        snowCap.localScale = new Vector3(6.5f, 0.22f, 1f);
        SpriteRenderer scSr = snowCap.GetComponent<SpriteRenderer>() ?? snowCap.gameObject.AddComponent<SpriteRenderer>();
        scSr.sprite = boxSprite;
        scSr.color = new Color(0.95f, 0.98f, 1f, 1f);
        scSr.sortingOrder = 8;

        // Shadow Text
        Transform textShadow = bannerRoot.Find("TextShadow");
        if (textShadow == null)
        {
            GameObject obj = new GameObject("TextShadow");
            obj.transform.SetParent(bannerRoot, false);
            textShadow = obj.transform;
        }
        textShadow.localPosition = new Vector3(0.04f, -0.04f, -0.03f);
        TextMesh tsTm = textShadow.GetComponent<TextMesh>() ?? textShadow.gameObject.AddComponent<TextMesh>();
        tsTm.text = "★  FINISH  ★";
        tsTm.fontSize = 44;
        tsTm.characterSize = 0.085f;
        tsTm.fontStyle = FontStyle.Bold;
        tsTm.alignment = TextAlignment.Center;
        tsTm.anchor = TextAnchor.MiddleCenter;
        tsTm.color = new Color(0f, 0f, 0f, 0.9f);
        MeshRenderer tsMr = textShadow.GetComponent<MeshRenderer>();
        if (tsMr != null) tsMr.sortingOrder = 9;

        // Main Text
        Transform mainText = bannerRoot.Find("MainText");
        if (mainText == null)
        {
            GameObject obj = new GameObject("MainText");
            obj.transform.SetParent(bannerRoot, false);
            mainText = obj.transform;
        }
        mainText.localPosition = new Vector3(0f, 0f, -0.04f);
        TextMesh mtTm = mainText.GetComponent<TextMesh>() ?? mainText.gameObject.AddComponent<TextMesh>();
        mtTm.text = "★  FINISH  ★";
        mtTm.fontSize = 44;
        mtTm.characterSize = 0.085f;
        mtTm.fontStyle = FontStyle.Bold;
        mtTm.alignment = TextAlignment.Center;
        mtTm.anchor = TextAnchor.MiddleCenter;
        mtTm.color = new Color(1f, 0.88f, 0.20f, 1f);
        MeshRenderer mtMr = mainText.GetComponent<MeshRenderer>();
        if (mtMr != null) mtMr.sortingOrder = 10;

        // Collider
        BoxCollider2D bc = finishLine.GetComponent<BoxCollider2D>();
        if (bc == null) bc = finishLine.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.offset = new Vector2(2.0f, 2.5f);
        bc.size = new Vector2(8.8f, 6.5f);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("SnowBoarder: 2-Trees Finish Gate with Horizontal Banner successfully built in Level1!");
    }

    public static void BuildMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera with Modern Midnight Sky background
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.07f, 0.14f, 1f); // Deep midnight alpine
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cameraObj.AddComponent<AudioListener>();
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(0, 0, -10);

        // 2. Main Menu Manager
        GameObject menuObj = new GameObject("MainMenuManager");
        menuObj.AddComponent<MainMenu>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    public static void BuildEndScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.03f, 0.06f, 0.12f, 1f); // Deep alpine dusk
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cameraObj.AddComponent<AudioListener>();
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(0, 0, -10);

        // 2. End Scene Manager
        GameObject endObj = new GameObject("EndSceneManager");
        endObj.AddComponent<EndSceneController>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/EndScene.unity");
    }

    public static void UpdateBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Level1.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/EndScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        AssetDatabase.SaveAssets();
    }
}
