using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
using System.Linq;

namespace InkFusion
{
    public partial class MainWindow : Window
    {
        
        public FolderControl ActiveFolderControl { get; set; }
        public static MainWindow Instance { get; private set; }
        public static string currentDirectory;
        private string rootDirectory;
        private string configPath;
        private bool isFolder;
        
        public string CurrentDirectory
        {
            get { return currentDirectory; }
        }



        public MainWindow()
        {
            InitializeComponent();
            rootDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Journals");
            configPath = System.IO.Path.Combine(rootDirectory, "config.inkfc");  // Diese Zeile wurde aktualisiert
            currentDirectory = rootDirectory;
            NavigateToDirectory(rootDirectory);
            Instance = this;
        }

        public void RegisterFolderDeleteHandler(FolderControl control)
        {
            control.OnFolderDelete += DeleteFolderInMainWindow;
        }

        
        public void OpenFolderInExplorer(string folderName)
        {
            string folderPath = System.IO.Path.Combine(currentDirectory, folderName);
            if (Directory.Exists(folderPath))
            {
                string parentPath = Directory.GetParent(folderPath).FullName;
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{folderPath}\"");
            }
            else
            {
                Console.WriteLine("The Folder doenst exist");
            }
        }
        public class FolderInfo
        {
            public string Color { get; set; }
            public string Timestamp { get; set; }
            public DateTime TimestampDateTime { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
            public string Dir { get; set; }
        }


        private void DeleteFolderInMainWindow(string folderName)
        {
            string folderPath = Path.Combine(currentDirectory, folderName);
            if (Directory.Exists(folderPath))
            {
                try
                {
                    // Lade die bestehende Konfiguration
                    Dictionary<string, FolderInfo> folderConfig;
                    if (File.Exists(configPath))
                    {
                        folderConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, FolderInfo>>(File.ReadAllText(configPath));
                    }
                    else
                    {
                        folderConfig = new Dictionary<string, FolderInfo>();
                    }

                    // Rekursiv alle Ordner und deren Unterverzeichnisse entfernen
                    DeleteFolderAndSubfoldersFromConfig(folderPath, folderConfig);

                    // Speichere die aktualisierte Konfiguration
                    File.WriteAllText(configPath, Newtonsoft.Json.JsonConvert.SerializeObject(folderConfig));

                    // Lösche den Ordner vom Dateisystem
                    Directory.Delete(folderPath, true);

                    // Aktualisiere die Ansicht
                    NavigateToDirectory(currentDirectory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error deleting folder: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("Does Not exist");
            }
        }

        private void DeleteFolderAndSubfoldersFromConfig(string folderPath, Dictionary<string, FolderInfo> folderConfig)
        {
            if (folderConfig.ContainsKey(folderPath))
            {
                folderConfig.Remove(folderPath);
            }

            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                DeleteFolderAndSubfoldersFromConfig(dir, folderConfig);
            }
        }



        private void RootLink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink clickedLink = (Hyperlink)sender;
            NavigateToDirectory((string)clickedLink.Tag);
        }

        

        

