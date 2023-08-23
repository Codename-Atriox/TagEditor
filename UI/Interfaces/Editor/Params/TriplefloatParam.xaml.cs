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
    /// Interaction logic for TriplefloatParam.xaml
    /// </summary>
    public partial class TriplefloatParam : UserControl
    {
        public TriplefloatParam(string name, byte[] _parent_block, int _block_offset){
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
        // TODO //
        private void loadValue(){
            Valuebox1.Text = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]).ToString();
            Valuebox2.Text = BitConverter.ToSingle(parent_block[(block_offset + 4)..(block_offset + 8)]).ToString();
            Valuebox3.Text = BitConverter.ToSingle(parent_block[(block_offset + 8)..(block_offset + 12)]).ToString();
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox1, error_marker1, 0);
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox2, error_marker2, 4);
        private void Button3_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox3, error_marker3, 8);
        private void Button_SaveValue(TextBox field, Separator error_marker, int offset){
            if (is_setting_up) return;
            if (!SaveValue(field, offset)) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(TextBox field, int offset){ // write string to array
            try{
                float value = Convert.ToSingle(field.Text);
                byte[] bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(parent_block, block_offset + offset);
                return true;
            }catch { return false; }
        }
    }
}