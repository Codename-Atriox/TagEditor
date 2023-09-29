using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static TagEditor.UI.Windows.TagInstance;

namespace TagEditor.UI.Interfaces.Editor
{
    /// <summary>
    /// Interaction logic for DiffExpand.xaml
    /// </summary>
    public partial class DiffExpand : UserControl
    {
        diffs_clump? parent;
        public DiffExpand(diffs_clump _parent){
            parent = _parent;
            InitializeComponent();
        }
        public void reload() {
            // here we just regenerate the ui if its open
            if (diff_popup.IsOpen == false) return;


            int diffs_count = 0;
            content_panel.Children.Clear();
            foreach (var diff_group in parent.groups){
                foreach (var diff in diff_group.Value.diffs){
                    var new_item = new DiffItem(diff_group.Value.target_line_number.ToString(), group_names[diff.Value.type], diff.Value.original_value, diff.Value.updated_value);
                    //new_item.LostFocus += Popup_LostFocus();
                    content_panel.Children.Add(new_item);
                    diffs_count++;
            }}

            lines_text.Text = "Line: " + parent.current_line_number;
            diffs_text.Text = "Diffs: " + diffs_count;

        }
        public void clear_references(){
            // im not really sure how this works, but this guy causes a circular reference which hypothetically would create a reference loop
            // where the machine wouldn't beable to garbage collect? although i'd have to assume c# already accounts for this
            // but just to play it safe, we break the reference loop so it definitely garbage collects
            parent = null; 
        }

        private void Button_Click(object sender, RoutedEventArgs e){
            if (diff_popup.IsOpen == true)
                return; // we should close the dropdown if its already open
            e.Handled = true;
            diff_popup.IsOpen = true;

            reload();

            scroll_panel.Focus();
            Keyboard.Focus(scroll_panel);
        }
        private void Popup_LostFocus(object sender, RoutedEventArgs e){ // close popup & save selected values

                diff_popup.IsOpen = false;
                content_panel.Children.Clear();
        }
    }
}
