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
    public partial class DoubleshortParam : UserControl{
        public DoubleshortParam(string name, byte[] _parent_block, int _block_offset){
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
        private void loadValue(){
            Valuebox1.Text = BitConverter.ToInt16(parent_block[block_offset..(block_offset + 2)]).ToString();
            Valuebox2.Text = BitConverter.ToInt16(parent_block[(block_offset + 2)..(block_offset + 4)]).ToString();
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox1, error_marker1, 0);
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox2, error_marker2, 2);
        private void Button_SaveValue(TextBox field, Separator error_marker, int offset){
            if (is_setting_up) return;
            if (!SaveValue(field, offset)) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(TextBox field, int offset){ // write string to array
            try{
                short value = Convert.ToInt16(field.Text);
                byte[] bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(parent_block, block_offset + offset);
                return true;
            }catch { return false; }
        }
    }
}
