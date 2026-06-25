using System;

// 한 판 플레이 중 변하는 런타임 상태를 저장합니다.
// 상태 변경은 명시적인 메서드로 처리하고, UI와 시스템은 이 모델을 읽거나 요청만 보냅니다.
namespace Cinderkeep.Gameplay
{
    // 플레이어의 체력, 스태미나, 레벨, 자원 지갑을 묶는 최상위 런타임 모델입니다.
    // 자원 증감 세부 규칙은 PlayerResourceWallet에 위임해 Model 비대화를 줄입니다.
    [Serializable]
    public sealed class PlayerModel
    {
        public const string ResourceWood = "Wood";
        public const string ResourceStone = "Stone";
        public const string ResourceIron = "Iron";
        public const string ResourceGold = "Gold";
        public const string ResourceMithril = "Mithril";
        public const string ResourceAdamantium = "Adamantium";
        public const string ResourceIronOre = "IronOre";
        public const string ResourceGoldOre = "GoldOre";
        public const string ResourceAdamantiumOre = "AdamantiumOre";
        public const string ResourceIronIngot = "IronIngot";
        public const string ResourceGoldIngot = "GoldIngot";
        public const string ResourceAdamantiumIngot = "AdamantiumIngot";

        private readonly PlayerResourceWallet _resourceWallet = new PlayerResourceWallet();

        private int _health;
        private int _maxHealth;
        private int _stamina;
        private int _maxStamina;
        private int _level;

        public int Health { get { return _health; } }
        public int MaxHealth { get { return _maxHealth; } }
        public int Stamina { get { return _stamina; } }
        public int MaxStamina { get { return _maxStamina; } }
        public int Level { get { return _level; } }

        public int Wood { get { return GetResourceAmount(ResourceWood); } }
        public int Stone { get { return GetResourceAmount(ResourceStone); } }
        public int Iron { get { return GetResourceAmount(ResourceIron); } }
        public int Gold { get { return GetResourceAmount(ResourceGold); } }
        public int Mithril { get { return GetResourceAmount(ResourceMithril); } }
        public int Adamantium { get { return GetResourceAmount(ResourceAdamantium); } }
        public int IronOre { get { return GetResourceAmount(ResourceIronOre); } }
        public int GoldOre { get { return GetResourceAmount(ResourceGoldOre); } }
        public int AdamantiumOre { get { return GetResourceAmount(ResourceAdamantiumOre); } }
        public int IronIngot { get { return GetResourceAmount(ResourceIronIngot); } }
        public int GoldIngot { get { return GetResourceAmount(ResourceGoldIngot); } }
        public int AdamantiumIngot { get { return GetResourceAmount(ResourceAdamantiumIngot); } }

        public event Action OnResourceChanged;
        public static event Action<string, int> ResourceAddedGlobal;

        public void InitializeDefault()
        {
            _maxHealth = 100;
            _health = _maxHealth;
            _maxStamina = 150;
            _stamina = _maxStamina;
            _level = 1;
            _resourceWallet.InitializeDefault();
        }

        public void AddResource(string resourceType, int amount)
        {
            if (_resourceWallet.TryAddResource(resourceType, amount) == false)
            {
                return;
            }

            NotifyResourceChanged();
            NotifyResourceAdded(resourceType, amount);
        }

        public bool UseResource(string resourceType, int amount)
        {
            if (_resourceWallet.TryUseResource(resourceType, amount) == false)
            {
                return false;
            }

            NotifyResourceChanged();
            return true;
        }

        public bool HasResource(string resourceType, int amount)
        {
            return _resourceWallet.HasResource(resourceType, amount);
        }

        public int GetResourceAmount(string resourceType)
        {
            return _resourceWallet.GetResourceAmount(resourceType);
        }

        private void NotifyResourceChanged()
        {
            if (OnResourceChanged == null)
            {
                return;
            }

            OnResourceChanged.Invoke();
        }

        private void NotifyResourceAdded(string resourceType, int amount)
        {
            if (ResourceAddedGlobal == null)
            {
                return;
            }

            ResourceAddedGlobal(resourceType, amount);
        }
    }
}
