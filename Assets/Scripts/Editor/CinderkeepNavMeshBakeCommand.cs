// 5.00 direction: Provides editor-only setup or validation tooling for the 5.00 production workflow.
// 5.01+ note: Keep this out of runtime builds and use it to speed scene wiring, QA checks, and team handoff.
// Unity 배치모드에서 NavMesh Bake 도구를 호출하기 위한 얇은 진입점입니다.
// 실제 Bake 로직은 CinderkeepNavMeshBakeTool이 담당합니다.
public static class CinderkeepNavMeshBakeCommand
{
    public static void Bake()
    {
        Cinderkeep.EditorTools.CinderkeepNavMeshBakeTool.BakeMapNavMeshesFromCommandLine();
    }
}
