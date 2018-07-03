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
using System.ComponentModel;//notify事件注册
namespace CTOP.Base
{
    /// <summary>
    /// DO15.xaml 的交互逻辑
    /// </summary>
    public partial class DI16 : UserControl
    {
        /***********************************************成员变量***********************************************/
        public CT2.CIoNode IOData;
        private Data UIData = new Data();
        /***********************************************方法***********************************************/
        //窗口初始化
        public DI16(CT2.CIoNode ioData)
        {
            InitializeComponent();

            UpdateInner(ioData);
            NID.Text = IOData.m_nodeID.ToString();

            HexValue.SetBinding(TextBlock.TextProperty, new Binding("HexValue") { Source = UIData });
        }

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

            int index=0;
            //找到数据结构中当前界面的模块的DO数据集合，找到之后退出循环                
            if (DataList.m_nodeID == IOData.m_nodeID)
            {
                UIData.HexValue = DataList.m_DataDIVal.ToString();
                //找到单个DO的数据集合的元素
                foreach (var data in DataList.m_DataDI)
                {
                    //循环次数判断
                    if (VisualTreeHelper.GetChildrenCount(CHGroup)==index)
                    {
                        return;
                    }
                    Label CH = (Label)VisualTreeHelper.GetChild(CHGroup, index);
                    //DO某个点上的数据
                    if (data==true)
                    {
                        CH.Template = this.FindResource("On") as ControlTemplate;
                    }
                    else
                    {
                        CH.Template = this.FindResource("Off") as ControlTemplate;
                    }
                    index++;
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

        private class Data : INotifyPropertyChanged
        {
            #region HexValue
            private string m_HexValue;
            public string HexValue
            {
                get { return m_HexValue; }
                set
                {
                    m_HexValue = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("HexValue"));
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

    }
}
