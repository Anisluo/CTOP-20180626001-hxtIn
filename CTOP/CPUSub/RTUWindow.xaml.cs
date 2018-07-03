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

namespace CTOP.CPUSub
{
    /// <summary>
    /// RTUWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RTUWindow : Window
    {
        public RTUWindow()
        {
            InitializeComponent();
        }
        public void Updata(CT2.CCommInfo cCommInfo)
        {
            Tool.mbTcpDriver.GetInfo(cCommInfo);
            if (cCommInfo.bIfRtuFromCfg)
            {
                Device1.Text = "配置文件";
            }
            else
            {
                Device1.Text = "默认配置";
            }
            Device2.Text = cCommInfo.RTUslaveAddress.ToString();
            Device3.Text = cCommInfo.BaudRS232.ToString();
            Device4.Text = cCommInfo.ParityRS232;
            Device5.Text = cCommInfo.DatabitRS232.ToString();
            Device6.Text = cCommInfo.StopbitRS232.ToString();
            Device7.Text = cCommInfo.BaudRS485.ToString();
            Device8.Text = cCommInfo.ParityRS485;
            Device9.Text = cCommInfo.DatabitRS485.ToString();
            Device10.Text = cCommInfo.StopbitRS485.ToString();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Tool.ViewCPUState = 0;
            this.Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
