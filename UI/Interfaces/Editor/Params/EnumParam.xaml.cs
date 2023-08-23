using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for EnumParam.xaml
    /// </summary>
    public partial class EnumParam : UserControl
    {
        public EnumParam(string name, byte[] _parent_block, int _block_offset, EnumType _int_type, string[] _enums)
        {
            parent_block = _parent_block;
            block_offset = _block_offset;
            _enum_type = _int_type;
            enums = _enums;
            InitializeComponent();
            enumbox.ItemsSource = enums;
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
        // flag stuff //
        string[] enums;
        EnumType _enum_type;
        public enum EnumType
        {
            _byte = 0,
            _short = 1,
            _int = 2,
        }
        uint selected_enum;

        private void loadValue()
        {
            switch ((sbyte)_enum_type)
            { // if we get errors about "too large or too small" then we aren't actually converting the values right
                case 0: // ubyte
                    byte value4 = Convert.ToByte(parent_block[block_offset]);
                    selected_enum = value4;
                    break;
                case 1: // ushort
                    ushort value5 = BitConverter.ToUInt16(parent_block[block_offset..(block_offset + 2)]);
                    selected_enum = value5;
                    break;
                case 2: // uint
                    uint value6 = BitConverter.ToUInt32(parent_block[block_offset..(block_offset + 4)]);
                    selected_enum = value6;
                    break;
            }
            enumbox.SelectedIndex = (int)selected_enum; // i dont think we;re going over the integer maximum of enum possibilities tbh
        }

        private bool SaveValue()
        { // write new value to array
            selected_enum = (uint)enumbox.SelectedIndex;
            try
            {
                switch ((byte)_enum_type)
                {
                    case 0: // ubyte
                        if (selected_enum > byte.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                        parent_block[block_offset] = (byte)selected_enum;
                        break;
                    case 1: // ushort
                        if (selected_enum > ushort.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                        assign_bytes(BitConverter.GetBytes((ushort)selected_enum));
                        break;
                    case 2: // uint
                        assign_bytes(BitConverter.GetBytes(selected_enum));
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        private void assign_bytes(byte[] new_bytes) => new_bytes.CopyTo(parent_block, block_offset);

        private void enumbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!is_setting_up)
            {
                SaveValue();

            }
        }
    }
}
