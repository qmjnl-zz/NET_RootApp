using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace RootApp.IO
{
    public class EntryFactory
    {
        private const IconInfo.IconSize iconSize = IconInfo.IconSize.Large;
        private const string defaultDirectoryPath = @"C:\Path";
        private const string defaultDirectoryExtension = "<directory>";
        private const string defaultFilePath = @"C:\Path";
        private const string defaultFileExtension = "<file>";

        private readonly Dictionary<string, ImageSource> icons = new Dictionary<string, ImageSource>();

        public EntryFactory()
        {
            if (!icons.ContainsKey(defaultDirectoryExtension))
            {
                icons.Add(defaultDirectoryExtension, IconInfo.GetSystemIcon(defaultDirectoryPath, iconSize, FileAttributes.Directory));
            }
            if (!icons.ContainsKey(defaultFileExtension))
            {
                icons.Add(defaultFileExtension, IconInfo.GetSystemIcon(defaultFilePath, iconSize, FileAttributes.Normal));
            }
        }

        public Entry GetDrive(DriveInfo info)
        {
            Entry entry = new Entry();
            try
            {
                entry.Type = EntryType.Drive;
                entry.Icon = IconInfo.GetSystemIcon(info.Name, iconSize);
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
                return new Entry
                {
                    Type = EntryType.Directory,
                    //Icon = icons[defaultDirectoryExtension],
                    Icon = IconInfo.GetFileIcon(path, iconSize),
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

                string extension = String.IsNullOrEmpty(info.Extension) ? defaultFileExtension : info.Extension;
                if (!icons.ContainsKey(extension))
                {
                    icons.Add(extension, IconInfo.GetSystemIcon(extension, iconSize, FileAttributes.Normal));
                }

                Entry entry = new Entry
                {
                    Type = EntryType.File,
                    //Icon = icons[extension],
                    Icon = IconInfo.GetFileIcon(path, iconSize),
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
