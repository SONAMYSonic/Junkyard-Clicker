using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CarRepairShopFinder : MonoBehaviour
{
    // 경기도 자동차정비업체 현황 API
    private const string API_KEY = "602f9b6fbe2b4eb49bdb587bea6caab6";
    private const string BASE_URL = "https://openapi.gg.go.kr/Carimprventrpsinfo";

    [Header("UI 참조")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button searchButton;       // 검색 버튼
    [SerializeField] private Button locationButton;    // 내 위치 버튼
    [SerializeField] private Transform listContent;   // ScrollView > Viewport > Content
    [SerializeField] private GameObject shopItemPrefab; // 목록 아이템 프리팹
    [SerializeField] private GameObject detailPanel;    // 상세 정보 패널

    [Header("상세 패널 내부")]
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailAddress;
    [SerializeField] private TMP_Text detailTel;
    [SerializeField] private TMP_Text detailKind;
    [SerializeField] private TMP_Text detailState;
    [SerializeField] private TMP_Text detailDistance;
    [SerializeField] private TMP_Text detailRegDate;
    [SerializeField] private TMP_Text detailArea;
    [SerializeField] private TMP_Text detailTime;
    [SerializeField] private Button detailCloseBtn;

    [Header("네이버 지도")]
    [SerializeField] private RawImage mapImage;           // 지도 이미지 표시
    [SerializeField] private Button openMapButton;        // "네이버 지도에서 보기" 버튼

    // 네이버 Static Map API
    private const string NAVER_MAP_CLIENT_ID = "3h38irmf53";
    private const string NAVER_MAP_CLIENT_SECRET = "j4K8yGOU9H8hwYX2nePeqMhKJ04ZoJkSvMxpw8or";
    private const string NAVER_STATIC_MAP_URL = "https://maps.apigw.ntruss.com/map-static/v2/raster";

    // 시/군 이름 → 대략적 좌표 매핑 (주요 지역)
    private Dictionary<string, (double lat, double lon)> sigunCoords = new Dictionary<string, (double, double)>
    {
        { "수원시", (37.2636, 127.0286) },
        { "성남시", (37.4200, 127.1267) },
        { "고양시", (37.6584, 126.8320) },
        { "용인시", (37.2411, 127.1776) },
        { "부천시", (37.5034, 126.7660) },
        { "안산시", (37.3219, 126.8309) },
        { "안양시", (37.3943, 126.9568) },
        { "남양주시", (37.6360, 127.2165) },
        { "화성시", (37.1995, 126.8313) },
        { "평택시", (36.9921, 127.1127) },
        { "의정부시", (37.7381, 127.0338) },
        { "시흥시", (37.3800, 126.8030) },
        { "파주시", (37.7126, 126.7681) },
        { "광명시", (37.4786, 126.8646) },
        { "김포시", (37.6153, 126.7156) },
        { "군포시", (37.3614, 126.9352) },
        { "광주시", (37.4095, 127.2550) },
        { "이천시", (37.2720, 127.4348) },
        { "양주시", (37.7853, 127.0458) },
        { "오산시", (37.1498, 127.0698) },
        { "구리시", (37.5943, 127.1295) },
        { "안성시", (37.0080, 127.2797) },
        { "포천시", (37.8949, 127.2002) },
        { "의왕시", (37.3449, 126.9685) },
        { "하남시", (37.5393, 127.2148) },
        { "여주시", (37.2983, 127.6375) },
        { "동두천시", (37.9035, 127.0606) },
        { "과천시", (37.4292, 126.9876) },
    };

    // 검색 결과 캐싱 (상세 정보 표시용)
    private List<RepairShop> cachedShops = new List<RepairShop>();

    private void Start()
    {
        Debug.Log($"[CarRepairShopFinder] Start() 호출됨. inputField={inputField}, listContent={listContent}, shopItemPrefab={shopItemPrefab}, detailPanel={detailPanel}");

        inputField.onSubmit.AddListener((text) =>
        {
            Debug.Log($"[CarRepairShopFinder] onSubmit 호출됨: '{text}'");
            SearchRepairShops(text).Forget();
        });

        searchButton.onClick.AddListener(() =>
        {
            Debug.Log($"[CarRepairShopFinder] 검색 버튼 클릭: '{inputField.text}'");
            SearchRepairShops(inputField.text).Forget();
        });

        locationButton.onClick.AddListener(() =>
        {
            Debug.Log("[CarRepairShopFinder] 내 위치 버튼 클릭");
            SearchByMyLocation().Forget();
        });

        detailPanel.SetActive(false);
        detailCloseBtn.onClick.AddListener(() => detailPanel.SetActive(false));

        // 시작 시 랜덤 지역으로 20개 로딩
        LoadRandomShops().Forget();
    }

    private async UniTaskVoid LoadRandomShops()
    {
        // 딕셔너리에서 랜덤 시/군 선택
        var keys = new List<string>(sigunCoords.Keys);
        string randomSigun = keys[UnityEngine.Random.Range(0, keys.Count)];
        var (userLat, userLon) = sigunCoords[randomSigun];

        string url = $"{BASE_URL}?KEY={API_KEY}&Type=json&pIndex=1&pSize=200&SIGUN_NM={UnityWebRequest.EscapeURL(randomSigun)}";
        string result = await GetWebText(url);

        if (string.IsNullOrEmpty(result) || result.TrimStart().StartsWith("<"))
        {
            Debug.LogWarning($"[초기 로딩] API 오류: {result}");
            return;
        }

        List<RepairShop> shops = ParseRepairShops(result);
        foreach (var shop in shops)
            shop.Distance = GetDistance(userLat, userLon, shop.Lat, shop.Lon);

        // 랜덤 셔플
        for (int i = shops.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shops[i], shops[j]) = (shops[j], shops[i]);
        }

        int count = Mathf.Min(20, shops.Count);
        cachedShops = shops.GetRange(0, count);
        DisplayShopList(cachedShops);
        Debug.Log($"[초기 로딩] {randomSigun} 정비업체 {count}개 랜덤 표시");
    }

    private async UniTaskVoid SearchByMyLocation()
    {
        double userLat, userLon;

#if UNITY_EDITOR
        // 에디터 테스트용 임시 좌표 (수원역 부근)
        userLat = 37.2660;
        userLon = 127.0001;
        Debug.Log($"[위치] 에디터 테스트 좌표 사용: ({userLat}, {userLon})");
#else
        // 모바일: 실제 GPS 사용
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("[위치] 위치 서비스가 비활성화되어 있습니다.");
            return;
        }

        Input.location.Start(10f, 5f);

        int timeout = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && timeout > 0)
        {
            await UniTask.Delay(1000);
            timeout--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning($"[위치] 위치 서비스 실패: {Input.location.status}");
            Input.location.Stop();
            return;
        }

        userLat = Input.location.lastData.latitude;
        userLon = Input.location.lastData.longitude;
        Debug.Log($"[위치] GPS 좌표: ({userLat}, {userLon})");
        Input.location.Stop();
