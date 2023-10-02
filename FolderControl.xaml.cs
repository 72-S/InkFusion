using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using static InkFusion.MainWindow;

namespace InkFusion
{
    public partial class FolderControl : UserControl
    {


        private static int instanceCounter = 0;
        private int instanceID;
        private string currentFolderName;

        public delegate void FolderDeleteHandler(string folderName);
        public event FolderDeleteHandler OnFolderDelete;

        public FolderControl()
        {
            InitializeComponent();
            instanceID = ++instanceCounter;  // Erhöhen des Zählers und Zuweisung einer ID für diese Instanz
            Console.WriteLine($"FolderControl constructor called. InstanceID: {instanceID}");
            this.MouseEnter += UserControl_MouseEnter;
            this.MouseLeave += UserControl_MouseLeave;

            this.Loaded += (sender, e) =>
            {
                Console.WriteLine("Loaded event triggered.");
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.RegisterFolderDeleteHandler(this);
                }

                if (this.DataContext is FolderInfo folderInfo)
                {
                    SetCurrentFolderName(folderInfo.Name);
                }
            };
        }

        public void SetCurrentFolderName(string folderName)
        {
            Console.WriteLine($"SetCurrentFolderName called with value: {folderName}");
            currentFolderName = folderName;
            Console.WriteLine($"currentFolderName set to: {currentFolderName}");
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("FolderButton_Click called.");
            FolderOptionsPopup.DataContext = this.DataContext;
            FolderOptionsPopup.IsOpen = true;
            var folderOptions = (FolderOptions)FolderOptionsPopup.Child;
            folderOptions.ParentFolderControl = this;
        }

        public void Edit_Folder_OK(string name, string color)
        {
            Console.WriteLine($"Edit_Folder_OK called. Current value of currentFolderName: {currentFolderName}");

            if (currentFolderName == null)
            {
                Console.WriteLine("currentFolderName is null.");
                return;
            }

            string foldername = currentFolderName;
            MainWindow.Instance.Edit_Folder(foldername, color, name);
        }

        public void DeleteFolder()
        {
            Console.WriteLine("DeleteFolder called.");
            if (DataContext is FolderInfo item)
            {
                OnFolderDelete?.Invoke(item.Name);
            }
        }

        public void ShowInExplorer()
        {
            Console.WriteLine("ShowInExplorer called.");
            if (DataContext is FolderInfo item)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.OpenFolderInExplorer(item.Name);
                }
            }
        }


        public void HidePopup()
        {   
            FolderOptionsPopup.IsOpen = false;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateColor("#3E3B3B");
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateColor("#1E1B1B");
        }

        private void AnimateColor(string toColor)
        {
            var rectangle = (Rectangle)this.FindName("HoverRectangle");
            if (rectangle != null)
            {
                ColorAnimation animation = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString(toColor),
                    Duration = new Duration(TimeSpan.FromMilliseconds(200))
                };

                Storyboard.SetTarget(animation, rectangle);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Fill.Color"));

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);

                storyboard.Begin();
            }
        }


    }
}