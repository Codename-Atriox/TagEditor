using Infinite_module_test;
using System;
using System.Collections.Generic;
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
using TagEditor.UI.Windows;
using static Infinite_module_test.tag_structs;
using static TagEditor.UI.Windows.TagExplorer;

namespace TagEditor.UI.Interfaces.Params{
    public partial class TagrefParam : UserControl{
        public string key;
        public TagrefParam(string name, byte[] _parent_block, int _block_offset, TagExplorer _tags_explorer, string _key){
            key = _key;
            tags_explorer = _tags_explorer;
            parent_block = _parent_block;
            block_offset = _block_offset;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
            is_setting_up = false;
        }
        public void reload(byte[] _parent_block, int _block_offset, string _key){
            key = _key;
            is_setting_up = true;
            parent_block = _parent_block;
            block_offset = _block_offset;
            loadValue();
            is_setting_up = false;
        }
        TagExplorer tags_explorer;
        string TagIDname = "";
        string TagName = "";

        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;
        private void loadValue(){
            uint tagID = BitConverter.ToUInt32(parent_block[(block_offset + 0x8)..(block_offset + 0xC)]);
            TagIDname = tagID.ToString("X8");
            TagName = tag_structs.get_shorttagname(tagID);
            TagIDBox.Content = TagName;
            ulong assetID = BitConverter.ToUInt64(parent_block[(block_offset + 0xC)..(block_offset + 0x14)]);
            AssetIDBox.Text = assetID.ToString("X16");

            int value_test = BitConverter.ToInt32(parent_block[(block_offset + 0x14)..(block_offset + 0x18)]);
            if (value_test == -1)
                GroupBox.Content = "NULL";
            else GroupBox.Content = Reverse(System.Text.Encoding.Default.GetString(parent_block[(block_offset+0x14)..(block_offset + 0x18)]));
                //BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]).ToString();
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        private static uint reverse_uint(uint input){
            uint output = ((input & 0xff) << 24)
                        | ((input & 0xff00) << 8)
                        | ((input & 0xff0000) >> 8)
                        | ((input & 0xff000000) >> 24);
            return output;
        }
        private void call_open_tag(object sender, RoutedEventArgs e)
        {
            tags_explorer.try_open_tag(TagName, TagIDname);
        }

        private void groups_popup_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void GroupBox_Click(object sender, RoutedEventArgs e){

            if (groups_popup.IsOpen == true)
                return; // we should close the dropdown if its already open
            e.Handled = true;
            groups_popup.IsOpen = true;

            // we now need to populate the list thing
            tags_explorer.main.module;



            groups_popup.Focus();
            Keyboard.Focus(groups_popup);
            
        }


        //private void Button_SaveValue(object sender, TextChangedEventArgs e){
        //    if (is_setting_up) return;
        //    if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
        //    else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        //}
        //private bool SaveValue(){ // write string to array
        //    try{float value = Convert.ToSingle(Valuebox.Text);
        //        if (is_fraction && (!(value >= 0.0f && value <= 1.0f))) return false; // factions are between 0 & 1
        //        byte[] bytes = BitConverter.GetBytes(value);
        //        bytes.CopyTo(parent_block, block_offset);
        //        return true;
        //    } catch { return false; }
        //}
    }
}
