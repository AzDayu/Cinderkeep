Cinderkeep Player Scripts

이 폴더는 공동 게임 본편의 Player 관련 스크립트를 두는 위치입니다.
플레이어 담당자는 개인 브랜치에서 먼저 최소 이동 스크립트를 이 폴더에 올립니다.

우선 작업할 파일 예시
- PlayerMovement.cs
- PlayerView.cs
- PlayerStatus.cs

현재 씬 연결 기준
- 씬: Assets/Scenes/MainGame/Cinderkeep_Game.unity
- 오브젝트: Player
- 카메라: Player/Transform_CameraRoot_FirstPerson/Camera_FirstPerson
- 이동 스크립트는 Player에 직접 붙입니다.
- 처음 커밋은 PlayerMovement.cs 하나만 있어도 됩니다.

작업 규칙
- PlayerMovement는 이동만 담당합니다.
- PlayerView는 카메라 회전만 담당합니다.
- PlayerStatus는 체력, 스태미나 같은 상태만 담당합니다.
- HUD는 직접 만지지 않고 나중에 UI 쪽과 연결합니다.
