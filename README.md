# Neo-Slasher

> 모바일 뱀서라이크 게임 Neo-Slasher를 리팩터링한 저장소

Unity 6000.3.9f1 · C# · Android

낮에 장비와 특성을 갖추고 밤에 몰려오는 적을 버티는 7일 생존 게임입니다. 기존 프로젝트를 들여와 씬별 매니저와 아이템 코드를 정리하고, 모바일 출시 준비(세이브 무결성, 앱 수명 주기, 오브젝트 풀)를 더했습니다.

## 기능

- 7일 차까지 이어지는 낮·밤 사이클과 이어하기
- 7일 안에 난이도의 목표 금액을 모으면 좋은 엔딩과 경험치 보상, 레벨이 오르면 특성 포인트
- 준비: 특성 고르기, 난이도 선택(권장 레벨, 목표 금액, 적 강화 확률)
- 낮: 상점, 장비 비교와 장착, 현상금과 드롭 등급이 다른 암살 단계 고르기
- 밤: 제한 시간 동안 일반·정예 적을 상대하는 자동 전투, 아이템 드롭
- 터치 조이스틱 이동
- 앱이 백그라운드로 가면 자동 저장

## 시작하기

Unity 6000.3.9f1과 Android Build Support 모듈이 필요합니다.

```powershell
git clone https://github.com/Neo-Slasher/Neo-Slasher_Refactoring.git
```

Unity Hub에서 저장소 폴더를 프로젝트로 추가해 열고, `Assets/Scenes/Main/MainScene.unity`에서 Play를 누릅니다.

## 문서

| 문서 | 내용 |
|---|---|
| [ARCHITECTURE.md](ARCHITECTURE.md) | 씬 흐름과 스크립트 구성 |

## 기여

빌드 명령과 작업 규칙은 [CONTRIBUTING.md](CONTRIBUTING.md)에 있습니다.
