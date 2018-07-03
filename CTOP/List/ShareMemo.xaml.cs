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

using CTOP.modbusTcp;
using System.ComponentModel;
using System.Collections.ObjectModel;//ObservableCollection
using System.Xml;
namespace CTOP.List
{
    /// <summary>
    /// DO.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class ShareMemo : UserControl
    {
        public ICollection<CT2ListM> listM;//外部也要使用
        public ShareMemo()
        {
            InitializeComponent();
            CListDB cListDB = new CListDB();
            listM = cListDB.GetTableM();
            tableM.ItemsSource = listM;
        }

        private void tableM_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListM cT2ListM = (CT2ListM)e.Row.DataContext;
                cT2ListM.eSearchSM = CT2List.ESearchStateMachine.Update;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableM_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListM cT2ListM = (CT2ListM)e.Row.DataContext;
                cT2ListM.eSearchSM = CT2List.ESearchStateMachine.Idle;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableM_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            CT2ListM cT2ListM = (CT2ListM)e.Row.DataContext;
            cT2ListM.eSearchSM = CT2List.ESearchStateMachine.Idle;
        }

        private void tableM_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CT2ListM cT2ListM = (CT2ListM)e.Row.DataContext;
            cT2ListM.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.SaveDesXml();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.OpenDesXml();
        }

        private void ClcMData(object sender, RoutedEventArgs e)
        {
            Tool.MDataClcState = 1;
            Tool.inforWindow = new FuncWindow.InforWindow();
            Tool.inforWindow.InforText.Text = "清空M变量数据中，请等待...";
            Tool.inforWindow.ShowDialog();
        }
    }

    
}
