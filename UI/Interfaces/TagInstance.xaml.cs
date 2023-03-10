using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;
using TagEditor.UI.Interfaces.Params;
using static Infinite_module_test.tag_structs;

namespace TagEditor.UI.Windows
{
    /// <summary>
    /// Interaction logic for TagInstance.xaml
    /// </summary>
    public partial class TagInstance : UserControl
    {
        public TagInstance(MainWindow _main, tag _loaded_tag, TabItem _container)
        {
            InitializeComponent();
            main = _main;
            loaded_tag = _loaded_tag;
            container= _container;
            container.Content = this;
        }
        MainWindow main;
        public TabItem container;
        tag loaded_tag;
        public void LoadTag_UI()
        {
            // resurs through each node thinbo in the root structure
            // search for guid's by using the root node
            // process each of the params in the matching guid struct
            // 
            if (loaded_tag.root.blocks.Count == 1)
                process_ref_param(loaded_tag.root.blocks[0], params_panel, 0);
            else
            {

            }
        }
        public void process_ref_param(tag.thing _struct, StackPanel container, int struct_offset){
            XmlNode? guid_test = loaded_tag.reference_root.SelectSingleNode('_'+_struct.GUID);
            if (guid_test == null) Debug.Assert(false, "failed to locate guid xml node?");

            foreach (XmlNode node in guid_test.ChildNodes){

                string? param_name = node.Attributes?["Name"]?.Value;
                if (param_name == null)  Debug.Assert(false, "param has no name???");

                int offset = Convert.ToInt32(node.Attributes?["Offset"]?.Value, 16);
                offset += struct_offset; // this is for structs & array structs, so we can offset to the correct position, even though we're reading a different group of params


                // note that we need to calucate the block offsewt, not the filew offset as teh file offset doesn't exist as soon as we load the tag
                // then we give them the bock offset, and nam, and then tell them to initalize their value IE read from that offset
                // we also need to assign the block that its offsetted from, so it can read & write
                // integer types nee the int length, and signed boolean
                int type = Convert.ToInt32(node.Name.Substring(1), 16);
                switch (type){
                    case 0x0: // _field_string
                        break;  
                    case 0x1: // _field_long_string
                        break;  
                    case 0x2: // _field_string_id
                        break;  
                    case 0x3: //
                        break;  
                    case 0x4:{ // _field_char_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.signed_char);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x5:{ // _field_short_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.signed_short);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x6:{ // _field_long_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.signed_int);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x7:{ // _field_int64_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.signed_long);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x8: // _field_angle
                        break;  
                    case  0x9: // _field_tag
                        break;  
                    case  0xA: // _field_char_enum
                        break; 
                    case  0xB: // _field_short_enum
                        break;  
                    case  0xC: // _field_long_enum
                        break;  
                    case  0xD: // _field_long_flags
                        break;  
                    case  0xE: // _field_word_flags
                        break;  
                    case  0xF: // _field_byte_flags
                        break;  
                    case 0x10: // _field_point_2d
                        break;  
                    case 0x11: // _field_rectangle_2d
                        break;  
                    case 0x12: // _field_rgb_color
                        break;  
                    case 0x13: // _field_argb_color 
                        break;   
                    case 0x14: // _field_real
                        break;  
                    case 0x15: // _field_real_fraction
                        break;  
                    case 0x16: // _field_real_point_2d
                        break;  
                    case 0x17: // _field_real_point_3d
                        break;  
                    case 0x18: // _field_real_vector_2d
                        break;  
                    case 0x19: // _field_real_vector_3d
                        break;   
                    case 0x1A:  // _field_real_quaternion
                        break; 
                    case 0x1B: // _field_real_euler_angles_2d
                        break;  
                    case 0x1C: // _field_real_euler_angles_3d
                        break;  
                    case 0x1D: // _field_real_plane_2d
                        break;  
                    case 0x1E: // _field_real_plane_3d
                        break;  
                    case 0x1F: // _field_real_rgb_color
                        break;  
                    case 0x20: // _field_real_argb_color
                        break;  
                    case 0x21: // _field_real_hsv_color
                        break;  
                    case 0x22: // _field_real_ahsv_color
                        break;  
                    case 0x23: // _field_short_bounds
                        break;
                    case 0x24: // _field_angle_bounds
                        break;
                    case 0x25: // _field_real_bounds
                        break;
                    case 0x26: // _field_real_fraction_bounds
                        break;
                    case 0x27: //
                        break;
                    case 0x28: //
                        break;
                    case 0x29: // _field_long_block_flags
                        break;
                    case 0x2A: // _field_word_block_flags
                        break;
                    case 0x2B: // _field_byte_block_flags
                        break;
                    case 0x2C: // _field_char_block_index
                        break;
                    case 0x2D: // _field_custom_char_block_index
                        break;
                    case 0x2E: // _field_short_block_index
                        break;
                    case 0x2F: // _field_custom_short_block_index
                        break;
                    case 0x30: // _field_long_block_index
                        break;
                    case 0x31: // _field_custom_long_block_index
                        break;
                    case 0x32: //
                        break;
                    case 0x33: //
                        break;
                    case 0x34: case 0x35:{ // _field_skip // _field_pad
                            short size = Convert.ToInt16(node.Attributes?["Length"]?.Value);
                            GarbageParameter pad_garb_val = new(param_name, _struct.tag_data, offset, size);
                            container.Children.Add(pad_garb_val);
                        }continue;
                    case 0x36:{ // _field_explanation
                            CommentParam new_val = new("// "+param_name);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x37:{ // _field_custom
                            // min max values for those few istances in the levl
                            string? min = node.Attributes?["Min"]?.Value;
                            string? max = node.Attributes?["Max"]?.Value;
                            if (min != null) param_name += " min: " + min;
                            if (max != null) param_name += " max: " + max;
                            CommentParam new_val = new("// "+param_name);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x38: // _field_struct 
                        break;
                    case 0x39: // _field_array
                        break;
                    case 0x3A: //
                        break;
                    case 0x3B: // end of struct
                        break;
                    case 0x3C:{ // _field_byte_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.unsigned_char);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x3D:{ // _field_word_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.unsigned_short);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x3E:{ // _field_dword_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.unsigned_int);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x3F:{ // _field_qword_integer
                            IntegerParam new_val = new(param_name, _struct.tag_data, offset, IntegerParam.IntType.unsigned_long);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x40: // _field_block_v2
                        break;
                    case 0x41: // _field_reference_v2
                        break;
                    case 0x42: // _field_data_v2
                        break;
                    case 0x43: // tag_resource
                        break;
                    case 0x44: // UNKNOWN
                        break;
                    case 0x45: // UNKNOWN
                        break;
                } // if we're still here, then the type was unimplemented, create new garbage block
                GarbageParameter garb_val = new(param_name, _struct.tag_data, offset, param_group_sizes[type]);
                container.Children.Add(garb_val);
            }
        }


        public static int[] testt = new int[]
        {
            // STRING //
            32,  //  _0 // _field_string
            256, //  _1 // _field_long_string

            // HASH //
            4,   //  _2 // _field_string_id 

            
            // SHORT RANGE //
            4,   // _23 // _field_short_bounds
            4,   // _10 // _field_point_2d
            4,   // _11 // _field_rectangle_2d

            // ANGLE //
            4,   //  _8 // _field_angle
            // ANGLE RANGE //
            8,   // _24 // _field_angle_bounds

            // TAG GROUP //
            4,   //  _9 // _field_tag

            // ENUMS //
            1,   //  _A // _field_char_enum
            2,   //  _B // _field_short_enum
            4,   //  _C // _field_long_enum

            // FLAGS //
            4,   //  _D // _field_long_flags
            2,   //  _E // _field_word_flags
            1,   //  _F // _field_byte_flags


            // BYTE COLORS //
            4,   // _12 // _field_rgb_color
            4,   // _13 // _field_argb_color 
            // HSV COLORS //
            4,   // _21 // _field_real_hsv_coloR
            4,   // _22 // _field_real_ahsv_color
            // FLOAT COLORS //
            12,  // _1F // _field_real_rgb_color
            16,  // _20 // _field_real_argb_color


            // FLOATS //
            4,   // _14 // _field_real
            4,   // _15 // _field_real_fraction

            // FLOAT RANGES //
            8,   // _25 // _field_real_bounds
            8,   // _26 // _field_real_fraction_bounds
            // FLOAT 2D //
            8,   // _16 // _field_real_point_2d
            8,   // _18 // _field_real_vector_2d
            8,   // _1B // _field_real_euler_angles_2d

            // FLOAT 3D //
            12,  // _17 // _field_real_point_3d
            12,  // _19 // _field_real_vector_3d
            12,  // _1C // _field_real_euler_angles_3d

            // QUARTERNION //
            16,  // _1A // _field_real_quaternion

            // PLANE 2D //
            12,  // _1D // _field_real_plane_2d

            // PLANE 3D //
            16,  // _1E // _field_real_plane_3d



            // COMMENT //
            0,   // _36 // _field_explanation
            0,   // _37 // _field_custom

            // STRUCT //
            0,   // _38 // _field_struct 

            // STRUCT ARRAY //
            0,   // _39 // _field_array

            // TAGBLOCK //
            20,  // _40 // _field_block_v2

            // TAG REFERENCE //
            28,  // _41 // _field_reference_v2

            // DATA REFERENCE //
            24,  // _42 // _field_data_v2

            // RESOURCE BLOCK //
            16,  // _43 // tag_resource


            ////////////////////
            // INTEGER PARAMS //
            ////////////////////
            
            // SIGNED INTEGERS //
            1,   //  _4 // _field_char_integer
            2,   //  _5 // _field_short_integer
            4,   //  _6 // _field_long_integer
            8,   //  _7 // _field_int64_integer
            // UNSIGNED INTEGERS //
            1,   // _3C // _field_byte_integer
            2,   // _3D // _field_word_integer
            4,   // _3E // _field_dword_integer
            8,   // _3F // _field_qword_integer
            
            // GARBAGE //
            0,   // _34 // _field_pad 
            0,   // _35 // _field_skip
            // UNIMPLEMENTED //
            4,   //  _3 // 
            4,   // _27 // 
            4,   // _28 //
            4,   // _32 // 
            4,   // _33 // 
            4,   // _3A // 
            4,   // _44 // UNKNOWN
            4,   // _45 // UNKNOWN
            // TAGBLOCK FLAGS 
            4,   // _29 // _field_long_block_flags
            4,   // _2A // _field_word_block_flags
            4,   // _2B // _field_byte_block_flags
            // TAGBLOCK INDEXES
            1,   // _2C // _field_char_block_index
            1,   // _2D // _field_custom_char_block_index
            2,   // _2E // _field_short_block_index
            2,   // _2F // _field_custom_short_block_index
            4,   // _30 // _field_long_block_index
            4,   // _31 // _field_custom_long_block_index
            
            0,   // _3B // end of struct
        };



        // these should all be correct except for the unknown/unlabelled ones
        public static short[] param_group_sizes = new short[]
        {
            32,  //  _0 // _field_string
            256, //  _1 // _field_long_string
            4,   //  _2 // _field_string_id
            4,   //  _3 // 
            1,   //  _4 // _field_char_integer
            2,   //  _5 // _field_short_integer
            4,   //  _6 // _field_long_integer
            8,   //  _7 // _field_int64_integer
            4,   //  _8 // _field_angle
            4,   //  _9 // _field_tag
            1,   //  _A // _field_char_enum
            2,   //  _B // _field_short_enum
            4,   //  _C // _field_long_enum
            4,   //  _D // _field_long_flags
            2,   //  _E // _field_word_flags
            1,   //  _F // _field_byte_flags
            4,   // _10 // _field_point_2d
            4,   // _11 // _field_rectangle_2d
            4,   // _12 // _field_rgb_color
            4,   // _13 // _field_argb_color 
            4,   // _14 // _field_real
            4,   // _15 // _field_real_fraction
            8,   // _16 // _field_real_point_2d
            12,  // _17 // _field_real_point_3d
            8,   // _18 // _field_real_vector_2d
            12,  // _19 // _field_real_vector_3d
            16,  // _1A // _field_real_quaternion
            8,   // _1B // _field_real_euler_angles_2d
            12,  // _1C // _field_real_euler_angles_3d
            12,  // _1D // _field_real_plane_2d
            16,  // _1E // _field_real_plane_3d
            12,  // _1F // _field_real_rgb_color
            16,  // _20 // _field_real_argb_color
            4,   // _21 // _field_real_hsv_colo
            4,   // _22 // _field_real_ahsv_color
            4,   // _23 // _field_short_bounds
            8,   // _24 // _field_angle_bounds
            8,   // _25 // _field_real_bounds
            8,   // _26 // _field_real_fraction_bounds
            4,   // _27 // 
            4,   // _28 //
            4,   // _29 // _field_long_block_flags
            4,   // _2A // _field_word_block_flags
            4,   // _2B // _field_byte_block_flags
            1,   // _2C // _field_char_block_index
            1,   // _2D // _field_custom_char_block_index
            2,   // _2E // _field_short_block_index
            2,   // _2F // _field_custom_short_block_index
            4,   // _30 // _field_long_block_index
            4,   // _31 // _field_custom_long_block_index
            4,   // _32 // 
            4,   // _33 // 
            0,   // _34 // _field_pad 
            0,   // _35 // _field_skip
            0,   // _36 // _field_explanation
            0,   // _37 // _field_custom
            0,   // _38 // _field_struct 
            0,   // _39 // _field_array
            4,   // _3A // 
            0,   // _3B // end of struct
            1,   // _3C // _field_byte_integer
            2,   // _3D // _field_word_integer
            4,   // _3E // _field_dword_integer
            8,   // _3F // _field_qword_integer
            20,  // _40 // _field_block_v2
            28,  // _41 // _field_reference_v2
            24,  // _42 // _field_data_v2
            16,  // _43 // tag_resource
            4,   // _44 // UNKNOWN
            4    // _45 // UNKNOWN
        };
    }
}