        public void FilelistClick(object sender, EventArgs e)
        {
            try
            {
                
                if (fileListView.SelectedItem is FolderInfo item)
                {
                    if (isFolder == true)
                    {
                        NavigateToDirectory(System.IO.Path.Combine(currentDirectory, item.Name));
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Log or handle the exception
                Console.WriteLine("Exception: " + ex.Message);
            }
        }




        private void NavigateToDirectory(string path)
        {
            currentDirectory = path;
            List<FolderInfo> items = new List<FolderInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            // Load the configuration file
            Dictionary<string, FolderInfo> folderConfig = new Dictionary<string, FolderInfo>();
            if (File.Exists(configPath))
            {
                folderConfig = JsonConvert.DeserializeObject<Dictionary<string, FolderInfo>>(File.ReadAllText(configPath));
            }

            try
            {
                foreach (var dir in dirInfo.GetDirectories())
                {
                    string dirPath = dir.FullName;

                    if (folderConfig.ContainsKey(dirPath))
                    {
                        string color = folderConfig[dirPath].Color;
                        string timestamp = folderConfig[dirPath].Timestamp;
                        DateTime timestampDateTime = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", null);

                        items.Add(new FolderInfo
                        {
                            Name = dir.Name,
                            Image = $"/images/{color}.png",
                            Dir = dir.FullName,
                            Color = color,
                            Timestamp = timestamp,
                            TimestampDateTime = timestampDateTime
                        });
                    }
                    else
                    {
                        items.Add(new FolderInfo { Name = dir.Name, Image = "/images/default.png", Dir = dir.FullName });
                    }
                    isFolder = true;
                }

                foreach (var file in dirInfo.GetFiles("*.inkf"))
                {
                    items.Add(new FolderInfo { Name = file.Name, Image = "/images/folder.png", Dir = file.FullName });
                    isFolder = false;
                }
            }
            catch (Exception)
            {
                // Error handling here
            }
            items.Sort((x, y) => DateTime.Compare(y.TimestampDateTime, x.TimestampDateTime));
            fileListView.ItemsSource = items;

            // Remove old event handlers
            foreach (var item in fileListView.Items)
            {
                var container = fileListView.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container != null)
                {
                    var control = container.FindName("FolderControl") as FolderControl;
                    if (control != null)
                    {
                        control.OnFolderDelete -= DeleteFolderInMainWindow;
                    }
                }
            }

            // Add new event handlers
            foreach (var item in items)
            {
                var container = fileListView.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container != null)
                {
                    var control = container.FindName("FolderControl") as FolderControl;
                    if (control != null)
                    {
                        control.OnFolderDelete += DeleteFolderInMainWindow;
                    }
                }
            }

            LoadFolders();
        }


