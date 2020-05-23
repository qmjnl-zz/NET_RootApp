using RootApp.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RootApp.Controls
{
    /// <summary>
    /// Interaction logic for ExplorerControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        //private ObservableCollection<Entry> listFileInfo = new ObservableCollection<Entry>();
        private EntryFactory entryFactory = new EntryFactory();

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set
            {
                if ((string)GetValue(PathProperty) != value)
                {
                    SetValue(PathProperty, value);
                }
            }
        }

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExplorerControl explorer = (ExplorerControl)d;
            explorer.GetEntries();
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
                "Path",
                typeof(string),
                typeof(ExplorerControl),
                new PropertyMetadata(String.Empty, new PropertyChangedCallback(PathChanged)));

        public ExplorerControl()
        {
            InitializeComponent();

            //listViewFile.ItemsSource = listFileInfo;
            //Path = null;
        }

        protected void GetEntries()
        {
            try
            {
                ObservableCollection<Entry> listFileInfo = new ObservableCollection<Entry>();

                // Drives
                if (Path == null || String.IsNullOrEmpty(Path))
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    listFileInfo.Clear();
                    foreach (DriveInfo drive in drives)
                    {
                        listFileInfo.Add(entryFactory.GetDrive(drive));
                    }
                }
                // Directories and files
                else
                {
                    listFileInfo.Clear();
                    listFileInfo.Add(entryFactory.GetParentDirectory(Path));

                    // *****

                    string[] directories = Directory.GetDirectories(Path);
                    foreach (var entry in directories)
                    {
                        listFileInfo.Add(entryFactory.GetDirectory(entry));
                    }

                    // *****

                    List<string> entries = new List<string>();
                    //entries.AddRange(Directory.GetDirectories(Path));
                    entries.AddRange(Directory.GetFiles(Path));

                    entries.ForEach(entry => listFileInfo.Add(entryFactory.GetFile(entry)));
                }

                listViewFile.ItemsSource = listFileInfo;

                if (listFileInfo.Count > 0)
                {
                    SelectEntry(0);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                string path = Path;
                Path = Directory.GetParent(Path)?.FullName;
                SelectEntry(path);
            }
        }

        private void SelectEntry(int index, bool scroll = false)
        {
            listViewFile.SelectedIndex = index;

            if (scroll == true)
            {
                listViewFile.ScrollIntoView(listViewFile.Items[listViewFile.SelectedIndex]);
            }

            listViewFile.UpdateLayout();
            ListViewItem item = listViewFile.ItemContainerGenerator.ContainerFromIndex(listViewFile.SelectedIndex) as ListViewItem;
            item?.Focus();
        }

        private void SelectEntry(string path, bool scroll = false)
        {
            for (int i = 0; i < listViewFile.Items.Count; i++)
            {
                Entry entry = listViewFile.Items[i] as Entry;
                if (entry != null && entry.FullName == path)
                {
                    SelectEntry(i, scroll);
                    return;
                }
            }
        }

        private void listViewFile_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (Path != null && !String.IsNullOrEmpty(Path))
                {
                    string path = Path;
                    Path = Directory.GetParent(Path)?.FullName;
                    SelectEntry(path, true);
                }
                return;
            }

            Entry entry = (Entry)((ListView)sender).SelectedItem;

            if (e.Key == Key.Enter && entry.Type == EntryType.ParentDirectory)
            {
                if (Path != null && !String.IsNullOrEmpty(Path))
                {
                    string path = Path;
                    Path = Directory.GetParent(Path)?.FullName;
                    SelectEntry(path, true);
                }
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (entry.Type == EntryType.Drive || entry.Type == EntryType.Directory)
                {
                    Path = entry.FullName;
                }
                return;
            }
        }
    }
}
