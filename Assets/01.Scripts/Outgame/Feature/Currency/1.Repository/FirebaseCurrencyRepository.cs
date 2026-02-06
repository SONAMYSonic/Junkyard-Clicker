using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseCurrencyRepository : ICurrencyRepository
{
    private const string COLLECTION_NAME = "Currencies";

    private FirebaseAuth _auth => FirebaseAuth.DefaultInstance;
    private FirebaseFirestore _db => FirebaseFirestore.DefaultInstance;

    public async UniTask Save(CurrencySaveData saveData)
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
            Debug.LogError("Firebase Currency Save Error: " + e.Message);
        }
    }

    public async UniTask<CurrencySaveData> Load()
    {
        try
        {
            string email = _auth.CurrentUser?.Email;
            if (string.IsNullOrEmpty(email)) return CurrencySaveData.Default;

            DocumentSnapshot snapshot = await _db.Collection(COLLECTION_NAME).Document(email).GetSnapshotAsync().AsUniTask();
            await UniTask.SwitchToMainThread();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<CurrencySaveData>();
            }

            return CurrencySaveData.Default;
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase Currency Load Error: " + e.Message);
            return CurrencySaveData.Default;
        }
    }
}
