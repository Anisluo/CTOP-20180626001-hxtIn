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
    /// AO.xaml 的交互逻辑
    /// </summary>
    public partial class AO : UserControl
    {
        public ICollection<CT2ListAO> listAO;
        public AO()
        {
            CListDB cListDB = new CListDB();
            InitializeComponent();
            listAO = cListDB.GetTableAO();
            tableAO.ItemsSource = listAO;
        }

        private void tableAO_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListAO cT2ListAO = (CT2ListAO)e.Row.DataContext;
                cT2ListAO.eSearchSM = CT2List.ESearchStateMachine.Update;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableAO_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListAO cT2ListAO = (CT2ListAO)e.Row.DataContext;
                cT2ListAO.eSearchSM = CT2List.ESearchStateMachine.Idle;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableAO_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            CT2ListAO cT2ListAO = (CT2ListAO)e.Row.DataContext;
            cT2ListAO.eSearchSM = CT2List.ESearchStateMachine.Idle;
        }

        private void tableAO_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CT2ListAO cT2ListAO = (CT2ListAO)e.Row.DataContext;
            cT2ListAO.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
        }
        public void UpdateNodeBelongs()
        {
            CListDB cListDB = new CListDB();
            cListDB.UpdateTableInfoNode(ref listAO,
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
