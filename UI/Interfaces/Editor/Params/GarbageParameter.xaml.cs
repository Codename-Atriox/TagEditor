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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagEditor.UI.Windows;
using static TagEditor.UI.Interfaces.Params.IntegerParam;

namespace TagEditor.UI.Interfaces.Params{
    /// <summary>
    /// Interaction logic for GarbageParameter.xaml
    /// </summary>
    public partial class GarbageParameter : UserControl{
        public GarbageParameter(TagInstance _callback, int _param_type, int _line_index, string name, byte[] _parent_block, int _block_offset, short _length){
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
        // GARBAGE PARAM STUFF //
        short length;

        private void loadValue(){
            Valuebox.Text = BitConverter.ToString(parent_block[block_offset..(block_offset + length)]).Replace("-", string.Empty);
            if (og_value == null) og_value = Valuebox.Text; // i think we should make it so they all use strings
        }

        
        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            try{string hex = Valuebox.Text.Replace(" ", string.Empty); // remove any whitespaces
                if (hex.Length != (length * 2)) throw new Exception(); // has to have correct amount of bytes
                byte[] bytes = hexstring_bytes(hex);
                SetValue(this, Valuebox, error_marker, Valuebox.Text, bytes, parent_block, block_offset);
                callback.set_diff(this, Namebox.Text, param_type, og_value, Valuebox.Text, line_index, parent_block, block_offset);
            }catch { 
                error_marker.Visibility = Visibility.Visible;
        }}
        private static void SetValue(GarbageParameter? target, TextBox? source, Separator? error, string bytestring, byte[] value, byte[] block, int offset){
            // update UI element if it exists
            if (target != null){
                target.is_setting_up = true;
                source.Text = bytestring;
                if (error.Visibility != Visibility.Collapsed) error.Visibility = Visibility.Collapsed;
                target.is_setting_up = false;
            }
            value.CopyTo(block, offset);
        }
        // diff'ing stuff
        TagInstance callback;
        int line_index;
        int param_type;
        string? og_value; // note that this will not always be the actual OG value
        // NOTE WE AREN'T ERROR CHECKING LENGHT, SO IF WE SOMEHOW GET THE WRONG LENGTH FROM THE OG VALUE THEN EVERYTHING WILL EXPLODE!!!!
        public static void revert_value(string old_value, GarbageParameter? target, byte[] block, int offset){
            byte[] bytes = hexstring_bytes(old_value); // i'd have to hope this never causes an error
            if (target != null){
                target.og_value = old_value; // this garuantees that the og value will be correct if a new diff is created
                SetValue(target, target.Valuebox, target.error_marker, old_value, bytes, block, offset);
            }else SetValue(null, null, null, old_value, bytes, block, offset);
        }
        private static byte[] hexstring_bytes(string hex){
            if (hex.Length % 2 == 1) throw new Exception("hexadecimal string cannot contain odd number of characters");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
