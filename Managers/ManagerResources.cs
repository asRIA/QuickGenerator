using System;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace QuickGenerator
{
    class ManagerResources:IDisposable
    {
        public static Bitmap MethodImage;
        public static Bitmap ImportImage;
        public static Bitmap ClipBoardImage;
        public static Bitmap EmptyBitmap;
        public static Bitmap AbbreviationBitmap;
        public static Bitmap GoToAbbreviationBitmap;
        public static Bitmap ClassImage;

        public  ManagerResources()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            // QuickGenerator 
            Stream st = assembly.GetManifestResourceStream("QuickGenerator.Resources.Method.png");

            MethodImage = new Bitmap(st);

            st = assembly.GetManifestResourceStream("QuickGenerator.Resources.Package.png");

            ImportImage = new Bitmap(st);

            st = assembly.GetManifestResourceStream("QuickGenerator.Resources.clipboard-icon.png");

            ClipBoardImage = new Bitmap(st);


            st = assembly.GetManifestResourceStream("QuickGenerator.Resources.Character-Map-icon.png");

            AbbreviationBitmap = new Bitmap(st);


            st = assembly.GetManifestResourceStream("QuickGenerator.Resources.goto_arrow.png");

            GoToAbbreviationBitmap = new Bitmap(st);


            EmptyBitmap = new Bitmap(1, 1);


            st = assembly.GetManifestResourceStream("QuickGenerator.Resources.Class.png");

            ClassImage = new Bitmap(st);


        }



        #region IDisposable 

        public void Dispose()
        {
            MethodImage.Dispose();
            MethodImage = null;
            ImportImage.Dispose();
            ImportImage = null;
            ClipBoardImage.Dispose();
            ClipBoardImage = null;
            EmptyBitmap.Dispose();
            EmptyBitmap = null;
            AbbreviationBitmap.Dispose();
            AbbreviationBitmap = null;

            GoToAbbreviationBitmap.Dispose();
            GoToAbbreviationBitmap = null;
            ClassImage.Dispose();
            ClassImage = null;
        }

        #endregion
    }
}
