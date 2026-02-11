
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupRepairShopUI
{
    [MenuItem("Tools/Setup Repair Shop UI")]
    public static void Setup()
    {
        // Canvas 찾기
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();

        // ===== ScrollView =====
        GameObject scrollViewGO = new GameObject("ScrollView", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect));
        scrollViewGO.transform.SetParent(canvasRT, false);
        scrollViewGO.layer = LayerMask.NameToLayer("UI");

        RectTransform scrollRT = scrollViewGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0.03f, 0.05f);
        scrollRT.anchorMax = new Vector2(0.97f, 0.9f);
        scrollRT.offsetMin = Vector2.zero;
        scrollRT.offsetMax = Vector2.zero;

        Image scrollImg = scrollViewGO.GetComponent<Image>();
        scrollImg.color = new Color(0.92f, 0.92f, 0.94f, 1f);

        ScrollRect scrollRect = scrollViewGO.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        // Viewport
        GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
        viewportGO.transform.SetParent(scrollViewGO.transform, false);
        viewportGO.layer = LayerMask.NameToLayer("UI");

        RectTransform vpRT = viewportGO.GetComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = Vector2.zero;
        vpRT.offsetMax = Vector2.zero;

        Image vpImg = viewportGO.GetComponent<Image>();
        vpImg.color = new Color(0.92f, 0.92f, 0.94f, 1f);

        Mask vpMask = viewportGO.GetComponent<Mask>();
        vpMask.showMaskGraphic = true;

        // Content
        GameObject contentGO = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentGO.transform.SetParent(viewportGO.transform, false);
        contentGO.layer = LayerMask.NameToLayer("UI");

        RectTransform contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.offsetMin = new Vector2(0, 0);
        contentRT.offsetMax = new Vector2(0, 0);

        VerticalLayoutGroup vlg = contentGO.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.spacing = 10;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        ContentSizeFitter csf = contentGO.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRT;
        scrollRect.viewport = vpRT;

        // ===== ShopItem (프리팹 템플릿) =====
        GameObject shopItemGO = new GameObject("ShopItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        shopItemGO.transform.SetParent(contentGO.transform, false);
        shopItemGO.layer = LayerMask.NameToLayer("UI");

        Image itemImg = shopItemGO.GetComponent<Image>();
        itemImg.color = new Color(0.97f, 0.97f, 0.99f, 1f);

        LayoutElement itemLE = shopItemGO.GetComponent<LayoutElement>();
        itemLE.preferredHeight = 180;
        itemLE.minHeight = 180;

        VerticalLayoutGroup itemVLG = shopItemGO.GetComponent<VerticalLayoutGroup>();
        itemVLG.padding = new RectOffset(20, 20, 10, 10);
        itemVLG.spacing = 5;
        itemVLG.childForceExpandWidth = true;
        itemVLG.childForceExpandHeight = false;
        itemVLG.childControlWidth = true;
        itemVLG.childControlHeight = true;

        // ShopItem 자식 텍스트 3개
        CreateTMPText("ShopName", shopItemGO.transform, "1. 정비소 이름", 36, FontStyles.Bold, new Color(0.15f, 0.15f, 0.15f));
        CreateTMPText("ShopAddress", shopItemGO.transform, "도로명주소", 28, FontStyles.Normal, new Color(0.35f, 0.35f, 0.35f));
        CreateTMPText("ShopTelDist", shopItemGO.transform, "전화번호  |  0.0km", 26, FontStyles.Normal, new Color(0.45f, 0.45f, 0.45f));

        shopItemGO.SetActive(false);

        // ===== DetailPanel =====
        GameObject detailGO = new GameObject("DetailPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
        detailGO.transform.SetParent(canvasRT, false);
        detailGO.layer = LayerMask.NameToLayer("UI");

        RectTransform detailRT = detailGO.GetComponent<RectTransform>();
        detailRT.anchorMin = new Vector2(0.05f, 0.15f);
        detailRT.anchorMax = new Vector2(0.95f, 0.85f);
        detailRT.offsetMin = Vector2.zero;
        detailRT.offsetMax = Vector2.zero;

        Image detailImg = detailGO.GetComponent<Image>();
        detailImg.color = new Color(1f, 1f, 1f, 0.98f);

        VerticalLayoutGroup detailVLG = detailGO.GetComponent<VerticalLayoutGroup>();
        detailVLG.padding = new RectOffset(40, 40, 30, 30);
        detailVLG.spacing = 15;
        detailVLG.childForceExpandWidth = true;
        detailVLG.childForceExpandHeight = false;
        detailVLG.childControlWidth = true;
        detailVLG.childControlHeight = true;

        // Detail 텍스트 8개
        CreateTMPText("DetailTitle", detailGO.transform, "정비소 이름", 40, FontStyles.Bold, new Color(0.1f, 0.1f, 0.1f));
        CreateTMPText("DetailAddress", detailGO.transform, "주소: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailTel", detailGO.transform, "전화: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailKind", detailGO.transform, "업종: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailState", detailGO.transform, "상태: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailDistance", detailGO.transform, "거리: - km", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailRegDate", detailGO.transform, "등록일: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
        CreateTMPText("DetailArea", detailGO.transform, "면적: -", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));

        // Close Button
        GameObject closeBtnGO = new GameObject("CloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        closeBtnGO.transform.SetParent(detailGO.transform, false);
        closeBtnGO.layer = LayerMask.NameToLayer("UI");

        Image closeBtnImg = closeBtnGO.GetComponent<Image>();
        closeBtnImg.color = new Color(0.85f, 0.2f, 0.2f, 1f);

        LayoutElement closeLE = closeBtnGO.GetComponent<LayoutElement>();
        closeLE.preferredHeight = 80;
        closeLE.minHeight = 80;

        CreateTMPText("CloseBtnText", closeBtnGO.transform, "닫기", 36, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);

        // ===== SerializeField 연결 =====
        CarRepairShopFinder finder = Object.FindFirstObjectByType<CarRepairShopFinder>();
        if (finder != null)
        {
            SerializedObject so = new SerializedObject(finder);
            so.FindProperty("listContent").objectReferenceValue = contentGO.transform;
            so.FindProperty("shopItemPrefab").objectReferenceValue = shopItemGO;
            so.FindProperty("detailPanel").objectReferenceValue = detailGO;

            // Detail 텍스트 연결
            so.FindProperty("detailName").objectReferenceValue = detailGO.transform.Find("DetailTitle").GetComponent<TMP_Text>();
            so.FindProperty("detailAddress").objectReferenceValue = detailGO.transform.Find("DetailAddress").GetComponent<TMP_Text>();
            so.FindProperty("detailTel").objectReferenceValue = detailGO.transform.Find("DetailTel").GetComponent<TMP_Text>();
            so.FindProperty("detailKind").objectReferenceValue = detailGO.transform.Find("DetailKind").GetComponent<TMP_Text>();
            so.FindProperty("detailState").objectReferenceValue = detailGO.transform.Find("DetailState").GetComponent<TMP_Text>();
            so.FindProperty("detailDistance").objectReferenceValue = detailGO.transform.Find("DetailDistance").GetComponent<TMP_Text>();
            so.FindProperty("detailRegDate").objectReferenceValue = detailGO.transform.Find("DetailRegDate").GetComponent<TMP_Text>();
            so.FindProperty("detailArea").objectReferenceValue = detailGO.transform.Find("DetailArea").GetComponent<TMP_Text>();
            so.FindProperty("detailCloseBtn").objectReferenceValue = closeBtnGO.GetComponent<Button>();

            // SearchButton 연결
            GameObject searchBtn = GameObject.Find("SearchButton");
            if (searchBtn != null)
                so.FindProperty("searchButton").objectReferenceValue = searchBtn.GetComponent<Button>();

            so.ApplyModifiedProperties();
            Debug.Log("[SetupUI] 모든 SerializeField 연결 완료!");
        }

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("[SetupUI] UI 생성 완료!");
    }

    [MenuItem("Tools/Add Location Btn + Time Text")]
    public static void AddLocationAndTime()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("Canvas not found!"); return; }
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();

        // ===== InputField 폭 줄이기 =====
        var inputGO = GameObject.Find("InputField (TMP)");
        if (inputGO != null)
        {
            RectTransform inputRT = inputGO.GetComponent<RectTransform>();
            inputRT.anchorMax = new Vector2(0.6f, inputRT.anchorMax.y);
        }

        // ===== SearchButton 위치 조정 (좁게) =====
        var searchBtnGO = GameObject.Find("SearchButton");
        if (searchBtnGO != null)
        {
            RectTransform searchRT = searchBtnGO.GetComponent<RectTransform>();
            searchRT.anchorMin = new Vector2(0.62f, searchRT.anchorMin.y);
            searchRT.anchorMax = new Vector2(0.77f, searchRT.anchorMax.y);
        }

        // ===== LocationButton 생성 =====
        GameObject locBtnGO = new GameObject("LocationButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        locBtnGO.transform.SetParent(canvasRT, false);
        locBtnGO.layer = LayerMask.NameToLayer("UI");

        RectTransform locRT = locBtnGO.GetComponent<RectTransform>();
        locRT.anchorMin = new Vector2(0.79f, 0.92f);
        locRT.anchorMax = new Vector2(0.97f, 0.97f);
        locRT.offsetMin = Vector2.zero;
        locRT.offsetMax = Vector2.zero;

        Image locImg = locBtnGO.GetComponent<Image>();
        locImg.color = new Color(0.2f, 0.75f, 0.4f, 1f);

        CreateTMPText("LocBtnText", locBtnGO.transform, "내 위치", 30, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);

        // ===== DetailPanel에 운영시간 텍스트 추가 (CloseButton 앞에) =====
        GameObject detailPanelGO = GameObject.Find("DetailPanel");
        if (detailPanelGO != null)
        {
            // DetailArea 다음, CloseButton 앞에 삽입
            Transform closeBtn = detailPanelGO.transform.Find("CloseButton");
            CreateTMPText("DetailTime", detailPanelGO.transform, "운영시간 : 없음 ~ 없음", 30, FontStyles.Normal, new Color(0.25f, 0.25f, 0.25f));
            // CloseButton을 맨 아래로 이동
            if (closeBtn != null) closeBtn.SetAsLastSibling();
        }

        // ===== SerializeField 연결 =====
        CarRepairShopFinder finder = Object.FindFirstObjectByType<CarRepairShopFinder>();
        if (finder != null)
        {
            SerializedObject so = new SerializedObject(finder);
            so.FindProperty("locationButton").objectReferenceValue = locBtnGO.GetComponent<Button>();

            if (detailPanelGO != null)
            {
                var detailTimeObj = detailPanelGO.transform.Find("DetailTime");
                if (detailTimeObj != null)
                    so.FindProperty("detailTime").objectReferenceValue = detailTimeObj.GetComponent<TMP_Text>();
            }

            so.ApplyModifiedProperties();
            Debug.Log("[SetupUI] locationButton + detailTime 연결 완료!");
        }

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("[SetupUI] 내 위치 버튼 + 운영시간 추가 완료!");
    }

    [MenuItem("Tools/Add Naver Map to DetailPanel")]
    public static void AddNaverMap()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("Canvas not found!"); return; }

        // DetailPanel을 CarRepairShopFinder의 SerializeField에서 가져오기 (비활성도 가능)
        CarRepairShopFinder finder = Object.FindFirstObjectByType<CarRepairShopFinder>();
        if (finder == null) { Debug.LogError("CarRepairShopFinder not found!"); return; }

        SerializedObject finderSO = new SerializedObject(finder);
        GameObject detailPanelGO = finderSO.FindProperty("detailPanel").objectReferenceValue as GameObject;
        if (detailPanelGO == null) { Debug.LogError("DetailPanel not found in SerializeField!"); return; }

        Transform closeBtn = detailPanelGO.transform.Find("CloseButton");

        // ===== RawImage (지도 이미지) =====
        GameObject mapGO = new GameObject("MapImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(LayoutElement));
        mapGO.transform.SetParent(detailPanelGO.transform, false);
        mapGO.layer = LayerMask.NameToLayer("UI");

        RawImage rawImg = mapGO.GetComponent<RawImage>();
        rawImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);   // 로딩 전 회색 배경

        LayoutElement mapLE = mapGO.GetComponent<LayoutElement>();
        mapLE.preferredHeight = 250;
        mapLE.minHeight = 200;

        // ===== "네이버 지도에서 보기" 버튼 =====
        GameObject openMapBtnGO = new GameObject("OpenMapButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        openMapBtnGO.transform.SetParent(detailPanelGO.transform, false);
        openMapBtnGO.layer = LayerMask.NameToLayer("UI");

        Image openMapImg = openMapBtnGO.GetComponent<Image>();
        openMapImg.color = new Color(0.12f, 0.76f, 0.38f, 1f);  // 네이버 그린

        LayoutElement openMapLE = openMapBtnGO.GetComponent<LayoutElement>();
        openMapLE.preferredHeight = 70;
        openMapLE.minHeight = 70;

        CreateTMPText("OpenMapBtnText", openMapBtnGO.transform, "네이버 지도에서 보기", 30, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);

        // CloseButton을 맨 아래로 이동
        if (closeBtn != null) closeBtn.SetAsLastSibling();

        // ===== SerializeField 연결 =====
        finderSO.Update();
        finderSO.FindProperty("mapImage").objectReferenceValue = rawImg;
        finderSO.FindProperty("openMapButton").objectReferenceValue = openMapBtnGO.GetComponent<Button>();
        finderSO.ApplyModifiedProperties();
        Debug.Log("[SetupUI] mapImage + openMapButton 연결 완료!");

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("[SetupUI] 네이버 지도 UI 추가 완료!");
    }

    static void CreateTMPText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color, TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        go.layer = LayerMask.NameToLayer("UI");

        TMP_Text tmp = go.GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = align;
    }
}
#endif
