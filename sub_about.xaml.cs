using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace IrAnalyse
{
    /// <summary>
    /// sub_about.xaml 的交互逻辑
    /// </summary>
    public partial class sub_about : UserControl
    {
        public sub_about()
        {
            InitializeComponent();
        }
        public static readonly RoutedEvent AboutMouseUpEvent = EventManager.RegisterRoutedEvent("AboutMouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_about));
        public event RoutedPropertyChangedEventHandler<object> AboutMouseUp
        {
            add { AddHandler(AboutMouseUpEvent, value); }
            remove { RemoveHandler(AboutMouseUpEvent, value); }
        }
 

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;

        }

        private void subabout_Click(object sender, RoutedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_about.AboutMouseUpEvent;
            this.RaiseEvent(args);
        }
    }
}
