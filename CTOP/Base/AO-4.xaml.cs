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

namespace CTOP.Base
{
    /// <summary>
    /// AO_4.xaml 的交互逻辑
    /// </summary>
    public partial class AO_4 : UserControl
    {
        /***********************************************成员变量***********************************************/
        public CT2.CIoNode IOData;

        /***********************************************方法***********************************************/

        //窗口初始化
        public AO_4(CT2.CIoNode ioData)
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
                foreach (var data in DataList.m_DataAO)//1 3 5 7
                {
                    if (9 == index)//循环次数判断
                    {
                        return;
                    }
                    TextBox CH = (TextBox)VisualTreeHelper.GetChild(CHGroup, index);
                    if (!CH.IsFocused)//不在编辑状态才刷新，判断依据：有无焦点
                    {
                        CH.Text = data.ToString();
                    }
                    index+=2;
                }
            }
        }

        /* 1.Enter按下说明用户已完成编辑，移除文本框焦点到隐形文本框 不管是不是正常输入
         * 2.隐形文本框获得焦点后，相当于四个文本框失去焦点，程序响应Text_LostFocus事件
         * 3.隐形文本框不可能响应键盘按下事件，响应此事件时一定是无焦点状态
        */
        private void KeyEnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            VText.Focus();
        }

        //四个文本框之中有一个失去焦点 说明用户已完成编辑
        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox cmd = (TextBox)e.OriginalSource;
            string chName = cmd.Name;
            int index = 0;
            try//防止输入文本出错
            {
                foreach (var item in CHGroup.Children)//找到触发enter的控件
                {
                    if (item is TextBox)
                    {
                        TextBox ch = (TextBox)item;
                        if (ch.Name == chName)
                        {
                            IOData.m_DataAO[index] = Convert.ToUInt16(ch.Text);
                            Tool.mbTcpDriver.WriteIoData(IOData);
                            break;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
            
        }
    }
}
