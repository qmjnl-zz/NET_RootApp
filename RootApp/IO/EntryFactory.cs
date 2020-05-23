using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace RootApp.IO
{
    public class EntryFactory
    {
        private const string defaultDirectoryPath = @"C:\Path";
        private const string defaultDirectoryExtension = "<directory>";
        private const string defaultFilePath = @"C:\Path";
        private const string defaultFileExtension = "<file>";

        private const Shell32.IconSize iconSize = Shell32.IconSize.Small;
        private readonly Dictionary<string, ImageSource> icons = new Dictionary<string, ImageSource>();

        public EntryFactory()
        {
            if (!icons.ContainsKey(defaultDirectoryExtension))
            {
                icons.Add(defaultDirectoryExtension, Shell32.GetIcon(defaultDirectoryPath, iconSize));
            }
            if (!icons.ContainsKey(defaultFileExtension))
            {
                icons.Add(defaultFileExtension, Shell32.GetIcon(defaultFilePath, iconSize, false));
            }
        }

        public Entry GetDrive(DriveInfo info)
        {
            Entry entry = new Entry();
            try
            {
                entry.Type = EntryType.Drive;
                entry.Icon = Shell32.GetIcon(info.Name, iconSize);
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
                    Icon = icons.ContainsKey(path) ? icons[path] : icons[defaultDirectoryExtension],
                    //Icon = Shell.GetIcon(path, iconSize /*Add no file attributes parameter*/),
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
                    icons.Add(extension, Shell32.GetIcon(extension, iconSize, false));
                }

                Entry entry = new Entry
                {
                    Type = EntryType.File,
                    Icon = icons[extension],
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
