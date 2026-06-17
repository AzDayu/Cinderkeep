using System;
using System.IO;
using UnityEngine;

namespace MainHub.CharacterSelect
{
    public static class MainHub_CharacterSelectionStorage
    {
        private const string SavePrefix = "MainHub.RememberedCharacter.";
        private const string LogRelativePath = "ReadMe/MainHub_CharacterSelection_Log.txt";

        private const string IdKey = SavePrefix + "id";
        private const string RoleKey = SavePrefix + "role";
        private const string OwnerKey = SavePrefix + "owner";
        private const string SceneKey = SavePrefix + "scene";
        private const string TimeKey = SavePrefix + "time";

        public static bool HasRememberedCharacter()
        {
            return !string.IsNullOrWhiteSpace(GetRememberedSceneName());
        }

        public static string GetRememberedSceneName()
        {
            return PlayerPrefs.GetString(SceneKey, string.Empty);
        }

        public static string GetRememberedOwnerName()
        {
            return PlayerPrefs.GetString(OwnerKey, string.Empty);
        }

        public static string GetRememberedRoleName()
        {
            return PlayerPrefs.GetString(RoleKey, string.Empty);
        }

        public static string GetRememberedTime()
        {
            return PlayerPrefs.GetString(TimeKey, string.Empty);
        }

        public static void SaveRememberedCharacter(MainHub_CharacterEntry entry, string sceneName)
        {
            if (entry == null || string.IsNullOrWhiteSpace(sceneName))
            {
                return;
            }

            string savedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerPrefs.SetString(IdKey, entry.Id);
            PlayerPrefs.SetString(RoleKey, entry.EnglishRole);
            PlayerPrefs.SetString(OwnerKey, entry.OwnerName);
            PlayerPrefs.SetString(SceneKey, sceneName);
            PlayerPrefs.SetString(TimeKey, savedTime);
            PlayerPrefs.Save();

            WriteLog("선택", entry.OwnerName, entry.EnglishRole, sceneName, savedTime);
        }

        public static void ClearRememberedCharacter()
        {
            string ownerName = GetRememberedOwnerName();
            string roleName = GetRememberedRoleName();
            string sceneName = GetRememberedSceneName();
            string savedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            PlayerPrefs.DeleteKey(IdKey);
            PlayerPrefs.DeleteKey(RoleKey);
            PlayerPrefs.DeleteKey(OwnerKey);
            PlayerPrefs.DeleteKey(SceneKey);
            PlayerPrefs.DeleteKey(TimeKey);
            PlayerPrefs.Save();

            WriteLog("취소", ownerName, roleName, sceneName, savedTime);
        }

        private static void WriteLog(string actionName, string ownerName, string roleName, string sceneName, string savedTime)
        {
            string logPath = GetLogPath();
            string directoryPath = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string line = savedTime
                + " | " + actionName
                + " | Owner=" + GetLogValue(ownerName)
                + " | Class=" + GetLogValue(roleName)
                + " | Scene=" + GetLogValue(sceneName)
                + Environment.NewLine;

            File.AppendAllText(logPath, line);
        }

        private static string GetLogPath()
        {
            return Path.Combine(Application.dataPath, LogRelativePath);
        }

        private static string GetLogValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "None";
            }

            return value;
        }
    }
}
