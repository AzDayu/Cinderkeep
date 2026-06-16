using OODong.Cinderkeep;

namespace OODong.UI
{
    public static class UIManagerExtension
    {
        public static void OpenCinderkeepInventory(this UIManager uiManager, CinderkeepHudView hudView)
        {
            hudView?.SetInventoryOpen(true);
        }

        public static void CloseCinderkeepInventory(this UIManager uiManager, CinderkeepHudView hudView)
        {
            hudView?.SetInventoryOpen(false);
        }
    }
}
