using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // error checking past, open tag for real
            tag test = new tag();
            if (!test.Load_tag_file(tag_path, plugins_path)){
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
