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
    /// AI.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class AI : UserControl
    {

        public ICollection<CT2ListAI> listAI;
        public AI()
        {
            InitializeComponent();
            CListDB cListDB = new CListDB();
            listAI = cListDB.GetTableAI();
            tableAI.ItemsSource = listAI;
        }


        private void tableAI_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListAI cT2ListAI = (CT2ListAI)e.Row.DataContext;
                cT2ListAI.eSearchSM = CT2List.ESearchStateMachine.Update;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }


        private void tableAI_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListAI cT2ListAI = (CT2ListAI)e.Row.DataContext;
                cT2ListAI.eSearchSM = CT2List.ESearchStateMachine.Idle;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableAI_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            CT2ListAI cT2ListAI = (CT2ListAI)e.Row.DataContext;
            cT2ListAI.eSearchSM = CT2List.ESearchStateMachine.Update;
            //cT2ListAI.eSearchSM = CT2List.ESearchStateMachine.Idle;
        }

        private void tableAI_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CT2ListAI cT2ListAI = (CT2ListAI)e.Row.DataContext;
            cT2ListAI.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
        }

        public void UpdateNodeBelongs()
        {
            CListDB cListDB = new CListDB();
            cListDB.UpdateTableInfoNode(ref listAI,
                                        Tool.UIlist);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.SaveDesXml();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Tool.OpenDesXml();
        }
    }
}
