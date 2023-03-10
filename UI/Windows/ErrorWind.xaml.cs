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
using System.Windows.Shapes;

namespace TagEditor.UI.Windows{
    public partial class ErrorWind : Window{
        public ErrorWind(string context, Exception ex){
            InitializeComponent();
            e_context.Text = context;
            e_message.Text = ex.Message;
            e_source.Text = ex.Source;
            e_stacktrace.Text = ex.StackTrace;
        }
    }
}
