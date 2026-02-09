#if !UNITY_WEBGL
using Firebase.Firestore;
#endif

#if !UNITY_WEBGL
[FirestoreData]
#endif
public class CurrencySaveData
{
#if !UNITY_WEBGL
    [FirestoreProperty]
#endif
    public double[] Currencies { get; set; }

    public static CurrencySaveData Default => new CurrencySaveData
    {
        Currencies = new double[(int)ECurrencyType.Count]
    };
}
