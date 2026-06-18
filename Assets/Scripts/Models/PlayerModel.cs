using System;

namespace Cinderkeep.Gameplay
{
    // 플레이 중 저장되어야 하는 플레이어 Instance Data입니다.
    // 변하지 않는 기획 데이터는 GameData, 변하는 저장 데이터는 Model 이름을 붙입니다.
    [Serializable]
    public sealed class PlayerModel
    {
        private int _health;
        private int _maxHealth;
        private int _level;

        public int Health
        {
            get
            {
                return _health;
            }
        }

        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
        }

        public int Level
        {
            get
            {
                return _level;
            }
        }

        public void InitializeDefault()
        {
            _maxHealth = 100;
            _health = _maxHealth;
            _level = 1;
        }
    }
}
