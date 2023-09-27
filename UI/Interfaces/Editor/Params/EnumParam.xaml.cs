﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TagEditor.UI.Interfaces.Editor.Params{
    /// <summary>
    /// Interaction logic for EnumParam.xaml
    /// </summary>
    public partial class EnumParam : UserControl{
        public string key;
        public EnumParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, EnumType _int_type, string[] _enums, string _key){
            key = _key;
            callback = _callback;
            line_index = _line_index;
            param_type = _param_type;
            parent_block = _parent_block;
            block_offset = _block_offset;
            _enum_type = _int_type;
            enums = _enums;
            InitializeComponent();
            enumbox.ItemsSource = enums;
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

        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;
        // flag stuff //
        string[] enums;
        EnumType _enum_type;
        public enum EnumType{
            _byte = 0,
            _short = 1,
            _int = 2,
        }
        uint selected_enum;

        private void loadValue(){
            switch ((sbyte)_enum_type){ // if we get errors about "too large or too small" then we aren't actually converting the values right
                case 0: // ubyte
                    byte value4 = Convert.ToByte(parent_block[block_offset]);
                    selected_enum = value4;
                    break;
                case 1: // ushort
                    ushort value5 = BitConverter.ToUInt16(parent_block[block_offset..(block_offset + 2)]);
                    selected_enum = value5;
                    break;
                case 2: // uint
                    uint value6 = BitConverter.ToUInt32(parent_block[block_offset..(block_offset + 4)]);
                    selected_enum = value6;
                    break;
            }
            enumbox.SelectedIndex = (int)selected_enum; // i dont think we;re going over the integer maximum of enum possibilities tbh
            if (og_value == null) og_value = enumbox.SelectedIndex.ToString();
        }

        private void enumbox_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (is_setting_up) return;
            // error handling is sorted here
            SetValue(this, enumbox.SelectedIndex, _enum_type, parent_block, block_offset);
            callback.set_diff(this, key, Namebox.Text, param_type, og_value, enumbox.SelectedIndex.ToString(), line_index, parent_block, block_offset);
        }

        
            
        private static void SetValue(EnumParam? target, int value, EnumType enum_type, byte[] block, int offset){
            byte[] enum_bytes;
            switch ((byte)enum_type){
                case 0: // ubyte
                    if (value > byte.MaxValue) Debug.Assert(false, "somehow we managed a larger enum than the allowed size");
                    enum_bytes = new byte[1] { (byte)value };
                    break;
                case 1: // ushort
                    if (value > ushort.MaxValue) Debug.Assert(false, "somehow we managed a larger enum than the allowed size");
                    enum_bytes = BitConverter.GetBytes((ushort)value);
                    break;
                case 2: // uint
                    enum_bytes = BitConverter.GetBytes(value);
                    break;
                default: throw new Exception("failed to match any enum group");
            }
            enum_bytes.CopyTo(block, offset);
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
        public static void revert_value(string old_value, EnumType enum_type, EnumParam? target, byte[] block, int offset){
            if (target != null){
                target.og_value = old_value;
                if (target.enumbox.IsDropDownOpen == true) target.enumbox.IsDropDownOpen = false;
                SetValue(target, Convert.ToInt32(old_value), enum_type, block, offset);
            }else SetValue(null, Convert.ToInt32(old_value), enum_type, block, offset);
        }
    }
}
