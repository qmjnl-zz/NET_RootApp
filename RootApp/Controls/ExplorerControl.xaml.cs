using RootApp.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RootApp.Controls
{
    /// <summary>
    /// Interaction logic for ExplorerControl.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        const string symbolSortAsc = "\u2191";
        const string symbolSortDesc = "\u2193";

        string sortPropertyName;
        ListSortDirection sortDirection;
        CollectionViewSource viewSource;

        //private ObservableCollection<Entry> listFileInfo = new ObservableCollection<Entry>();
        private readonly EntryFactory entryFactory;

        #region Path

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

        #endregion

        #region IconSize


        private static void IconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExplorerControl explorer = (ExplorerControl)d;
            explorer.CoerceValue(ImageSizeProperty);
        }

        public IconInfo.IconSize IconSize
        {
            get { return (IconInfo.IconSize)GetValue(IconSizeProperty); }
            set
            {
                if ((IconInfo.IconSize)GetValue(IconSizeProperty) != value)
                {
                    SetValue(IconSizeProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                "IconSize",
                typeof(IconInfo.IconSize),
                typeof(ExplorerControl),
                new PropertyMetadata(IconInfo.IconSize.Small, new PropertyChangedCallback(IconSizeChanged)));


        #endregion

        #region ImageSize

        //private static void ImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //ExplorerControl explorer = (ExplorerControl)d;
        //    //explorer.GetEntries();
        //}

        public static object CoerceImageSize(DependencyObject d, object baseValue)
        {
            ExplorerControl explorer = (ExplorerControl)d;
            return IconInfo.GetImageSize(explorer.IconSize);
        }

        public System.Drawing.Size ImageSize
        {
            get { return (System.Drawing.Size)GetValue(ImageSizeProperty); }
            private set
            {
                if ((System.Drawing.Size)GetValue(ImageSizeProperty) != value)
                {
                    SetValue(ImageSizeProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for ImageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register(
                "ImageSize",
                typeof(System.Drawing.Size),
                typeof(ExplorerControl),
                new PropertyMetadata(
                    IconInfo.GetImageSize(IconInfo.IconSize.Small),
                    null/*new PropertyChangedCallback(ImageSizeChanged)*/,
                    new CoerceValueCallback(CoerceImageSize)));

        #endregion

        public ExplorerControl()
        {
            InitializeComponent();

            IconSize = IconInfo.IconSize.Small;
            entryFactory = new EntryFactory(IconSize);

            viewSource = (CollectionViewSource)FindResource("viewSource");

            Sort(headerName);
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
                    listFileInfo.Add(entryFactory.GetTopLevel(Path));

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

                viewSource.Source = listFileInfo;
                listViewFile.ItemsSource = viewSource.View;

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
                if (listViewFile.Items[i] is Entry entry && entry.FullName == path)
                {
                    SelectEntry(i, scroll);
                    return;
                }
            }
        }

        private void ListViewFile_KeyDown(object sender, KeyEventArgs e)
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

            if (e.Key == Key.Enter && entry.Type == EntryType.TopLevel)
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
                if (entry.Type == EntryType.Directory)
                {
                    SelectEntry(0, true);
                }
                return;
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader)sender;
            Sort(header);
            SelectEntry(listViewFile.SelectedIndex, true);
        }

        private void Sort(GridViewColumnHeader header)
        {
            GridView gridView = (GridView)listViewFile.View;
            foreach (var item in gridView.Columns)
            {
                GridViewColumnHeader itemHeader = (GridViewColumnHeader)item.Header;
                itemHeader.Content = itemHeader.Tag.ToString();
            }

            if (String.IsNullOrEmpty(sortPropertyName))
            {
                //sortPropertyName = "Name";
                //sortPropertyName = nameof(Entry.Name);
                sortPropertyName = headerName.Tag.ToString();
                sortDirection = ListSortDirection.Ascending;
            }
            else
            {
                if (sortPropertyName == header.Tag.ToString())
                {
                    sortDirection = sortDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }
                else
                {
                    sortPropertyName = header.Tag.ToString();
                    sortDirection = ListSortDirection.Ascending;
                }
            }

            viewSource.SortDescriptions.Clear();
            //viewSource.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
            viewSource.SortDescriptions.Add(new SortDescription(nameof(Entry.Type), ListSortDirection.Ascending));
            viewSource.SortDescriptions.Add(new SortDescription(sortPropertyName, sortDirection));
            viewSource.SortDescriptions.Add(new SortDescription(headerName.Tag.ToString(), ListSortDirection.Ascending));

            header.Content = (sortDirection == ListSortDirection.Ascending ? symbolSortAsc : symbolSortDesc) + " " + header.Tag.ToString();
        }
    }
}
