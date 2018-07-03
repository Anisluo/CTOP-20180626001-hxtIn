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
    /// IOGroup.xaml 的交互逻辑
    /// </summary>
    public partial class IOGroupView : UserControl
    {
        CPU cpu;
        public IOGroupView()
        {
            InitializeComponent();

        }
        public void Init()
        {
            IOGroup.Children.Clear();

            #region Init BUS
            CTOP.Base.BackBusLeft busLeft = new CTOP.Base.BackBusLeft();
            IOGroup.Children.Add(busLeft);
            #endregion

            #region Init CPU
            cpu = new CPU();
            IOGroup.Children.Add(cpu);
            #endregion

            #region InitUI
            foreach (var node in Tool.UIlist)
            {
                if (node.m_typeCode == 2)
                {
                    DO15 temp = new DO15(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
                else if (node.m_typeCode == 22)
                {
                    DO8R temp = new DO8R(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
                else if (node.m_typeCode == 1)
                {
                    DI16 temp = new DI16(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
                else if (node.m_typeCode == 3)
                {
                    AI8 temp = new AI8(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
                else if (node.m_typeCode == 6)
                {
                    AO_4 temp = new AO_4(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
                else if (node.m_typeCode == 19)
                {
                    AI8C temp = new AI8C(node);
                    IOGroup.Children.Add(temp);

                    CTOP.Base.BackBusMiddle busMid = new CTOP.Base.BackBusMiddle();
                    IOGroup.Children.Add(busMid);
                }
            }
            if (Tool.UIlist.Count!=0)//有IO模块的需要添加结束导轨且删除原有的导轨
            {
                IOGroup.Children.RemoveAt(IOGroup.Children.Count - 1);
                CTOP.Base.BackBusRight busRight = new CTOP.Base.BackBusRight();
                IOGroup.Children.Add(busRight);
            }
            else//无IO模块的直接添加结束导轨
            {
                CTOP.Base.BackBusRight busRight = new CTOP.Base.BackBusRight();
                IOGroup.Children.Add(busRight);
            }
            #endregion

        }

        public void Update()
        {
            
            #region UpdateUI_IO
            /*查找界面上有多少IO，将数据list拆开分解后传入各IO模型界面类内部
             */
            foreach (var IO in IOGroup.Children)//界面全部刷新，全部遍历
            {
                if (IO is DO15)
                {
                    DO15 tIo = (DO15)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if (item.m_nodeID == tIo.IOData.m_nodeID)//找到后就退出内层遍历
                        {
                            //数据锁
                            if (tIo.IsUpdate)//从数据更新到UI
                            {
                                tIo.UpdateInputData(item);
                            }
                            else
                            {
                                tIo.UpdateAll();//从UI更新到数据
                            }
                            break;
                        }
                    }
                }
                else if (IO is DO8R)
                {
                    DO8R tIo = (DO8R)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if (item.m_nodeID == tIo.IOData.m_nodeID)//找到后就退出内层遍历
                        {
                            //数据锁
                            if (tIo.IsUpdate)//从数据更新到UI
                            {
                                tIo.UpdateInputData(item);
                            }
                            else//从UI更新到数据
                            {
                                tIo.UpdateAll();
                            }
                            break;
                        }
                    }
                }
                else if (IO is DI16)
                {
                    DI16 tIo = (DI16)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if (item.m_nodeID == tIo.IOData.m_nodeID)//找到后就退出内层遍历
                        {
                            tIo.UpdateInputData(item);
                            break;
                        }
                    }
                }
                else if (IO is AO_4)
                {
                    AO_4 tIo = (AO_4)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if (item.m_nodeID == tIo.IOData.m_nodeID)//找到后就退出内层遍历
                        {
                            tIo.UpdateInputData(item);
                            break;
                        }
                    }
                }
                else if (IO is AI8)
                {
                    AI8 tIo = (AI8)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if (item.m_nodeID == tIo.IOData.m_nodeID)//找到后就退出内层遍历
                        {
                            tIo.UpdateInputData(item);
                            break;
                        }
                    }
                }
                else if (IO is AI8C)
                {
                    AI8C tIo = (AI8C)IO;
                    foreach (var item in Tool.UIlist)
                    {
                        if ( item.m_nodeID.ToString() == tIo.modelView.NodeID)//找到后就退出内层遍历 
                            //不能再加上是否在线的标志，否则掉线后无法更新UI
                        {
                            //for (int i = 0; i < item.m_DataAI.Count; i++)
                            //{
                            //    item.m_DataAI[i] = T;
                            //}
                            
                            tIo.UpdateInputData(item);
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        public void DisableCPU()
        {
            cpu.IsEnabled = false;
            cpu.Opacity = 0.4;
        }


        //支持缩放
        private void IOGroup_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = IOCanvas;
            e.Mode = ManipulationModes.All;
        }

        //支持缩放
        private void IOGroup_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var deltaManipulation = e.DeltaManipulation;
            Point touchPos = e.ManipulationOrigin;

            touchPos.X -= originOffsetX;
            touchPos.Y -= originOffsetY;
            //原有中心点
            Point untransformedOldCenter = new Point(this.sfr.CenterX, this.sfr.CenterY);
            Point transformedOldCenter = this.sfr.Transform(untransformedOldCenter);
            //新中心点
            Point untransformedNewCenter = this.IOScroll.RenderTransform.Inverse.Transform(touchPos);
            Point transformedNewCenter = touchPos;

            //位置修正
            double adjustX = transformedNewCenter.X - transformedOldCenter.X - untransformedNewCenter.X + untransformedOldCenter.X;
            double adjustY = transformedNewCenter.Y - transformedOldCenter.Y - untransformedNewCenter.Y + untransformedOldCenter.Y;

            //更新
            this.tlt.X = adjustX;
            this.tlt.Y = adjustY;
            sfr.CenterX = untransformedNewCenter.X;
            sfr.CenterY = untransformedNewCenter.Y;

            double ceof = 1.0;
            if (deltaManipulation.Scale.X > deltaManipulation.Scale.Y)
            {
                ceof = deltaManipulation.Scale.X;
            }
            else
            {
                ceof = deltaManipulation.Scale.Y;
            }

            sfr.ScaleX = sfr.ScaleX * ceof;
            sfr.ScaleY = sfr.ScaleY * ceof;

            double x = e.DeltaManipulation.Translation.X;
            double y = e.DeltaManipulation.Translation.Y;

            double left = Canvas.GetLeft(IOScroll);
            Canvas.SetLeft(IOScroll, left + x);
            originOffsetX += x;

            double top = Canvas.GetTop(IOScroll);
            Canvas.SetTop(IOScroll, top + y);
            originOffsetY += y;
        }

        //支持鼠标单独移动滑动页面
        private void IOGroup_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ctrl键按下+鼠标滚动 缩放画面
            if ((Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {//ctrl键按下

                Point mousePos = e.GetPosition(IOCanvas);
                mousePos.X -= originOffsetX;
                mousePos.Y -= originOffsetY;
                //原有中心点
                Point untransformedOldCenter = new Point(this.sfr.CenterX, this.sfr.CenterY);
                Point transformedOldCenter = this.sfr.Transform(untransformedOldCenter);
                //新中心点
                Point untransformedNewCenter = this.IOScroll.RenderTransform.Inverse.Transform(mousePos);
                Point transformedNewCenter = mousePos;
                //位置修正
                double adjustX = transformedNewCenter.X - transformedOldCenter.X - untransformedNewCenter.X + untransformedOldCenter.X;
                double adjustY = transformedNewCenter.Y - transformedOldCenter.Y - untransformedNewCenter.Y + untransformedOldCenter.Y;
                //更新
                this.tlt.X = adjustX;
                this.tlt.Y = adjustY;
                sfr.CenterX = untransformedNewCenter.X;
                sfr.CenterY = untransformedNewCenter.Y;

                if (e.Delta > 0)
                {
                    sfr.ScaleX = sfr.ScaleX * 1.05;
                    sfr.ScaleY = sfr.ScaleY * 1.05;
                }
                else
                {
                    sfr.ScaleX = sfr.ScaleX * 0.95;
                    sfr.ScaleY = sfr.ScaleY * 0.95;
                }
                return;
            }
            //鼠标滚动移动画面 记录IOScroll与Canvas的相对偏移
            if (e.Delta < 0)
            {
                double left = Canvas.GetLeft(IOScroll);
                Canvas.SetLeft(IOScroll, left - 20);
                originOffsetX -= 20;
            }
            else
            {
                double right = Canvas.GetLeft(IOScroll);
                Canvas.SetLeft(IOScroll, right + 20);
                originOffsetX += 20;
            }
        }


        double originOffsetX = 0, originOffsetY = 0;

        private void SetOriginView()
        {
            originOffsetX = 0;
            originOffsetY = 0;
            sfr.ScaleX = 0.9;
            sfr.ScaleY = 0.9;
            tlt.X = 0;
            tlt.Y = 0;
            sfr.CenterX = 0;
            sfr.CenterY = 0;
            Canvas.SetLeft(IOScroll, 0);
            Canvas.SetTop(IOScroll, 0);
        }


        //支持视图布局还原
        private void TextBlock_Click(object sender, RoutedEventArgs e)
        {
            SetOriginView();
        }

        private void IOTitle_TouchDown(object sender, TouchEventArgs e)
        {
            SetOriginView();
        }

        //支持内容拖拽
        bool isMouseRightBtnDown = false;
        Point lastMousePt;

        private void IOGroup_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.ScrollAll;
            isMouseRightBtnDown = true;
        }

        private void IOGroup_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
            isMouseRightBtnDown = false;
        }
        private void IOGroup_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this.IOCanvas);
            if (!isMouseRightBtnDown)
            {
                lastMousePt = point;
                return;
            }
            double x = point.X - lastMousePt.X;
            double y = point.Y - lastMousePt.Y;

            double left = Canvas.GetLeft(IOScroll);
            Canvas.SetLeft(IOScroll, left + x);
            originOffsetX += x;

            double top = Canvas.GetTop(IOScroll);
            Canvas.SetTop(IOScroll, top + y);
            originOffsetY += y;

            lastMousePt = point;
        }

    }

}
