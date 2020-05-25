using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RootApp.IO
{
    public static class IconInfo
    {
        public enum IconSize
        {
            Large = 0,
            Small = 1,
            ExtraLarge = 2,
            SysSmall = 3,
            Jumbo = 4
        }

        public static Size GetIconSize(IconSize iconSize)
        {
            int length;

            switch (iconSize)
            {
                case IconSize.Large:
                    length = 32;
                    break;
                case IconSize.Small:
                    length = 16;
                    break;
                case IconSize.ExtraLarge:
                    length = 48;
                    break;
                case IconSize.Jumbo:
                    length = 256;
                    break;
                default:
                    length = 16;
                    break;
            }

            return new Size(length, length);
        }

        // *****

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
            public static int Size => Marshal.SizeOf(typeof(SHFILEINFO));
        }

        [Flags]
        public enum SHGFI
        {
            SHGFI_ICON = 0x000000100,
            SHGFI_DISPLAYNAME = 0x000000200,
            SHGFI_TYPENAME = 0x000000400,
            SHGFI_ATTRIBUTES = 0x000000800,
            SHGFI_ICONLOCATION = 0x000001000,
            SHGFI_EXETYPE = 0x000002000,
            SHGFI_SYSICONINDEX = 0x000004000,
            SHGFI_LINKOVERLAY = 0x000008000,
            SHGFI_SELECTED = 0x000010000,
            SHGFI_ATTR_SPECIFIED = 0x000020000,
            SHGFI_LARGEICON = 0x000000000,
            SHGFI_SMALLICON = 0x000000001,
            SHGFI_OPENICON = 0x000000002,
            SHGFI_SHELLICONSIZE = 0x000000004,
            SHGFI_PIDL = 0x000000008,
            SHGFI_USEFILEATTRIBUTES = 0x000000010,
            SHGFI_ADDOVERLAYS = 0x000000020,
            SHGFI_OVERLAYINDEX = 0x000000040
        }

        [Flags]
        public enum IMAGELISTDRAWFLAGS : uint
        {
            ILD_NORMAL = 0X00000000,
            ILD_TRANSPARENT = 0x00000001,
            ILD_BLEND25 = 0X00000002,
            ILD_FOCUS = ILD_BLEND25,
            ILD_BLEND50 = 0X00000004,
            ILD_SELECTED = ILD_BLEND50,
            ILD_BLEND = ILD_BLEND50,
            ILD_MASK = 0X00000010,
            ILD_IMAGE = 0X00000020,
            ILD_ROP = 0X00000040,
            ILD_OVERLAYMASK = 0x00000F00,
            ILD_PRESERVEALPHA = 0x00001000,
            ILD_SCALE = 0X00002000,
            ILD_DPISCALE = 0X00004000,
            ILD_ASYNC = 0X00008000
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            FileAttributes dwFileAttributes,
            ref SHFILEINFO psfi,
            int cbFileInfo,
            SHGFI uFlags
        );

        [DllImport("comctl32.dll", SetLastError = true)]
        private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, IMAGELISTDRAWFLAGS flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // *****

        public static ImageSource GetSystemIcon(string path, IconSize iconSize, FileAttributes fileAttributes = new FileAttributes())
        {
            Size size = GetIconSize(iconSize);
            SHFILEINFO shfi = new SHFILEINFO();

            IntPtr iconHandle = IntPtr.Zero;
            Icon icon;

            ImageSource imageSource = null;

            try
            {
                IntPtr imageList = SHGetFileInfo(
                    path,
                    fileAttributes,
                    ref shfi,
                    SHFILEINFO.Size,
                    SHGFI.SHGFI_SYSICONINDEX | SHGFI.SHGFI_USEFILEATTRIBUTES | (iconSize == IconSize.Small ? SHGFI.SHGFI_SMALLICON : 0));
                iconHandle = ImageList_GetIcon(imageList, shfi.iIcon, IMAGELISTDRAWFLAGS.ILD_TRANSPARENT | IMAGELISTDRAWFLAGS.ILD_IMAGE);
                icon = (Icon)Icon.FromHandle(iconHandle).Clone();

                imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
            catch (Exception)
            {

            }
            finally
            {
                if (shfi.hIcon != IntPtr.Zero) DestroyIcon(shfi.hIcon);
                if (iconHandle != IntPtr.Zero) DestroyIcon(iconHandle);
            }

            return imageSource;
        }

        public static ImageSource GetFileIcon(string path, IconSize iconSize, FileAttributes fileAttributes = new FileAttributes())
        {
            Size size = GetIconSize(iconSize);
            SHFILEINFO shfi = new SHFILEINFO();

            IntPtr iconHandle = IntPtr.Zero;
            Icon icon;

            ImageSource imageSource = null;

            try
            {
                IntPtr imageList = SHGetFileInfo(
                    path,
                    fileAttributes,
                    ref shfi,
                    SHFILEINFO.Size,
                    SHGFI.SHGFI_SYSICONINDEX | SHGFI.SHGFI_ICON | (iconSize == IconSize.Small ? SHGFI.SHGFI_SMALLICON : 0));
                iconHandle = ImageList_GetIcon(imageList, shfi.iIcon, IMAGELISTDRAWFLAGS.ILD_TRANSPARENT | IMAGELISTDRAWFLAGS.ILD_IMAGE);

                // Without Link
                icon = (Icon)Icon.FromHandle(iconHandle).Clone();
                // With Link
                icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();

                imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
            catch (Exception)
            {

            }
            finally
            {
                if (shfi.hIcon != IntPtr.Zero) DestroyIcon(shfi.hIcon);
                if (iconHandle != IntPtr.Zero) DestroyIcon(iconHandle);
            }

            return imageSource;
        }
    }
}
