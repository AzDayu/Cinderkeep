using UnityEngine;

namespace Cinderkeep.Gameplay
{
    // 게임 전체 생성 주기를 잡는 최상위 매니저입니다.
    // 규칙: 게임 영역에서는 GameManager만 싱글톤으로 둡니다.
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameDataManager GameDataManager_GameDataManager;
        [SerializeField] private GameObjectManager GameObjectManager_GameObjectManager;
        [SerializeField] private UIManager UIManager_UIManager;
        [SerializeField] private SoundManager SoundManager_SoundManager;

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

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            // 생성 주기 순서:
            // 1. Static Data 로드
            // 2. 동적 오브젝트 생성 관리자 준비
            // 3. 사운드 관리자 준비
            // 4. UI 관리자 준비
            // 5. 저장이 필요한 Instance Model 기본값 준비
            InitializeManager(GameDataManager_GameDataManager, "GameDataManager");
            InitializeManager(GameObjectManager_GameObjectManager, "GameObjectManager");
            InitializeManager(SoundManager_SoundManager, "SoundManager");
            InitializeManager(UIManager_UIManager, "UIManager");

            _playerModel.InitializeDefault();
            _gameRunModel.InitializeDefault();
            _isInitialized = true;
        }

        public void StartNewGame()
        {
            Initialize();
            _gameRunModel.StartRun();

            if (UIManager_UIManager != null)
            {
                UIManager_UIManager.OpenHud();
            }
        }

        public void EndGame()
        {
            if (!_gameRunModel.IsPlaying)
            {
                return;
            }

            _gameRunModel.EndRun();

            if (UIManager_UIManager != null)
            {
                UIManager_UIManager.OpenGameOverPanel();
            }
        }

        public GameDataManager GetGameDataManager()
        {
            return GameDataManager_GameDataManager;
        }

        public GameObjectManager GetGameObjectManager()
        {
            return GameObjectManager_GameObjectManager;
        }

        public UIManager GetUIManager()
        {
            return UIManager_UIManager;
        }

        public SoundManager GetSoundManager()
        {
            return SoundManager_SoundManager;
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
