﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Controls;
using static Infinite_module_test.tag_structs;
using static TagEditor.MainWindow;


namespace TagEditor.UI.Windows{
    public partial class TagViewer : UserControl{
        public TagViewer(MainWindow _main){
            InitializeComponent();
            main = _main;
        }
        MainWindow main;
        Dictionary<string, TagInstance> Tabs = new();
        public void OpenTag(string tag_path, string plugins_path){
            if (Tabs.ContainsKey(tag_path)){
                TagInstance target_tab = Tabs[tag_path];
                TagsTabs.SelectedIndex = TagsTabs.Items.IndexOf(target_tab.container);
                return;
            }
            if (!File.Exists(tag_path)){ // fail via file not valid
                main.DisplayNote(tag_path + " is not a valid file", null, error_level.WARNING);
                return;
            }
            using (FileStream fs = new FileStream(tag_path, FileMode.Open)){
                try{byte[] bytes = new byte[4];
                    fs.Read(bytes, 0, 4);
                    if (!bytes.SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 })){ // fail via file not a tag file
                        main.DisplayNote(tag_path + " is not a tag file", null, error_level.WARNING);
                        return;
                }}catch{ // fail via error opening file
                    main.DisplayNote(tag_path + " failed to open", null, error_level.WARNING);
                    return;
            }}
            // check to see if the name is a resource file name
            if (tag_path.Contains("[")){
                main.DisplayNote(tag_path + " appears to be a tag struct resource file, not a real tag", null, error_level.WARNING);
                return;
            }
            // error checking past, open tag for real

            // lets first get a list of all the resource file that this guy probably owns
            var folder = Path.GetDirectoryName(tag_path);
            string tag_file_name = Path.GetFileName(tag_path);
            List<KeyValuePair<byte[], bool>> resource_list = new();
            
