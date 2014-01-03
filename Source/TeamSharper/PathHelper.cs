using System;
using System.IO;

namespace C24.TeamSharper
{
    public static class PathHelper
    {
        public static string MakeFilePathAbsoluteToDirectory(string filePath, string baseDirectory)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName == null)
            {
                throw new ArgumentException("filePath must be an absolute or relative path to a file.", "filePath");
            }
            return Path.GetFullPath(Path.Combine(baseDirectory, filePath));
        }

        public static string MakeFilePathRelativeToDirectory(string filePath, string baseDirectory)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName == null)
            {
                throw new ArgumentException("filePath must be an absolute path to a file.", "filePath");
            }
            string directory = Path.GetDirectoryName(filePath);
            string newRelativeDirectory = GetNavigationPath(EnsureTrailingBackslash(baseDirectory), EnsureTrailingBackslash(directory));
            return Path.Combine(newRelativeDirectory, fileName);
        }

        public static string GetNavigationPath(string sourceDirectory, string destinationDirectory)
        {
            if (!Path.IsPathRooted(destinationDirectory))
            {
                return RemoveTrailingBackslash(destinationDirectory);
            }

            return RemoveTrailingBackslash(new Uri(sourceDirectory, UriKind.Absolute)
                .MakeRelativeUri(new Uri(destinationDirectory, UriKind.Absolute))
                .ToString()
                .Replace('/', '\\'));
        }

        private static string RemoveTrailingBackslash(string path)
        {
            return path.TrimEnd('\\');
        }

        private static string EnsureTrailingBackslash(string path)
        {
            return string.Concat(RemoveTrailingBackslash(path), '\\');
        }
    }
}
