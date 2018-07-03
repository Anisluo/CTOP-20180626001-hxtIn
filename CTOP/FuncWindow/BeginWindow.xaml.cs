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
//using System.Threading;
using System.Windows.Media.Animation;//动画类工具
using System.ComponentModel;
namespace CTOP.FuncWindow
{

    public partial class BeginWindow : Window
    {
        private bool IsExpand = true;//xmal设计时是展开的
        private BackgroundWorker backgroundWorker;
        private string strRes = "";
        public BeginWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker)this.FindResource("backgroundWorker");
            UIExpand();
            PLC_IP.Focus();
            this.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    this.ResizeMode = ResizeMode.NoResize;
                }
            };
            this.PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (this.ResizeMode == ResizeMode.NoResize)
                {
                    this.ResizeMode = ResizeMode.CanResize;
                }
            };
            Tool.OpenIPXml();
            PLC_IP.Text = Tool.CPUIP;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)e.OriginalSource;
            string str = cmd.Content.ToString();
            if (str=="取消" || str == "")
            {
                Tool.IsInit = false;
                this.Close();
                return;
            }

            Tool.CPUIP = PLC_IP.Text.Trim();
            Tool.WriteOffMS = Convert.ToInt32(WriteOffMS.SelectionBoxItem.ToString());
            Tool.RedOffMS = Convert.ToInt32(RedOffMS.SelectionBoxItem.ToString());
            Tool.Retry = Convert.ToInt32(Retry.SelectionBoxItem.ToString());
            Tool.WaitToRetryMS = Convert.ToInt32(WaitToRetryMS.SelectionBoxItem.ToString());

            MyTitle.Text = "与控制器连接中...";
            MyTitle.Visibility = Visibility.Visible;
            MainContent.IsEnabled = false;
            Close.IsEnabled = false;
            backgroundWorker.RunWorkerAsync();



            string cycle;
            cycle = DelayT.SelectionBoxItem.ToString();
            Tool.MSCycle = Convert.ToInt32(cycle);
            //cycle = AnimMS.SelectionBoxItem.ToString();
            //Tool.AnimMS = Convert.ToInt32(cycle);
            Tool.OnlineCount = 5000 / Tool.MSCycle;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            UIExpand();
        }

        private void UIExpand()
        {
            if (IsExpand)
            {
                SetContent.Visibility = Visibility.Collapsed;
                DoubleAnimation heightAnimation = new DoubleAnimation(this.Height, 130, new Duration(TimeSpan.FromSeconds(0.16)),FillBehavior.Stop);
                this.BeginAnimation(Border.HeightProperty, heightAnimation);
                
                IsExpand = false;
            }
            else
            {
                SetContent.Visibility = Visibility.Visible;
                DoubleAnimation heightAnimation = new DoubleAnimation(this.Height, 350, new Duration(TimeSpan.FromSeconds(0.16)), FillBehavior.Stop);
                this.BeginAnimation(Border.HeightProperty, heightAnimation);
                
                IsExpand = true;
            }
        }

        private void BeginCommu()
        {
            strRes = Tool.mbTcpDriver.Init(Tool.CPUIP, 502);
            if (strRes != "NO_ERR")
            {
                Tool.IsOnline = false;
                strRes = "请检查IP地址或电脑与控制器的连接";
            }
            else
            {
                Tool.mbTcpDriver.SetParam(Tool.RedOffMS, Tool.Retry, Tool.WaitToRetryMS, Tool.WriteOffMS, 50);
                strRes = Tool.mbTcpDriver.GetIoNode(ref Tool.UIlist);
                if (strRes != "NO_ERR")
                {
                    Tool.IsOnline = false;
                    strRes = "连接方返回数据错误";
                }
                else
                {
                    Tool.IsOnline = true;
                }
            }
            if (Tool.IsOnline)//通讯已经初始化后 再判断报文协议是否是辰竹控制器
            {
                CT2.CDeviceInfo cDeviceInfo = new CT2.CDeviceInfo();//CPU信息
                strRes=Tool.mbTcpDriver.GetInfo(cDeviceInfo);
                if (strRes != "NO_ERR")
                {
                    Tool.IsOnline = false;
                    strRes = "连接方不是CTOP控制器";
                }
                else
                {
                    if (!cDeviceInfo.m_DeviceModel.Contains("CPU"))
                    {
                        Tool.IsOnline = false;
                        strRes = "连接方不是CTOP控制器";
                    }
                }

            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BeginCommu();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Thread cancelled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "An Error Occurred");
            }
            else
            {
                if (Tool.IsOnline)
                {
                    this.Close();
                    Tool.IsInit = true;
                    MyTitle.Visibility = Visibility.Collapsed;
                    Tool.SaveIPXml();
                }
                else
                {
                    MyTitle.Visibility = Visibility.Visible;
                    //MyTitle.Background =new  SolidColorBrush(Colors.Red);
                    Tool.IsInit = false;
                    MyTitle.Text = "连接失败：" + strRes;
                    ConnectBtn.Content = "重试";
                }
                MainContent.IsEnabled = true;
                Close.IsEnabled = true;
                PLC_IP.Focus();
            }
        }
    }

}
