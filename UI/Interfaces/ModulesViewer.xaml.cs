﻿using System;
using System.Collections;
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
        string[] module_names_display = new string[0];
        List<string> modules_names = new();
        List<module_structs.module> active_modules = new();
        module_structs.module? selected_module = null;
        public void update_modules(List<module_structs.module> new_modules){
            active_modules = new_modules;
            modules_names.Clear();

            foreach (var module in new_modules)
                modules_names.Add(module.module_name);

            module_names_display = modules_names.ToArray();
            modulesview.SelectedIndex = -1;
            modulesview.ItemsSource = module_names_display;
        }
        public void update_module_stats(){
            if (selected_module == null){
                files_count.Text = "Files: NULL";
                changes_count.Text = "Tags Committed: NULL";
            }else {
                int int_files_count = selected_module.module_info.FileCount;
                int int_resources_count = selected_module.module_info.ResourceCount;
                int int_tags_count = int_files_count - int_resources_count;
                files_count.Text = "Files: " + int_tags_count + "\"" + int_resources_count + "\"" + int_files_count;
                // count number of changes ready to be compiled
                int comitted_tags = 0;
                foreach(var v in selected_module.file_groups)
                    foreach(var file in v.Value)
                        if (file.file.blocks != null && file.file.has_been_edited == true) // we only commit tags that are loaded & edited
                            comitted_tags++;
                changes_count.Text = "Tags Committed: " + comitted_tags;
            }
        }
        private void modulesview_SelectionChanged(object sender, SelectionChangedEventArgs e){
            // error checking
            if (modulesview.SelectedIndex < 0) selected_module = null;
            else if (modulesview.SelectedIndex >= active_modules.Count) selected_module = null; // we should put an exception here
            else selected_module = active_modules[modulesview.SelectedIndex];
            update_module_stats();
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
        private void Button_CopyModule(object sender, RoutedEventArgs e){
            if (selected_module == null) return;
            StringBuilder sb = new();
            sb.AppendLine(selected_module.module_name);
            sb.AppendLine("Head: " + selected_module.module_info.Head);
            sb.AppendLine("Version: " + selected_module.module_info.Version);
            sb.AppendLine("ModuleId: " + selected_module.module_info.ModuleId);
            sb.AppendLine("FileCount: " + selected_module.module_info.FileCount);
            sb.AppendLine("Manifest00_index: " + selected_module.module_info.Manifest00_index);
            sb.AppendLine("Manifest01_index: " + selected_module.module_info.Manifest01_index);
            sb.AppendLine("Manifest02_index: " + selected_module.module_info.Manifest02_index);
            sb.AppendLine("ResourceIndex: " + selected_module.module_info.ResourceIndex);
            sb.AppendLine("StringsSize: " + selected_module.module_info.StringsSize);
            sb.AppendLine("ResourceCount: " + selected_module.module_info.ResourceCount);
            sb.AppendLine("BlockCount: " + selected_module.module_info.BlockCount);
            sb.AppendLine("BuildVersion: " + selected_module.module_info.BuildVersion);
            sb.AppendLine("Checksum: " + selected_module.module_info.Checksum);
            sb.AppendLine("Unk_0x040: " + selected_module.module_info.Unk_0x040);
            sb.AppendLine("Unk_0x044: " + selected_module.module_info.Unk_0x044);
            sb.AppendLine("Unk_0x048: " + selected_module.module_info.Unk_0x048);
            sb.AppendLine("Unk_0x04C: " + selected_module.module_info.Unk_0x04C);
            Clipboard.SetText(sb.ToString());
        }

    }
}