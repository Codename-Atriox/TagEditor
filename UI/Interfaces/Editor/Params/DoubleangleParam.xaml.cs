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
    public partial class DoubleangleParam : UserControl{
        public DoubleangleParam(string name, byte[] _parent_block, int _block_offset){
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
        // FLOAT STUFF //
        private void loadValue(){
            double radians = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]);
            radians *= 180 / Math.PI;
            Valuebox1.Text = ((float)radians).ToString();
            radians        = BitConverter.ToSingle(parent_block[(block_offset + 4)..(block_offset + 8)]);
            radians *= 180 / Math.PI;
            Valuebox2.Text = ((float)radians).ToString();
        }

        private void Button1_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox1, error_marker1, 0);
        private void Button2_SaveValue(object sender, TextChangedEventArgs e) => Button_SaveValue(Valuebox2, error_marker2, 4);
        private void Button_SaveValue(TextBox field, Separator error_marker, int offset){
            if (is_setting_up) return;
            if (!SaveValue(field, offset)) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(TextBox field, int offset){ // write string to array
            try{double value = Convert.ToDouble(field.Text);
                value *= Math.PI / 180;
                byte[] bytes = BitConverter.GetBytes((float)value);
                bytes.CopyTo(parent_block, block_offset+ offset);
                return true;
            }catch { return false; }
        }
    }
}