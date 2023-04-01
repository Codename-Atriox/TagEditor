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

namespace TagEditor.UI.Interfaces.Params
{
    /// <summary>
    /// Interaction logic for HashParam.xaml
    /// </summary>
    public partial class HashParam : UserControl
    {
        public HashParam(string name, byte[] _parent_block, int _block_offset)
        {
            parent_block = _parent_block;
            block_offset = _block_offset;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
            is_setting_up = false;
        }
        bool is_setting_up = true;
        byte[] parent_block;
        int block_offset;

        private void loadValue()
        {
            Valuebox.Text = BitConverter.ToString(parent_block[block_offset..(block_offset + 4)]).Replace("-", string.Empty);
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e)
        {
            if (is_setting_up) return;
            if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue()
        { // write new value to array
            try
            {
                string hexstring = Valuebox.Text.Replace(" ", string.Empty);
                if (hexstring.Length != (8)) return false;
                // for the purpose of triggering an error, else we'd convert straight into the array
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++) bytes[i] = Convert.ToByte(hexstring.Substring(i * 2, 2), 16);
                for (int i = 0; i < 4; i++) parent_block[i + block_offset] = bytes[i];
                return true;
            }
            catch { return false; }
        }
    }
}
