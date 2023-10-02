using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace InkFusion
{
    public partial class FolderOptions : UserControl
    {
        

        public FolderControl ParentFolderControl { get; set; }
        

        public FolderOptions()
        {
            InitializeComponent();
        }

        private void Customize(object sender, RoutedEventArgs e)
        {
            FolderControl parentFolderControl = ParentFolderControl;
            parentFolderControl.HidePopup();

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ActiveFolderControl = parentFolderControl;
            }

            MainWindow.Instance.Edit_Folder_Click();
            EditFolder editFolder = new EditFolder();
            editFolder.EditFolder_Loaded();
        }



        private void DeleteFolder(object sender, RoutedEventArgs e)
        {
            FolderControl parentFolderControl = ParentFolderControl;
            parentFolderControl.DeleteFolder();
        }


        private void ShowInFolder(object sender, RoutedEventArgs e)
        {
            FolderControl parentFolderControl = ParentFolderControl;
            parentFolderControl.ShowInExplorer();
        }



         }
        }

        


    
