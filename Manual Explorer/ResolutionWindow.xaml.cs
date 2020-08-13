using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for ResolutionWindow.xaml
    /// </summary>
    public partial class ResolutionWindow : Window, INotifyPropertyChanged
    {

        double screenWidth;
        double screenHeight;

        double windowWidth;
        double windowHeight;
        SetResolution resolutionSetter = new SetResolution();
        public ResolutionWindow()
        {
            InitializeComponent();
            this.DataContext = resolutionSetter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double getHeight; //getHeight

        public double SetHeight //Height
        {
            get { return getHeight; } //getHeight
            set { getHeight = value; } //getHeight
        }

        private double getWidth; //getWIdth

        public double SetWidth
        {
            get { return getWidth; } //getWidth

            set
            {
                if (value != getWidth) //getWidth
                {
                    getWidth = value; //getWidth
                }
            }
        }

        private void ResizeWindow(object sender, RoutedEventArgs e) //button click
        {
            screenWidth = Int16.Parse(Get_Screen_Width.Text);
            screenHeight = Int16.Parse(Get_Screen_Height.Text);


            windowWidth = (900 * screenWidth) / 1920;
            windowHeight = (700 * screenHeight) / 1080;

            //resolutionSetter.Width = windowWidth;
            //resolutionSetter.Height = windowHeight;
            //MessageBox.Show("Height: " + windowHeight);
            //MessageBox.Show("Width: " + windowWidth);
        }

        //public double setWidth()
        //{
        //    windowWidth = (900 * screenWidth) / 1920;
        //    resolutionSetter.Width = windowWidth;
        //    return windowWidth;
        //}
        //public double setHeight()
        //{
        //    return windowHeight = (900 * screenHeight) / 1920;
        //}
    }

    public class SetResolution
    {

        

    }
}
