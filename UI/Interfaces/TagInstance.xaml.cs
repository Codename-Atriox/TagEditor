using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using TagEditor.UI.Interfaces.Editor;
using static Infinite_module_test.tag_structs;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using TagEditor.UI.Interfaces.Editor.Params;

namespace TagEditor.UI.Windows{
    public partial class TagInstance : UserControl{
        public TagInstance(MainWindow _main, tag _loaded_tag, TabItem _container){
            InitializeComponent();
            main = _main;
            loaded_tag = _loaded_tag;
            container = _container;
            container.Content = this;

            indexes_panel.ItemsSource = line_indexes;
            types_panel.ItemsSource = line_groups;
        }
        MainWindow main;
        public TabItem container;
        tag loaded_tag;
        expand_link root_expand;
        public void LoadTag_UI(){
            if (loaded_tag.root.blocks.Count != 1)
            {

            }
            current_line_index = 0; // related to below line
            root_expand = recurs_map_expands(loaded_tag.root.GUID);


            XmlNode? root_struct_node = loaded_tag.reference_root.SelectSingleNode('_' + loaded_tag.root.GUID);
            string? param_name = root_struct_node.Attributes?["Name"]?.Value;

            StructParam param = new(param_name, loaded_tag.root.blocks[0], 0, loaded_tag.root.GUID, root_expand);
            params_panel.Children.Add(param);
            line_indexes.Add("0");
            line_groups.Add("root");
            setup_struct_element(root_expand, param, 0);
        }
        int current_line_index;
        expand_link recurs_map_expands(string struct_guid){
            XmlNode? guid_test = loaded_tag.reference_root.SelectSingleNode('_' + struct_guid);
            if (guid_test == null) Debug.Assert(false, "failed to locate guid xml node?");
            expand_link output = new(this);
            output.caculated_line_index = current_line_index;
            output.child_line_count = guid_test.ChildNodes.Count; // we could realistically just use this value, and not have to store it
            output.total_contained_lines = output.child_line_count;

            for (int i = 0; i < guid_test.ChildNodes.Count; i++){
                current_line_index++; 
                XmlNode node = guid_test.ChildNodes[i];
                int type = Convert.ToInt32(node.Name.Substring(1), 16);
                switch (type){
                    case 0x38:
                    case 0x39:
                    case 0x40:
                    case 0x43:
                        string next_guid = node.Attributes?["GUID"]?.Value;
                        expand_link new_child = recurs_map_expands(next_guid);
                        output.total_contained_lines += new_child.total_contained_lines;
                        output.child_links.Add(i, new_child);
                        break;
            }}
            return output;
        }
        public class expand_link{
            public expand_link(TagInstance parent){
                send_info_to = parent;
            }
            // only needs to be assigned for items that are automatically opened
            public int literal_line_index; // needs to be updated when any expand_link is either opened or closed

            public int caculated_line_index;
            public int child_line_count;
            public int total_contained_lines;
            // offset, child expander
            public Dictionary<int, expand_link> child_links = new();
            public TagInstance send_info_to; // needs to be given on construction
            // these will be assigned during tab opening and whatever (AKA RUNTIME)
            public BlockExpand expand_button; // needs to be assigned when the containing element is opened
            public UserControl struct_UI_element;

