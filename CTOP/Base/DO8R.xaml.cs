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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Converters;//转换类工具
using System.Windows.Media.Animation;//动画类工具
using CTOP.modbusTcp;//通讯
using System.ComponentModel;//notify事件注册
using System.Collections.ObjectModel;//ObservableCollection
namespace CTOP.Base
{
    /// <summary>
    /// DO15.xaml 的交互逻辑
    /// </summary>
    public partial class DO8R : UserControl
    {
        /***********************************************成员变量***********************************************/
        public CT2.CIoNode IOData;
        private Data UIData=new Data ();
        public bool IsUpdate = true;//数据刷新用
        private bool IsClickAll = false;//content 用
        private ObservableCollection<Data> valList = new ObservableCollection<Data>();
        /***********************************************方法***********************************************/
        //窗口初始化
        public DO8R(CT2.CIoNode ioData)
        {
            InitializeComponent();

            UpdateInner(ioData);
            NID.Text = IOData.m_nodeID.ToString();


            for (int i = 1; i < 9; i++)
            {
                Data t = new Data();
                t.Index = i.ToString();
                t.Val = "False";
                t.Name = "#676A6C";
                valList.Add(t);
            }
            CHGroup.ItemsSource = valList;
        }

        //刷新内部数据，使能禁用界面
        private void UpdateInner(CT2.CIoNode DataList)
        {
            IOData = DataList;
            if (IOData.m_isOnline)
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            }
            else
            {
                this.Opacity = 0.4;
                this.IsEnabled = false;
            }
        }

        //输入数据刷新
        public void UpdateInputData(CT2.CIoNode DataList)
        {
            UpdateInner(DataList);
            //找到数据结构中当前界面的模块的DO数据集合，找到之后退出循环                
            if (DataList.m_nodeID == IOData.m_nodeID)
            {

                List<string> list = new List<string>();
                foreach (var item in DataList.m_DataDO)
                {
                    list.Add(item.ToString());
                }
                int i = 0;
                foreach (var item in valList)
                {
                    item.Val = list[i];
                    if (item.Val=="True")
                    {
                        item.Name = "white";
                    }
                    else if (item.Val == "False")
                    {
                        item.Name = "#676A6C";
                    }
                    i++;
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {//Ctrl按下时，不放大，因为ctrl+mouse 自动放大
                return;
            }

            // 整体放大1.1倍
            var scaler = this.LayoutTransform as ScaleTransform;
            if (scaler == null)
            {
                scaler = new ScaleTransform(1.0, 1.0);
                this.LayoutTransform = scaler;
            }
            DoubleAnimation animator = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(Tool.AnimMS)),
            };
            animator.To = 1.15;
            scaler.BeginAnimation(ScaleTransform.ScaleXProperty, animator);
            scaler.BeginAnimation(ScaleTransform.ScaleYProperty, animator);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            //整体还原
            var scaler = this.LayoutTransform as ScaleTransform;
            if (scaler == null)
            {
                scaler = new ScaleTransform(1.0, 1.0);
                this.LayoutTransform = scaler;
            }
            DoubleAnimation animator = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(Tool.AnimMS)),
            };
            animator.To = 1;
            scaler.BeginAnimation(ScaleTransform.ScaleXProperty, animator);
            scaler.BeginAnimation(ScaleTransform.ScaleYProperty, animator);
        }

        //输出数据刷新
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is CheckBox cmd)
            {
                string index = cmd.Tag.ToString();
                foreach (var item in valList)
                {
                    if (item.Index==index)
                    {
                        int i = Convert.ToInt16(index);
                        IOData.m_DataDO[i-1] = !IOData.m_DataDO[i-1];
                        Tool.mbTcpDriver.WriteIoData(IOData);
                        break;
                    }
                }
            }
        }

        public class Data : INotifyPropertyChanged
        {
            #region Val
            private string val;
            public string Val
            {
                get { return val; }
                set
                {
                    val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Val"));
                }
            }
            #endregion

            #region Index
            private string index;
            public string Index
            {
                get { return index; }
                set
                {
                    index = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Index"));
                }
            }
            #endregion

            #region Name
            private string name;
            public string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name"));
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

        private void CheckBox_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Source is CheckBox cmd)
            {
                string index = cmd.Tag.ToString();
                foreach (var item in valList)
                {
                    if (item.Index == index)
                    {
                        int i = Convert.ToInt16(index);
                        IOData.m_DataDO[i - 1] = !IOData.m_DataDO[i - 1];
                        Tool.mbTcpDriver.WriteIoData(IOData);
                        break;
                    }
                }
            }
        }

        private void ButtonAll_Click(object sender, RoutedEventArgs e)
        {
            IsUpdate = false;
        }

        public void UpdateAll()
        {
            if (IsClickAll)
            {
                for (int i = 0; i < 8; i++)
                {
                    IOData.m_DataDO[i] = true;
                    Tool.mbTcpDriver.WriteIoData(IOData);
                }
                //AllBtn.Content = "全关";
                IsClickAll = false;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    IOData.m_DataDO[i] = false;
                    Tool.mbTcpDriver.WriteIoData(IOData);
                }
                //AllBtn.Content = "全开";
                IsClickAll = true;
            }
            IsUpdate = true;
        }
    }
}
