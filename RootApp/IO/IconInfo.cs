using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RootApp.IO
{
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

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left, top, right, bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        int x, y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;
        public int yBitmap;
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }

    [ComImportAttribute()]
    [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImageList
    {
        [PreserveSig] int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
        [PreserveSig] int ReplaceIcon(int i, IntPtr hicon, ref int pi);
        [PreserveSig] int SetOverlayImage(int iImage, int iOverlay);
        [PreserveSig] int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
        [PreserveSig] int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
        [PreserveSig] int Draw(ref IMAGELISTDRAWPARAMS pimldp);
        [PreserveSig] int Remove(int i);
        [PreserveSig] int GetIcon(int i, int flags, ref IntPtr picon);
        [PreserveSig] int GetImageInfo(int i, ref IMAGEINFO pImageInfo);
        [PreserveSig] int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
        [PreserveSig] int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);
        [PreserveSig] int Clone(ref Guid riid, ref IntPtr ppv);
        [PreserveSig] int GetImageRect(int i, ref RECT prc);
        [PreserveSig] int GetIconSize(ref int cx, ref int cy);
        [PreserveSig] int SetIconSize(int cx, int cy);
        [PreserveSig] int GetImageCount(ref int pi);
        [PreserveSig] int SetImageCount(int uNewCount);
        [PreserveSig] int SetBkColor(int clrBk, ref int pclr);
        [PreserveSig] int GetBkColor(ref int pclr);
        [PreserveSig] int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
        [PreserveSig] int EndDrag();
        [PreserveSig] int DragEnter(IntPtr hwndLock, int x, int y);
        [PreserveSig] int DragLeave(IntPtr hwndLock);
        [PreserveSig] int DragMove(int x, int y);
        [PreserveSig] int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
        [PreserveSig] int DragShowNolock(int fShow);
        [PreserveSig] int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);
        [PreserveSig] int GetItemFlags(int i, ref int dwFlags);
        [PreserveSig] int GetOverlayImage(int iOverlay, ref int piIndex);
    };

    class IconInfo
    {
        const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
        const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";

        public const int SHIL_LARGE = 0x0;
        public const int SHIL_SMALL = 0x1;
        public const int SHIL_EXTRALARGE = 0x2;
        public const int SHIL_SYSSMALL = 0x3;
        public const int SHIL_JUMBO = 0x4;
        public const int SHIL_LAST = 0x4;

        public const int ILD_TRANSPARENT = 0x00000001;
        public const int ILD_IMAGE = 0x00000020;

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

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        public static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("shell32.dll")]
        public static extern uint SHGetIDListFromObject([MarshalAs(UnmanagedType.IUnknown)] object iUnknown, out IntPtr ppidl);

        [DllImport("shell32.dll")]
        public static extern int SHGetIconOverlayIndex(string pszIconPath, int iIconIndex);

        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            FileAttributes dwFileAttributes,
            ref SHFILEINFO psfi,
            int cbFileInfo,
            SHGFI uFlags
        );

        public enum IconType
        {
            System,
            File
        }

        public static ImageSource GetSystemIcon(string path, IconSize iconSize, IconType iconType, FileAttributes fileAttributes = new FileAttributes())
        {
            Size size = GetIconSize(iconSize);

            SHGFI flags = SHGFI.SHGFI_SYSICONINDEX | (iconSize == IconSize.Small ? SHGFI.SHGFI_SMALLICON : SHGFI.SHGFI_LARGEICON);
            switch (iconType)
            {
                case IconType.System:
                    flags |= SHGFI.SHGFI_USEFILEATTRIBUTES;
                    break;
                case IconType.File:
                    flags |= SHGFI.SHGFI_ICON;
                    break;
                default:
                    break;
            }

            SHFILEINFO shfi = new SHFILEINFO();

            IntPtr iconHandle = IntPtr.Zero;
            ImageSource imageSource = null;

            try
            {
                SHGetFileInfo(path, fileAttributes, ref shfi, SHFILEINFO.Size, flags);

                IImageList imageList = null;
                Guid guid = new Guid(IID_IImageList);

                SHGetImageList((int)iconSize, ref guid, ref imageList);
                imageList.GetIcon(shfi.iIcon, (int)(IMAGELISTDRAWFLAGS.ILD_TRANSPARENT | IMAGELISTDRAWFLAGS.ILD_IMAGE), ref iconHandle);

                using (Icon icon = (Icon)Icon.FromHandle(iconHandle).Clone())
                {
                    imageSource = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
                }
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
