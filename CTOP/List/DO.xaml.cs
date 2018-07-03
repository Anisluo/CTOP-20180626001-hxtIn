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
    public partial class DO : UserControl
    {
        
        public ICollection<CT2ListDO> listDO;//外部也要使用
        public DO()
        {
            InitializeComponent();
            CListDB cListDB = new CListDB();
            listDO = cListDB.GetTableDO();
            tableDO.ItemsSource = listDO;
        }

        private void tableDO_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListDO cT2ListDO = (CT2ListDO)e.Row.DataContext;
                cT2ListDO.eSearchSM = CT2List.ESearchStateMachine.Update;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableDO_UnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                CT2ListDO cT2ListDO = (CT2ListDO)e.Row.DataContext;
                cT2ListDO.eSearchSM = CT2List.ESearchStateMachine.Idle;
            }
            catch (Exception)
            {
                //进入exception,说明已经下拉到底了
            }
        }

        private void tableDO_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            CT2ListDO cT2ListDO = (CT2ListDO)e.Row.DataContext;
            cT2ListDO.eSearchSM = CT2List.ESearchStateMachine.Idle;
        }

        private void tableDO_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CT2ListDO cT2ListDO = (CT2ListDO)e.Row.DataContext;
            cT2ListDO.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
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
            cListDB.UpdateTableInfoNode(ref listDO,
                                        Tool.UIlist);
        }

        //datagrid 套按钮，无法响应开始编辑事件，这里需要单独写点击事件，并且需要修改底层添加index，确保知道按钮索引
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button)
            {
                Button cmd = (Button)e.OriginalSource;
                foreach (var item in listDO)
                {
                    if (item.Index==cmd.Tag.ToString())
                    {
                        
                        item.Val= !item.Val;
                        item.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
                        break;
                    }
                }
            }
        }

        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Source is Button)
            {
                Button cmd = (Button)e.OriginalSource;
                foreach (var item in listDO)
                {
                    if (item.Index == cmd.Tag.ToString())
                    {

                        item.Val = !item.Val;
                        item.eSearchSM = CT2List.ESearchStateMachine.Deupdate;
                        break;
                    }
                }
            }
        }
    }

    
}
