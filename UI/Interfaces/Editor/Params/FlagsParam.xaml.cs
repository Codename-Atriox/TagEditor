using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static TagEditor.UI.Interfaces.Editor.Params.EnumParam;
using TagEditor.UI.Windows;

namespace TagEditor.UI.Interfaces.Editor.Params{
    /// <summary>
    /// Interaction logic for FlagsParam.xaml
    /// </summary>
    public partial class FlagsParam : UserControl{
        public string key;
        public FlagsParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, FlagType _int_type, string[] _flags, string _key){
            key = _key;
            callback = _callback;
            line_index = _line_index;
            param_type = _param_type;
            parent_block = _parent_block;
            block_offset = _block_offset;
            flag_type = _int_type;
            flags = _flags;
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
        byte[] parent_block;
        int block_offset;
        public bool is_setting_up = true;
        // flag stuff //
        string[] flags;
        FlagType flag_type;
        public enum FlagType{
            _byte = 0,
            _short = 1,
            _int = 2,
        } 
        uint flags_value;

        private void loadValue(){
            switch ((sbyte)flag_type){ // if we get errors about "too large or too small" then we aren't actually converting the values right
                case 0: // ubyte
                    byte value4 = Convert.ToByte(parent_block[block_offset]);
                    flags_value = value4;
                    break;
                case 1: // ushort
                    ushort value5 = BitConverter.ToUInt16(parent_block[block_offset..(block_offset + 2)]);
                    flags_value = value5;
                    break;
                case 2: // uint
                    uint value6 = BitConverter.ToUInt32(parent_block[block_offset..(block_offset + 4)]);
                    flags_value = value6;
                    break;
            }
            reload_selected_flags_string();
            if (og_value == null) og_value = flags_value.ToString();
        }
        private void reload_selected_flags_string(){
            string new_Selected_flags = "";
            for (int i = 0; i < flags.Length; i++)
                if ((flags_value & as_bit_index(i)) != 0) // then this item is checked
                    new_Selected_flags += "\"" + flags[i] + "\" ";
            if (new_Selected_flags == string.Empty) ValueBox.Content = "NONE";
            else ValueBox.Content = new_Selected_flags;
        }

        private async void Open_Dropdown(object sender, RoutedEventArgs e){
            if (popup_thing.IsOpen == true)
                return; // we should close the dropdown if its already open
            e.Handled = true;
            popup_thing.IsOpen = true;

            for (int i = 0; i < flags.Length; i++){
                CheckBox flag_item = new();
                flag_item.IsChecked = ((flags_value & as_bit_index(i)) != 0);
                flag_item.Tag = i;
                flag_item.Content = flags[i];
                flag_item.Click += CheckBox_Click;
                flag_item.LostFocus += Popup_LostFocus;
                popup_panel.Children.Add(flag_item);
            }
            box_thinger.Focus();
            Keyboard.Focus(box_thinger);
        }
        private void Popup_LostFocus(object sender, RoutedEventArgs e){ // close popup & save selected values
            if (!popup_thing.IsKeyboardFocusWithin){
                popup_thing.IsOpen = false;
                popup_panel.Children.Clear();
        }}

        private void CheckBox_Click(object sender, RoutedEventArgs e){
            if (is_setting_up) return; // i dont think this should ever trigger 
            CheckBox senda = e.Source as CheckBox;
            bool isChecked = senda.IsChecked == true;
            int? flag_index = senda.Tag as int?;
            if (flag_index == null) Debug.Assert(false, "goofball checkbox");

            uint numerous = as_bit_index((int)flag_index);

            if (isChecked) flags_value |= numerous;
            else flags_value &= ~numerous;

            // error handling is sorted here
            SetValue(this, flags_value, flag_type, parent_block, block_offset);
            callback.set_diff(this, key, Namebox.Text, param_type, og_value, flags_value.ToString(), line_index, parent_block, block_offset);
        }
        private uint as_bit_index(int i) => (uint)1 << i;






        private static void SetValue(FlagsParam? target, uint value, FlagType flag_type, byte[] block, int offset){
            byte[] flag_bytes;
            switch ((byte)flag_type){
                case 0: // ubyte
                    if (value > byte.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                    flag_bytes = new byte[1] { (byte)value };
                    break;
                case 1: // ushort
                    if (value > ushort.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                    flag_bytes = BitConverter.GetBytes((ushort)value);
                    break;
                case 2: // uint
                    flag_bytes = BitConverter.GetBytes(value);
                    break;
                default: throw new Exception("failed to match any enum group");
            }
            flag_bytes.CopyTo(block, offset);
            // update UI element if it exists
            if (target != null){
                target.is_setting_up = true;
                // set the index
                target.loadValue();
                target.is_setting_up = false;
        }}
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        string? og_value; // note that this will not always be the actual OG value
        public static void revert_value(string old_value, FlagType flag_type, FlagsParam? target, byte[] block, int offset){
            if (target != null){
                target.og_value = old_value;
                if (target.popup_thing.IsOpen == true) target.popup_thing.IsOpen = false;
                SetValue(target, Convert.ToUInt32(old_value), flag_type, block, offset);
            }else SetValue(null, Convert.ToUInt32(old_value), flag_type, block, offset);
        }

    }
}
