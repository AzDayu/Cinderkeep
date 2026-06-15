namespace OODong.Muckhold
{
    public enum MuckholdItemId
    {
        None = 0,
        Pickaxe = 1,
        Stone = 2,
        Ore = 3,
        Apple = 4,
        Arrow = 5
    }

    public static class MuckholdItemCatalog
    {
        public static string GetDisplayName(MuckholdItemId itemId)
        {
            switch (itemId)
            {
                case MuckholdItemId.Pickaxe:
                    return "Pickaxe";
                case MuckholdItemId.Stone:
                    return "Stone";
                case MuckholdItemId.Ore:
                    return "Ore";
                case MuckholdItemId.Apple:
                    return "Apple";
                case MuckholdItemId.Arrow:
                    return "Arrow";
                default:
                    return "Empty";
            }
        }

        public static bool CanAssignQuickSlot(MuckholdItemId itemId)
        {
            return itemId != MuckholdItemId.None;
        }
    }
}
