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
    /// BusWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BusWindow : Window
    {
        private Data DataList = new Data();
        public BusWindow()
        {
            InitializeComponent();
            Device1.SetBinding(TextBlock.TextProperty, new Binding("Info1") { Source = DataList });
            Device2.SetBinding(TextBlock.TextProperty, new Binding("Info2") { Source = DataList });
            Device3.SetBinding(TextBlock.TextProperty, new Binding("Info3") { Source = DataList });
            Device4.SetBinding(TextBlock.TextProperty, new Binding("Info4") { Source = DataList });
            Device5.SetBinding(TextBlock.TextProperty, new Binding("Info5") { Source = DataList });
        }

        public void UpdateIOGroup(CT2.CBackplaneInfo cBackplaneInfo)
        {
            Tool.mbTcpDriver.GetInfo(cBackplaneInfo);
            if (cBackplaneInfo.bIfErr)
            {
                DataList.Info1 = "故障";
            }
            else
            {
                DataList.Info1 = "正常";
            }
            DataList.Info2 = cBackplaneInfo.BpStatus;
            DataList.Info3 = cBackplaneInfo.BpErrCode.ToString();
            DataList.Info4 = cBackplaneInfo.ErrCountRecv.ToString();
            DataList.Info5 = cBackplaneInfo.ErrCountSend.ToString();
        }

        private class Data : INotifyPropertyChanged
        {
            #region Info1
            private string m_Info1;
            public string Info1
            {
                get { return m_Info1; }
                set
                {
                    m_Info1 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Info1"));
                }
            }
            #endregion

            #region Info2
            private string m_Info2;
            public string Info2
            {
                get { return m_Info2; }
                set
                {
                    m_Info2 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Info2"));
                }
            }
            #endregion

            #region Info3
            private string m_Info3;
            public string Info3
            {
                get { return m_Info3; }
                set
                {
                    m_Info3 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Info3"));
                }
            }
            #endregion

            #region Info4
            private string m_Info4;
            public string Info4
            {
                get { return m_Info4; }
                set
                {
                    m_Info4 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Info4"));
                }
            }
            #endregion

            #region Info5
            private string m_Info5;
            public string Info5
            {
                get { return m_Info5; }
                set
                {
                    m_Info5 = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Info5"));
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