            foreach (var item in Directory.GetFiles(folder)){
                string file_name = Path.GetFileName(item);
                if (file_name.Length > tag_file_name.Length+4 && file_name.StartsWith(tag_file_name)){
                    // get index of file, just incase the function that retrives all the files doesn't do it alphabetically
                    // then either insert or add 

                    string resource_number = file_name.Substring(tag_file_name.Length + 5);
                    int resource_index = Convert.ToInt32(resource_number);

                    try{
                        byte[] resource_bytes = File.ReadAllBytes(item);
                        // test whether the first 4 bytes are the 'hscu' magic (or whatever its supposed to be)
                        bool is_standalone_resource = resource_bytes[0..4].SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 });
                        // pop it at the end if the currently index is too high (this is probably a terrible idea)
                        if (resource_index >= resource_list.Count) resource_list.Add(new KeyValuePair<byte[], bool>(resource_bytes, is_standalone_resource));
                        else                    resource_list.Insert(resource_index, new KeyValuePair<byte[], bool>(resource_bytes, is_standalone_resource));
                    }catch{ main.DisplayNote("resource file: \"" + item + "\" is unable to be opened, disregarding", null, error_level.WARNING);}
            }}
            // anomaly check // make sure all entries are of either type, else this will become very difficult to manage
            if (resource_list.Count > 0){
                bool inital = resource_list[0].Value;
                foreach (var item in resource_list){
                    if (item.Value != inital){
                        main.DisplayNote(item + " does not have a matching chunked/non-chunked status, please submit this scenario to the C:A developers", null, error_level.WARNING);
            }}}
            


            tag test = new tag(plugins_path, resource_list);
            try{
                byte[] tagbytes = File.ReadAllBytes(tag_path);
                if (!test.Load_tag_file(tagbytes)){
                    main.DisplayNote(tag_path + " was not able to be loaded as a tag", null, error_level.WARNING);
                    return;
            }} catch{ main.DisplayNote(tag_path + " returned an error (likely due to file read attempt)", null, error_level.WARNING);}

            // load new tag tab here
            TabItem new_tag = new();
            new_tag.Header = Path.GetFileName(tag_path);
            TagsTabs.Items.Add(new_tag);
            TagInstance tag_interface = new TagInstance(main, test, new_tag);
            Tabs.Add(tag_path, tag_interface);
            tag_interface.LoadTag_UI();
            TagsTabs.SelectedIndex = TagsTabs.Items.IndexOf(tag_interface.container);
        }
        public void OpenModuleTag(directory_item item, string plugins_path){
            if (Tabs.ContainsKey(item.name)){
                TagInstance target_tab = Tabs[item.name];
                TagsTabs.SelectedIndex = TagsTabs.Items.IndexOf(target_tab.container);
                return;}
            // TODO: i think we missed a couple of things that we were supposed to check
            // check to make sure the code is working right
            if (item.is_folder) { 
                main.DisplayNote(item.name + " is a folder, not a tag! failure of epic portportions!!", null, error_level.WARNING);
                return;}
            if (!item.is_module) { 
                main.DisplayNote(item.name + " is a local-disk tag, not a module tag!!", null, error_level.WARNING);
                return;}
            // check to see if the name is a resource file name
            if (item.name.Contains("[")){
                main.DisplayNote(item.name + " appears to be a tag struct resource file, not a real tag", null, error_level.WARNING);
                return;}

            // error checking past, open tag for real

            // lets first get a list of all the resource file that this guy probably owns
            List<KeyValuePair<byte[], bool>> resource_list = new();
            try{ // idk why we have a try catch here
                List<byte[]> resulting_resources = item.source_module.get_tag_resource_list(item.module_file_index);
                foreach (byte[] resource in resulting_resources) {

                    bool is_standalone_resource = resource[0..4].SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 }); // test for those 4 chars at the top of the file
                    resource_list.Add(new KeyValuePair<byte[], bool>(resource, is_standalone_resource));
            }}catch{
                main.DisplayNote(item.name + " failed to read resources", null, error_level.WARNING);
                return;
            }
            
            // get the resource from the module



            // anomaly check // make sure all entries are of either type, else this will become very difficult to manage
            if (resource_list.Count > 0){
                bool inital = resource_list[0].Value;
                foreach (var resource in resource_list){
                    if (resource.Value != inital){
                        main.DisplayNote(resource + " does not have a matching chunked/non-chunked status!!", null, error_level.WARNING);
            }}}
            
            // we forgot to actually load the tag bytes from module
            // + for some reason we are failing to read resources

            tag test = new tag(plugins_path, resource_list);
            byte[] tagbytes = null;
            try{
                tagbytes = item.source_module.get_tag_bytes(item.module_file_index);
                if (!test.Load_tag_file(tagbytes)){
                    main.DisplayNote(item.name + " was not able to be loaded as a tag", null, error_level.WARNING);
                    return;
            }} catch{ 
                main.DisplayNote(item.name + " returned an error (likely due to file read attempt)", null, error_level.WARNING); return;}

            //DEBUG
            // write original tag to file & resources too
            //File.WriteAllBytes("C:\\Users\\Joe bingle\\Downloads\\tag testing\\og", tagbytes);
            //for (int i = 0; i < resource_list.Count; i++)
            //    File.WriteAllBytes("C:\\Users\\Joe bingle\\Downloads\\tag testing\\og_res_" + i, resource_list[i].Key);

            //tag.compiled_tag testoutput = test.compile();
            //File.WriteAllBytes("C:\\Users\\Joe bingle\\Downloads\\tag testing\\recompiled", testoutput.tag_bytes);
            //for (int i = 0; i < testoutput.resource_bytes.Count; i++)
            //    File.WriteAllBytes("C:\\Users\\Joe bingle\\Downloads\\tag testing\\recompiled_res_" + i, testoutput.resource_bytes[i]);
            


            // load new tag tab here
            TabItem new_tag = new();
            new_tag.Header = Path.GetFileName(item.name);
            TagsTabs.Items.Add(new_tag);
            TagInstance tag_interface = new TagInstance(main, test, new_tag);
            Tabs.Add(item.name, tag_interface);
            tag_interface.LoadTag_UI();
            TagsTabs.SelectedIndex = TagsTabs.Items.IndexOf(tag_interface.container);
        }
    }
}
