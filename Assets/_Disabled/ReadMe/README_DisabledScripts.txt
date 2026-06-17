Cinderkeep Disabled Scripts Guide
=================================

Purpose
-------
This folder stores scripts that are intentionally disabled during branch merge work.
Unity does not compile files ending with .cs.disabled.

Current Use
-----------
- Assets/_Disabled/Scripts/Cinderkeep contains the previous !! Play Our Game gameplay scripts.
- Main_Lobby, character select, workspace, and hub BGM scripts are still active.

2026-06-17 추가 이동
--------------------
- MainHub_Bootstrap.cs.disabled
  씬이 로드될 때마다 로그만 남기던 스크립트입니다. 실제 기능 연결이 없어 비활성화했습니다.
- MainHub_RebuildRequestRunner.cs.disabled
  Assets/_Recovery/RebuildMainLobby.request 파일을 감지해 자동으로 허브를 재생성하던 복구용 스크립트입니다.
  현재는 수동 메뉴/배치 검증으로 관리하므로 자동 실행 충돌을 막기 위해 비활성화했습니다.

유지한 스크립트
---------------
- MainHub_PlayModeStartScene.cs
  프로젝트를 열 때 Main_Lobby가 먼저 보이게 하는 스크립트라서 유지합니다.
  단, MainLobbyGroup 밖에 임시 카메라를 만드는 코드는 제거했습니다.
- GameDataTester.cs
  Enemy JSON 연결 확인용 테스트 컴포넌트라서 아직 유지합니다.
- Cinderkeep_TestMapSceneBuilder.cs
  Cinderkeep_Game 테스트맵과 Player 작업 흐름에 필요할 수 있어 아직 유지합니다.

Move Rule
---------
- Move the .cs file and its .meta file together.
- Rename ScriptName.cs to ScriptName.cs.disabled.
- Rename ScriptName.cs.meta to ScriptName.cs.disabled.meta.

Restore Rule
------------
- Rename ScriptName.cs.disabled back to ScriptName.cs.
- Rename ScriptName.cs.disabled.meta back to ScriptName.cs.meta.
- Move both files back to the active Scripts folder.
- Open Unity and verify compile errors before merging.

Important
---------
Do not delete scripts from this folder until the team decides whether to merge, rewrite, or remove them permanently.
