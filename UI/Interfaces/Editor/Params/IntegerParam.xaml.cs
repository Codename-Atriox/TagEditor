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
    /// Interaction logic for IntegerParam.xaml
    /// </summary>
    public partial class IntegerParam : UserControl
    {
        public IntegerParam(string name, byte[] _parent_block, int _block_offset, IntType _int_type)
        {
            parent_block = _parent_block;
            block_offset = _block_offset;
            int_type = _int_type;
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
        // integer stuff //
        IntType int_type;
        public enum IntType{
            signed_char    = 0,
            signed_short   = 1,
            signed_int     = 2,
            signed_long    = 3,
            unsigned_char  = 4,
            unsigned_short = 5,
            unsigned_int   = 6,
            unsigned_long  = 7
        } // check 0b0100 for signed, then subtract it

        private void loadValue(){
            switch ((sbyte)int_type){ // if we get errors about "too large or too small" then we aren't actually converting the values right
                case 0: // byte
                    sbyte value0 = (sbyte)parent_block[block_offset];
                    Valuebox.Text = value0.ToString();
                    break;
                case 1: // short
                    short value1 = BitConverter.ToInt16(parent_block[block_offset..(block_offset + 2)]);
                    Valuebox.Text = value1.ToString();
                    break;
                case 2: // int
                    int value2 = BitConverter.ToInt32(parent_block[block_offset..(block_offset + 4)]);
                    Valuebox.Text = value2.ToString();
                    break;
                case 3: // long
                    long value3 = BitConverter.ToInt64(parent_block[block_offset..(block_offset + 8)]);
                    Valuebox.Text = value3.ToString();
                    break;
                case 4: // ubyte
                    byte value4 = Convert.ToByte(parent_block[block_offset]);
                    Valuebox.Text = value4.ToString();
                    break;
                case 5: // ushort
                    ushort value5 = BitConverter.ToUInt16(parent_block[block_offset..(block_offset + 2)]);
                    Valuebox.Text = value5.ToString();
                    break;
                case 6: // uint
                    uint value6 = BitConverter.ToUInt32(parent_block[block_offset..(block_offset + 4)]);
                    Valuebox.Text = value6.ToString();
                    break;
                case 7: // ulong
                    long value7 = BitConverter.ToInt64(parent_block[block_offset..(block_offset + 8)]);
                    Valuebox.Text = value7.ToString();
                    break;
        }}
        private void Button_SaveValue(object sender, TextChangedEventArgs e){
            if (is_setting_up) return;
            if (!SaveValue()) 
                error_marker.Visibility = Visibility.Visible;
            else if (error_marker.Visibility != Visibility.Collapsed) error_marker.Visibility = Visibility.Collapsed;
        }
        private bool SaveValue(){ // write new value to array
            try{switch ((byte)int_type){
                    case 0: // byte
                        sbyte value0 = Convert.ToSByte(Valuebox.Text);
                        parent_block[block_offset] = Convert.ToByte(value0);
                        break;
                    case 1: // short
                        short value = Convert.ToInt16(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value));
                        break;
                    case 2: // int
                        int value1 = Convert.ToInt32(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value1));
                        break;
                    case 3: // long
                        long value2 = Convert.ToInt64(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value2));
                        break;
                    case 4: // ubyte
                        byte value6 = Convert.ToByte(Valuebox.Text);
                        parent_block[block_offset] = value6;
                        break;
                    case 5: // ushort
                        ushort value3 = Convert.ToUInt16(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value3));
                        break;
                    case 6: // uint
                        uint value4 = Convert.ToUInt32(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value4));
                        break;
                    case 7: // ulong
                        ulong value5 = Convert.ToUInt64(Valuebox.Text);
                        assign_bytes(BitConverter.GetBytes(value5));
                        break;
                }
                return true;
            } catch { return false; }
        }
        private void assign_bytes(byte[] new_bytes){
            new_bytes.CopyTo(parent_block, block_offset);
        }
    }
}
