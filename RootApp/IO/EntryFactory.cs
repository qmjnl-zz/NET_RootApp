using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace RootApp.IO
{
    public class EntryFactory
    {
        private const IconInfo.IconSize iconSize = IconInfo.IconSize.Small;
        private const string defaultDirectoryPath = @"C:\Path";
        private const string defaultDirectoryExtension = "<directory>";
        private const string defaultFilePath = @"C:\Path";
        private const string defaultFileExtension = "<file>";

        private List<string> fileExtensions = new List<string> { "exe", "lnk" };
        IconInfo2.IconType iconType = IconInfo2.IconType.File;

        private readonly Dictionary<string, ImageSource> icons = new Dictionary<string, ImageSource>();

        public EntryFactory()
        {
            if (!icons.ContainsKey(defaultDirectoryExtension))
            {
                icons.Add(defaultDirectoryExtension, IconInfo2.GetSystemIcon(
                    defaultDirectoryPath,
                    iconSize,
                    IconInfo2.IconType.System,
                    FileAttributes.Directory
                ));
            }

            if (!icons.ContainsKey(defaultFileExtension))
            {
                icons.Add(defaultFileExtension, IconInfo2.GetSystemIcon(
                    defaultFilePath,
                    iconSize,
                    IconInfo2.IconType.System,
                    FileAttributes.Normal
                ));
            }
        }

        public Entry GetDrive(DriveInfo info)
        {
            Entry entry = new Entry();
            try
            {
                entry.Type = EntryType.Drive;
                entry.Icon = IconInfo2.GetSystemIcon(info.Name, iconSize, IconInfo2.IconType.File);
                entry.FullName = info.Name;
                entry.Name = info.Name;
                entry.Size = info.TotalSize;
            }
            catch (Exception) { }
            return entry;
        }

        public Entry GetParentDirectory(string path)
        {
            try
            {
                return new Entry
                {
                    Type = EntryType.ParentDirectory,
                    FullName = path,
                    Date = File.GetLastWriteTime(path)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Entry GetDirectory(string path)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);

                string iconPath = iconType == IconInfo2.IconType.File ? path : defaultDirectoryExtension;
                if (!icons.ContainsKey(iconPath))
                {
                    icons.Add(iconPath, IconInfo2.GetSystemIcon(iconPath, iconSize, iconType, FileAttributes.Directory));
                }

                return new Entry
                {
                    Type = EntryType.Directory,
                    Icon = icons[iconPath],
                    FullName = info.FullName,
                    Name = info.Name,
                    Date = File.GetLastWriteTime(path)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Entry GetFile(string path)
        {
            try
            {
                FileInfo info = new FileInfo(path);

                string iconPath = String.IsNullOrEmpty(info.Extension) ? defaultFileExtension : info.Extension;
                if (!icons.ContainsKey(iconPath))
                {
                    icons.Add(iconPath, IconInfo2.GetSystemIcon(iconPath, iconSize, IconInfo2.IconType.System, FileAttributes.Normal));
                }

                if (iconType == IconInfo2.IconType.File && fileExtensions.Contains(iconPath.Replace(".", "")))
                {
                    iconPath = path;
                    if (!icons.ContainsKey(iconPath))
                    {
                        icons.Add(iconPath, IconInfo2.GetSystemIcon(iconPath, iconSize, iconType, FileAttributes.Normal));
                    }
                }

                Entry entry = new Entry
                {
                    Type = EntryType.File,
                    Icon = icons[iconPath],
                    FullName = info.FullName,
                    Name = info.Name.Substring(0, info.Name.Length - info.Extension.Length),
                    Extension = info.Extension,
                    Size = info.Length,
                    Date = File.GetLastWriteTime(path)
                };

                if (entry.Name.Length == 0)
                {
                    entry.Name = entry.Extension;
                    entry.Extension = "";
                }
                else
                {
                    entry.Extension = info.Extension.Replace(".", "");
                }

                return entry;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
