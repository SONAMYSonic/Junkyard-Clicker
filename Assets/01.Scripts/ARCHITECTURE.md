# Junkyard Clicker - Architecture Guide

## 아키텍처 개요

이 프로젝트는 도메인 주도 설계(DDD) 원칙을 따르며, 다음과 같은 패턴을 사용합니다:

- **Manager-Domain-Repository 패턴**: 비즈니스 로직 분리
- **MVVM 패턴**: UI와 비즈니스 로직 분리
- **Service Locator 패턴**: 의존성 관리

## 폴더 구조

```
Assets/01.Scripts/
├── Core/                          # 핵심 시스템
│   ├── ServiceLocator/            # 서비스 로케이터 패턴
│   │   ├── ServiceLocator.cs      # 서비스 등록/조회
│   │   └── ServiceInstaller.cs    # 서비스 설치
│   ├── GameEvents.cs              # 이벤트 시스템
│   ├── GameManager.cs             # 게임 매니저
│   ├── GameBootstrapper.cs        # 게임 초기화
│   └── Enums.cs                   # 공통 열거형
│
├── Ingame/Feature/                # 인게임 기능 (도메인 방식)
│   ├── Car/                       # 차량 시스템
│   │   ├── 2.Domain/
│   │   │   ├── CarState.cs        # 차량 상태 도메인
│   │   │   ├── CarPartState.cs    # 파츠 상태 도메인
│   │   │   ├── CarEntity.cs       # 차량 엔티티
│   │   │   ├── CarPartEntity.cs   # 파츠 엔티티
│   │   │   ├── CarRewardCalculator.cs  # 보상 계산
│   │   │   └── CarSpawnSelector.cs     # 스폰 선택
│   │   └── 3.Manager/
│   │       ├── ICarManager.cs     # 인터페이스
│   │       └── CarManager.cs      # 구현체
│   │
│   ├── Damage/                    # 데미지 시스템
│   │   ├── 2.Domain/
│   │   │   ├── DamageInfo.cs      # 데미지 정보 값 객체
│   │   │   ├── IDamageCalculator.cs
│   │   │   └── UpgradeBasedDamageCalculator.cs
│   │   ├── 3.Manager/
│   │   │   ├── IDamageManager.cs
│   │   │   └── DamageManager.cs
│   │   └── AutoDamageService.cs   # 자동 데미지 서비스
│   │
│   ├── Input/                     # 입력 시스템
│   │   └── InputHandler.cs
│   │
│   └── Feedback/                  # 피드백 시스템
│       ├── 2.Domain/
│       │   ├── FeedbackConfig.cs
│       │   └── FeedbackType.cs
│       └── 3.Manager/
│           ├── IFeedbackManager.cs
│           └── FeedbackManagerNew.cs
│
├── Outgame/Feature/               # 아웃게임 기능 (기존 도메인 방식)
│   ├── Currency/
│   │   ├── 1.Repository/
│   │   ├── 2.Domain/
│   │   └── 3.Manager/
│   ├── Upgrade/
│   │   ├── 1.Repository/
│   │   ├── 2.Domain/
│   │   └── 3.Manager/
│   └── Account/
│       ├── 1.Repository/
│       ├── 2.Domain/
│       └── 3.Manager/
│
├── UI/                            # UI 시스템 (MVVM 패턴)
│   ├── MVVM/                      # MVVM 기반 클래스
│   │   ├── IObservable.cs
│   │   ├── Observable.cs
│   │   ├── ViewModelBase.cs
│   │   └── ViewBase.cs
│   ├── ViewModels/                # ViewModel
│   │   ├── CurrencyViewModel.cs
│   │   ├── UpgradeViewModel.cs
│   │   └── HpBarViewModel.cs
│   └── Views/                     # View
│       ├── CurrencyView.cs
│       ├── UpgradeButtonView.cs
│       └── HpBarView.cs
│
├── Data/                          # ScriptableObject 데이터
├── Feedback/                      # 피드백 컴포넌트 (기존)
├── Car/                           # 차량 컴포넌트 (기존, 레거시)
├── Input/                         # 입력 핸들러 (기존, 레거시)
└── Resource/                      # 리소스 핸들러 (기존, 레거시)
```

## 레이어 구조

### 1. Repository Layer (1.Repository/)
- 데이터 영속성 처리
- PlayerPrefs, Firebase 등 저장소 추상화
- 예: `ICurrencyRepository`, `LocalCurrencyRepository`

### 2. Domain Layer (2.Domain/)
- 비즈니스 로직 및 규칙
- 상태 관리 (State 클래스)
- 도메인 서비스 (Calculator, Selector 등)
- 값 객체 (Value Objects)

### 3. Manager Layer (3.Manager/)
- 도메인 객체 오케스트레이션
- 외부 시스템과의 통합
- MonoBehaviour로 Unity 생명주기 관리

## 디자인 패턴

### Service Locator
```csharp
// 서비스 등록
ServiceLocator.Register<ICarManager>(carManager);

// 서비스 조회
var carManager = ServiceLocator.Get<ICarManager>();

// 안전한 조회
if (ServiceLocator.TryGet<IDamageManager>(out var damageManager))
{
    // 사용
}
```

### MVVM
```csharp
// ViewModel
public class CurrencyViewModel : ViewModelBase
{
    public Observable<string> Money { get; } = new Observable<string>("0");
}

// View
public class CurrencyView : ViewBase<CurrencyViewModel>
{
    protected override void BindViewModel()
    {
        ViewModel.Money.OnValueChanged += UpdateMoneyText;
    }
}
```

### Domain-Manager 패턴
```csharp
// Domain (상태 관리)
public class CarState
{
    public int ApplyDamage(int damage) { ... }
}

// Manager (오케스트레이션)
public class CarManager : ICarManager
{
    public void ApplyDamage(int damage)
    {
        _currentCar.State.ApplyDamage(damage);
        OnDamageDealt?.Invoke(damage);
    }
}
```

## 마이그레이션 가이드

### 기존 코드 (레거시)
- `JunkyardClicker.Car.Car` - 기존 차량 클래스
- `JunkyardClicker.Input.ClickHandler` - 기존 입력 핸들러
- `JunkyardClicker.Resource.AutoDamageHandler` - 기존 자동 데미지

### 새로운 코드 (권장)
- `JunkyardClicker.Ingame.Car.CarEntity` - 새 차량 엔티티
- `JunkyardClicker.Ingame.Input.InputHandler` - 새 입력 핸들러
- `JunkyardClicker.Ingame.Damage.AutoDamageService` - 새 자동 데미지

### 호환성
- 기존 `GameEvents`는 레거시 코드와 새 코드 모두에서 사용 가능
- ServiceLocator를 통해 새로운 매니저에 접근 권장
- 점진적으로 새 구조로 마이그레이션 가능

## 확장 가이드

### 새 기능 추가 시
1. `Ingame/Feature/{FeatureName}/` 폴더 생성
2. `2.Domain/` 폴더에 도메인 로직 구현
3. `3.Manager/` 폴더에 인터페이스 및 매니저 구현
4. 필요시 `1.Repository/` 폴더에 저장소 구현
5. ServiceLocator에 등록

### UI 추가 시
1. `UI/ViewModels/` 폴더에 ViewModel 생성
2. `UI/Views/` 폴더에 View 생성
3. ViewBase<TViewModel> 상속하여 바인딩 구현
