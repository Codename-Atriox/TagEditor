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
    public partial class DoubleangleParam : UserControl{
        public DoubleangleParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset){
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
        private void loadValue(){
            double radians = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]);
            radians *= 180 / Math.PI;
            Valuebox1.Text = ((float)radians).ToString();
            if (og_value1 == null) og_value1 = radians;
            radians        = BitConverter.ToSingle(parent_block[(block_offset + 4)..(block_offset + 8)]);
            radians *= 180 / Math.PI;
            Valuebox2.Text = ((float)radians).ToString();
            if (og_value2 == null) og_value2 = radians;
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue();
        private void Button_SaveValue(){
            if (is_setting_up) return;
            double value1;
            double value2;
            try{value1 = Convert.ToDouble(Valuebox1.Text);
            }catch{error_marker1.Visibility = Visibility.Visible;return;}
            try{value2 = Convert.ToDouble(Valuebox2.Text);
            }catch{error_marker2.Visibility = Visibility.Visible;return; }
            // we can only set values & submit the diff if both values passed
            SetValue(value1, 0, error_marker1);
            SetValue(value2, 4, error_marker2);
            callback.set_diff(this, Namebox.Text, param_type, og_value1.ToString() + ", " + og_value2.ToString(), value1.ToString() + ", " + value2.ToString(), line_index);
        }
        private void SetValue(double value, int offset, Separator error_marker){ // write string to array
            value *= Math.PI / 180;
            byte[] bytes = BitConverter.GetBytes((float)value);
            bytes.CopyTo(parent_block, block_offset+ offset);
            if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        double? og_value1; // note that this will not always be the actual OG value
        double? og_value2;
        public void revertValue(double og1, double og2){
            SetValue(og1, 0, error_marker1);
            SetValue(og2, 4, error_marker2);
        }
    }
}