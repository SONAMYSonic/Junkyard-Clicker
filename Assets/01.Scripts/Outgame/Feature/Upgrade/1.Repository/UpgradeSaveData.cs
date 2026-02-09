#if !UNITY_WEBGL
using Firebase.Firestore;
#endif

#if !UNITY_WEBGL
[FirestoreData]
#endif
public class UpgradeSaveData
{
#if !UNITY_WEBGL
    [FirestoreProperty]
#endif
    public int[] Levels { get; set; }

    public static UpgradeSaveData Default => new UpgradeSaveData
    {
        Levels = new int[(int)EUpgradeType.Count]
    };
}
