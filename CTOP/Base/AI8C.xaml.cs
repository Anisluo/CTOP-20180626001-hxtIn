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

using System.Windows.Media.Animation;//缩放动画
using System.ComponentModel;//notify事件注册
using CTOP.Overview;
namespace CTOP.Base
{
    /// <summary>
    /// AI8C.xaml 的交互逻辑
    /// </summary>
    public partial class AI8C : UserControl
    {

        /*********************************************成员变量*********************************************/
        public class ModelView : INotifyPropertyChanged
        {
            #region NodeID
            private string m_NodeID;
            public string NodeID
            {
                get { return m_NodeID; }
                set
                {
                    m_NodeID = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("NodeID"));
                }
            }
            #endregion

            #region FilterType
            private string m_FilterType;
            public string FilterType
            {
                get { return m_FilterType; }
                set
                {
                    m_FilterType = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FilterType"));
                }
            }
            #endregion

            public bool IsNodeOnline//不需要支持属性更改通知，因为不和UI相关，只是内部计算
            {
                get;set;
            }

            #region CH1Val
            private string m_CH1Val;
            public string CH1Val
            {
                get { return m_CH1Val; }
                set
                {
                    m_CH1Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH1Val"));
                }
            }
            #endregion

            #region CH2Val
            private string m_CH2Val;
            public string CH2Val
            {
                get { return m_CH2Val; }
                set
                {
                    m_CH2Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH2Val"));
                }
            }
            #endregion

            #region CH3Val
            private string m_CH3Val;
            public string CH3Val
            {
                get { return m_CH3Val; }
                set
                {
                    m_CH3Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH3Val"));
                }
            }
            #endregion

            #region CH4Val
            private string m_CH4Val;
            public string CH4Val
            {
                get { return m_CH4Val; }
                set
                {
                    m_CH4Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH4Val"));
                }
            }
            #endregion

            #region CH5Val
            private string m_CH5Val;
            public string CH5Val
            {
                get { return m_CH5Val; }
                set
                {
                    m_CH5Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH5Val"));
                }
            }
            #endregion

            #region CH6Val
            private string m_CH6Val;
            public string CH6Val
            {
                get { return m_CH6Val; }
                set
                {
                    m_CH6Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH6Val"));
                }
            }
            #endregion

            #region CH7Val
            private string m_CH7Val;
            public string CH7Val
            {
                get { return m_CH7Val; }
                set
                {
                    m_CH7Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH7Val"));
                }
            }
            #endregion

            #region CH8Val
            private string m_CH8Val;
            public string CH8Val
            {
                get { return m_CH8Val; }
                set
                {
                    m_CH8Val = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CH8Val"));
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

        }//modelview类
        public ModelView modelView = new ModelView();//modelview对象
        private AI8COverview aI8COverview = new AI8COverview();

        /*********************************************公开方法*********************************************/
        //窗口初始化
        public AI8C(CT2.CIoNode DataList)
        {
            InitializeComponent();
            //数据绑定
            CH1ValText.SetBinding(Label.ContentProperty, new Binding("CH1Val") { Source = modelView });
            CH2ValText.SetBinding(Label.ContentProperty, new Binding("CH2Val") { Source = modelView });
            CH3ValText.SetBinding(Label.ContentProperty, new Binding("CH3Val") { Source = modelView });
            CH4ValText.SetBinding(Label.ContentProperty, new Binding("CH4Val") { Source = modelView });
            CH5ValText.SetBinding(Label.ContentProperty, new Binding("CH5Val") { Source = modelView });
            CH6ValText.SetBinding(Label.ContentProperty, new Binding("CH6Val") { Source = modelView });
            CH7ValText.SetBinding(Label.ContentProperty, new Binding("CH7Val") { Source = modelView });
            CH8ValText.SetBinding(Label.ContentProperty, new Binding("CH8Val") { Source = modelView });
            NodeIDText.SetBinding(TextBlock.TextProperty, new Binding("NodeID") { Source = modelView });
            AI8CTypeText.SetBinding(TextBlock.TextProperty, new Binding("FilterType") { Source = modelView });
            //IO节点信息先刷新
            modelView.IsNodeOnline = DataList.m_isOnline;
            modelView.NodeID = DataList.m_nodeID.ToString();
            modelView.FilterType= DataList.AI8C_FilterType;
            if (modelView.IsNodeOnline)
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            }
            else
            {
                this.Opacity = 0.4;
                this.IsEnabled = false;
            }
            //IO数据信息在IO模型初始化之后刷新，所以此处尚没有IO的数据信息，不能直接刷新该IO内各通道的值
        }

        //输入数据刷新，在线时才刷新
        public void UpdateInputData(CT2.CIoNode DataList)
        {
            modelView.IsNodeOnline = DataList.m_isOnline;
            if (modelView.IsNodeOnline)
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            }
            else
            {
                this.Opacity = 0.4;
                this.IsEnabled = false;
                //return;
            }

            modelView.CH1Val = DataList.m_DataAI8C[0].ToString("#0.000");
            modelView.CH2Val = DataList.m_DataAI8C[1].ToString("#0.000");
            modelView.CH3Val = DataList.m_DataAI8C[2].ToString("#0.000");
            modelView.CH4Val = DataList.m_DataAI8C[3].ToString("#0.000");
            modelView.CH5Val = DataList.m_DataAI8C[4].ToString("#0.000");
            modelView.CH6Val = DataList.m_DataAI8C[5].ToString("#0.000");
            modelView.CH7Val = DataList.m_DataAI8C[6].ToString("#0.000");
            modelView.CH8Val = DataList.m_DataAI8C[7].ToString("#0.000");
        }


        /*********************************************私有方法*********************************************/
        //鼠标进入窗口时动画
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {//Ctrl按下时，不放大，因为ctrl+mouse 自动放大
                return;
            }
            
            //CTOPMain.pCurrentWin.test.Width = 100;
            //CTOPMain.pCurrentWin.test.Content = aI8COverview;
            var scaler = this.RenderTransform as ScaleTransform;//LayoutTransform RenderTransform
            if (scaler == null)
            {
                scaler = new ScaleTransform(1, 1);//scaler = new ScaleTransform(1.0, 1.0);
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
            CTOPMain.pCurrentWin.test.Content = null;
            var scaler = this.RenderTransform as ScaleTransform;
            if (scaler == null)
            {
                scaler = new ScaleTransform(1, 1);
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
