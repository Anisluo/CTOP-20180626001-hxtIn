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


using System.Windows.Media.Animation;
//using System.Reflection;
//using System.ComponentModel;
//using System.Collections.ObjectModel;
using CTOP.modbusTcp;//通讯
namespace CTOP.Base
{
    /// <summary>
    /// AO_4.xaml 的交互逻辑
    /// </summary>
    public partial class AI8 : UserControl
    {
        /***********************************************字段***********************************************/
        //public int CHType { get; set; }

        //public bool IsOnline { get; set; }

        //public int NodeID
        //{
        //    get { return (int)GetValue(NodeIDProperty); }
        //    set { SetValue(NodeIDProperty, value); }
        //}
        //// Using a DependencyProperty as the backing store for NodeID.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NodeIDProperty =
        //    DependencyProperty.Register("NodeID", typeof(int), typeof(AO_4), new PropertyMetadata(null));

        /***********************************************成员变量***********************************************/
        public CT2.CIoNode IOData;

        /***********************************************方法***********************************************/

        //窗口初始化
        public AI8(CT2.CIoNode ioData)
        {
            InitializeComponent();

            UpdateInner(ioData);
            NID.Text = IOData.m_nodeID.ToString();
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

        //鼠标进入窗口时动画
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {//Ctrl按下时，不放大，因为ctrl+mouse 自动放大
                return;
            }

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

        //鼠标离开窗口时动画
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
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

        //输入数据刷新，注册过且在线
        public void UpdateInputData(CT2.CIoNode DataList)
        {
            UpdateInner(DataList);

            int index = 1;
            if (DataList.m_nodeID == IOData.m_nodeID)
            {
                //循环得到单个AO各通道的数据
                foreach (var data in DataList.DataAIStr)//1 3 5 7 9 11 13 15 17 19
                {
                    //循环次数判断
                    if (17== index)
                    {
                        return;
                    }
                    Label CH = (Label)VisualTreeHelper.GetChild(CHGroup, index);
                    CH.Content = data;
                    index+=2;
                }
            }
        }

        private void AIType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tool.AI8ITypeVal = AIType.SelectedIndex;
        }
        //string tempstr=AIType.SelectionBoxItem.ToString();


    }
}
