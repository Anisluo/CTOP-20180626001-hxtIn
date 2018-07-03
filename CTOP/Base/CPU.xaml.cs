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

using CTOP.CPUSub;
using System.Windows.Media.Animation;
namespace CTOP.Base
{
    /// <summary>
    /// CPU.xaml 的交互逻辑
    /// </summary>
    public partial class CPU : UserControl
    {
        public CPU()
        {
            InitializeComponent();
        }

        private void CPUIP_Click(object sender, RoutedEventArgs e)
        {
            if (!Tool.IsInit ||!Tool.IsOnline)
            {
                return;
            }
            //Tool.CloseWindow();

            Tool.Ip = new IPWindow();
            Tool.Ip.Topmost = true;
            Tool.Ip.ShowInTaskbar = false;
            Tool.Ip.Show();
            Tool.ViewCPUState = 2;
        }

        private void CPURTU_Click(object sender, RoutedEventArgs e)
        {
            if (!Tool.IsInit || !Tool.IsOnline)
            {
                return;
            }
            //Tool.CloseWindow();

            Tool.Rtu = new RTUWindow();
            Tool.Rtu.Topmost = true;
            Tool.Rtu.ShowInTaskbar = false;
            Tool.Rtu.Show();
            Tool.ViewCPUState = 3;
        }

        private void CPUSet_Click(object sender, RoutedEventArgs e)
        {
            if (!Tool.IsInit || !Tool.IsOnline)
            {
                return;
            }
            //Tool.CloseWindow();

            Tool.Set = new SetWindow();
            Tool.Set.Topmost = true;
            Tool.Set.ShowInTaskbar = false;
            Tool.Set.Show();

            Tool.ViewCPUState = 4;
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
    }
}
