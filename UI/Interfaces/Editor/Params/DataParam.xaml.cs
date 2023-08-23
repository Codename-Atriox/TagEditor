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

namespace TagEditor.UI.Interfaces.Editor.Params
{
    /// <summary>
    /// Interaction logic for DataParam.xaml
    /// </summary>
    public partial class DataParam : UserControl
    {
        public DataParam(string name, byte[] _datter, byte[] _parent_block, int _block_offset, short _length)
        {

            InitializeComponent();

            Namebox.Text = name + " [" + _datter.Length + "]";
            data = _datter;


            Valuebox.Text = BitConverter.ToString(_parent_block[_block_offset..(_block_offset + _length)]).Replace("-", string.Empty);
        }
        public void reload(string name, byte[] _datter, byte[] _parent_block, int _block_offset, short _length){

            Namebox.Text = name + " [" + _datter.Length + "]";
            data = _datter;
            Valuebox.Text = BitConverter.ToString(_parent_block[_block_offset..(_block_offset + _length)]).Replace("-", string.Empty);
        }
        byte[] data;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string dat_as_hex = BitConverter.ToString(data).Replace('-', ' ');
            Clipboard.SetText(dat_as_hex);
        }
    }
}