#endif

        // 가장 가까운 시/군 찾기
        string closestSigun = null;
        double closestDist = double.MaxValue;
        foreach (var kv in sigunCoords)
        {
            double dist = GetDistance(userLat, userLon, kv.Value.lat, kv.Value.lon);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestSigun = kv.Key;
            }
        }

        if (closestSigun == null) return;
        Debug.Log($"[위치] 가장 가까운 지역: {closestSigun} ({closestDist:F1}km)");

        // 해당 지역 API 호출 후 내 GPS 좌표 기준 거리순 정렬
        string url = $"{BASE_URL}?KEY={API_KEY}&Type=json&pIndex=1&pSize=200&SIGUN_NM={UnityWebRequest.EscapeURL(closestSigun)}";
        string result = await GetWebText(url);

        if (string.IsNullOrEmpty(result) || result.TrimStart().StartsWith("<"))
        {
            Debug.LogError($"API 오류 응답: {result}");
            return;
        }

        List<RepairShop> shops = ParseRepairShops(result);
        foreach (var shop in shops)
            shop.Distance = GetDistance(userLat, userLon, shop.Lat, shop.Lon);

        shops.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        int count = Mathf.Min(10, shops.Count);
        cachedShops = shops.GetRange(0, count);
        DisplayShopList(cachedShops);

        inputField.text = $"{closestSigun} (내 위치)";
        Debug.Log($"[위치] {closestSigun} 근처 정비업체 {count}개 표시 완료");
    }

    private async UniTaskVoid SearchRepairShops(string sigunName)
    {
        sigunName = sigunName.Trim();

        if (!sigunCoords.ContainsKey(sigunName))
        {
            Debug.LogWarning($"'{sigunName}' 좌표 정보가 없습니다.");
            return;
        }

        var (userLat, userLon) = sigunCoords[sigunName];

        string url = $"{BASE_URL}?KEY={API_KEY}&Type=json&pIndex=1&pSize=200&SIGUN_NM={UnityWebRequest.EscapeURL(sigunName)}";
        string result = await GetWebText(url);

        if (string.IsNullOrEmpty(result) || result.TrimStart().StartsWith("<"))
        {
            Debug.LogError($"API 오류 응답: {result}");
            return;
        }

        List<RepairShop> shops = ParseRepairShops(result);

        foreach (var shop in shops)
        {
            shop.Distance = GetDistance(userLat, userLon, shop.Lat, shop.Lon);
        }
        shops.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        int count = Mathf.Min(10, shops.Count);
        cachedShops = shops.GetRange(0, count);
        DisplayShopList(cachedShops);
        Debug.Log($"{sigunName} 근처 정비업체 {count}개 표시 완료");
    }

    private void DisplayShopList(List<RepairShop> shops)
    {
        // 기존 목록 클리어 (프리팹 템플릿은 보존)
        for (int c = listContent.childCount - 1; c >= 0; c--)
        {
            var child = listContent.GetChild(c).gameObject;
            if (child == shopItemPrefab) continue;
            Destroy(child);
        }

        for (int i = 0; i < shops.Count; i++)
        {
            var shop = shops[i];
            GameObject item = Instantiate(shopItemPrefab, listContent);
            item.SetActive(true);

            // LayoutElement 확인 및 보정
            var le = item.GetComponent<LayoutElement>();
            if (le != null)
            {
                le.preferredHeight = 180;
                le.minHeight = 180;
            }

            // RectTransform 크기 보정
            RectTransform itemRT = item.GetComponent<RectTransform>();
            if (itemRT != null)
            {
                itemRT.anchorMin = new Vector2(0, 0);
                itemRT.anchorMax = new Vector2(1, 1);
                itemRT.sizeDelta = new Vector2(0, 180);
            }

            TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length >= 3)
            {
                texts[0].text = $"{i + 1}. {shop.Name}";
                texts[1].text = shop.Address;
                texts[2].text = $"{shop.Tel}  |  {shop.Distance:F1}km";
            }

            int index = i;
            Button btn = item.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => ShowDetail(cachedShops[index]));
            }
        }

        // 레이아웃 강제 갱신
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
    }

    private void ShowDetail(RepairShop shop)
    {
        detailPanel.SetActive(true);
        detailName.text = shop.Name;
        detailAddress.text = $"주소 : {shop.Address}";
        detailTel.text = $"전화 : {shop.Tel}";
        detailKind.text = $"업종 : {shop.Kind}";
        detailState.text = $"상태 : {shop.State}";
        detailDistance.text = $"거리 : {shop.Distance:F2} km";
        detailRegDate.text = $"등록일 : {shop.RegDate}";
        detailArea.text = $"면적 : {shop.Area}";
        detailTime.text = $"운영시간 : {shop.OpenTime} ~ {shop.CloseTime}";

        // 네이버 Static Map 로드
        LoadMapImage(shop.Lon, shop.Lat, shop.Name).Forget();

        // "네이버 지도에서 보기" 버튼 연결
        if (openMapButton != null)
        {
            openMapButton.onClick.RemoveAllListeners();
            openMapButton.onClick.AddListener(() =>
            {
                string mapUrl = $"https://map.naver.com/p/search/{UnityWebRequest.EscapeURL(shop.Address)}";
                Application.OpenURL(mapUrl);
            });
        }
    }

    private async UniTaskVoid LoadMapImage(double lon, double lat, string shopName)
    {
        if (mapImage == null) return;

        // 로딩 중 표시 (회색)
        mapImage.color = new Color(0.85f, 0.85f, 0.85f, 1f);

        // 네이버 Static Map API 호출
        // center=경도,위도 (lon,lat 순서!)
        // markers=type:d|size:mid|pos:경도 위도
        string url = $"{NAVER_STATIC_MAP_URL}?w=800&h=400&center={lon},{lat}&level=16" +
                     $"&markers=type:d|size:mid|pos:{lon} {lat}" +
                     $"&scale=2&format=png";

        Debug.Log($"[지도] Static Map 요청: {url}");

        var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", NAVER_MAP_CLIENT_ID);
        req.SetRequestHeader("X-NCP-APIGW-API-KEY", NAVER_MAP_CLIENT_SECRET);
        req.downloadHandler = new DownloadHandlerBuffer();

        try
        {
            // UniTask가 HTTP 에러를 예외로 던지지 않도록 직접 처리
            var op = req.SendWebRequest();
            while (!op.isDone) await UniTask.Yield();

            Debug.Log($"[지도] 응답 코드: {req.responseCode}, result: {req.result}");

            if (req.result == UnityWebRequest.Result.Success)
            {
                byte[] imageData = req.downloadHandler.data;
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(imageData))
                {
                    mapImage.texture = tex;
                    mapImage.color = Color.white;

                    // 이미지 비율에 맞춰 LayoutElement 높이 자동 조정
                    var layoutElem = mapImage.GetComponent<LayoutElement>();
                    if (layoutElem != null && tex.width > 0)
                    {
                        float parentWidth = mapImage.rectTransform.rect.width;
                        if (parentWidth <= 0)
                            parentWidth = ((RectTransform)mapImage.transform.parent).rect.width - 50f;
                        float ratio = (float)tex.height / tex.width;
                        layoutElem.preferredHeight = parentWidth * ratio;
                        layoutElem.minHeight = parentWidth * ratio;
                    }

                    Debug.Log($"[지도] '{shopName}' 지도 로드 완료 ({tex.width}x{tex.height})");
                }
                else
                {
                    Debug.LogWarning("[지도] 이미지 파싱 실패");
                }
            }
            else
            {
                string responseText = "";
                if (req.downloadHandler?.data != null)
                    responseText = System.Text.Encoding.UTF8.GetString(req.downloadHandler.data);
                Debug.LogWarning($"[지도] 실패: HTTP {req.responseCode} | {req.error}\n응답본문: {responseText}");
                mapImage.texture = null;
                mapImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[지도] 예외: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private List<RepairShop> ParseRepairShops(string json)
    {
        List<RepairShop> shops = new List<RepairShop>();
        var wrapper = JsonUtility.FromJson<ApiResponse>(FixJson(json));
        if (wrapper == null || wrapper.row == null) return shops;

        foreach (var row in wrapper.row)
        {
            if (string.IsNullOrEmpty(row.REFINE_WGS84_LAT) || string.IsNullOrEmpty(row.REFINE_WGS84_LOGT))
                continue;
            if (!string.IsNullOrEmpty(row.CLSBIZ_DE))
                continue;

            double.TryParse(row.REFINE_WGS84_LAT, out double lat);
            double.TryParse(row.REFINE_WGS84_LOGT, out double lon);

            shops.Add(new RepairShop
            {
                Name = row.FACLT_NM ?? "이름없음",
                Address = row.REFINE_ROADNM_ADDR ?? row.REFINE_LOTNO_ADDR ?? "주소없음",
                Tel = row.TELNO ?? row.MNGINST_TELNO ?? "전화없음",
                Kind = row.FACLT_KIND_NM ?? "-",
                State = row.BSN_STATE_NM == "1" ? "영업중" : row.BSN_STATE_NM ?? "-",
                RegDate = row.REGIST_DE ?? "-",
                Area = row.AR > 0 ? $"{row.AR} m²" : "-",
                OpenTime = FormatTime(row.BSN_BGNG_TM),
                CloseTime = FormatTime(row.BSN_END_TM),
                Lat = lat,
                Lon = lon
            });
        }
        return shops;
    }

    private string FixJson(string json)
    {
        int rowStart = json.IndexOf("\"row\":");
        if (rowStart < 0) return "{\"row\":[]}";
        int arrayStart = json.IndexOf('[', rowStart);
        int depth = 0;
        int arrayEnd = arrayStart;
        for (int i = arrayStart; i < json.Length; i++)
        {
            if (json[i] == '[') depth++;
            else if (json[i] == ']') depth--;
            if (depth == 0) { arrayEnd = i; break; }
        }
        string rowArray = json.Substring(arrayStart, arrayEnd - arrayStart + 1);
        return "{\"row\":" + rowArray + "}";
    }

    private string FormatTime(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "없음";
        raw = raw.Trim();
        // "0900" → "09:00", "1800" → "18:00"
        if (raw.Length == 4 && int.TryParse(raw, out _))
            return $"{raw.Substring(0, 2)}:{raw.Substring(2, 2)}";
        // "09:00" 이미 포맷된 경우
        if (raw.Contains(":")) return raw;
        return raw;
    }

    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        double dLat = (lat2 - lat1) * Math.PI / 180.0;
        double dLon = (lon2 - lon1) * Math.PI / 180.0;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                   + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
                   * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private async UniTask<string> GetWebText(string url)
    {
        var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        var request = await req.SendWebRequest();
        byte[] rawData = request.downloadHandler.data;
        string txt = System.Text.Encoding.UTF8.GetString(rawData);
        if (txt.TrimStart().StartsWith("<"))
        {
            txt = System.Text.Encoding.GetEncoding("euc-kr").GetString(rawData);
        }
        return txt;
    }
}

// === 도메인 클래스 ===
public class RepairShop
{
    public string Name;
    public string Address;
    public string Tel;
    public string Kind;     // 업체 종류
    public string State;    // 영업 상태
    public string RegDate;  // 등록일
    public string Area;     // 면적
    public string OpenTime;  // 운영시작시각
    public string CloseTime; // 운영종료시각
    public double Lat;
    public double Lon;
    public double Distance;
}

// === JSON 파싱용 클래스 ===
[Serializable]
public class ApiResponse
{
    public ApiRow[] row;
}

[Serializable]
public class ApiRow
{
    public string SIGUN_NM;
    public string FACLT_NM;
    public string FACLT_KIND_NM;
    public string BSN_STATE_NM;
    public string CLSBIZ_DE;
    public string REGIST_DE;
    public float AR;
    public string TELNO;
    public string MNGINST_TELNO;
    public string BSN_BGNG_TM;   // 운영시작시각
    public string BSN_END_TM;    // 운영종료시각
    public string REFINE_LOTNO_ADDR;
    public string REFINE_ROADNM_ADDR;
    public string REFINE_WGS84_LAT;
    public string REFINE_WGS84_LOGT;
}
