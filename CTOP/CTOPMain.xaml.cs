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

using CTOP.Base;//各IO模型界面
using CTOP.List;//各列表界面
using CTOP.modbusTcp;//通讯
using CTOP.CPUSub;
using CTOP.FuncWindow;
using System.Reflection;//反射
//using System.Globalization;//开关按钮圆形按钮颜色填充转换类
using System.Xml;
using System.Threading;
using System.Windows.Interop;//支持窗口句柄
using System.ComponentModel;

namespace CTOP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    // 全局静态类对象
    public static class Tool
    {
        public static  MbTcpDriver mbTcpDriver=new MbTcpDriver();//全局通讯类
        public static List<CT2.CIoNode> UIlist=new List<CT2.CIoNode>();//全局通讯数据类
        public static bool IsInit=false;//通讯类是否完成初始化
        public static bool IsOnline;//通讯在线状态
        public static bool Switch;//通讯开关
        public static int MSCycle;//通讯类定时器刷新间隔，ms
        public static int OnlineCount;//模块在线状态刷新间隔，s
        public static int AnimMS=40;//模块缩放动画持续时间，ms

        public static int WriteOffMS;//通讯写超时时间
        public static int RedOffMS;//通讯写超时时间
        public static int Retry;//通讯重试次数
        public static int WaitToRetryMS;//等待重试间隔

        public static int ViewState = 0;//当前展示的界面代号
        public static int ViewCPUState = 0;//当前展示的CPU信息的界面代号
        public static CT2.CSelfTestInfo cSelfTestInfo = new CT2.CSelfTestInfo();//自检开关
        public static CT2.CRtcSettingInfo cRtcSettingInfo = new CT2.CRtcSettingInfo();
        public static string CPUIP ;//控制器IP地址
        public static int AI8ITypeVal;
        public static int MDataClcState=0;
        //全局窗口类，不能直接new
        public static BusWindow Bus;
        public static IPWindow Ip;
        public static RTUWindow Rtu;
        public static SetWindow Set;
        public static WeChatWindow WeChat;
        public static HelpWindow Help;
        public static InforWindow inforWindow;

        public static DO DOListView = new DO();
        public static DI DIListView = new DI();
        public static AO AOListView = new AO();
        public static AI AIListView = new AI();
        public static ShareMemo Mzone = new ShareMemo();
        public static void CloseWindow()
        {
            if (Bus!=null)//类是否实例化
            {
                if (Bus.IsLoaded)//实例化后有可能被关闭了，需要判断是否加载过
                {
                    Bus.Close();
                }
            }
            
            if (Ip != null)
            {
                if (Ip.IsLoaded)
                {
                    Ip.Close();
                }
            }
            if (Rtu != null)
            {
                if (Rtu.IsLoaded)
                {
                    Rtu.Close();
                }
            }
            if (Set != null)
            {
                if (Set.IsLoaded)
                {
                    Set.Close();
                }
            }
            if (WeChat != null)
            {
                if (WeChat.IsLoaded)
                {
                    WeChat.Close();
                }
            }
            if (Help != null)
            {
                if (Help.IsLoaded)
                {
                    Help.Close();
                }
            }
            if (inforWindow != null)
            {
                if (inforWindow.IsLoaded)
                {
                    inforWindow.Close();
                }
            }
        }
        public static void CPUSelfDiage()
        {
            cSelfTestInfo.bSwitchSetTest = true;
            mbTcpDriver.WriteInfo(cSelfTestInfo);
            Thread.Sleep(1000);
            mbTcpDriver.GetInfo(cSelfTestInfo);
            //while (cSelfTestInfo.bSwitchSetTest)
            //{
            //    mbTcpDriver.GetInfo(cSelfTestInfo);
            //}
        }
        public static void WriteRTC(string Datetime)
        {
            cRtcSettingInfo.RtcSettingTime = Datetime;
            cRtcSettingInfo.bIfSetting = true;
            mbTcpDriver.WriteInfo(cRtcSettingInfo);
            mbTcpDriver.GetInfo(cRtcSettingInfo);
        }
        public static void SaveDesXml()
        {
            string URL = "";
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "Xml files (*.xml)|*.xml";
            //openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            //saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "保存描述文件";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                URL = saveFileDialog.FileName;
            }
            else
            {
                return;
            }

            CT2ListDO tDO;
            CT2ListDI tDI;
            CT2ListAO tAO;
            CT2ListAI tAI;

            XmlDocument xml = new XmlDocument();
            XmlNode ListIO = xml.CreateElement("ListIO");
            XmlNode ListDO = xml.CreateElement("ListDO");
            XmlNode ListDI = xml.CreateElement("ListDI");
            XmlNode ListAO = xml.CreateElement("ListAO");
            XmlNode ListAI = xml.CreateElement("ListAI");

            foreach (var item in DOListView.listDO)
            {
                XmlNode T = xml.CreateElement("Description");
                tDO = (CT2ListDO)item;
                T.InnerText = tDO.Description;
                ListDO.AppendChild(T);
            }

            foreach (var item in DIListView.listDI)
            {
                XmlNode T = xml.CreateElement("Description");
                tDI = (CT2ListDI)item;
                T.InnerText = tDI.Description;
                ListDI.AppendChild(T);
            }

            foreach (var item in AOListView.listAO)
            {
                XmlNode T = xml.CreateElement("Description");
                tAO = (CT2ListAO)item;
                T.InnerText = tAO.Description;
                ListAO.AppendChild(T);
            }

            foreach (var item in AIListView.listAI)
            {
                XmlNode T = xml.CreateElement("Description");
                tAI = (CT2ListAI)item;
                T.InnerText = tAI.Description;
                ListAI.AppendChild(T);
            }

            ListIO.AppendChild(ListDO);
            ListIO.AppendChild(ListDI);
            ListIO.AppendChild(ListAO);
            ListIO.AppendChild(ListAI);
            xml.AppendChild(ListIO);
            xml.Save(URL);
        }
        public static void OpenDesXml()
        {
            string URL = "";
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            //openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.Title = "导入描述文件";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                URL = openFileDialog1.FileName; 
            }
            else
            {
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(URL);
            XmlNode ListDO = xml.SelectSingleNode("ListIO/ListDO");
            XmlNode ListDI = xml.SelectSingleNode("ListIO/ListDI");
            XmlNode ListAO = xml.SelectSingleNode("ListIO/ListAO");
            XmlNode ListAI = xml.SelectSingleNode("ListIO/ListAI");

            if (ListDO != null)
            {
                int i = 0;
                List<string> ListDes = new List<string>();
                XmlNodeList xNlist = ListDO.ChildNodes;
                foreach (XmlNode xCur in xNlist)
                {
                    ListDes.Add(xCur.InnerText);
                }
                foreach (var item in DOListView.listDO)
                {
                    item.Description = ListDes[i];
                    i++;
                }
            }

            if (ListDI != null)
            {
                int i = 0;
                List<string> ListDes = new List<string>();
                XmlNodeList xNlist = ListDI.ChildNodes;
                foreach (XmlNode xCur in xNlist)
                {
                    ListDes.Add(xCur.InnerText);
                }
                foreach (var item in DIListView.listDI)
                {
                    item.Description = ListDes[i];
                    i++;
                }
            }

            if (ListAO != null)
            {
                int i = 0;
                List<string> ListDes = new List<string>();
                XmlNodeList xNlist = ListAO.ChildNodes;
                foreach (XmlNode xCur in xNlist)
                {
                    ListDes.Add(xCur.InnerText);
                }
                foreach (var item in AOListView.listAO)
                {
                    item.Description = ListDes[i];
                    i++;
                }
            }

            if (ListAI != null)
            {
                int i = 0;
                List<string> ListDes = new List<string>();
                XmlNodeList xNlist = ListAI.ChildNodes;
                foreach (XmlNode xCur in xNlist)
                {
                    ListDes.Add(xCur.InnerText);
                }
                foreach (var item in AIListView.listAI)
                {
                    item.Description = ListDes[i];
                    i++;
                }
            }
        }
        public static void SaveIPXml()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlNode IP = xml.CreateElement("IP");
                IP.InnerText = CPUIP;
                xml.AppendChild(IP);
                xml.Save("IP.xml");
            }
            catch (Exception)
            {
                return;
            }

        }
        public static void OpenIPXml()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("IP.xml");
                XmlNode IP = xml.SelectSingleNode("IP");
                CPUIP = IP.InnerText;
            }
            catch (Exception)
            {
                return;
            }
        }
    }

    //背景颜色转换转换器
    //public class GradeState : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return null;
    //        dynamic data = value;
    //        //DO8R.Data data = (DO8R.Data)value;
    //        //if (data.Val=="true")
    //        if ((bool)data)
    //        {
    //            return new SolidColorBrush(Colors.White);
    //        }

    //        //#676A6C
    //        //Color.FromArgb(透明度, red数字, green数字, blue数字); //这四个数字范围都是（0-255）
    //        else
    //            return new SolidColorBrush(Color.FromRgb(0x67,0x6a,0x6c));
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    

    public partial class CTOPMain : Window
    {
        public static CTOPMain pCurrentWin = null;
        //private DispatcherTimer readDataTimer = new DispatcherTimer();//通讯定时器类 会造成UI卡顿

        private CT2.CBackplaneInfo cBackplaneInfo = new CT2.CBackplaneInfo();
        private CT2.CClientTcpInfo cClientTcpInfo = new CT2.CClientTcpInfo();
        private CT2.CCommInfo cCommInfo = new CT2.CCommInfo();//通讯信息

        private CT2.CDeviceInfo cDeviceInfo = new CT2.CDeviceInfo();//CPU信息
        private CT2.CCpuInfo cCpuInfo = new CT2.CCpuInfo();
        private CT2.CRtcInfo cRtcInfo = new CT2.CRtcInfo();//RTC信息

        //视图界面类
        private IOGroupView IOGroupView = new IOGroupView();
        
        private BackgroundWorker backgroundWorker;
        private int TempCount = 0;
        //private int ErrCommuCount = 0;//通讯错误计数器

        //窗口初始化
        public CTOPMain()
        {
            InitializeComponent();
            pCurrentWin = this;
            backgroundWorker = (BackgroundWorker)this.FindResource("backgroundWorker");
            //Switch_IsOn.DataContext = Tool.Switch;
            this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;

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
            //App.StoreDb.GetDescriptions("DO");
            //UIThread = new Thread(UpdateData);
            //#region Init_Timer
            //readDataTimer.Tick += new EventHandler(UpdateData);
            //readDataTimer.Interval = new TimeSpan(0, 0, 0, 0, Tool.MSCycle);
            //#endregion
        }

        //重写或者this.SourceInitialized += new EventHandler
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwnd = PresentationSource.FromVisual(this) as HwndSource;
            if (hwnd != null)
            {
                hwnd.AddHook(new HwndSourceHook(this.WndProc));
            }
        }
        
        private class NativeMethods
        {
            public const int WM_NCHITTEST = 0x84;
            public const int HTCAPTION = 2;
            public const int HTLEFT = 10;
            public const int HTRIGHT = 11;
            public const int HTTOP = 12;
            public const int HTTOPLEFT = 13;
            public const int HTTOPRIGHT = 14;
            public const int HTBOTTOM = 15;
            public const int HTBOTTOMLEFT = 16;
            public const int HTBOTTOMRIGHT = 17;
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int GripSize = 16;//拐角宽度  
            int BorderSize = 7;//边框宽度  
            Window win = (Window)System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual;
            if (msg == NativeMethods.WM_NCHITTEST)
            {
                int x = lParam.ToInt32() << 16 >> 16, y = lParam.ToInt32() >> 16;
                Point pos = win.PointFromScreen(new Point(x, y));//鼠标位置

                //Bottom
                if (pos.X > GripSize &&
                    pos.X < win.ActualWidth - GripSize &&
                    pos.Y >= win.ActualHeight - BorderSize)
                {
                    handled = true;
                    return (IntPtr)NativeMethods.HTBOTTOM;
                }
                   
                //Right
                else if(pos.Y > GripSize &&
                    pos.X > win.ActualWidth - BorderSize &&
                    pos.Y < win.ActualHeight - GripSize)
                {
                    handled = true;
                    return (IntPtr)NativeMethods.HTRIGHT;
                }

                // Top, Left, Right, Corners, Etc.
                //HTBOTTOMRIGHT
                else if(pos.X > win.ActualWidth - GripSize &&
                     pos.Y >= win.ActualHeight - GripSize)
                {
                    handled = true;
                    return (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                }
            }
            return IntPtr.Zero;
        }

        //窗口拖拽
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //窗口关闭/最大化/最小化等功能按钮
        private void UI_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button))
            {
                return;
            }
            Button cmd = (Button)e.Source;
            string name = cmd.Name;

            if (name == "Min")
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (name == "Max")
            {
                if (this.Width < SystemParameters.WorkArea.Width)
                {
                    this.Left = SystemParameters.WorkArea.Left;
                    this.Top = SystemParameters.WorkArea.Top;
                    this.Width = SystemParameters.WorkArea.Width;
                    this.Height = SystemParameters.WorkArea.Height;
                }
                else
                {
                    this.Width = 850.5;
                    this.Height = 571.5;
                    this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
                    this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
                }
            }
            else if (name == "UIClose")
            {
                //readDataTimer.Stop();
                Tool.CloseWindow();
                this.Close();
            }
            else if (name == "WeChat")
            {
                
                Tool.CloseWindow();
                Tool.WeChat = new WeChatWindow();
                Tool.WeChat.Show();
            }
            else if (name == "Web")
            {
                System.Diagnostics.Process.Start("http://www.chenzhu-inst.com/");
            }
            else if (name == "Cus")
            {
                System.Diagnostics.Process.Start("http://wpa.b.qq.com/cgi/wpa.php?ln=1&key=XzkzODAwNjMyM18zNjMzMjZfNDAwODgxMDc4MF8yXw");
            }
            else if (name == "Help")
            {
                Tool.CloseWindow();
                Tool.Help = new HelpWindow();
                Tool.Help.Show();
            }
            else { }
        }

        //全体数据刷新处理 计算Tool.IsOnline状态
        private void UpdateData()
        {
            if (!Tool.IsInit)//未点击按钮，啥也不做
            {
                return;
            }
            if (!Tool.Switch)//手动停止通讯掉线 清空数据
            {
                Tool.IsOnline = false;
                return;
            }

            Thread.Sleep(Tool.MSCycle);
            if (Tool.ViewState == 1)//刷新模型后台数据
            {
                string strRes = Tool.mbTcpDriver.GetIoData(ref Tool.UIlist);
                if (strRes != "NO_ERR")//意外停止通讯
                {
                    Tool.IsOnline = false;
                    return;
                }
                Tool.IsOnline = true;
            }
            else if (Tool.ViewState == 2)//刷新DO列表后台数据
            {
                Tool.mbTcpDriver.UpdateTable(ref Tool.DOListView.listDO);
                Tool.mbTcpDriver.DeupdateTable(ref Tool.DOListView.listDO);
            }
            else if (Tool.ViewState == 3)//刷新DI列表后台数据
            {
                Tool.mbTcpDriver.UpdateTable(ref Tool.DIListView.listDI);
            }
            else if (Tool.ViewState == 4)//刷新AO列表后台数据
            {
                Tool.mbTcpDriver.UpdateTable(ref Tool.AOListView.listAO);
                Tool.mbTcpDriver.DeupdateTable(ref Tool.AOListView.listAO);
            }
            else if (Tool.ViewState == 5)//刷新AI列表后台数据
            {
                Tool.mbTcpDriver.UpdateTable(ref Tool.AIListView.listAI);
            }
            else if (Tool.ViewState == 6)//刷新M列表后台数据
            {
                if (Tool.MDataClcState == 1)//清空过程等待状态
                {
                    string strRes = Tool.mbTcpDriver.ClcAllofMData(ref Tool.Mzone.listM);
                    if (strRes != "NO_ERR")//意外停止通讯
                    {
                        Tool.IsOnline = false;
                        Tool.MDataClcState = 3;//错误情况
                        return;
                    }
                    Tool.MDataClcState = 2;//清空完成
                }
                else if (Tool.MDataClcState == 2 || Tool.MDataClcState == 0)//清空完成状态或正常状态
                {
                    Tool.mbTcpDriver.UpdateTable(ref Tool.Mzone.listM);
                    Tool.mbTcpDriver.DeupdateTable(ref Tool.Mzone.listM);
                }
            }
            //并行的CPU运行信息
            if (Tool.ViewCPUState == 1)//刷新CPU视图
            {
                Tool.mbTcpDriver.GetInfo(cBackplaneInfo);
            }
            else if (Tool.ViewCPUState == 2)//刷新CPU的IP视图
            {
                Tool.mbTcpDriver.GetInfo(cClientTcpInfo);
                Tool.mbTcpDriver.GetInfo(cCommInfo);
            }
            else if (Tool.ViewCPUState == 3)//刷新CPU的RTU视图
            {
                Tool.mbTcpDriver.GetInfo(cCommInfo);
            }
            else if (Tool.ViewCPUState == 4)//刷新CPU的Set视图
            {
                //Tool.mbTcpDriver.GetInfo(cCpuInfo);
                //Tool.mbTcpDriver.GetInfo(cRtcInfo);
                //Tool.mbTcpDriver.GetInfo(cDeviceInfo);
            }

            //以Tool.OnlineCount间隔刷新模块在线信息
            TempCount++;
            if (TempCount == Tool.OnlineCount)
            {
                TempCount = 0;
                if (Tool.mbTcpDriver.RefreshIoNodeInfo(ref Tool.UIlist) != "NO_ERR")//意外停止通讯
                {
                    Tool.IsOnline = false;
                    return;
                }
                Tool.IsOnline = true;
            }
        }

        private void UpdateUI()
        {
             
            if (!Tool.IsInit)//未点击按钮，啥也不做
            {
                return;
            }
            if (!Tool.IsOnline)
            {
                if (!Tool.Switch)//手动停止通讯掉线 清空数据，不弹出提示框
                {
                    foreach (var item in Tool.UIlist)
                    {
                        item.ClearData();
                    }
                    Tool.CloseWindow();
                    IOGroupView.Update();
                    IOGroupView.DisableCPU();
                    //手动掉线后回到视图界面
                    View.Content = IOGroupView;
                    MyTab.SelectedIndex=0;
                    return;
                }

                foreach (var item in Tool.UIlist)//自动停止通讯掉线 清空数据
                {
                    item.ClearData();
                }
                Switch_IsOn.IsChecked = false;
                Tool.IsInit = false;//需要从头初始化
                Tool.CloseWindow();
                IOGroupView.Update();
                IOGroupView.DisableCPU();
                //自动掉线后回到视图界面
                MyTab.SelectedIndex = 0;
                View.Content = IOGroupView;
                ErrWindow errWindow = new ErrWindow(); //错误提示窗口弹出
                errWindow.ShowDialog();
                return;
            }

            #region 界面刷新
            if (Tool.ViewState == 1)//刷新模型视图
            {
                IOGroupView.Update();
            }
            //并行的CPU运行信息
            if (Tool.ViewCPUState == 1)//刷新CPU视图
            {
                Tool.Bus.UpdateIOGroup(cBackplaneInfo);
            }
            else if (Tool.ViewCPUState == 2)//刷新CPU的IP视图
            {
                Tool.Ip.UpdateData(cClientTcpInfo);
                Tool.Ip.Update0(cCommInfo);
            }
            else if (Tool.ViewCPUState == 3)//刷新CPU的RTU视图
            {
                Tool.Rtu.Updata(cCommInfo);
            }
            else if (Tool.ViewCPUState == 4)//刷新CPU的Set视图
            {
                Tool.mbTcpDriver.GetInfo(cCpuInfo);
                Tool.mbTcpDriver.GetInfo(cRtcInfo);
                Tool.mbTcpDriver.GetInfo(cDeviceInfo);
                Tool.Set.UpdateData(cCpuInfo, cRtcInfo);
                Tool.Set.Update0(cDeviceInfo, cCpuInfo);
            }
            #endregion

            backgroundWorker.RunWorkerAsync();
        }

        //Tab选项改变
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Tool.IsInit)
            {
                return;
            }
            if (e.Source is TabControl)
            {
                int index=MyTab.SelectedIndex;
                Back.Background= new SolidColorBrush(Colors.White);
                switch (index)
                {
                    case 0://模型视图
                        Tool.ViewState = 1;
                        IOGroupView.Init();
                        View.Content = IOGroupView;
                        Back.Background = new SolidColorBrush(Color.FromArgb(0x19,0x87,0xce,0xfa));
                        break;
                    case 1://DO列表视图
                        Tool.ViewState = 2;
                        View.Content = Tool.DOListView;
                        Tool.DOListView.UpdateNodeBelongs();
                        break;
                    case 2://DI列表视图
                        Tool.ViewState = 3;
                        View.Content = Tool.DIListView;
                        Tool.DIListView.UpdateNodeBelongs();
                        break;
                    case 3://AO列表视图
                        Tool.ViewState = 4;
                        View.Content = Tool.AOListView;
                        Tool.AOListView.UpdateNodeBelongs();
                        break;
                    case 4://AI列表视图
                        Tool.ViewState = 5;
                        View.Content = Tool.AIListView;
                        Tool.AIListView.UpdateNodeBelongs();
                        break;
                    case 5://M列表视图
                        Tool.ViewState = 6;
                        View.Content = Tool.Mzone;
                        break;
                    default:
                        break;
                }
            }
        }

        //注释留有用处
        //点击开关，并确认通讯成功后，开关状态显示才更新，而不是直接更新

        //两个因素控制开关，模块在线状态，是否手动强制。强制级别高于模块在线状态
        private void Switch_IsOn_Unchecked(object sender, RoutedEventArgs e)
        {
            Tool.IsOnline = false;
            //Tool.IsInit = false;//不能直接清空标志，手动掉线后，也需要清空数据
            Tool.Switch = false;
            Switch_IsOn.Content = "离线";
        }

        private void Switch_IsOn_Checked(object sender, RoutedEventArgs e)
        {
            BeginCom();
        }

        //开始通讯
        private void BeginCom()
        {
            Type type = this.GetType();
            Assembly assembly = type.Assembly;
            Window win = (Window)assembly.CreateInstance(
                type.Namespace + ".FuncWindow." + "BeginWindow");
            win.ShowDialog();

            if (!Tool.IsInit)
            {
                Switch_IsOn.IsChecked = false;
                Switch_IsOn.Content = "离线";
                return;
            }
            else
            {
                View.Content = null;
            }
            Switch_IsOn.Content = "在线";
            Tool.IsOnline = true;
            Tool.Switch = true;

            backgroundWorker.RunWorkerAsync();
            Tool.ViewState = 1;
            IOGroupView.Init();
            //通讯成功后默认模型视图
            View.Content = IOGroupView;
            MyTab.SelectedIndex = 0;


        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            UpdateData();
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
                if (Tool.MDataClcState == 2)//清空完成状态
                {
                    Tool.MDataClcState = 0;//切回正常状态
                    Tool.inforWindow.InforText.Text = "清空完成";
                    Tool.inforWindow.CloseBtn.IsEnabled = true;
                }
                //不可能是清空等待状态
                //else if(Tool.Mzone.IsClcAll == 1)//清空过程等待状态
                //{
                //    Tool.Mzone.state.Text = "清空中";
                //}
                else if (Tool.MDataClcState == 0)//正常状态
                {
                    //Tool.Mzone.state.Visibility = Visibility.Hidden;
                }
                else if (Tool.MDataClcState == 3)//清空错误
                {
                    Tool.MDataClcState = 0;//切回正常状态
                    Tool.inforWindow.InforText.Text = "清空错误";
                }
                UpdateUI();
            }
        }
    }
}