        public void LoadFolders()
        {


            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }



            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "{}");
            }
            else
            {
                Dictionary<string, FolderInfo> folderConfig =
                    JsonConvert.DeserializeObject<Dictionary<string, FolderInfo>>(File.ReadAllText(configPath));
            }

            string[] subfolders = Directory.GetDirectories(rootDirectory);

            if (subfolders.Length <= 0)
            {
                NoNotebooksPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                NoNotebooksPlaceholder.Visibility = Visibility.Collapsed;
                pathTextBlock.Inlines.Clear();

                if (currentDirectory != rootDirectory)
                {
                    Hyperlink rootLink = new Hyperlink(new Run("Journals"))
                    {
                        Tag = rootDirectory,
                        Style = (Style)FindResource("CustomHyperlinkStyle")
                    };
                    rootLink.Click += RootLink_Click;
                    pathTextBlock.Inlines.Add(rootLink);

                }
                else
                {
                    pathTextBlock.Inlines.Add("Journals");
                }

                string displayPath = currentDirectory.Substring(rootDirectory.Length);
                string[] parts = displayPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string pathSoFar = rootDirectory;

                if (parts.Length > 0)
                {
                    pathTextBlock.Inlines.Add(" > ");
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    pathSoFar = System.IO.Path.Combine(pathSoFar, parts[i]);
                    if (pathSoFar != currentDirectory)
                    {
                        Hyperlink link = new Hyperlink(new Run(parts[i]))
                        {
                            Tag = pathSoFar,
                            Style = (Style)FindResource("CustomHyperlinkStyle")
                        };
                        link.Click += (sender, e) =>
                        {
                            Hyperlink clickedLink = (Hyperlink)sender;
                            NavigateToDirectory((string)clickedLink.Tag);
                        };
                        pathTextBlock.Inlines.Add(link);
                    }
                    else
                    {
                        pathTextBlock.Inlines.Add(new Run(parts[i]));
                    }

                    if (i < parts.Length - 1)
                    {
                        pathTextBlock.Inlines.Add(" > ");
                    }
                }
            }
        }

        public void CreateNewFolder(string newFolderName, string color)
        {
            // Erstelle den neuen Ordnerpfad
            string newFolderPath = Path.Combine(currentDirectory, newFolderName);
            newFolderName = newFolderName.TrimEnd('.');
            // Erstelle den neuen Ordner
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }

            // Lade die bestehende Konfiguration
            Dictionary<string, FolderInfo> folderConfig;
            if (File.Exists(configPath))
            {
                folderConfig = JsonConvert.DeserializeObject<Dictionary<string, FolderInfo>>(File.ReadAllText(configPath));
            }
            else
            {
                folderConfig = new Dictionary<string, FolderInfo>();
            }

            // Füge die Informationen für den neuen Ordner hinzu oder aktualisiere sie
            folderConfig[newFolderPath] = new FolderInfo
            {
                Name = newFolderName,
                Dir = newFolderPath,
                Color = color,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TimestampDateTime = DateTime.Now
            };

            // Speichere die aktualisierte Konfiguration
            File.WriteAllText(configPath, JsonConvert.SerializeObject(folderConfig));

            // Aktualisiere die Ansicht
            NavigateToDirectory(currentDirectory);
        }
        
        


        public void Edit_Folder(string folderName, string color, string newName)
        {
            newName = newName.TrimEnd('.');
            string folderPath = Path.Combine(currentDirectory, folderName);
            string newfolderPath = currentDirectory +"\\"+ newName;
            Console.WriteLine(folderPath, newfolderPath);
            Dictionary<string, FolderInfo> folderConfig;
            if (File.Exists(configPath))
            {
                folderConfig = JsonConvert.DeserializeObject<Dictionary<string, FolderInfo>>(File.ReadAllText(configPath));
            }
            else
            {
                folderConfig = new Dictionary<string, FolderInfo>();
            }

            if (Directory.Exists(folderPath) && folderConfig.ContainsKey(folderPath))
            {
                Directory.Move(folderPath, newfolderPath);

                FolderInfo oldFolderInfo = folderConfig[folderPath];
                folderConfig.Remove(folderPath);

                FolderInfo newFolderInfo = new FolderInfo
                {
                    Name = newName,
                    Dir = newfolderPath,
                    Color = color,
                    Timestamp = oldFolderInfo.Timestamp,
                    TimestampDateTime = oldFolderInfo.TimestampDateTime
                };
                folderConfig[newfolderPath] = newFolderInfo;

                File.WriteAllText(configPath, JsonConvert.SerializeObject(folderConfig));
                NavigateToDirectory(currentDirectory);
            }
            else
            {
                Console.WriteLine($"Folder {folderPath} does not exist in either the file system or the config.");
            }
        }

        


        private void New_Folder_Click(object sender, EventArgs e)
        {
            CreateFolder.Visibility = Visibility.Visible;
            OverlayBackground.Visibility = Visibility.Visible;

        }

        
        public void New_Folder_Cancel()
        {
            
            CreateFolder.Visibility = Visibility.Hidden;
            OverlayBackground.Visibility = Visibility.Collapsed;
        }

        public void Edit_Folder_Click()
        {
            if (ActiveFolderControl == null)
            {
                Console.WriteLine("Keine FolderControl-Instanz ausgewählt.");
                return;
            }

            // Setzen Sie den Status des EditFolder UserControls auf sichtbar
            EditFolder.Visibility = Visibility.Visible;
            OverlayBackground.Visibility = Visibility.Visible;

            // Übergabe der aktuellen Folder-Informationen an das EditFolder UserControl
            if (ActiveFolderControl.DataContext is FolderInfo folderInfo)
            {
                EditFolder.AssociatedFolderInfo = folderInfo;
            }
        }


        public void Edit_Folder_Cancel()
        {
            
            EditFolder.Visibility = Visibility.Collapsed;
            OverlayBackground.Visibility = Visibility.Collapsed;
        }
        private void New_Notebook_Click(object sender, EventArgs e)
        {
            NavigateToDirectory(currentDirectory);

        }


    }

    
}