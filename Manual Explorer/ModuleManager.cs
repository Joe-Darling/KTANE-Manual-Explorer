using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Manual_Explorer
{
    class ModuleManager
    {
        private static ModuleManager instance = null;
        DrawingManager drawingManager = new DrawingManager();
        private Dictionary<string, List<BitmapImage>> modules = new Dictionary<string, List<BitmapImage>>();
        private Dictionary<ImageSource, Canvas> savedDrawings = new Dictionary<ImageSource, Canvas>();

        private ModuleManager()
        {
            // For now we will just initialize all modules.
            string initialDirectory = "C:\\ManualHelper.Test";
            string secondaryDirectory = "F:\\V\\ManualHelper.Test";
            string[] allFiles;
            try
            {
                allFiles = Directory.GetFiles(initialDirectory, "*.bmp", SearchOption.AllDirectories);
            }
            catch(Exception e)
            {
               allFiles = Directory.GetFiles(secondaryDirectory, "*.bmp", SearchOption.AllDirectories);
            }
             
            BitmapImage bitmap = new BitmapImage();

            modules = new Dictionary<string, List<BitmapImage>>();

            foreach (string file in allFiles)
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                string moduleName = file.Substring(0, file.LastIndexOf('-'));
                moduleName = moduleName.Split('\\').Last().ToLower();
                if (modules.ContainsKey(moduleName))
                {
                    modules[moduleName].Add(bitmap);
                }
                else
                {
                    modules[moduleName] = new List<BitmapImage>() { bitmap };
                }
                Trace.WriteLine(moduleName);
            }
        }

        public static ModuleManager GetInstance()
        {
            if(instance == null)
            {
                instance = new ModuleManager();
            }

            return instance;
        }

        public bool DoesModuleExist(string moduleName)
        {
            return modules.ContainsKey(moduleName);
        }

        public Dictionary<string, List<BitmapImage>>.KeyCollection GetModuleNames()
        {
            return modules.Keys;
        }

        public List<BitmapImage> GetManualPages(string manual)
        {
            return modules[manual];
        }

        public static T ElementClone<T>(T element)
        {
            T clone = default(T);
            MemoryStream memStream = ElementToStream(element);
            clone = ElementFromStream<T>(memStream);
            return clone;
        }

        /// <summary>
        /// Saves an element as MemoryStream.
        /// </summary>
        public static MemoryStream ElementToStream(object element)
        {
            MemoryStream memStream = new MemoryStream();
            XamlWriter.Save(element, memStream);
            return memStream;
        }

        /// <summary>
        /// Rebuilds an element from a MemoryStream.
        /// </summary>
        public static T ElementFromStream<T>(MemoryStream elementAsStream)
        {
            object reconstructedElement = null;

            if (elementAsStream.CanRead)
            {
                elementAsStream.Seek(0, SeekOrigin.Begin);
                reconstructedElement = XamlReader.Load(elementAsStream);
                elementAsStream.Close();
            }

            return (T)reconstructedElement;
        }

        public void SaveDrawing(ImageSource currPage, Canvas drawing)
        {
            if (!savedDrawings.ContainsKey(currPage))
            {
                Canvas clonedCanvas = ElementClone<Canvas>(drawing);
                savedDrawings.Add(currPage, clonedCanvas);
            }
            Trace.WriteLine(savedDrawings.Count);
            //drawingManager.ClearPage(drawing);
        }

        public void CheckToSave(Canvas leftCanvas, Canvas rightCanvas, ImageSource leftPage, ImageSource rightPage)
        {
            if (CanvasContentCheck(leftCanvas))
            {
                SaveDrawing(leftPage, leftCanvas);//ConvertWriteableBitmapToBitmapImage(SaveAsWriteableBitmap(leftCanvas)));
            }
            if (CanvasContentCheck(rightCanvas))
            {
                SaveDrawing(rightPage, rightCanvas);//ConvertWriteableBitmapToBitmapImage(SaveAsWriteableBitmap(rightCanvas)));
            }
        }

        public bool CanvasContentCheck(Canvas canvas) //true if something is drawn
        {
            return canvas.Children.Count != 0;
        }

        public Canvas WhichCanvasToUse(ImageSource pageToLoad)
        {
            if (savedDrawings.ContainsKey(pageToLoad))
            {
                Trace.WriteLine(savedDrawings[pageToLoad].Children.Count);
                return savedDrawings[pageToLoad];
            }
            else
            {
                return new Canvas();
            }
        }
    }
}
