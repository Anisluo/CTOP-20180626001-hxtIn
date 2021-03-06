﻿using System;
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
namespace CTOP.Base
{
    /// <summary>
    /// BackBus.xaml 的交互逻辑
    /// </summary>
    public partial class BackBusLeft : UserControl
    {
        public BackBusLeft()
        {
            InitializeComponent();
        }

        private void CPU_Click(object sender, RoutedEventArgs e)
        {
            if (!Tool.IsInit)
            {
                return;
            }

            Tool.Bus = new BusWindow();
            Tool.Bus.Show();
            Tool.Bus.ShowInTaskbar = false;
            Tool.Bus.Topmost = true;
            Tool.ViewCPUState = 1;
        }
    }
}
