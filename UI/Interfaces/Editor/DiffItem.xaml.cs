using System.Windows.Controls;

namespace TagEditor.UI.Interfaces.Editor
{
    /// <summary>
    /// Interaction logic for DiffItem.xaml
    /// </summary>
    public partial class DiffItem : UserControl
    {
        public DiffItem(string linenumber, string param_type, string og_value, string updated_value)
        {
            InitializeComponent();
            line_0.Text = linenumber;
            line_1.Text = linenumber;
            type_0.Text = param_type;
            type_1.Text = param_type;
            old_value.Text = og_value;
            new_value.Text = updated_value;
        }
    }
}
