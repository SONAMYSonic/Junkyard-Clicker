using System;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class Dog
{
    [FirestoreDocumentId]           // 문서의 고유 식별자가 자동으로 맵핑된다.
    public string Id {get; set;}
    
    [FirestoreProperty]
    public string Name {get; set;}  // 필드가 아니라, get / set 프로퍼티로 만들어야 한다.
    
    [FirestoreProperty]
    public int Age {get; set;}
    
    public Dog(){}                  // 기본 생성자가 무조건 있어야한다. -> 무결성이 깨진다.
    // 그러나 보통 Dog가 아니라, DogSaveData 등 저장 전용 Data 클래스는 무결성 신경 안쓴다.

    public Dog(string name, int age)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new System.ArgumentNullException("이름 비어있기 금지금지~");
        }
        
        if (age < 0)
        {
            throw new System.ArgumentOutOfRangeException("나이는 음수 금지금지~");
        }
        
        Name = name;
        Age = age;
    }
}
