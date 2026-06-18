using System;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Cinderkeep.Gameplay
{
    // 플레이 중 저장되어야 하는 플레이어 Instance Data입니다.
    // 변하지 않는 기획 데이터는 GameData, 변하는 저장 데이터는 Model 이름을 붙입니다.
    [Serializable]
    public sealed class PlayerModel
    {
        private int _health;
        private int _maxHealth;
        private int _stamina;
        private int _maxStamina;
        private int _level;


        private int _wood;
        private int _stone;
        private int _iron;
        private int _mithril;
        private int _adamantium;


        public int Health { get { return _health; } }
        public int MaxHealth { get { return _maxHealth; } }
        public int Stamina { get { return _stamina; } }
        public int MaxStamina { get { return _maxStamina; } }
        public int Level { get { return _level; } }


        public int Wood { get { return _wood; } }
        public int Stone { get { return _stone; } }
        public int Iron { get { return _iron; } }
        public int Mithril { get { return _mithril; } }
        public int Adamantium { get { return _adamantium; } }


        public event Action OnResourceChanged;


        public void InitializeDefault()
        {
            _maxHealth = 100;
            _health = _maxHealth;
            _level = 1;

            _wood = 0;
            _stone = 0;
            _iron = 0;
            _mithril = 0;
            _adamantium = 0;
        }


        public void AddResource(string resourceType, int amount)
        {
            if (resourceType == "Stone") _stone += amount;
            else if (resourceType == "Wood") _wood += amount;
            else if (resourceType == "Iron") _iron += amount;
            else if (resourceType == "Mithril") _mithril += amount;
            else if (resourceType == "Adamantium") _adamantium += amount;

            OnResourceChanged?.Invoke();
        }

        public bool UseResource(string resourceType, int amount)
        {
            bool isSuccess = false;
            switch (resourceType)
            {
                case "Wood":
                    if (_wood >= amount) { _wood -= amount; isSuccess = true; }
                    break;
                case "Stone":
                    if (_stone >= amount) { _stone -= amount; isSuccess = true; }
                    break;
                case "Iron":
                    if (_iron >= amount) { _iron -= amount; isSuccess = true; }
                    break;
                case "Mithril":
                    if (_mithril >= amount) { _mithril -= amount; isSuccess = true; }
                    break;
                case "Adamantium":
                    if (_adamantium >= amount) { _adamantium -= amount; isSuccess = true; }
                    break;
            }

            if (isSuccess == true)
            {
                OnResourceChanged?.Invoke();
            }

            return isSuccess;
        }





    }
}
