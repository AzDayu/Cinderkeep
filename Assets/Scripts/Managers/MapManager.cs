using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;

namespace Cinderkeep.Gameplay
{
    // GameManager 초기화 순서에 맞춰 모듈형 맵 청크를 준비하는 매니저입니다.
    // 중앙 청크는 CinderHeart 방어, 전투, 건축 테스트가 가능한 평평한 구역으로 유지합니다.
    public sealed class MapManager : MonoBehaviour, IGameInitializable
    {
        [Header("Chunk Settings")]
        [SerializeField] private bool _generateOnInitialize;
        [SerializeField] private float _chunkSize = 120f;
        [SerializeField] private int _mapRadius = 2;

        [Header("Chunk Prefabs")]
        [SerializeField] private GameObject _centerChunkPrefab;
        [SerializeField] private List<GameObject> _normalChunkPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> _edgeChunkPrefabs = new List<GameObject>();

        private Dictionary<Vector2Int, GameObject> _spawnedChunks = new Dictionary<Vector2Int, GameObject>();
        private bool _isInitialized;

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            if (_generateOnInitialize == true)
            {
                GenerateModularMap();
            }

            _isInitialized = true;
        }

        public void GenerateModularMap()
        {
            if (CheckCanGenerateMap() == false)
            {
                return;
            }

            ClearGeneratedChunks();

            for (int x = -_mapRadius; x <= _mapRadius; x++)
            {
                for (int z = -_mapRadius; z <= _mapRadius; z++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, z);
                    GameObject chunkPrefab = SelectChunkPrefabCanBeNull(gridPosition);

                    if (chunkPrefab != null)
                    {
                        SpawnChunk(gridPosition, chunkPrefab);
                    }
                }
            }
        }

        public void ClearGeneratedChunks()
        {
            foreach (GameObject chunk in _spawnedChunks.Values)
            {
                if (chunk != null)
                {
                    Destroy(chunk);
                }
            }

            _spawnedChunks.Clear();
        }

        private bool CheckCanGenerateMap()
        {
            if (_chunkSize <= 0f)
            {
                Debug.LogWarning("MapManager: Chunk size must be bigger than zero.");
                return false;
            }

            if (_mapRadius < 0)
            {
                Debug.LogWarning("MapManager: Map radius cannot be negative.");
                return false;
            }

            if (_centerChunkPrefab == null)
            {
                Debug.LogWarning("MapManager: Center chunk prefab is empty.");
                return false;
            }

            if (_normalChunkPrefabs == null || _normalChunkPrefabs.Count == 0)
            {
                Debug.LogWarning("MapManager: Normal chunk prefab list is empty.");
                return false;
            }

            if (_edgeChunkPrefabs == null || _edgeChunkPrefabs.Count == 0)
            {
                Debug.LogWarning("MapManager: Edge chunk prefab list is empty.");
                return false;
            }

            return true;
        }

        private GameObject SelectChunkPrefabCanBeNull(Vector2Int gridPosition)
        {
            if (CheckIsCenterPosition(gridPosition) == true)
            {
                return _centerChunkPrefab;
            }

            if (CheckIsEdgePosition(gridPosition) == true)
            {
                return GetRandomPrefabCanBeNull(_edgeChunkPrefabs);
            }

            return GetRandomPrefabCanBeNull(_normalChunkPrefabs);
        }

        private bool CheckIsCenterPosition(Vector2Int gridPosition)
        {
            return gridPosition.x == 0 && gridPosition.y == 0;
        }

        private bool CheckIsEdgePosition(Vector2Int gridPosition)
        {
            bool isEdgeX = Mathf.Abs(gridPosition.x) == _mapRadius;
            bool isEdgeZ = Mathf.Abs(gridPosition.y) == _mapRadius;

            return isEdgeX || isEdgeZ;
        }

        private GameObject GetRandomPrefabCanBeNull(List<GameObject> prefabs)
        {
            if (prefabs == null || prefabs.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, prefabs.Count);
            return prefabs[randomIndex];
        }

        private void SpawnChunk(Vector2Int gridPosition, GameObject prefab)
        {
            Vector3 worldPosition = new Vector3(gridPosition.x * _chunkSize, 0f, gridPosition.y * _chunkSize);
            Quaternion rotation = GetChunkRotation(gridPosition);

            GameObject chunk = Instantiate(prefab, worldPosition, rotation, transform);
            chunk.name = "MapChunk_" + gridPosition.x + "_" + gridPosition.y;

            _spawnedChunks.Add(gridPosition, chunk);
        }

        private Quaternion GetChunkRotation(Vector2Int gridPosition)
        {
            if (CheckIsCenterPosition(gridPosition) == true)
            {
                return Quaternion.identity;
            }

            return Quaternion.Euler(0f, GetRandomRightAngle(), 0f);
        }

        private float GetRandomRightAngle()
        {
            int randomStep = Random.Range(0, 4);
            return randomStep * 90f;
        }
    }
}
