# Cinderkeep

Cinderkeep Unity 팀 프로젝트 저장소입니다.

이 저장소는 9명 팀원이 함께 쓰는 공간이므로, Git 명령어를 많이 아는 것보다
브랜치 규칙, Pull Request 검토, Unity 씬 충돌 방지가 더 중요합니다.

## 현재 저장소 기준

이전 테스트용 작업과 오래된 역할 브랜치는 `archive/` 아래에 보관합니다.

`archive/`는 폐기 또는 보관용 공간입니다. 새 작업은 `archive/*`에서 시작하지 않습니다.

앞으로 새 작업은 아래 흐름을 기준으로 진행합니다.

```text
feature/*
  -> develop
  -> main
```

- `main`: 발표, 제출, 팀 공식 안정 버전입니다.
- `develop`: 팀원 작업을 먼저 합쳐서 테스트하는 통합 브랜치입니다.
- `feature/*`: 팀원 개인 기능 작업 브랜치입니다.
- `fix/*`: 버그 수정 브랜치입니다.
- `docs/*`: 문서 정리 브랜치입니다.
- `release-v1`, `release-v2`: 발표 전 백업용 릴리즈 브랜치입니다.

## 처음 한 번만 할 일

`develop` 브랜치가 GitHub에 아직 없다면, 팀장이 먼저 최신 `main` 기준으로 `develop`을 만들어 push합니다.

```bash
git fetch origin
git switch main
git pull --ff-only origin main
git switch -c develop
git push -u origin develop
```

이후 팀원은 `main`이 아니라 `develop`에서 자기 `feature/*` 브랜치를 만듭니다.

GitHub 설정이 가능하면 `main`은 보호 규칙을 걸어 직접 push를 막고, Pull Request를 통해서만 합치도록 운영합니다.

## 가장 중요한 팀 규칙

팀원은 `main`에 직접 push하지 않습니다.

팀원 작업 흐름은 아래 순서입니다.

```text
1. Fetch
2. develop 최신화
3. develop에서 내 feature 브랜치 만들기
4. 내 브랜치에서 작업
5. Commit
6. Push
7. Pull Request를 develop으로 생성
8. 팀장이 확인 후 Merge
```

팀장은 `develop`에 모인 작업을 Unity에서 실행해 본 뒤, 문제가 없을 때만 `main`에 합칩니다.

## Fork에서 팀원이 매일 하는 순서

작업 시작 전:

```text
1. Fetch를 누른다.
2. develop 브랜치로 이동한다.
3. Pull을 누른다.
4. 최신 develop에서 내 브랜치를 만든다.
5. 내 브랜치에서 작업한다.
```

작업 종료 후:

```text
1. Local Changes에서 변경 파일을 확인한다.
2. 내가 수정한 파일만 Stage한다.
3. Commit한다.
4. Push한다.
5. GitHub에서 Pull Request를 만든다.
6. 팀장에게 브랜치 이름과 작업 내용을 보낸다.
```

## 브랜치 이름 규칙

팀원 작업 브랜치는 가능하면 기능 단위로 만듭니다.

```text
feature/main-lobby-ui
feature/player-movement
feature/battle-prototype
feature/inventory-ui
fix/camera-error
docs/team-git-guide
```

초보자는 아래처럼 자기 역할과 작업명을 같이 써도 됩니다.

```text
feature/bard-hub-ui
feature/tanker-defense-ui
feature/mage-skill-effect
feature/archer-attack-test
```

## Pull Request 규칙

Pull Request는 팀장이 확인하고 합치기 위한 요청입니다.

팀원은 작업이 끝나면 바로 `main`에 합치지 말고, 아래 정보를 적어서 PR을 만듭니다.

```text
작업자:
브랜치:
작업 내용:
확인할 씬:
테스트 방법:
주의 사항:
```

예시:

```text
작업자: 김성광
브랜치: feature/tanker-defense-ui
작업 내용: 탱커 개인 씬에 방어 UI 초안 추가
확인할 씬: Tanker_KimSeonggwang
테스트 방법: Unity 실행 후 탱커 씬 열어서 UI 위치 확인
주의 사항: 아직 실제 체력 시스템과 연결하지 않음
```

## 팀장 Merge 체크리스트

팀장은 팀원 PR을 바로 합치지 않습니다.

