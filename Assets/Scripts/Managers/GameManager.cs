using UnityEngine;

using UnityEngine.Serialization;

namespace Cinderkeep.Gameplay
{
    // 게임 전체 생성 주기를 잡는 최상위 매니저입니다.
    // 현재 목표는 3일/15분 MVP 루프를 안정적으로 시작하고 끝내는 것입니다.
    // 1일차는 채집/기초 제작, 2일차는 FlameHeart 방어, 3일차는 보스 클리어가 기준입니다.
    // 이후 7일 루프 확장은 이 초기화 흐름을 유지한 채 Model과 매니저 기능을 늘립니다.
    // 규칙: 게임 영역에서는 GameManager만 싱글톤으로 둡니다.
    public sealed class GameManager : MonoBehaviour
    {
        [FormerlySerializedAs("GameDataManager_GameDataManager")]
        [SerializeField] private GameDataManager _gameDataManager;
        [FormerlySerializedAs("GameObjectManager_GameObjectManager")]
        [SerializeField] private GameObjectManager _gameObjectManager;
        [FormerlySerializedAs("UIManager_UIManager")]
        [SerializeField] private UIManager _uiManager;
        [FormerlySerializedAs("SoundManager_SoundManager")]
        [SerializeField] private SoundManager _soundManager;
        [FormerlySerializedAs("MapManager_MapManager")]
        [SerializeField] private MapManager _mapManager;

        private PlayerModel _playerModel = new PlayerModel();
        private GameRunModel _gameRunModel = new GameRunModel();
        private bool _isInitialized;

        public static GameManager Inst { get; private set; }

        public PlayerModel PlayerModel
        {
            get
            {
                return _playerModel;
            }
        }

        public GameRunModel GameRunModel
        {
            get
            {
                return _gameRunModel;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        private void Awake()
        {
            RegisterSingleton();
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (Inst == this)
            {
                Inst = null;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            // 생성 주기 순서:
            // 1. 3일 MVP 루프에 필요한 Static Data 로드
            // 2. 몬스터, 자원, 건축물 생성을 맡을 관리자 준비
            // 3. 테스트맵과 이후 모듈형 맵 생성을 맡을 관리자 준비
            // 4. BGM과 효과음 관리자 준비
            // 5. HUD, 인벤토리, 게임오버 UI 관리자 준비
            // 6. 저장이 필요한 Instance Model 기본값 준비
            InitializeManager(_gameDataManager, "GameDataManager");
            InitializeManager(_gameObjectManager, "GameObjectManager");
            InitializeManager(_mapManager, "MapManager");
            InitializeManager(_soundManager, "SoundManager");
            InitializeManager(_uiManager, "UIManager");

            _playerModel.InitializeDefault();
            _gameRunModel.InitializeDefault();
            _isInitialized = true;
        }

        public void StartNewGame()
        {
            Initialize();
            _gameRunModel.StartRun();

            if (_uiManager != null)
            {
                _uiManager.OpenHud();
            }
        }

        public void EndGame()
        {
            if (!_gameRunModel.IsPlaying)
            {
                return;
            }

            _gameRunModel.EndRun();

            if (_uiManager != null)
            {
                _uiManager.OpenGameOverPanel();
            }
        }

        public GameDataManager GetGameDataManager()
        {
            return _gameDataManager;
        }

        public GameObjectManager GetGameObjectManager()
        {
            return _gameObjectManager;
        }

        public UIManager GetUIManager()
        {
            return _uiManager;
        }

        public SoundManager GetSoundManager()
        {
            return _soundManager;
        }

        public MapManager GetMapManager()
        {
            return _mapManager;
        }

        private void RegisterSingleton()
        {
            if (Inst == null)
            {
                Inst = this;
                return;
            }

            if (Inst == this)
            {
                return;
            }

            Debug.LogWarning("GameManager duplicate was found. Only the first GameManager will stay active.");
            Destroy(gameObject);
        }

        private void InitializeManager<TManager>(TManager manager, string managerName)
            where TManager : MonoBehaviour, IGameInitializable
        {
            if (manager == null)
            {
                Debug.LogWarning(managerName + " reference is empty. Assign it in the Inspector before gameplay work.");
                return;
            }

            manager.Initialize();
        }
    }
}
