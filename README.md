# 3Match

<aside>

> 📌 같은 색의 과일을 3개 이상 맞춰 없애는 모바일 3매치 퍼즐 게임입니다. 제한된 이동 횟수 안에 최대한 높은 점수를 얻는 것이 목표입니다.

</aside>

<p align="left">
  <img src="Image/스크린샷_1.webp" alt="3Match 이미지1" width="250"/>
  <img src="Image/스크린샷_2.webp" alt="3Match 이미지2" width="250"/>
  <img src="Image/스크린샷_3.webp" alt="3Match 이미지3" width="250"/>
</p>

🔗 [유튜브](https://www.youtube.com/watch?v=BotbQtM0h7g&t=1s)  
🔗 [문서](https://abaft-yarn-52e.notion.site/KukuPang-247c32f2552880ddab12c5a2c0570950?pvs=74)  

| 항목 | 내용 |
| --- | --- |
| 🎮 게임 이름 | **3Match** |
| 🕹 장르 | 퍼즐, 3매치 |
| 🛠 사용 기술 | Unity 6, C#, DOTween, Addressables, UI Toolkit, Google AdMob |
| 👤 역할 | 개인 프로젝트 |
| 📅 개발 기간 | 2025.07.18 ~ 2025.08.06 |
| 👥 개발 인원 | 개발 1명 |

## ✅ 수행한 역할

### 🔹 시스템 개발
- 보드 생성, 스왑, 매치 판정, 중력, 리필로 이어지는 **3매치 코어 루프** 전체 설계 및 구현.
- `MatchChecker`를 통해 가로·세로·2x2 사각형 매치를 판정하고, 가능한 수를 탐색하는 **힌트 시스템** 구현.
- 중복 없는 큐 자료구조 `UniqueQueue<T>`를 직접 만들어 매치 대상 타일을 관리.
- `AssetManager`로 Addressables 에셋을 참조 카운트 기반으로 로드/해제하도록 설계.

### 🔹 콘텐츠 개발
- 4개 이상 매치 시 특수 아이템(가로/세로 줄 제거)이 생성되고, 사용 시 이펙트와 함께 한 줄을 제거하는 아이템 시스템 구현.
- 매치 가능한 수가 없을 때 보드를 다시 섞는 **ReRoll** 기능 구현.
- 이동 횟수, 점수, 최고 점수 저장 및 이동 횟수 소진 시 광고 팝업(Google AdMob) 연동.
- DOTween을 활용한 스왑, 제거, 팝업 연출과 Shader Graph 기반의 Shine 셰이더 제작.

### 🔹 기타 시스템
- IMGUI와 UI Toolkit을 이용해 **레벨 에디터 툴 2종** 직접 제작.
- PC(마우스)와 Android(터치) 입력을 모두 지원하도록 입력 처리 분리.

---
## 🔹 주요 시스템 구성

### 보드 및 매치 시스템
- `Board.cs`에서 입력 → 스왑 → 매치 판정 → 제거 → 중력 → 리필 → 연쇄 매치 확인까지의 **흐름을 중앙에서 제어**하도록 구성했습니다.
- 타일 생성은 `TileController`, 아이템 생성/리필은 `ItemController`, 입력은 `BoardInput`으로 **역할을 분리**해 각 기능을 독립적으로 수정할 수 있게 만들었습니다.
- `MatchChecker.cs`는 기준 타일을 중심으로 좌/중/우, 상/중/하 3칸과 네 방향 2x2 사각형을 검사하며, 매치가 없으면 **스왑을 자동으로 되돌리도록** 처리했습니다.
- 스왑 시뮬레이션으로 매치 가능한 수를 찾아, 일정 시간 동안 매치가 없으면 해당 과일을 깜빡여 **힌트를 표시**합니다.

### 아이템 시스템
- `ItemHandler.cs`에서 매치된 타일의 모양을 분석해 가로·세로·사각형·5개 매치를 구분하고, 플레이어가 움직인 타일 위치에 **특수 아이템을 생성**하도록 구현했습니다.
- 아이템이 매치에 포함되면 해당 행 또는 열 전체를 제거 대상에 추가하고 `ItemEffect`로 방향성 있는 이펙트를 출력합니다.

### 레벨 에디터
- `LevelData`(ScriptableObject)에 보드 크기와 칸별 색상 데이터를 저장하고, `LevelDataContainer`에서 레벨을 무작위로 선택해 불러옵니다.
- `LevelEditorWindow.cs`(IMGUI)는 그리드에서 칸을 직접 클릭해 색을 칠하는 방식으로 레벨을 편집합니다.
- `TextLevelEditor.cs`(UI Toolkit)는 `R`, `O`, `Y`, `G`, `B` 같은 **문자로 레벨을 작성하고 에셋과 양방향으로 변환**할 수 있어 빠른 레벨 제작이 가능합니다.
- 커스텀 인스펙터에서 바로 에디터 창을 열 수 있도록 연결해 작업 흐름을 단축했습니다.

### 에셋 관리 (Addressables)
- `AssetManager.cs`에서 `AssetReference`의 GUID를 키로 로드된 에셋을 캐싱하고, **참조 카운트가 0이 되면 해제**하도록 설계해 중복 로드와 메모리 누수를 방지했습니다.
- 타일·아이템 프리팹, 레벨 데이터, 사운드 등을 Addressables로 관리하도록 리팩터링했습니다.

### UI 및 사운드
- `MonoSingleton<T>`과 `[DontDestroyOnLoad]` 커스텀 어트리뷰트로 매니저 클래스를 일관되게 관리했습니다.
- `BasePopup`을 상속받는 팝업들을 `PopupManager`에서 `PopupType`으로 통합 관리하며, 배경과 내부 요소가 순차적으로 페이드되도록 연출했습니다.
- `Counter` 클래스의 값 변경 이벤트로 이동 횟수와 점수 UI가 자동으로 갱신되도록 구현했습니다.
- `AudioSO` 기반으로 효과음과 BGM 데이터를 관리하고, BGM은 페이드 인으로 재생합니다.
