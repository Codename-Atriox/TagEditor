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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TagEditor.UI.Windows.TagInstance;

namespace TagEditor.UI.Interfaces.Editor
{
    /// <summary>
    /// Interaction logic for BlockExpand.xaml
    /// </summary>
    public partial class BlockExpand : UserControl
    {
        public BlockExpand(expand_link _parent)
        {
            InitializeComponent();
            parent = _parent;
        }
        public expand_link parent;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            parent.expand(false);

        }
        //=> parent.expand();
    }
}
