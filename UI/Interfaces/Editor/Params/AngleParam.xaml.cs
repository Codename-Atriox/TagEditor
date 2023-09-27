using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
using System.Xml.Linq;
using TagEditor.UI.Windows;

namespace TagEditor.UI.Interfaces.Params{
    /// <summary>
    /// Interaction logic for AngleParam.xaml
    /// </summary>
    public partial class AngleParam : UserControl{
        public AngleParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset){
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
        // ANGLE STUFF // 
        // there is no angle stuff //
        private void loadValue(){
            double radians = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]);
            radians *= 180 / Math.PI; // convert to degrees
            Valuebox.Text = ((float)radians).ToString();
            // store og value
            if (og_value == null) og_value = Valuebox.Text;
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            try{SetValue(this, Valuebox, error_marker, Convert.ToDouble(Valuebox.Text), parent_block, block_offset);
                callback.set_diff(this, Namebox.Text, param_type, og_value, Valuebox.Text, line_index, parent_block, block_offset);
            }catch { 
                error_marker.Visibility = Visibility.Visible;
        }}
        private static void SetValue(AngleParam? target, TextBox? source, Separator? error, double value, byte[] block, int offset){
            // update UI element if it exists
            if (target != null){
                target.is_setting_up = true;
                source.Text = ((float)value).ToString();
                if (error.Visibility != Visibility.Collapsed) error.Visibility = Visibility.Collapsed;
                target.is_setting_up = false;
            }
            value *= Math.PI / 180;
            byte[] bytes = BitConverter.GetBytes((float)value);
            bytes.CopyTo(block, offset);
        }
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        string? og_value; // note that this will not always be the actual OG value
        public static void revert_value(string old_value, AngleParam? target, byte[] block, int offset){
            if (target != null){
                target.og_value = old_value;
                SetValue(target, target.Valuebox, target.error_marker, Convert.ToDouble(old_value), block, offset);
            }else SetValue(null, null, null, Convert.ToDouble(old_value), block, offset);
        }
    }
}
