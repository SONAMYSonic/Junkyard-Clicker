using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CarRepairShopFinder : MonoBehaviour
{
    // 경기도 자동차정비업체 현황 API
    private const string API_KEY = "602f9b6fbe2b4eb49bdb587bea6caab6";
    private const string BASE_URL = "https://openapi.gg.go.kr/Carimprventrpsinfo";

    [SerializeField] private TMP_InputField inputField; // 시/군 이름 입력용

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

    private async void Start()
    {
        // InputField에 엔터 입력 시 검색 실행
        inputField.onEndEdit.AddListener((text) =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SearchRepairShops(text).Forget();
            }
        });
    }

    /// <summary>
    /// 입력받은 시/군 이름으로 근처 정비업체 상위 10개 검색
    /// </summary>
    private async UniTaskVoid SearchRepairShops(string sigunName)
    {
        sigunName = sigunName.Trim();

        // 1. 입력한 시/군의 좌표 가져오기
        if (!sigunCoords.ContainsKey(sigunName))
        {
            Debug.LogWarning($"'{sigunName}' 좌표 정보가 없습니다. 등록된 시/군 이름을 입력해주세요.");
            return;
        }

        var (userLat, userLon) = sigunCoords[sigunName];
        Debug.Log($"검색 기준 위치: {sigunName} (위도: {userLat}, 경도: {userLon})");

        // 2. API에서 해당 시/군의 정비업체 목록 가져오기 (최대 200개)
        string url = $"{BASE_URL}?KEY={API_KEY}&Type=json&pIndex=1&pSize=200&SIGUN_NM={UnityWebRequest.EscapeURL(sigunName)}";
        string result = await GetWebText(url);

        if (string.IsNullOrEmpty(result))
        {
            Debug.LogError("API 응답이 비어있습니다.");
            return;
        }

        // 3. JSON 파싱 → RepairShop 리스트 생성
        List<RepairShop> shops = ParseRepairShops(result);
        Debug.Log($"총 {shops.Count}개 정비업체를 가져왔습니다.");

        // 4. 거리 계산 후 가까운 순으로 정렬
        foreach (var shop in shops)
        {
            shop.Distance = GetDistance(userLat, userLon, shop.Lat, shop.Lon);
        }
        shops.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // 5. 상위 10개 출력
        int count = Mathf.Min(10, shops.Count);
        Debug.Log($"\n===== {sigunName} 근처 정비업체 TOP {count} =====");
        for (int i = 0; i < count; i++)
        {
            var shop = shops[i];
            Debug.Log($"{i + 1}. {shop.Name} | {shop.Distance:F1}km | {shop.Address} | {shop.Tel}");
        }
    }

    /// <summary>
    /// API JSON 응답을 파싱하여 RepairShop 리스트로 변환
    /// </summary>
    private List<RepairShop> ParseRepairShops(string json)
    {
        List<RepairShop> shops = new List<RepairShop>();

        // Unity JsonUtility는 최상위 배열/중첩 구조 파싱이 어려우므로 간단한 수동 파싱
        // "row" 배열에서 각 업체 정보 추출
        var wrapper = JsonUtility.FromJson<ApiResponse>(FixJson(json));

        if (wrapper == null || wrapper.row == null) return shops;

        foreach (var row in wrapper.row)
        {
            // 좌표가 없는 업체는 거리 계산 불가 → 건너뛰기
            if (string.IsNullOrEmpty(row.REFINE_WGS84_LAT) || string.IsNullOrEmpty(row.REFINE_WGS84_LOGT))
                continue;

            // 폐업한 업체 건너뛰기 (BSN_STATE_NM이 "1"이면 영업중)
            if (!string.IsNullOrEmpty(row.CLSBIZ_DE))
                continue;

            double.TryParse(row.REFINE_WGS84_LAT, out double lat);
            double.TryParse(row.REFINE_WGS84_LOGT, out double lon);

            shops.Add(new RepairShop
            {
                Name = row.FACLT_NM ?? "이름없음",
                Address = row.REFINE_ROADNM_ADDR ?? row.REFINE_LOTNO_ADDR ?? "주소없음",
                Tel = row.MNGINST_TELNO ?? "전화없음",
                Lat = lat,
                Lon = lon
            });
        }

        return shops;
    }

    /// <summary>
    /// API 응답 JSON에서 row 배열만 추출하여 JsonUtility가 파싱 가능하도록 변환
    /// </summary>
    private string FixJson(string json)
    {
        // 원본 구조: {"Carimprventrpsinfo":[{"head":[...]},{"row":[...]}]}
        // JsonUtility용 구조: {"row":[...]}
        int rowStart = json.IndexOf("\"row\":");
        if (rowStart < 0) return "{\"row\":[]}";

        int arrayStart = json.IndexOf('[', rowStart);
        int depth = 0;
        int arrayEnd = arrayStart;

        for (int i = arrayStart; i < json.Length; i++)
        {
            if (json[i] == '[') depth++;
            else if (json[i] == ']') depth--;

            if (depth == 0)
            {
                arrayEnd = i;
                break;
            }
        }

        string rowArray = json.Substring(arrayStart, arrayEnd - arrayStart + 1);
        return "{\"row\":" + rowArray + "}";
    }

    /// <summary>
    /// 두 좌표 사이의 거리를 km 단위로 계산 (Haversine 공식)
    /// </summary>
    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // 지구 반지름 (km)
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
        var txt = (await UnityWebRequest.Get(url).SendWebRequest()).downloadHandler.text;
        return txt;
    }
}

// === 도메인 클래스 ===

/// <summary>
/// 정비업체 정보 (거리 포함)
/// </summary>
public class RepairShop
{
    public string Name;
    public string Address;
    public string Tel;
    public double Lat;
    public double Lon;
    public double Distance; // km 단위
}

// === JSON 파싱용 클래스 ===

[Serializable]
public class ApiResponse
{
    public List<ApiRow> row;
}

[Serializable]
public class ApiRow
{
    public string SIGUN_NM;
    public string FACLT_NM;
    public string BSN_STATE_NM;
    public string CLSBIZ_DE;
    public string MNGINST_TELNO;
    public string REFINE_LOTNO_ADDR;
    public string REFINE_ROADNM_ADDR;
    public string REFINE_WGS84_LAT;
    public string REFINE_WGS84_LOGT;
}
