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
using System.Windows.Threading;//定时器
using CircularGauge;
using System.Windows.Converters;

namespace CTOP.CPUSub
{
    
    /// <summary>
    /// SetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow : Window
    {
        private Data DataList = new Data();
        private DispatcherTimer readDataTimer = new DispatcherTimer();//通讯定时器类
        public SetWindow()
        {
            InitializeComponent(); 
            //CpuInfo1.SetBinding(TextBlock.TextProperty, new Binding("MaxTime_SchedLock") { Source = DataList });
            //CpuInfo2.SetBinding(TextBlock.TextProperty, new Binding("MaxTime_interrupt") { Source = DataList });
            //CpuInfo3.SetBinding(TextBlock.TextProperty, new Binding("CpuClikFreq") { Source = DataList });
            //CpuInfo4.SetBinding(TextBlock.TextProperty, new Binding("TaskCtxSwCtr") { Source = DataList });
            //CpuInfo5.SetBinding(TextBlock.TextProperty, new Binding("UcosVer") { Source = DataList });
            //CpuInfo6.SetBinding(TextBlock.TextProperty, new Binding("CPU_Usage") { Source = DataList });
            CpuInfo7.SetBinding(TextBlock.TextProperty, new Binding("CPU_MaxUsage") { Source = DataList });

            Info1.SetBinding(TextBlock.TextProperty, new Binding("Info1") { Source = DataList });
            Info2.SetBinding(TextBlock.TextProperty, new Binding("Info2") { Source = DataList });
            Info3.SetBinding(TextBlock.TextProperty, new Binding("Info3") { Source = DataList });
            Info4.SetBinding(TextBlock.TextProperty, new Binding("Info4") { Source = DataList });
            myGauge2.SetBinding(CircularGaugeControl.DialTextProperty, new Binding("CPU_Usage") { Source = DataList });
            myGauge2.SetBinding(CircularGaugeControl.CurrentValueProperty, new Binding("CPU_Usage") { Source = DataList });

            readDataTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            readDataTimer.Start();
        }
        public void Update0(CT2.CDeviceInfo cDeviceInfo, //设备信息
                            CT2.CCpuInfo cCpuInfo//CPU运行信息
                            )
        {
            Device1.Text = cDeviceInfo.m_FirmwareVer;
            Device2.Text = cDeviceInfo.m_HardwareVer;
            Device3.Text = cDeviceInfo.m_DeviceModel;
            Device4.Text = cDeviceInfo.m_strSerialNum;

            //CpuInfo1.Text = cCpuInfo.u32MaxTime_SchedLock.ToString(); ;
            //CpuInfo2.Text = cCpuInfo.i32MaxTime_interrupt.ToString();
            //CpuInfo3.Text = cCpuInfo.CpuClikFreq;
            //CpuInfo5.Text = cCpuInfo.ucosVer;
        }
        public void UpdateData(CT2.CCpuInfo cCpuInfo,//CPU运行信息
            CT2.CRtcInfo cRtcInfo)//温度电压信息
        {
            //DataList.TaskCtxSwCtr = cCpuInfo.i32TaskCtxSwCtr.ToString();
            DataList.CPU_Usage = cCpuInfo.CPU_Usage.ToString();
            DataList.CPU_MaxUsage = cCpuInfo.CPU_MaxUsage.ToString();
            
            DataList.Info1 = cRtcInfo.CpuCoreTemprature;
            DataList.Info2 = cRtcInfo.CpuCoreVoltage;
            DataList.Info3 = cRtcInfo.BatteryVoltage;
            DataList.Info4 = cRtcInfo.RtcCurrentTime;
        }

        private class Data : INotifyPropertyChanged
        {
            #region FirmwareVer
            private string m_FirmwareVer;
            public string FirmwareVer
            {
                get { return m_FirmwareVer; }
                set
                {
                    m_FirmwareVer = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FirmwareVer"));
                }
            }
            #endregion

            #region HardwareVer
            private string m_HardwareVer;
            public string HardwareVer
            {
                get { return m_HardwareVer; }
                set
                {
                    m_HardwareVer = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("HardwareVer"));
                }
            }
            #endregion

            #region DeviceModel
            private string m_DeviceModel;
            public string DeviceModel
            {
                get { return m_DeviceModel; }
                set
                {
                    m_DeviceModel = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DeviceModel"));
                }
            }
            #endregion

            #region strSerialNum
            private string m_strSerialNum;
            public string strSerialNum
            {
                get { return m_strSerialNum; }
                set
                {
                    m_strSerialNum = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("strSerialNum"));
                }
            }
            #endregion


            #region MaxTime_SchedLock
            private string m_u32MaxTime_SchedLock;
            public string MaxTime_SchedLock
            {
                get { return m_u32MaxTime_SchedLock; }
                set
                {
                    m_u32MaxTime_SchedLock = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MaxTime_SchedLock"));
                }
            }
            #endregion

            #region MaxTime_interrupt
            private string m_MaxTime_interrupt;
            public string MaxTime_interrupt
            {
                get { return m_MaxTime_interrupt; }
                set
                {
                    m_MaxTime_interrupt = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MaxTime_interrupt"));
                }
            }
            #endregion

            #region CpuClikFreq
            private string m_CpuClikFreq;
            public string CpuClikFreq
            {
                get { return m_CpuClikFreq; }
                set
                {
                    m_CpuClikFreq = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CpuClikFreq"));
                }
            }
            #endregion

            #region TaskCtxSwCtr
            private string m_TaskCtxSwCtr;
            public string TaskCtxSwCtr
            {
                get { return m_TaskCtxSwCtr; }
                set
                {
                    m_TaskCtxSwCtr = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TaskCtxSwCtr"));
                }
            }
            #endregion

            #region UcosVer
            private string m_UcosVer;
            public string UcosVer
            {
                get { return m_UcosVer; }
                set
                {
                    m_UcosVer = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("UcosVer"));
                }
            }
            #endregion

            #region CPU_Usage
            private string m_CPU_Usage;
            public string CPU_Usage
            {
                get { return m_CPU_Usage; }
                set
                {
                    m_CPU_Usage = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CPU_Usage"));
                }
            }
            #endregion

            #region CPU_MaxUsage
            private string m_CPU_MaxUsage;
            public string CPU_MaxUsage
            {
                get { return m_CPU_MaxUsage; }
                set
                {
                    m_CPU_MaxUsage = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CPU_MaxUsage"));
                }
            }
            #endregion


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

            #region Event
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            }
            #endregion

        }

        //自检
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dia1.Text = "";
            Dia2.Text = "";
            Dia3.Text = "";
            Dia4.Text = "";
            Dia5.Text = "";
            Tool.CPUSelfDiage();
            Dia1.Text = Tool.cSelfTestInfo.RamTestRes;
            Dia2.Text = Tool.cSelfTestInfo.TFCardTestRes;
            Dia3.Text = Tool.cSelfTestInfo.TFCardUsed.ToString()+" MB";//TFCardUsed
            Dia4.Text = Tool.cSelfTestInfo.TFCardTotal.ToString() + " MB";
            Dia5.Text = Tool.cSelfTestInfo.CfgFileTestRes;
        }

        //写入时钟
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Tool.WriteRTC(this.tbDateText.Text);
        }

        public void ShowCurTimer(object sender, EventArgs e)
        {
            //获得年月日
            this.tbDateText.Text = DateTime.Now.ToString("yyyy/MM/dd")+" "+ DateTime.Now.ToString("HH:mm:ss");   //yyyy/MM/dd
            //获得时分秒
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
            Tool.ViewCPUState = 0;
            readDataTimer.Stop();
        }
    }
}
