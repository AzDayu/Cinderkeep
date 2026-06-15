namespace OODong.Muckhold
{
    public interface IMuckholdInteractable
    {
        string GetPrompt();
        bool CanInteract(MuckholdFirstPersonPlayer player);
        void Interact(MuckholdFirstPersonPlayer player);
    }
}
