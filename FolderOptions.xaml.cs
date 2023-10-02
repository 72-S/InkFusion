using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace InkFusion
{
    public partial class FolderOptions : UserControl
    {
        private string folderPath;

        public FolderControl ParentFolderControl { get; set; }
        

        public FolderOptions()
        {
            InitializeComponent();
        }

        private void Customize(object sender, RoutedEventArgs e)
        {
            FolderControl parentFolderControl = ParentFolderControl;
            parentFolderControl.HidePopup();
            MainWindow.Instance.Edit_Folder_Click();
            

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

        


    
