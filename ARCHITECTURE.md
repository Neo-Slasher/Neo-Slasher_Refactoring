# 구조

## 씬 흐름

Build Settings 순서는 MainScene, CutScene, PreparationScene, DayScene, NightScene입니다. 씬은 `Assets/Scenes/<이름>/`에 있습니다.

```
MainScene -> CutScene -> PreparationScene -> DayScene -> NightScene
```

- 새 게임은 CutScene, 이어하기는 DayScene으로 갑니다.
- NightScene이 끝나면 7일 차 전에는 날짜를 올려 저장하고 DayScene으로, 7일 차가 끝나면 엔딩을 판정하고 CutScene으로 갑니다.

| 씬 | 매니저 | 하는 일 |
|---|---|---|
| Main | MainManager | 새 게임, 이어하기, 설정 |
| CutScene | CutSceneManager | 난이도와 엔딩에 따른 스토리 |
| Prepare | PreparationManager, PreparationTraitManager | 특성 고르기, 난이도 선택 |
| Day | DayManager, ShopManager, EquipmentManager, AssassinationManager | 상점, 장비, 암살 단계 고르기 |
| Night | NightManager, TimerManager, ItemManager | 적 생성, 제한 시간 전투, 보상과 드롭 |

## 스크립트

| 경로 | 내용 |
|---|---|
| `Assets/Scripts/Managers/` | 씬을 넘어 사는 매니저(GameManager, DataManager, SoundManager, PoolManager, JoystickManager) |
| `Assets/Scripts/Scene/` | 씬별 매니저. 하위 폴더가 씬 이름 |
| `Assets/Scripts/Character/` | 플레이어 캐릭터. 이동, 공격, 체력, 애니메이션, 특성을 컴포넌트로 나눔 |
| `Assets/Scripts/Enemy/` | Enemy 기반 클래스와 NormalEnemy, EliteEnemy |
| `Assets/Scripts/Item/` | BaseItem을 상속한 아이템별 클래스 |
| `Assets/Scripts/Trait/` | 특성 효과 |
| `Assets/Scripts/Projectile/` | 투사체 |
| `Assets/Scripts/Data/` | 플레이어, 난이도, 아이템, 몬스터, 설정 같은 데이터 클래스 |
| `Assets/Scripts/Utility/` | 오브젝트 풀(GameObjectPool), 세이브 파일(SaveFile), 히트박스, 터치 입력, Logger |
| `Assets/Scripts/UI/` | 체력바, 설정 창 |
| `Assets/Editor/` | 명령줄 빌드(BuildScript), 씬 정리(MissingScriptCleaner) |
| `Assets/Resources/` | 런타임에 읽는 JSON 데이터와 아이콘 |

## 지켜야 할 규칙

- 적과 일부 아이템 오브젝트는 PoolManager로 꺼내고 돌려놓습니다. 풀 대상은 IPoolable을 구현합니다. PoolManager는 씬마다 새로 만들어집니다.
- 세이브는 SaveFile을 거쳐 씁니다. 체크섬과 `.bak` 백업, 임시 파일 교체로 저장합니다.
