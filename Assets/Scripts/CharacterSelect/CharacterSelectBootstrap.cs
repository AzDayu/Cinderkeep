using UnityEngine;
using UnityEngine.EventSystems;

namespace OODong.CharacterSelect
{
    public static class CharacterSelectBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ReportSceneSetup()
        {
            if (Object.FindFirstObjectByType<CharacterSelectCanvas>() == null)
            {
                Debug.LogWarning("CharacterSelectBootstrap: Character Select Canvas is not placed in the scene hierarchy.");
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                Debug.LogWarning("CharacterSelectBootstrap: EventSystem is not placed in the scene hierarchy.");
            }
        }
    }
}
