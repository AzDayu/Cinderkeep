using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace OODong.TeamDocs
{
    public sealed class ReadMeFolderOpenButton : MonoBehaviour
    {
        [SerializeField] private string _folderRelativePath = "Assets/ReadMe";

        public string FolderRelativePath => _folderRelativePath;

        public void SetFolderRelativePath(string folderRelativePath)
        {
            _folderRelativePath = folderRelativePath;
        }

        public void OpenFolder()
        {
            string folderPath = GetFolderPath();
            Directory.CreateDirectory(folderPath);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe", $"\"{folderPath}\"")
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
#else
            Application.OpenURL(new Uri(folderPath).AbsoluteUri);
#endif
        }

        private string GetFolderPath()
        {
            if (Path.IsPathRooted(_folderRelativePath))
            {
                return _folderRelativePath;
            }

            string normalizedPath = _folderRelativePath.Replace('\\', '/');
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();
            return Path.GetFullPath(Path.Combine(projectRoot, normalizedPath));
        }
    }
}
