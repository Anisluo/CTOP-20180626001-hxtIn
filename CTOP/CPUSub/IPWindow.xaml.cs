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

using System.ComponentModel;//notify事件注册
namespace CTOP.CPUSub
{
    /// <summary>
    /// IPWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IPWindow : Window
    {
        private Data DataList = new Data();
        public IPWindow()
        {
            InitializeComponent();
            IP1.SetBinding(TextBlock.TextProperty, new Binding("IP1") { Source = DataList });
            IP2.SetBinding(TextBlock.TextProperty, new Binding("Port1") { Source = DataList });
            IP3.SetBinding(TextBlock.TextProperty, new Binding("IP2") { Source = DataList });
            IP4.SetBinding(TextBlock.TextProperty, new Binding("Port2") { Source = DataList });
            IP5.SetBinding(TextBlock.TextProperty, new Binding("IP3") { Source = DataList });
            IP6.SetBinding(TextBlock.TextProperty, new Binding("Port3") { Source = DataList });
            IP7.SetBinding(TextBlock.TextProperty, new Binding("IP4") { Source = DataList });
            IP8.SetBinding(TextBlock.TextProperty, new Binding("Port4") { Source = DataList });
        }
        private class Data : INotifyPropertyChanged
        {
            #region IP1
            private string m_FirmwareVer;
            public string IP1
            {
                get { return m_FirmwareVer; }
                set
                {
                    m_FirmwareVer = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IP1"));
                }
            }
            #endregion

            #region Port1
            private string m_HardwareVer;
            public string Port1
            {
                get { return m_HardwareVer; }
                set
                {
                    m_HardwareVer = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Port1"));
                }
            }
            #endregion

            #region IP2
            private string m_DeviceModel;
            public string IP2
            {
                get { return m_DeviceModel; }
                set
                {

                    m_DeviceModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IP2"));
                }
            }
            #endregion

            #region Port2
            private string m_strSerialNum;
            public string Port2
            {
                get { return m_strSerialNum; }
                set
                {
                    m_strSerialNum = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Port2"));
                }
            }
            #endregion


            #region IP3
            private string m_u32MaxTime_SchedLock;
            public string IP3
            {
                get { return m_u32MaxTime_SchedLock; }
                set
                {
                    m_u32MaxTime_SchedLock = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IP3"));
                }
            }
            #endregion

            #region Port3
            private string m_MaxTime_interrupt;
            public string Port3
            {
                get { return m_MaxTime_interrupt; }
                set
                {
                    m_MaxTime_interrupt = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Port3"));
                }
            }
            #endregion

            #region IP4
            private string m_CpuClikFreq;
            public string IP4
            {
                get { return m_CpuClikFreq; }
                set
                {
                    m_CpuClikFreq = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IP4"));
                }
            }
            #endregion

            #region Port4
            private string m_TaskCtxSwCtr;
            public string Port4
            {
                get { return m_TaskCtxSwCtr; }
                set
                {
                    m_TaskCtxSwCtr = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Port4"));
                }
            }
            #endregion


            #region Event
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            }
            #endregion
        }

        public void Update0(CT2.CCommInfo cCommInfo)
        {
            Tool.mbTcpDriver.GetInfo(cCommInfo);
            if (cCommInfo.bIfIpFromCfg)
            {
                Device1.Text = "配置文件";
            }
            else
            {
                Device1.Text = "默认配置";
            }
            Device2.Text = cCommInfo.LocalIP;
            Device3.Text = cCommInfo.RemoteIP;
            Device4.Text = cCommInfo.Netmask;
            Device5.Text = cCommInfo.Gateway;
            Device6.Text = cCommInfo.MacAddress;
        }

        public void UpdateData(CT2.CClientTcpInfo cClientTcpInfo)
        {
            Tool.mbTcpDriver.GetInfo(cClientTcpInfo);
            DataList.IP1 = cClientTcpInfo.clientIP1;
            DataList.Port1 = cClientTcpInfo.clientPort1.ToString();
            DataList.IP2 = cClientTcpInfo.clientIP2;
            DataList.Port2 = cClientTcpInfo.clientPort2.ToString();
            DataList.IP3 = cClientTcpInfo.clientIP3;
            DataList.Port3 = cClientTcpInfo.clientPort3.ToString();
            DataList.IP4 = cClientTcpInfo.clientIP4;
            DataList.Port4 = cClientTcpInfo.clientPort4.ToString();
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
