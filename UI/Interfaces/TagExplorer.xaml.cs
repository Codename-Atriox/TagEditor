using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TagEditor.UI.Windows{
    public partial class TagExplorer : System.Windows.Controls.UserControl{
        public MainWindow main; // only used for output. IE sending a call to the main window to open a tag, errors
        public TagExplorer(MainWindow _main){
            main = _main;
            InitializeComponent();
            fileTemplate = (DataTemplate)FindResource("FileItem");
        }
        DataTemplate fileTemplate = null;
        public void OpenDirectory(bool clear_previous){
            System.Windows.Forms.FolderBrowserDialog openFolderDialogue = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFolderDialogue.ShowDialog();
            string directory = openFolderDialogue.SelectedPath;
            if ((result == DialogResult.OK) && (!string.IsNullOrWhiteSpace(directory)) && Directory.Exists(directory)){
                if (clear_previous) Button_CloseDirectory(null, null); // only due of formality, else we'd have that single line
                tag_view.Items.Add(CreateTreeDirectory(new DirectoryInfo(directory)));
        }}

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e){
            TreeViewItem item = e.Source as TreeViewItem;
            if (item.Tag is DirectoryInfo){
                if (item.Items.Count != 1 || item.Items[0] != "Loading...") return;
                item.Items.Clear();
                DirectoryInfo expandedDir = (item.Tag as DirectoryInfo);
                try{foreach (DirectoryInfo subDir in expandedDir.GetDirectories()) item.Items.Add(CreateTreeDirectory(subDir));
                    foreach (FileInfo subDir in expandedDir.GetFiles()){
                        var new_item = CreateTreeFile(subDir);
                        item.Items.Add(new_item);
                        if ((!FilenameMatches(subDir.Name)) || (subDir.Name.Contains("[") && (ShowResources.IsChecked != true))) new_item.Visibility = Visibility.Collapsed;
                }} catch { main.DisplayNote("Unable to open child of "+ expandedDir.FullName, null, MainWindow.error_level.NOTE);}}
            else main.TagViewer_OpenTag(item.Tag as string);
        }
        public void TreeViewItem_Closed(object sender, RoutedEventArgs e){
            if (ClearChildren.IsChecked == true){ // heck you nullable boolean
                TreeViewItem item = e.Source as TreeViewItem;
                item.Items.Clear();
                item.Items.Add("Loading...");
        }}
        private TreeViewItem CreateTreeDirectory(DirectoryInfo o){
            TreeViewItem item = CreateTreeDir(o);
            item.Items.Add("Loading...");
            return item;
        }
        private TreeViewItem CreateTreeDir(DirectoryInfo o)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = o.Name;
            item.Tag = o;
            return item;
        }
        private TreeViewItem CreateTreeFile(FileInfo o){
            TreeViewItem item = new TreeViewItem();
            item.Header = o.Name;
            item.HeaderTemplate = fileTemplate;
            item.Tag = o.FullName;
            return item;
        }
        private void Button_OpenAll(object sender, RoutedEventArgs e) => Recurs_OpenAll(tag_view.Items);
        private void Recurs_OpenAll(ItemCollection items){
            foreach (System.Windows.Controls.TreeViewItem item in items){
                if (item.Items.Count == 0 || item.IsExpanded == true) return;
                item.IsExpanded = true;
                TreeViewItem_Expanded(null, new RoutedEventArgs(null, item));
                Recurs_OpenAll(item.Items);
        }}
        private void Button_CloseAll(object sender, RoutedEventArgs e){
            foreach(TreeViewItem item in tag_view.Items){
                item.IsExpanded = false;
                item.Items.Clear();
                item.Items.Add("Loading...");
        }}
        private void Button_OpenDirectory(object sender, RoutedEventArgs e) => OpenDirectory(true);
        private void Button_AddDirectory(object sender, RoutedEventArgs e) => OpenDirectory(false);
        private void Button_CloseDirectory(object sender, RoutedEventArgs e) => tag_view.Items.Clear();

        private void Toggle_ShowResources(object sender, RoutedEventArgs e) => Recurs_ToggleResources(tag_view.Items);
        private void Recurs_ToggleResources(ItemCollection items){
            foreach (System.Windows.Controls.TreeViewItem item in items){
                DirectoryInfo? expandedDir = item.Tag as DirectoryInfo;
                if (expandedDir != null && item.IsExpanded) Recurs_ToggleResources(item.Items);
                else if ((item.Header as string).Contains("[")) item.Visibility = (ShowResources.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
        }}
        // its filtern time, enter key must be entered when searching all mode
        private void TextBlock_KeyDown(object sender, System.Windows.Input.KeyEventArgs e){
            if (SearchAll.IsChecked == true && e.Key == Key.Enter) SearchMatches(tag_view.Items);}
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e){
            if (SearchAll.IsChecked != true) SearchMatches(tag_view.Items);}
        public bool SearchMatches(ItemCollection items){
            bool output = false;
            foreach (System.Windows.Controls.TreeViewItem item in items)
                output |= SearchMatch(item);
            return output;
        }
        public bool SearchMatch(TreeViewItem item){
            DirectoryInfo? expandedDir = item.Tag as DirectoryInfo;
            string name = item.Header as string;
            bool had_match = false;
            if (expandedDir != null){
                if ((SearchFolderNames.IsChecked == true) && name_matches(name)) had_match = true;
                if (!item.IsExpanded && (SearchAll.IsChecked == false)) goto END; // nothing more to check
                if (item.Items.Count == 0) goto END; // no children to check
                if (item.Items[0] != "Loading..."){ // if the children are already loaded
                    if (!SearchMatches(item.Items)) 
                        goto END;
                    had_match = true;
                    item.IsExpanded = true; 
                }else{ // if the children are not loaded, load them
                    TreeViewItem? result = SearchClosedMatch(expandedDir);
                    if (result == null) goto END; // no children, go to end
                    item.Items.Clear();
                    item.IsExpanded = true;
                    had_match = true;
                    for (int i = 0; i < result.Items.Count; i++){ // BIG MASSIVE BALLS, JESUS ON A TRACTOR
                        TreeViewItem v = result.Items[i] as TreeViewItem;
                        result.Items.RemoveAt(i);
                        item.Items.Add(v);
            }}}
            else if (FilenameMatches(name)){ // can't use goto false, cause thats reserved for folders
                item.Visibility = Visibility.Visible;
                return true;
            }
            else
            {
                item.Visibility = Visibility.Collapsed;
                return false;
            }
        END:
            if (had_match) item.Visibility = Visibility.Visible;
            else if (HideFolders.IsChecked == true) item.Visibility = Visibility.Collapsed;
            return had_match;
        }
        public TreeViewItem? SearchClosedMatch(DirectoryInfo directory){
            List<TreeViewItem> matched_children = new();
            bool match = false;
            foreach (DirectoryInfo subDir in directory.GetDirectories()){
                TreeViewItem? subItem = SearchClosedMatch(subDir);
                if (subItem != null){
                    matched_children.Add(subItem);
                    match = true;
            }}
            if (!match){
                foreach (FileInfo file in directory.GetFiles()) 
                    if (FilenameMatches(file.Name) && ((!file.Name.Contains("[")) || ShowResources.IsChecked == true)) match = true;
                if ((SearchFolderNames.IsChecked == true) && name_matches(directory.Name)) match = true;
            }
            if (match){
                TreeViewItem output = CreateTreeDir(directory);
                output.IsExpanded = true;
                foreach (DirectoryInfo subDir in directory.GetDirectories()){
                    bool was_found_before = false;
                    for (int i = 0; i < matched_children.Count; i++){
                        TreeViewItem matched_item = matched_children[i];
                        if (subDir.Name == matched_item.Header as string){
                            was_found_before = true;
                            output.Items.Add(matched_item);
                            matched_item.IsExpanded = true;
                            matched_children.RemoveAt(i); // cleanup for a little bit of efficiency
                            break;
                    }}
                    if (!was_found_before){
                        TreeViewItem hiddenitem = CreateTreeDirectory(subDir);
                        if (HideFolders.IsChecked == true) hiddenitem.Visibility = Visibility.Collapsed;
                        output.Items.Add(hiddenitem);
                }}
                foreach (FileInfo subFile in directory.GetFiles()){
                    TreeViewItem new_item = CreateTreeFile(subFile);
                    output.Items.Add(new_item);
                    if (!FilenameMatches(subFile.Name) || (subFile.Name.Contains("[") && (ShowResources.IsChecked == false)))
                        new_item.Visibility = Visibility.Collapsed;
                }
                return output;
            }
            return null;
        }
        public bool FilenameMatches(string name){
            if ((ShowResources.IsChecked == true) || (!name.Contains("["))) if (name_matches(name)) return true;
            return false;
        }

        private void HideFolders_Click(object sender, RoutedEventArgs e){
            if (HideFolders.IsChecked != true)
                Recurse_UnhideFolders(tag_view.Items);
        }
        private void Recurse_UnhideFolders(ItemCollection items){
            foreach (System.Windows.Controls.TreeViewItem item in items){
                DirectoryInfo? expandedDir = item.Tag as DirectoryInfo;
                if (expandedDir != null){
                    item.Visibility = Visibility.Visible;
                    Recurse_UnhideFolders(item.Items);
        }}}
        private bool name_matches(string name) => name?.IndexOf(SearchBox.Text, (CaseSensitve.IsChecked == true) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;

        private void Button_ClearSearch(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

    }
}
