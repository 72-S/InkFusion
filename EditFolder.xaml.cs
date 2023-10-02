using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InkFusion
{
    /// <summary>
    /// Interaction logic for EditFolder.xaml
    /// </summary>
    public partial class EditFolder : UserControl
    {

        private bool isMouseOverEllipse = false;
        private Ellipse selectedEllipse;
        private string selectedColor;
        public EditFolder()
        {
            InitializeComponent();
            selectedColor = "red";
        }

        private void FolderNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FolderNameTextBox.Text == "Enter a name...")
            {
                FolderNameTextBox.Text = "";
            }
        }

        private void ClearTextBox_Click(object sender, RoutedEventArgs e)
        {
            FolderNameTextBox.Text = "";
        }

        private void ColorSelected_Red(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "red");
        private void ColorSelected_Purple(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "purple");
        private void ColorSelected_Blue(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "blue");
        private void ColorSelected_Pink(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "pink");
        private void ColorSelected_Green(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "green");
        private void ColorSelected_Yellow(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "yellow");
        private void ColorSelected_Violet(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "violet");
        private void ColorSelected_Orange(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "orange");
        private void ColorSelected_Cyan(object sender, MouseButtonEventArgs e) => ColorSelected(sender, e, "cyan");

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {


                var originalBrush = ellipse.Fill as SolidColorBrush;
                if (originalBrush != null)
                {
                    var originalColor = originalBrush.Color;
                    var darkerColor = Color.FromArgb(originalColor.A,
                        (byte)(originalColor.R * 0.8),
                        (byte)(originalColor.G * 0.8),
                        (byte)(originalColor.B * 0.8));

                    ellipse.Fill = new SolidColorBrush(darkerColor);
                }
            }
        }


        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {



                var darkerBrush = ellipse.Fill as SolidColorBrush;
                if (darkerBrush != null)
                {
                    var darkerColor = darkerBrush.Color;
                    var originalColor = Color.FromArgb(darkerColor.A,
                        (byte)(darkerColor.R / 0.8),
                        (byte)(darkerColor.G / 0.8),
                        (byte)(darkerColor.B / 0.8));

                    ellipse.Fill = new SolidColorBrush(originalColor);
                }
            }
        }


        private void ColorSelected(object sender, MouseButtonEventArgs e, string color)
        {
            if (selectedEllipse != null)
            {
                selectedEllipse.Width = 28;
                selectedEllipse.Height = 28;
            }

            selectedEllipse = sender as Ellipse;
            selectedEllipse.Width = 40;
            selectedEllipse.Height = 40;
            selectedColor = color;

        }

        private void OkButton_CLick(object sender, EventArgs e)
        {
            string folderName = FolderNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(folderName) || folderName == "Enter a name...")
            {
                folderName = "Unnamed";
            }

            // Überprüfen, ob der Ordnername bereits existiert und ggf. eine Nummer anhängen
            folderName = GenerateUniqueFolderName(folderName);

            // Wenn keine Farbe ausgewählt wurde, verwenden Sie Rot als Standardfarbe
            if (string.IsNullOrEmpty(selectedColor))
            {
                selectedColor = "red";
            }

            MainWindow.Instance.New_Folder_Cancel();
            MainWindow.Instance.CreateNewFolder(folderName, selectedColor);

            if (selectedEllipse != null)
            {
                selectedEllipse.Width = 28;
                selectedEllipse.Height = 28;
            }

            FolderNameTextBox.Text = "Enter a name...";
            selectedColor = "red";
        }


        private void Test(object sender, EventArgs e)
        {
            FolderControl newFolderControl = new FolderControl();
            newFolderControl.Edit_Folder_OK();
        }
        private string GenerateUniqueFolderName(string baseName)
        {
            string folderName = baseName;
            int counter = 1;

            while (Directory.Exists(System.IO.Path.Combine(MainWindow.Instance.CurrentDirectory, folderName)))
            {
                folderName = $"{baseName}{counter}";
                counter++;
            }

            return folderName;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.Edit_Folder_Cancel();
            FolderNameTextBox.Text = "Enter a name...";
            if (selectedEllipse != null)
            {
                selectedEllipse.Width = 28;
                selectedEllipse.Height = 28;
            }
        }
    }


}
