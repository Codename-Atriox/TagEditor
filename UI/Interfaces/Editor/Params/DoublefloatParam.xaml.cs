﻿using System;
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

namespace TagEditor.UI.Interfaces.Params{
    public partial class DoublefloatParam : UserControl{
        public DoublefloatParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, bool _is_fraction){
            callback = _callback;
            line_index = _line_index;
            param_type = _param_type;
            parent_block = _parent_block;
            block_offset = _block_offset;
            is_fraction = _is_fraction;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
            is_setting_up = false;
        }
        public void reload(byte[] _parent_block, int _block_offset){
            is_setting_up = true;
            parent_block = _parent_block;
            block_offset = _block_offset;
            loadValue();
            is_setting_up = false;
        }
        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;
        // FLOAT STUFF //
        bool is_fraction = false;
        // TODO // 
        private void loadValue() {
            Valuebox1.Text = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]).ToString();
            Valuebox2.Text = BitConverter.ToSingle(parent_block[(block_offset + 4)..(block_offset + 8)]).ToString();
            if (og_value == null) og_value = Valuebox1.Text + ", " + Valuebox2.Text;
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();

        private void Button_SaveValue(){
            if (is_setting_up) return;
            float value1;
            float value2;
            try{value1 = Convert.ToSingle(Valuebox1.Text);
                if (is_fraction && (!(value1 >= 0.0f && value1 <= 1.0f))) throw new Exception();
            }catch{error_marker1.Visibility = Visibility.Visible;return;}
            try{value2 = Convert.ToSingle(Valuebox2.Text);
                if (is_fraction && (!(value2 >= 0.0f && value2 <= 1.0f))) throw new Exception();
            }catch{error_marker2.Visibility = Visibility.Visible;return; }
            // we can only set values & submit the diff if both values passed
            SetValue(this, Valuebox1, error_marker1, value1, parent_block, block_offset);
            SetValue(this, Valuebox2, error_marker2, value2, parent_block, block_offset+4);
            callback.set_diff(this, Namebox.Text, param_type, og_value, value1.ToString() + ", " + value2.ToString(), line_index, parent_block, block_offset);
        }
        private static void SetValue(DoublefloatParam? target, TextBox? source, Separator? error, float value, byte[] block, int offset){
            // update UI element if it exists
            if (target != null){
                target.is_setting_up = true;
                source.Text = value.ToString();
                if (error.Visibility != Visibility.Collapsed) error.Visibility = Visibility.Collapsed;
                target.is_setting_up = false;
            }
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(block, offset);
        }
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        string? og_value; // note that this will not always be the actual OG value
        public static void revert_value(string old_value, DoublefloatParam? target, byte[] block, int offset){
            string[] values = old_value.Split(", ");
            if (target != null){
                target.og_value = old_value;
                SetValue(target, target.Valuebox1, target.error_marker1, Convert.ToSingle(values[0]), block, offset);
                SetValue(target, target.Valuebox2, target.error_marker2, Convert.ToSingle(values[1]), block, offset+4);
            } else {
                SetValue(null, null, null, Convert.ToSingle(values[0]), block, offset);
                SetValue(null, null, null, Convert.ToSingle(values[1]), block, offset+4);
            }
        }
    }
}
