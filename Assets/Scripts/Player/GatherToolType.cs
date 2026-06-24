// 5.00 direction: Handles one part of first-person player control, status, combat, gathering, or building.
// 5.01+ note: Keep input, state, and action effects separated so quickslots, tools, weapons, and tutorials remain maintainable.
// 채집 대상이 어떤 도구를 요구하는지 구분하는 enum입니다.
// None은 맨손 상호작용, Axe는 나무, Pickaxe는 바위와 광석에 사용합니다.
public enum GatherToolType
{
    None,
    Axe,
    Pickaxe
}
