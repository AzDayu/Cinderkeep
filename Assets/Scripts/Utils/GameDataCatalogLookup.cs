using System.Collections.Generic;

namespace Cinderkeep.Gameplay
{
    // GameDataManager의 반복 조회 코드를 줄이는 공용 helper입니다.
    // 각 Catalog는 GameDataManager가 소유하고, ID 기반 null-safe 조회만 이 클래스가 맡습니다.
    public static class GameDataCatalogLookup
    {
        public static TData GetById<TData>(IReadOnlyDictionary<string, TData> source, string id)
            where TData : GameDataBase
        {
            if (source == null || string.IsNullOrEmpty(id))
            {
                return null;
            }

            TData data;
            if (source.TryGetValue(id, out data) == false)
            {
                return null;
            }

            return data;
        }
    }
}
