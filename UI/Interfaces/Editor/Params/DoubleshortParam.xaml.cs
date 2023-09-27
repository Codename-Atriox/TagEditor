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

namespace TagEditor.UI.Interfaces.Params{
    public partial class DoubleshortParam : UserControl{
        public string key;
        public DoubleshortParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, string _key){
            key = _key;
            callback = _callback;
            line_index = _line_index;
            param_type = _param_type;
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
        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;
        private void loadValue(){
            Valuebox1.Text = BitConverter.ToInt16(parent_block[block_offset..(block_offset + 2)]).ToString();
            Valuebox2.Text = BitConverter.ToInt16(parent_block[(block_offset + 2)..(block_offset + 4)]).ToString();
            if (og_value == null) og_value = Valuebox1.Text + ", " + Valuebox2.Text;
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();

        private void Button_SaveValue(){
            if (is_setting_up) return;
            short value1;
            short value2;
            try{value1 = Convert.ToInt16(Valuebox1.Text);
            }catch{error_marker1.Visibility = Visibility.Visible;return;}
            try{value2 = Convert.ToInt16(Valuebox2.Text);
            }catch{error_marker2.Visibility = Visibility.Visible;return; }
            // we can only set values & submit the diff if both values passed
            SetValue(this, Valuebox1, error_marker1, value1, parent_block, block_offset);
            SetValue(this, Valuebox2, error_marker2, value2, parent_block, block_offset+2);
            callback.set_diff(this, key, Namebox.Text, param_type, og_value, value1.ToString() + ", " + value2.ToString(), line_index, parent_block, block_offset);
        }
        private static void SetValue(DoubleshortParam? target, TextBox? source, Separator? error, short value, byte[] block, int offset){
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
        public static void revert_value(string old_value, DoubleshortParam? target, byte[] block, int offset){
            string[] values = old_value.Split(", ");
            if (target != null){
                target.og_value = old_value;
                SetValue(target, target.Valuebox1, target.error_marker1, Convert.ToInt16(values[0]), block, offset);
                SetValue(target, target.Valuebox2, target.error_marker2, Convert.ToInt16(values[1]), block, offset+2);
            } else {
                SetValue(null, null, null, Convert.ToInt16(values[0]), block, offset);
                SetValue(null, null, null, Convert.ToInt16(values[1]), block, offset+2);
            }
        }
    }
}
