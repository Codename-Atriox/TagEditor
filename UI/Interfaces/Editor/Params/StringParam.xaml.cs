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
    public partial class StringParam : UserControl{
        public string key;
        public StringParam(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, short _length, string _key){
            key = _key;
            callback = _callback;
            line_index = _line_index;
            param_type = _param_type;
            parent_block = _parent_block;
            block_offset = _block_offset;
            length = _length;
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
        // GARBAGE PARAM STUFF //
        short length;

        private void loadValue(){
            Valuebox.Text = Encoding.UTF8.GetString(parent_block, block_offset, length);
            if (og_value == null) og_value = Valuebox.Text;
        }


        
        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            // the exception is put into the setvalue function
            try{SetValue(this, Valuebox, error_marker, Valuebox.Text, length, parent_block, block_offset);
                callback.set_diff(this, key, Namebox.Text, param_type, og_value, Valuebox.Text, line_index, parent_block, block_offset);
            }catch { 
                error_marker.Visibility = Visibility.Visible;
        }}
        private static void SetValue(StringParam? target, TextBox? source, Separator? error, string value, int length, byte[] block, int offset){
            if (value.Length > length) throw new Exception("value string was too long to fit into buffer");
            byte[] bytes = new byte[length];
            for (int i = 0; i < value.Length; i++) bytes[i] = (byte)value[i];
            // update UI element if it exists
            if (target != null){
                target.is_setting_up = true;
                source.Text = value;
                if (error.Visibility != Visibility.Collapsed) error.Visibility = Visibility.Collapsed;
                target.is_setting_up = false;
            }
            bytes.CopyTo(block, offset);
        }
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        string? og_value; // note that this will not always be the actual OG value
        public static void revert_value(string old_value, int string_length, StringParam? target, byte[] block, int offset){
            if (target != null){
                target.og_value = old_value;
                SetValue(target, target.Valuebox, target.error_marker, old_value, string_length, block, offset);
            }else SetValue(null, null, null, old_value, string_length, block, offset);
        }
    }
}
