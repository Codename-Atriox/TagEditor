using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TagEditor.UI.Interfaces.Editor.Params
{
    /// <summary>
    /// Interaction logic for FlagsParam.xaml
    /// </summary>
    public partial class FlagsParam : UserControl
    {
        public FlagsParam(string name, byte[] _parent_block, int _block_offset, FlagType _int_type, string[] _flags)
        {
            parent_block = _parent_block;
            block_offset = _block_offset;
            flag_type = _int_type;
            flags = _flags;
            InitializeComponent();
            loadValue();
            Namebox.Text = name;
        }
        public void reload(byte[] _parent_block, int _block_offset){
            parent_block = _parent_block;
            block_offset = _block_offset;
            loadValue();
        }
        byte[] parent_block;
        int block_offset;
        // flag stuff //
        string[] flags;
        FlagType flag_type;
        public enum FlagType{
            _byte = 0,
            _short = 1,
            _int = 2,
        } 
        uint flags_value;

        private void loadValue()
        {
            switch ((sbyte)flag_type)
            { // if we get errors about "too large or too small" then we aren't actually converting the values right
                case 0: // ubyte
                    byte value4 = Convert.ToByte(parent_block[block_offset]);
                    flags_value = value4;
                    break;
                case 1: // ushort
                    ushort value5 = BitConverter.ToUInt16(parent_block[block_offset..(block_offset + 2)]);
                    flags_value = value5;
                    break;
                case 2: // uint
                    uint value6 = BitConverter.ToUInt32(parent_block[block_offset..(block_offset + 4)]);
                    flags_value = value6;
                    break;
            }
            reload_selected_flags_string();
        }
        private void reload_selected_flags_string()
        {
            string new_Selected_flags = "";
            for (int i = 0; i < flags.Length; i++)
            {
                if ((flags_value & as_bit_index(i)) != 0) // then this item is checked
                {
                    new_Selected_flags += "\"" + flags[i] + "\" ";
                }
            }
            if (new_Selected_flags == string.Empty) ValueBox.Content = "NONE";
            else ValueBox.Content = new_Selected_flags;
        }

        private bool SaveValue()
        { // write new value to array
            try
            {
                switch ((byte)flag_type)
                {
                    case 0: // ubyte
                        if (flags_value > byte.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                        parent_block[block_offset] = (byte)flags_value;
                        break;
                    case 1: // ushort
                        if (flags_value > ushort.MaxValue) Debug.Assert(false, "somehow we managed a larger flag than the allowed size");
                        assign_bytes(BitConverter.GetBytes((ushort)flags_value));
                        break;
                    case 2: // uint
                        assign_bytes(BitConverter.GetBytes(flags_value));
                        break;
                }
                return true;
            }
            catch { return false; }
        }
        private void assign_bytes(byte[] new_bytes) => new_bytes.CopyTo(parent_block, block_offset);
        

        private async void Open_Dropdown(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            popup_thing.IsOpen = true;

            for (int i = 0; i < flags.Length; i++)
            {
                CheckBox flag_item = new();
                flag_item.IsChecked = ((flags_value & as_bit_index(i)) != 0);
                flag_item.Tag = i;
                flag_item.Content = flags[i];
                flag_item.Click += CheckBox_Click;
                flag_item.LostFocus += Popup_LostFocus;
                popup_panel.Children.Add(flag_item);
            }
            box_thinger.Focus();
            Keyboard.Focus(box_thinger);




        }

        private void Popup_LostFocus(object sender, RoutedEventArgs e) // close popup & save selected values
        {
            // track all the checked boxes
            //SaveValue();
            if (!popup_thing.IsKeyboardFocusWithin)
            {
                popup_thing.IsOpen = false;
                popup_panel.Children.Clear();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
                CheckBox senda = e.Source as CheckBox;
                bool isChecked = senda.IsChecked == true;
                int? flag_index = senda.Tag as int?;
                if (flag_index == null) Debug.Assert(false, "goofball checkbox");

                uint numerous = as_bit_index((int)flag_index);

                if (isChecked) flags_value |= numerous;
                else flags_value &= ~numerous;

                SaveValue();
                reload_selected_flags_string();
        }
        private uint as_bit_index(int i)
        { return (uint)1 << i;}




    }
}