            public bool is_opened = false;
            public bool is_on_screen = false;
            public void wipe_child()
            {

                foreach (var v in child_links)
                { // cleanup expand buttons of the child elements that we're about to destroy
                    if (v.Value.is_opened) v.Value.expand(false);
                    v.Value.literal_line_index = 0;
                    send_info_to.expanders_panel.Children.Remove(v.Value.expand_button);
                    v.Value.expand_button = null;
                    v.Value.struct_UI_element = null;
                    v.Value.is_on_screen = false;
                }
            }
            public void expand(bool reload){ // this function is directly called by the expand button control, opposed to the struct UI element, thats why this even exists, so those two can be entirely unrelated
                if (!is_on_screen) Debug.Assert(false, "bad. this element is not visible and is attempting to expand");

                if (is_opened && reload) expand(false); // to close it so we can reopne it
                // we need to figure out what our struct type is
                // struct
                // array 
                // tagblock
                // resource block
                if (is_opened){
                    wipe_child();
                    if (struct_UI_element is StructParam)
                    {
                        StructParam test = struct_UI_element as StructParam;
                        test.params_panel.Children.Clear();
                        test.expand_indicator.Visibility = Visibility.Visible;
                    }
                    else if (struct_UI_element is ArrayParam)
                    {
                        ArrayParam test = struct_UI_element as ArrayParam;
                        test.params_panel.Children.Clear();
                        test.expand_indicator.Visibility = Visibility.Visible;
                    }
                    else if (struct_UI_element is TagblockParam)
                    {
                        TagblockParam test = struct_UI_element as TagblockParam;
                        test.params_panel.Children.Clear();
                        test.expand_indicator.Visibility = Visibility.Visible;
                    }
                    else if(struct_UI_element is ResourceParam)
                    {
                        ResourceParam test = struct_UI_element as ResourceParam;
                        test.params_panel.Children.Clear();
                        test.expand_indicator.Visibility = Visibility.Visible;
                    }

                    expand_button.visual.Text = "+";
                    is_opened = false;
                    send_info_to.lines_altered(literal_line_index+1, -child_line_count);
                }else{
                    if (struct_UI_element == null) Debug.Assert(false, "bad. no binded control");
                    if (struct_UI_element is TagblockParam && (struct_UI_element as TagblockParam).tag_data.blocks.Count == 0) return; // failed because tagblock not openable

                    expand_button.visual.Text = "-";
                    is_opened = true;
                    send_info_to.lines_altered(literal_line_index + 1, child_line_count);
                    if (struct_UI_element is StructParam)
                    {
                        StructParam test = struct_UI_element as StructParam;
                        test.expand_indicator.Visibility = Visibility.Collapsed;
                        send_info_to.Expand_struct(test.tag_data, test.guid, test.params_panel, test.struct_offset, this);
                    }
                    else if (struct_UI_element is ArrayParam)
                    {
                        ArrayParam test = struct_UI_element as ArrayParam;
                        test.expand_indicator.Visibility = Visibility.Collapsed;
                        send_info_to.Expand_struct(test.tag_data, test.guid, test.params_panel, test.struct_offset, this);
                    }
                    else if (struct_UI_element is TagblockParam)
                    {
                        TagblockParam test = struct_UI_element as TagblockParam;
                        test.expand_indicator.Visibility = Visibility.Collapsed;
                        send_info_to.Expand_struct(test.tag_data.blocks[test.selected_index], test.tag_data.GUID, test.params_panel, 0, this);
                    }
                    else if(struct_UI_element is ResourceParam)
                    {
                        ResourceParam test = struct_UI_element as ResourceParam;
                        test.expand_indicator.Visibility = Visibility.Collapsed;
                        send_info_to.Expand_struct(test.tag_data.blocks[0], test.tag_data.GUID, test.params_panel, 0, this);
                    }
                }
                send_info_to.attempt_to_update_lists();
        }}
        // note this can be + or -
        void lines_altered(int at_line, int num_of_lines_changed){
            if (num_of_lines_changed < 0){ // aka we're removing lines
                for (int i = 0; i < -num_of_lines_changed; i++){
                    line_indexes.RemoveAt(at_line);
                    line_groups.RemoveAt(at_line);
            }}
            if (at_line <= current_highlighted_line){ // reposition the selected line to reflect the position after the change
                if ((num_of_lines_changed < 0) && (at_line-num_of_lines_changed > current_highlighted_line))
                    current_highlighted_line = at_line-1;
                else current_highlighted_line += num_of_lines_changed;
                selec_border.RenderTransform = new TranslateTransform(0, current_highlighted_line * 18);
            }
            recurs_lines_altered(root_expand, at_line, num_of_lines_changed);
        }
        void recurs_lines_altered(expand_link link, int at_line, int num_of_lines_changed){
            if ((!link.is_on_screen) || (!link.is_opened)) return; // only is_opened is needed????
            foreach (var baby_link in link.child_links){
                if (baby_link.Value.is_on_screen == false) continue; // this type has not be inplemented yet, skip for now // REPLACE WITH ERROR ONCE IMPLEMENTED //

                if (baby_link.Value.literal_line_index >= at_line){ // so if this is past the point where they'd be affected
                    baby_link.Value.literal_line_index += num_of_lines_changed;
                    // recalculate visual position
                    baby_link.Value.expand_button.RenderTransform = new TranslateTransform(0, baby_link.Value.literal_line_index * 18);
                }
                recurs_lines_altered(baby_link.Value, at_line, num_of_lines_changed);
        }}
        void attempt_to_update_lists(){
            indexes_panel.ItemsSource = line_indexes;
            types_panel.ItemsSource = line_groups;
        }

