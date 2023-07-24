using System;
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
            List<KeyValuePair<string, bool>> resource_list = new();
            /*
            foreach (var item in Directory.GetFiles(folder)){
                string file_name = Path.GetFileName(item);
                if (file_name.StartsWith(tag_file_name) && file_name.Length > tag_file_name.Length){
                    // get index of file, just incase the function that retrives all the files doesn't do it alphabetically
                    // then either insert or add 
                    int resource_index = Convert.ToInt32(file_name.Substring(tag_file_name.Length+1).Split("-")[0]);

                    using (FileStream fs = new FileStream(item, FileMode.Open)){
                        try{byte[] bytes = new byte[4];
                            fs.Read(bytes, 0, 4);
                            bool is_standalone_resource = bytes.SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 });
                            if (resource_index >= resource_list.Count) resource_list.Add(new KeyValuePair<string, bool>(item, is_standalone_resource));
                            else resource_list.Insert(resource_index, new KeyValuePair<string, bool>(item, is_standalone_resource));
                        }catch{ main.DisplayNote("resource file: \"" + item + "\" is unable to be opened, disregarding", null, error_level.WARNING);}
            }}}
            // anomaly check // make sure all entries are of either type, else this will become very difficult to manage
            bool inital = resource_list[0].Value;
            foreach (var item in resource_list){
                if (item.Value != inital){
                    main.DisplayNote(item + " does not have a matching chunked/non-chunked status, please submit this scenario to the C:A developers", null, error_level.WARNING);
            }}
            */


            tag test = new tag(plugins_path, resource_list);
            if (!test.Load_tag_file(tag_path)){
                main.DisplayNote(tag_path + " was not able to be loaded as a tag", null, error_level.WARNING);
                return;
            }
            // load new tag tab here
            TabItem new_tag = new();
            new_tag.Header = Path.GetFileName(tag_path);
            TagsTabs.Items.Add(new_tag);
            TagInstance tag_interface = new TagInstance(main, test, new_tag);
            Tabs.Add(tag_path, tag_interface);
            tag_interface.LoadTag_UI();
            TagsTabs.SelectedIndex = TagsTabs.Items.IndexOf(tag_interface.container);
        }
    }
}
