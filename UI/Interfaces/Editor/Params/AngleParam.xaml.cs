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

namespace TagEditor.UI.Interfaces.Params
{
    /// <summary>
    /// Interaction logic for AngleParam.xaml
    /// </summary>
    public partial class AngleParam : UserControl
    {
        public AngleParam(string name, byte[] _parent_block, int _block_offset)
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
        // ANGLE STUFF // 
        // there is no angle stuff //
        private void loadValue(){
            double radians = BitConverter.ToSingle(parent_block[block_offset..(block_offset + 4)]);
            radians *= 180 / Math.PI; // convert to degrees
            Valuebox.Text = ((float)radians).ToString();
        }

        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            if (!SaveValue()) error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(){ // write angle to array
            try{double value = Convert.ToDouble(Valuebox.Text);
                value *= Math.PI / 180;
                byte[] bytes = BitConverter.GetBytes((float)value);
                bytes.CopyTo(parent_block, block_offset);
                return true;
            }catch { return false; }
        }
    }
}
