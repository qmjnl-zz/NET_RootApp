using System;
using System.Windows.Media;

namespace RootApp.IO
{
    public class Entry
    {
        public EntryType Type { get; set; }
        public ImageSource Icon { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime? Date { get; set; }
    }
}
