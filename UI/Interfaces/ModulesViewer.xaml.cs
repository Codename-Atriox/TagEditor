using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infinite_module_test;

namespace TagEditor.UI.Interfaces
{
    /// <summary>
    /// Interaction logic for ModulesViewer.xaml
    /// </summary>
    public partial class ModulesViewer : UserControl{
        MainWindow main;
        public ModulesViewer(MainWindow _main){
            main = _main;
            InitializeComponent();
        }
        List<string> modules_names = new();
        List<module_structs.module> active_modules = new();
        module_structs.module? selected_module = null;
        public void update_modules(List<module_structs.module> new_modules){
            active_modules = new_modules;
            modules_names.Clear();

            foreach (var module in new_modules)
                modules_names.Add(System.IO.Path.GetFileName(module.module_file_path));

            modulesview.SelectedIndex = -1;
            modulesview.ItemsSource = modules_names;
        }
        private void modulesview_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (modulesview.SelectedIndex <= 0){
                selected_module = null;
                files_count.Text = "Files: NULL";
                resources_count.Text = "Resources: NULL";
                return;}
            if (modulesview.SelectedIndex >= active_modules.Count)
                return; // should be an exception but whatever
            selected_module = active_modules[modulesview.SelectedIndex];
            // update selected module info

            files_count.Text = "Files: " + selected_module.module_info.FileCount.ToString();
            resources_count.Text = "Resources: " + selected_module.module_info.ResourceCount.ToString();
        }

        private void Button_UnpackModule(object sender, RoutedEventArgs e){
            if (selected_module == null) return;
        }
        private void Button_AddTag(object sender, RoutedEventArgs e){
            if (selected_module == null) return;
        }
        private void Button_CompileModule(object sender, RoutedEventArgs e){
            if (selected_module == null) return;
        }

    }
}
