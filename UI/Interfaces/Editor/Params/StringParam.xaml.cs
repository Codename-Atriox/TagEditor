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

namespace TagEditor.UI.Interfaces.Params{
    public partial class StringParam : UserControl{
        public StringParam(string name, byte[] _parent_block, int _block_offset, short _length){
            parent_block = _parent_block;
            block_offset = _block_offset;
            length = _length;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
            is_setting_up = false;
        }
        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;
        // GARBAGE PARAM STUFF //
        short length;

        private void loadValue(){
            Valuebox.Text = Encoding.UTF8.GetString(parent_block, block_offset, length);
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(){ // write string to array
            try{byte[] bytes = new byte[length];
                if (Valuebox.Text.Length > length) return false;
                for (int i = 0; i < Valuebox.Text.Length; i++) bytes[i] = (byte)Valuebox.Text[i];
                for (int i = 0; i < length; i++) parent_block[i + block_offset] = bytes[i];
                return true;
            }catch { return false; }
        }
    }
}
