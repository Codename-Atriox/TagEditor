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
using static TagEditor.UI.Interfaces.Params.IntegerParam;

namespace TagEditor.UI.Interfaces.Params
{
    /// <summary>
    /// Interaction logic for GarbageParameter.xaml
    /// </summary>
    public partial class GarbageParameter : UserControl
    {
        public GarbageParameter(string name, byte[] _parent_block, int _block_offset, short _length)
        {
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
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(){ // write new value to array
            try{string hexstring = Valuebox.Text.Replace(" ", string.Empty);
                if (hexstring.Length != (length*2)) return false;
                // for the purpose of triggering an error, else we'd convert straight into the array
                byte[] bytes = new byte[length];
                for (int i = 0; i < length; i++) bytes[i] = Convert.ToByte(hexstring.Substring(i * 2, 2), 16);
                bytes.CopyTo(parent_block, block_offset);
                return true;
            } catch{ return false;}
        }
    }
}