        void setup_struct_element(expand_link data_link, UserControl target, int line_index){
            data_link.struct_UI_element = target; // inherit the target for expanding
            data_link.literal_line_index = line_index;
            data_link.is_on_screen = true;

            data_link.expand_button = new(data_link){
                RenderTransform = new TranslateTransform(0, line_index * 18)
            };
            // we need a panel 
            expanders_panel.Children.Add(data_link.expand_button);

            //if (struct_UI_element is StructParam)
            //else if (struct_UI_element is StructParam)
            //else if (struct_UI_element is StructParam)
            //else if (struct_UI_element is StructParam)
            //StructParam test = target as StructParam;
            //data_link.expand(); // not a good idea to expand as we're setting up, because it would prove to be quite difficult to know which line the next struct would be at, seeing as this doesn't return that information
            //Expand_struct(test.tag_data, test.guid, test.params_panel, test.struct_offset, data_link);
        }

        ObservableCollection<string> line_indexes = new();
        ObservableCollection<string> line_groups = new();
        public void Expand_struct(tag.thing _struct, string struct_guid, StackPanel container, int struct_offset, expand_link expandus_linkus){

            container.Children.Clear(); // knock out any previous elements here, although it would be more efficient to reuse them
            XmlNode? guid_test = loaded_tag.reference_root.SelectSingleNode('_' + struct_guid);
            if (guid_test == null) Debug.Assert(false, "failed to locate guid xml node?");

            int current_line = expandus_linkus.literal_line_index;
            int theoretical_line = expandus_linkus.caculated_line_index;

            for (int i = 0; i < guid_test.ChildNodes.Count; i++){

                XmlNode node = guid_test.ChildNodes[i];
                string? param_name = node.Attributes?["Name"]?.Value;
                if (param_name == null)  Debug.Assert(false, "param has no name???");

                int offset = Convert.ToInt32(node.Attributes?["Offset"]?.Value, 16);
                offset += struct_offset; // this is for structs & array structs, so we can offset to the correct position, even though we're reading a different group of params


                // note that we need to calucate the block offset, not the filew offset as teh file offset doesn't exist as soon as we load the tag
                // then we give them the bock offset, and nam, and then tell them to initalize their value IE read from that offset
                // we also need to assign the block that its offsetted from, so it can read & write
                // integer types nee the int length, and signed boolean
                current_line++;
                theoretical_line++;
                int type = Convert.ToInt32(node.Name.Substring(1), 16);
                if (line_groups.Count < current_line) line_groups.Add(group_names[type]);
                else line_groups.Insert(current_line, group_names[type]);

                // shouldn't be separate, but whatever for now
                // figure out what index this guy should actually be at
                if (line_indexes.Count < current_line) line_indexes.Add(theoretical_line.ToString());
                else line_indexes.Insert(current_line, theoretical_line.ToString());

                switch (type){
                    case 0x0:{ // _field_string
                            StringParam pad_garb_val = new(param_name, _struct.tag_data, offset, 32);
                            container.Children.Add(pad_garb_val);
                        }continue;  
                    case 0x1:{ // _field_long_string
                            StringParam pad_garb_val = new(param_name, _struct.tag_data, offset, 256);
                            container.Children.Add(pad_garb_val);
                        }continue; 
                    case 0x2:{ // _field_string_id
                            HashParam pad_garb_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(pad_garb_val);
                        }continue;
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
                    case 0x8:{ // _field_angle
                            AngleParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case  0x9: // _field_tag
                        break;  
                    case  0xA:{ // _field_char_enum
                            string[] enum_names = new string[node.ChildNodes.Count];
                            for (int i2 = 0; i2 < enum_names.Length; i2++)
                                enum_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                            EnumParam new_val = new(param_name, _struct.tag_data, offset, EnumParam.EnumType._byte, enum_names);
                            container.Children.Add(new_val);
                        }continue;
                    case  0xB:{ // _field_short_enum
                            string[] enum_names = new string[node.ChildNodes.Count];
                            for (int i2 = 0; i2 < enum_names.Length; i2++)
                                enum_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                            EnumParam new_val = new(param_name, _struct.tag_data, offset, EnumParam.EnumType._short, enum_names);
                            container.Children.Add(new_val);
                        }continue;
                    case  0xC:{ // _field_long_enum
                            string[] enum_names = new string[node.ChildNodes.Count];
                            for (int i2 = 0; i2 < enum_names.Length; i2++)
                                enum_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                            EnumParam new_val = new(param_name, _struct.tag_data, offset, EnumParam.EnumType._int, enum_names);
                            container.Children.Add(new_val);
                        }continue;
                    case  0xD:{ // _field_long_flags
                            string[] flag_names = new string[32];
                            for (int i2 = 0; i2 < flag_names.Length; i2++){
                                if (node.ChildNodes.Count > i2) flag_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                                else flag_names[i2] = "Flag" + i2;
                            }
                            FlagsParam new_val = new(param_name, _struct.tag_data, offset, FlagsParam.FlagType._int, flag_names);
                            container.Children.Add(new_val);
                        }continue;
                    case  0xE:{ // _field_word_flags
                            string[] flag_names = new string[16];
                            for (int i2 = 0; i2 < flag_names.Length; i2++){
                                if (node.ChildNodes.Count > i2) flag_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                                else flag_names[i2] = "Flag" + i2;
                            }
                            FlagsParam new_val = new(param_name, _struct.tag_data, offset, FlagsParam.FlagType._short, flag_names);
                            container.Children.Add(new_val);
                        }continue;
                    case  0xF:{ // _field_byte_flags
                            string[] flag_names = new string[8];
                            for (int i2 = 0; i2 < flag_names.Length; i2++){
                                if (node.ChildNodes.Count > i2) flag_names[i2] = node.ChildNodes[i2].Attributes?["n"]?.Value;
                                else flag_names[i2] = "Flag" + i2;
                            }
                            FlagsParam new_val = new(param_name, _struct.tag_data, offset, FlagsParam.FlagType._byte, flag_names);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x10:{ // _field_point_2d
                            DoubleshortParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x11:{ // _field_rectangle_2d
                            DoubleshortParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x12: // _field_rgb_color
                        break;  
                    case 0x13: // _field_argb_color 
                        break;   
                    case 0x14:{ // _field_real
                            FloatParam new_val = new(param_name, _struct.tag_data, offset, false);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x15:{ // _field_real_fraction
                            FloatParam new_val = new(param_name, _struct.tag_data, offset, true);
                            container.Children.Add(new_val);
                        }continue;  
                    case 0x16:{ // _field_real_point_2d
                            DoublefloatParam new_val = new(param_name, _struct.tag_data, offset, false);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x17:{ // _field_real_point_3d
                            TriplefloatParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x18:{ // _field_real_vector_2d
                            DoublefloatParam new_val = new(param_name, _struct.tag_data, offset, false);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x19:{ // _field_real_vector_3d
                            TriplefloatParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x1A:  // _field_real_quaternion
                        break; 
                    case 0x1B:{ // _field_real_euler_angles_2d
                            DoublefloatParam new_val = new(param_name, _struct.tag_data, offset, false);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x1C:{ // _field_real_euler_angles_3d
                            TriplefloatParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
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
                    case 0x23:{ // _field_short_bounds
                            DoubleshortParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x24:{ // _field_angle_bounds
                            DoubleangleParam new_val = new(param_name, _struct.tag_data, offset);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x25:{ // _field_real_bounds
                            DoublefloatParam new_val = new(param_name, _struct.tag_data, offset, false);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x26:{ // _field_real_fraction_bounds
                            DoublefloatParam new_val = new(param_name, _struct.tag_data, offset, true);
                            container.Children.Add(new_val);
                        }continue;
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
                    case 0x38:{ // _field_struct 
                            string next_guid = node.Attributes?["GUID"]?.Value;
                            expand_link struct_link = expandus_linkus.child_links[i];
                            StructParam param = new(param_name, _struct, offset, next_guid, struct_link);
                            container.Children.Add(param);
                            setup_struct_element(struct_link, param, current_line);
                            theoretical_line += struct_link.total_contained_lines;
                        }continue;
                    case 0x39:{ // _field_array
                            string next_guid = node.Attributes?["GUID"]?.Value;
                            int array_length = Convert.ToInt32(node.Attributes?["Count"]?.Value);
                            expand_link struct_link = expandus_linkus.child_links[i];
                            ArrayParam param = new(param_name, _struct, offset, next_guid, struct_link, array_length);
                            container.Children.Add(param);
                            setup_struct_element(struct_link, param, current_line);
                            theoretical_line += struct_link.total_contained_lines;
                        }continue;
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
                    case 0x40:{ // _field_block_v2
                            if (!_struct.tag_block_refs.ContainsKey((ulong)offset)) 
                                break;

                            expand_link struct_link = expandus_linkus.child_links[i];
                            TagblockParam param = new(param_name, _struct.tag_block_refs[(ulong)offset], struct_link);
                            container.Children.Add(param);
                            setup_struct_element(struct_link, param, current_line);
                            theoretical_line += struct_link.total_contained_lines;
                        }continue;
                    case 0x41: // _field_reference_v2
                        break;
                    case 0x42:{ // _field_data_v2
                            DataParam new_val = new(param_name, _struct.tag_resource_refs[(ulong)offset], _struct.tag_data, offset, param_group_sizes[type]);
                            container.Children.Add(new_val);
                        }continue;
                    case 0x43:{ // tag_resource
                            expand_link struct_link = expandus_linkus.child_links[i];
                            ResourceParam param = new(param_name, _struct.resource_file_refs[(ulong)offset], struct_link);
                            container.Children.Add(param);
                            setup_struct_element(struct_link, param, current_line);
                            theoretical_line += struct_link.total_contained_lines;
                        }continue;
                    case 0x44: // UNKNOWN
                        break;
                    case 0x45: // UNKNOWN
                        break;
                } // if we're still here, then the type was unimplemented, create new garbage block
                GarbageParameter garb_val = new(param_name, _struct.tag_data, offset, param_group_sizes[type]);
                container.Children.Add(garb_val);
            }
        }

        // DEBUG ONLY, HAS NO FUNCTION OTHER THAN FOR MENTALLY SORTING GROUPS //
        public static int[] testt = new int[]
        {
            


            // TAG REFERENCE //
            28,  // _41 // _field_reference_v2


            // TAG GROUP //
            4,   //  _9 // _field_tag

            // BYTE COLORS //
            4,   // _12 // _field_rgb_color
            4,   // _13 // _field_argb_color 
            // HSV COLORS //
            4,   // _21 // _field_real_hsv_coloR
            4,   // _22 // _field_real_ahsv_color
            // FLOAT COLORS //
            12,  // _1F // _field_real_rgb_color
            16,  // _20 // _field_real_argb_color

            // QUARTERNION //
            16,  // _1A // _field_real_quaternion

            // PLANE 2D //
            12,  // _1D // _field_real_plane_2d

            // PLANE 3D //
            16,  // _1E // _field_real_plane_3d








            // COMPLETED //

            
            // ENUMS //
            1,   //  _A // _field_char_enum
            2,   //  _B // _field_short_enum
            4,   //  _C // _field_long_enum

            // FLAGS //
            4,   //  _D // _field_long_flags
            2,   //  _E // _field_word_flags
            1,   //  _F // _field_byte_flags

            // SHORT RANGE //
            4,   // _23 // _field_short_bounds
            4,   // _10 // _field_point_2d
            4,   // _11 // _field_rectangle_2d
            
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
            
            // ANGLE //
            4,   //  _8 // _field_angle
            // ANGLE RANGE //
            8,   // _24 // _field_angle_bounds

            // STRING //
            32,  //  _0 // _field_string
            256, //  _1 // _field_long_string

            // HASH //
            4,   //  _2 // _field_string_id 
            
            // COMMENT //
            0,   // _36 // _field_explanation
            0,   // _37 // _field_custom

            // STRUCT //
            0,   // _38 // _field_struct 

            // STRUCT ARRAY //
            0,   // _39 // _field_array

            // TAGBLOCK //
            20,  // _40 // _field_block_v2

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
        // more intuitive/userfriendly group names, some may need slight reworks however
        public static string[] group_names = new string[]
        {
            "string32",
            "string256",
            "stringID",
            "UNKNOWN",
            "sbyte",
            "short",
            "int",
            "long",
            "angle",
            "group",
            "enum8",
            "enum16",
            "enum32",
            "flags32",
            "flags16",
            "flags8",
            "point2D", 
            "rectangle2D",
            "rgb",
            "argb",
            "float",
            "fraction",
            "float2D",
            "float3D",
            "vector2D",
            "vector3D",
            "quarternion",
            "euler2D",
            "euler3D",
            "plane2D",
            "plane3D",
            "rgb_float",
            "argb_float",
            "hsv",
            "ahsv",
            "short_r",
            "angle_r",
            "float_r",
            "fraction_r",
            "UNKNOWN",
            "UNKNOWN",
            "b_flags32",
            "b_flags16",
            "b_flags8",
            "block8",
            "cblock8",
            "block16",
            "cblock16",
            "block32",
            "cblock32",
            "UNKNOWN",
            "UNKNOWN",
            "padding",
            "skip",
            "comment",
            "c_comment",
            "struct",
            "array",
            "UNKNOWN",
            "struct_end",
            "byte",
            "ushort",
            "uint",
            "ulong",
            "tagblock",
            "tagref",
            "data",
            "resource",
            "UNKNOWN",
            "UNKNOWN",
        };
        int current_highlighted_line = -1;
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // find where it was on screen, we only care about the Y axis
            // then calculate which line it was, recalculate so we know the position of the start of that line
            // finally create a border that simply highlights this box
            int clicked_line = (int)Math.Truncate(Mouse.GetPosition(editor_container).Y / 18);

            if (clicked_line >= line_indexes.Count) return; // clicked outside

            current_highlighted_line = clicked_line;
            if (selec_border.Visibility == Visibility.Collapsed) selec_border.Visibility = Visibility.Visible;


            selec_border.RenderTransform = new TranslateTransform(0, current_highlighted_line*18);

        }
    }
}
