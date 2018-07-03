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

namespace CTOP.FuncWindow
{
    /// <summary>
    /// WeChatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WeChatWindow : Window
    {
        public WeChatWindow()
        {
            InitializeComponent();
            //this.ShowInTaskbar = false;
            //this.Topmost = true;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tool.WeChat.Close();
        }
    }
}
