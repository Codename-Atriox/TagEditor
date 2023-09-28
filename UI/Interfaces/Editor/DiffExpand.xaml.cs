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
        public void reload()
        {

        }
        public void clear_references(){
            // im not really sure how this works, but this guy causes a circular reference which hypothetically would create a reference loop
            // where the machine wouldn't beable to garbage collect? although i'd have to assume c# already accounts for this
            // but just to play it safe, we break the reference loop so it definitely garbage collects
            parent = null; 
        }
    }
}