Merge 전 반드시 아래 순서로 확인합니다.

```text
1. Fetch
2. PR 브랜치 확인
3. Unity 실행
4. Console 에러 확인
5. 관련 씬 열기
6. 게임 플레이 또는 기능 테스트
7. 충돌 여부 확인
8. 문제가 없으면 develop에 Merge
9. develop 전체 테스트 후 main에 Merge
```

팀원이 "됐어요"라고 말해도, Unity 실행과 Console 확인 전에는 Merge하지 않습니다.

## Unity 씬 작업 규칙

Unity 프로젝트에서 가장 위험한 충돌은 씬 파일 충돌입니다.

특히 아래 파일은 여러 명이 동시에 수정하지 않습니다.

```text
Assets/Scenes/Main_Lobby.unity
Assets/Scenes/MainGame/Cinderkeep_Game.unity
Assets/Scenes/MainWorkspace/Cinderkeep_Workspace.unity
Assets/Scenes/MainWorkspace/MainWorkspaceRoom_ForBuild.unity
```

개인 작업은 먼저 자기 캐릭터 씬에서 진행합니다.

```text
Bard_CheonWooyoung
Tanker_KimSeonggwang
Mage_JeongDongwon
Archer_KwonSeonghyeok
Healer_KangHeewon
Assassin_KimDonghyuk
Blacksmith_ChoiJaeho
Spellblade_KimMinseok
Summoner_JiJaewook
```

공용 씬을 수정해야 할 때는 팀장에게 먼저 말하고, 누가 언제 수정하는지 정한 뒤 작업합니다.

## 커밋 제목 규칙

커밋 제목은 누가 봐도 무엇을 바꿨는지 알 수 있어야 합니다.

좋은 예시:

```text
Feat: 인벤토리 UI 초안 추가
Feat: 대사 시스템 기본 구조 구현
Fix: 카메라 전환 오류 수정
Refactor: UI 매니저 구조 정리
Docs: 팀원 Fork 사용법 추가
Chore: 유료 에셋 제외 규칙 정리
```

나쁜 예시:

```text
수정
최종
진짜최종
test
asdf
```

## Git Ignore와 유료 에셋 규칙

Unity에서 아래 폴더는 Git에 올리지 않습니다.

```text
Library/
Temp/
Obj/
Logs/
Build/
Builds/
```

유료 에셋은 Public GitHub 저장소에 올리면 안 됩니다.

유료 에셋은 반드시 아래 폴더에만 보관합니다.

```text
Assets/ThirdParty_PriAsset/
```

이 폴더는 Git에 올라가지 않도록 ignore 처리합니다.

팀원에게 유료 에셋을 공유해야 할 때는 GitHub가 아니라 Google Drive, Unity Package, 직접 파일 전달을 사용합니다.

## Fork에서 매일 확인할 것

Fork에서는 아래 세 가지를 자주 봅니다.

```text
History: 누가 언제 무엇을 커밋했는지 확인
Branches: 살아있는 브랜치와 오래된 브랜치 확인
Pull Requests: 팀원이 올린 작업 확인
```

오래된 테스트 브랜치나 폐기된 작업은 `archive/` 아래에 보관합니다.

## 발표 전 백업 규칙

발표 전에는 안정적인 상태에서 릴리즈 브랜치를 만듭니다.

```text
release-v1
release-v2
```

릴리즈 브랜치가 있으면 이후 작업 중 문제가 생겨도 발표 가능한 버전으로 돌아갈 수 있습니다.

## 팀원이 지금 당장 익힐 것

지금 단계에서는 고급 Git 기능보다 아래 항목이 중요합니다.

```text
1. Branch 생성
2. Pull Request 생성
3. Merge 흐름 이해
4. Conflict가 났을 때 멈추고 팀장에게 말하기
5. Fork History 보기
6. Unity Scene 충돌 피하기
7. Git Ignore 확인
8. Release 백업 브랜치 이해
```

`rebase`, `cherry-pick`, `submodule` 같은 고급 기능은 지금 당장 몰라도 됩니다.

## 추가 팀 문서

팀원용 자세한 Fork 사용법은 아래 문서에 정리합니다.

```text
Readme/팀원들_포크_이용방법.txt
Readme/팀원들_포크_이용방법.md
Assets/_TeamGuide/
```
