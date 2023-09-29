using System.Windows.Controls;
using TagEditor.UI.Windows;
using static TagEditor.UI.Windows.TagInstance;

namespace TagEditor.UI.Interfaces.Editor{
    /// <summary>
    /// Interaction logic for DiffItem.xaml
    /// </summary>
    public partial class DiffItem : UserControl{
        TagInstance callback;
        param_diff diff;
        public DiffItem(TagInstance _callback, param_diff _diff){
            diff = _diff;
            callback = _callback;
            InitializeComponent();
            line_0.Text = _diff.line_group.target_line_number.ToString();
            line_1.Text = _diff.line_group.target_line_number.ToString();
            type_0.Text = TagInstance.group_names[_diff.type];
            type_1.Text = TagInstance.group_names[_diff.type];
            old_value.Text = _diff.original_value;
            new_value.Text = _diff.updated_value;
        }

        public void Button_Click(object sender, System.Windows.RoutedEventArgs e){
            e.Handled = true;
            callback.revert_diff(diff);
        }
    }
}
