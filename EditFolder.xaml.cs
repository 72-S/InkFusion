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
using static InkFusion.MainWindow;

namespace InkFusion
{
    /// <summary>
    /// Interaction logic for EditFolder.xaml
    /// </summary>
    public partial class EditFolder : UserControl
    {
        private FolderInfo _associatedFolderInfo;
        public FolderInfo AssociatedFolderInfo
        {
            get { return _associatedFolderInfo; }
            set
            {
                _associatedFolderInfo = value;
                EditFolder_Loaded();
            }
        }

        

        private Ellipse selectedEllipse;
        private string selectedColor;
        private string nextName;
        public EditFolder()
        {
            InitializeComponent();
            selectedColor = "red";
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


        private void SelectEllipseBasedOnColor(string color)
        {
            // Liste der Ellipsennamen und ihre Farben
            Dictionary<string, string> colorToEllipseMap = new Dictionary<string, string>
            {
                { "red", "RedEllipse" },
                { "purple", "PurpleEllipse" },
                { "blue", "BlueEllipse" },
                { "pink", "PinkEllipse" },
                { "green", "GreenEllipse" },
                { "yellow", "YellowEllipse" },
                { "violet", "VioletEllipse" },
                { "orange", "OrangeEllipse" },
                { "cyan", "CyanEllipse" }
            };

            // Setzen Sie die Größe aller Ellipsen zurück
            foreach (var ellipseName in colorToEllipseMap.Values)
            {
                Ellipse ellipse = (Ellipse)this.FindName(ellipseName);
                if (ellipse != null)
                {
                    ellipse.Width = 28;
                    ellipse.Height = 28;
                }
            }

            // Finden Sie die ausgewählte Ellipse und setzen Sie ihre Größe
            if (colorToEllipseMap.ContainsKey(color.ToLower()))
            {
                string selectedEllipseName = colorToEllipseMap[color.ToLower()];
                Ellipse selectedEllipse = (Ellipse)this.FindName(selectedEllipseName);
                if (selectedEllipse != null)
                {
                    selectedEllipse.Width = 40;
                    selectedEllipse.Height = 40;
                    this.selectedEllipse = selectedEllipse;  // Setzen Sie die globale Variable
                    this.selectedColor = color.ToLower();    // Setzen Sie die globale Variable
                }
            }
        }

        public void EditFolder_Loaded()
        {
            Console.WriteLine("EditFolder_Loaded called.");
            if (AssociatedFolderInfo != null)
            {
                Console.WriteLine($"AssociatedFolderInfo.Name = {AssociatedFolderInfo.Name}, AssociatedFolderInfo.Color = {AssociatedFolderInfo.Color}");
                FolderNameTextBox.Text = AssociatedFolderInfo.Name;
                SelectEllipseBasedOnColor(AssociatedFolderInfo.Color);
            }
            else
            {
                Console.WriteLine("AssociatedFolderInfo is null.");
            }
        }



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



        private void Test(object sender, EventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            string newName = FolderNameTextBox.Text.Trim();
            newName = newName.TrimEnd('.');
            
            // Überprüfen, ob der neue Name leer ist oder dem alten Namen entspricht
            if (string.IsNullOrEmpty(newName) || newName.Equals(AssociatedFolderInfo.Name, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Der neue Name ist entweder leer oder identisch mit dem alten Namen. Änderungen werden nicht gespeichert.");
                MainWindow.Instance.Edit_Folder_Cancel();
                return;
            }

            if (mainWindow != null && mainWindow.ActiveFolderControl != null)
            {
                mainWindow.Edit_Folder(AssociatedFolderInfo.Name, selectedColor, newName);
                MainWindow.Instance.Edit_Folder_Cancel();
            }
            
        }




        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.Edit_Folder_Cancel();
            if (selectedEllipse != null)
            {
                selectedEllipse.Width = 28;
                selectedEllipse.Height = 28;
            }
        }
    }


}
