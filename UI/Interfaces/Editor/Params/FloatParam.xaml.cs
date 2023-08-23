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

namespace TagEditor.UI.Interfaces.Params{
    public partial class FloatParam : UserControl{
        public FloatParam(string name, byte[] _parent_block, int _block_offset, bool _is_fraction){
            parent_block = _parent_block;
            block_offset = _block_offset;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
            is_setting_up = false;
            is_fraction = _is_fraction;
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
        private void loadValue(){
            Valuebox.Text = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]).ToString();
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(){ // write string to array
            try{float value = Convert.ToSingle(Valuebox.Text);
                if (is_fraction && (!(value >= 0.0f && value <= 1.0f))) return false; // factions are between 0 & 1
                byte[] bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(parent_block, block_offset);
                return true;
            } catch { return false; }
        }
    }
}
