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

using System.Threading;
using System.Collections.ObjectModel;
namespace CTOP.UDP
{
    /// <summary>
    /// UdpControlBase.xaml 的交互逻辑
    /// </summary>
    public partial class UdpControlBase : UserControl
    {
        private UdpTool udpTool;
        private Thread thread;
        private JsonTool jsonTool;
        private bool IsThreadStart = false;

        public UdpControlBase()
        {
            InitializeComponent();
            udpTool = new UdpTool();
            thread = new Thread(FindDevice);
            jsonTool = new JsonTool();
            //table.ItemsSource = jsonlist;//由于itemsource不支持从线程调度器外的线程更新collectionview 这里UDP接收线程要放在外面
        }

        private void BtnClkFindDevice(object sender, RoutedEventArgs e)
        {
            string Txstr = "";
            table.ItemsSource = null;
            BtnFindDevice.IsEnabled = false;

            udpTool.Init(8089, "255.255.255.255", 8088);
            udpTool.StartRec();
            jsonTool.SetJsonString(ref Txstr, jsonTool);
            udpTool.Send(Txstr);

            udpTool.ClcJsonListData();
            if (!IsThreadStart)
            {
                IsThreadStart = true;
                thread.Start();
            }
            
        }
        private void FindDevice()
        {
            while (true)
            {
                SetSourcedelegate setSourcedelegate = SetSource;
                this.Dispatcher.Invoke(setSourcedelegate);
            }
        }

        private delegate void SetSourcedelegate();
        private void SetSource()
        {
            if (udpTool.IsRecOK)
            {
                BtnFindDevice.IsEnabled = true;
                udpTool.IsRecOK = false;
                table.ItemsSource = udpTool.jsonlist;
            }
        }

        private void SaveBtn_Touch(object sender, TouchEventArgs e)
        {
            
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button cmd)
            {
                if ((string)cmd.Tag == udpTool.jsonlist[Convert.ToInt32(cmd.Tag)].Index)
                {
                    string str = "";
                    jsonTool.CMD = "Write";
                    jsonTool.SetJsonString(ref str, jsonTool);
                    udpTool.Send(str);
                }
            }
        }

        private void table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            jsonTool = (JsonTool)e.Row.DataContext;
        }

        private void table_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

            jsonTool = (JsonTool)e.Row.DataContext;
        }
    }
}
