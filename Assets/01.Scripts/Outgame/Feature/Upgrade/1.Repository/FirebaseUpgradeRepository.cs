using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseUpgradeRepository : IUpgradeRepository
{
    private const string COLLECTION_NAME = "Upgrades";

    private FirebaseAuth _auth = FirebaseAuth.DefaultInstance;
    private FirebaseFirestore _db = FirebaseFirestore.DefaultInstance;

    public async UniTaskVoid Save(UpgradeSaveData saveData)
    {
        try
        {
            string email = _auth.CurrentUser?.Email;
            if (string.IsNullOrEmpty(email)) return;

            await _db.Collection(COLLECTION_NAME).Document(email).SetAsync(saveData).AsUniTask();
            await UniTask.SwitchToMainThread();
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase Upgrade Save Error: " + e.Message);
        }
    }

    public async UniTask<UpgradeSaveData> Load()
    {
        try
        {
            string email = _auth.CurrentUser?.Email;
            if (string.IsNullOrEmpty(email)) return UpgradeSaveData.Default;

            DocumentSnapshot snapshot = await _db.Collection(COLLECTION_NAME).Document(email).GetSnapshotAsync().AsUniTask();
            await UniTask.SwitchToMainThread();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<UpgradeSaveData>();
            }

            return UpgradeSaveData.Default;
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase Upgrade Load Error: " + e.Message);
            return UpgradeSaveData.Default;
        }
    }
}
