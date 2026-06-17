using UnityEngine;

namespace MainHub.CharacterSelect
{
    public static class MainHub_Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ReportSceneSetup()
        {
            // 장면 전체 검색은 MainHub 규칙에서 금지합니다.
            // 생성 검증은 Editor의 MainHub_SceneBuilder가 담당합니다.
            Debug.Log("MainHub_Bootstrap: scene loaded.");
        }
    }
}