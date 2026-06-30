using System;
using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 정식 프리팹 연결이 비어 있을 때 런타임 대체 건축물을 만듭니다.
    // BuildingManager는 설치 흐름만 관리하고, 대체 외형 규칙은 이 팩토리에 맡깁니다.
    public static class BuildingFallbackFactory
    {
        public static GameObject Create(BuildingData buildingData, Vector3 buildPosition, Quaternion buildRotation)
        {
            if (buildingData == null)
            {
                Debug.LogWarning("BuildingFallbackFactory: 건축 데이터가 없어 임시 건축물을 만들 수 없습니다.");
                return null;
            }

            GameObject createdBuilding = GameObject.CreatePrimitive(ResolvePrimitiveType(buildingData));
            createdBuilding.name = "RuntimeFallback_" + buildingData.Id;
            createdBuilding.transform.position = buildPosition;
            createdBuilding.transform.rotation = buildRotation;
            createdBuilding.transform.localScale = ResolveScale(buildingData);
            ApplyColor(createdBuilding, buildingData);

            Debug.LogWarning("BuildingFallbackFactory: 프리팹 연결이 없어 임시 건축물을 생성했습니다. building=" + buildingData.Id);
            return createdBuilding;
        }

        private static PrimitiveType ResolvePrimitiveType(BuildingData buildingData)
        {
            if (IsType(buildingData, "Tower"))
            {
                return PrimitiveType.Cylinder;
            }

            return PrimitiveType.Cube;
        }

        private static Vector3 ResolveScale(BuildingData buildingData)
        {
            if (buildingData == null)
            {
                return Vector3.one;
            }

            if (IsType(buildingData, "Tower"))
            {
                return new Vector3(1.25f, 2.8f, 1.25f);
            }

            if (IsType(buildingData, "Trap"))
            {
                return new Vector3(2.1f, 0.18f, 2.1f);
            }

            if (IsType(buildingData, "Station"))
            {
                return new Vector3(1.6f, 1.2f, 1.6f);
            }

            return new Vector3(2.6f, 1.8f, 0.45f);
        }

        private static void ApplyColor(GameObject createdBuilding, BuildingData buildingData)
        {
            Renderer targetRenderer = createdBuilding == null ? null : createdBuilding.GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                return;
            }

            Color materialKeyColor;
            if (buildingData != null
                && GameDataMaterialColorResolver.TryResolveColor(buildingData.MaterialKey, out materialKeyColor))
            {
                targetRenderer.material.color = materialKeyColor;
                return;
            }

            targetRenderer.material.color = ResolveTierColor(buildingData);
        }

        private static Color ResolveTierColor(BuildingData buildingData)
        {
            if (buildingData == null)
            {
                return Color.white;
            }

            switch (buildingData.Tier)
            {
                case 2:
                    return new Color(0.45f, 0.58f, 0.68f);
                case 3:
                    return new Color(0.95f, 0.74f, 0.22f);
                case 4:
                    return new Color(0.58f, 0.28f, 0.82f);
                default:
                    return new Color(0.42f, 0.26f, 0.14f);
            }
        }

        private static bool IsType(BuildingData buildingData, string buildingType)
        {
            return buildingData != null
                && string.Equals(buildingData.BuildingType, buildingType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
