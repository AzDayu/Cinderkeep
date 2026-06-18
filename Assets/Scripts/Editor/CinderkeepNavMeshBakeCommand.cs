// Unity 배치모드에서 NavMesh Bake 도구를 호출하기 위한 얇은 진입점입니다.
// 실제 Bake 로직은 CinderkeepNavMeshBakeTool이 담당합니다.
public static class CinderkeepNavMeshBakeCommand
{
    public static void Bake()
    {
        Cinderkeep.EditorTools.CinderkeepNavMeshBakeTool.BakeMapNavMeshesFromCommandLine();
    }
}
