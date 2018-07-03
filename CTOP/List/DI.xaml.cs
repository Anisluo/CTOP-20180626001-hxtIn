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
namespace CTOP.List
{
    /// <summary>
    /// DI.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class DI : UserControl
    {
        public ICollection<CT2ListDI> listDI;

        public DI()
        {
            InitializeComponent();
            CListDB cListDB = new CListDB();
            listDI = cListDB.GetTableDI();
            tableDI.ItemsSource = listDI;
        }
        private void tableDI_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListDI cT2ListDI = (CT2ListDI)e.Row.DataContext;
                cT2ListDI.eSearchSM = CT2List.ESearchStateMachine.Update;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableDI_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListDI cT2ListDI = (CT2ListDI)e.Row.DataContext;
                cT2ListDI.eSearchSM = CT2List.ESearchStateMachine.Idle;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableDI_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            CT2ListDI cT2ListDI = (CT2ListDI)e.Row.DataContext;
            cT2ListDI.eSearchSM = CT2List.ESearchStateMachine.Update;
            //cT2ListDI.eSearchSM = CT2List.ESearchStateMachine.Idle;
        }

        private void tableDI_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CT2ListDI cT2ListDI = (CT2ListDI)e.Row.DataContext;
            cT2ListDI.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.SaveDesXml();
        }
        
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.OpenDesXml();
        }

        public void UpdateNodeBelongs()
        {
            CListDB cListDB = new CListDB();
            cListDB.UpdateTableInfoNode(ref listDI,
                                        Tool.UIlist);
        }
    }


}
