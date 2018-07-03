using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;//ObservableCollection
using System.ComponentModel;

namespace CTOP.modbusTcp
{

    class CListDB
    {
        public void UpdateTableInfoNode(ref ICollection<CT2ListAI> table,
                List<CT2.CIoNode> listIO)
        {
            string[] TableAInodeID = new string[800];
            foreach (var item in listIO)
            {
                if (item.CheckIfExistAI())
                {
                    for (int i = item.m_AI_AddrStart;
                        i < item.m_AI_AddrEnd+1; i++)
                    {
                        TableAInodeID[(i - 200)/2] = 
                            string.Format("#{0:D}", item.m_nodeID);
                    }
                }
            }
            int index = 0;
            foreach (var item in table)
            {
                item.Node=TableAInodeID[index];
                index++;
            }
        }

        public void UpdateTableInfoNode(ref ICollection<CT2ListAO> table,
                List<CT2.CIoNode> listIO)
        {
            string[] TableAOnodeID = new string[800];
            foreach (var item in listIO)
            {
                if (item.CheckIfExistAO())
                {
                    for (int i = item.m_AO_AddrStart;
                        i < item.m_AO_AddrEnd+1; i++)
                    {
                        TableAOnodeID[(i - 200)/2] =
                            string.Format("#{0:D}", item.m_nodeID);
                    }
                }
            }
            int index = 0;
            foreach (var item in table)
            {
                item.Node = TableAOnodeID[index];
                index++;
            }
        }

        public void UpdateTableInfoNode(ref ICollection<CT2ListDI> table,
                List<CT2.CIoNode> listIO)
        {
            string[] TableDInodeID = new string[1600];
            foreach (var item in listIO)
            {
                if (item.CheckIfExistDI())
                {
                    for (int i = item.m_DI_AddrStart*8;
                        i < (item.m_DI_AddrEnd+1)*8; i++)
                    {
                        TableDInodeID[i] =
                            string.Format("#{0:D}", item.m_nodeID);
                    }
                }
            }
            int index = 0;
            foreach (var item in table)
            {
                item.Node = TableDInodeID[index];
                index++;
            }
        }

        public void UpdateTableInfoNode(ref ICollection<CT2ListDO> table,
                List<CT2.CIoNode> listIO)
        {
            string[] TableDOnodeID = new string[1600];
            foreach (var item in listIO)
            {
                if (item.CheckIfExistDO())
                {
                    for (int i = item.m_DO_AddrStart * 8;
                        i < (item.m_DO_AddrEnd + 1) * 8; i++)
                    {
                        TableDOnodeID[i] =
                        string.Format("#{0:D}", item.m_nodeID);
                    }
                }
            }
            int index = 0;
            foreach (var item in table)
            {
                item.Node = TableDOnodeID[index];
                index++;
            }
        }


    


        public ICollection<CT2ListAO> GetTableAO()
        {
            ObservableCollection<CT2ListAO> tableAO = new ObservableCollection<CT2ListAO>();
            for (int i = 0; i < 800; i++)
            {
                CT2ListAO listAO = new CT2ListAO();
                listAO.mbAddr = (ushort)i;
                tableAO.Add(listAO);
            }
            return tableAO;
        }

        public ICollection<CT2ListAI> GetTableAI()
        {
            ObservableCollection<CT2ListAI> tableAI = new ObservableCollection<CT2ListAI>();
            for (int i = 0; i < 800; i++)
            {
                CT2ListAI listAI = new CT2ListAI();
                listAI.mbAddr = (ushort)i;
                tableAI.Add(listAI);
            }
            return tableAI;
        }

        public ICollection<CT2ListDO> GetTableDO()
        {
            ObservableCollection<CT2ListDO> tableDO = new ObservableCollection<CT2ListDO>();
            for (int i = 0; i < 1600; i++)
            {
                CT2ListDO listDO = new CT2ListDO();
                listDO.mbAddr = (ushort)i;
                //hxt
                listDO.Index = listDO.mbAddr.ToString();
                listDO.ColorName = "Gray";
                //hxt
                tableDO.Add(listDO);
            }
            return tableDO;
        }

        public ICollection<CT2ListDI> GetTableDI()
        {
            ObservableCollection<CT2ListDI> tableDI = new ObservableCollection<CT2ListDI>();
            for (int i = 0; i < 1600; i++)
            {
                CT2ListDI listDI = new CT2ListDI();
                listDI.mbAddr = (ushort)i;
                tableDI.Add(listDI);
            }
            return tableDI;
        }

        public ICollection<CT2ListM> GetTableM()
        {
            ObservableCollection<CT2ListM> tableM = new ObservableCollection<CT2ListM>();
            for (int i = 1000; i < 10500; i++)
            {
                CT2ListM listM = new CT2ListM();
                listM.mbAddr = (ushort)i;
                tableM.Add(listM);
            }
            return tableM;
        }


    }

    public class CT2List : INotifyPropertyChanged
    {
        //       public bool bIfUpdateNeeded;
        //       public bool bIfDeupdateNeeded=false;
        public enum ESearchStateMachine
        {
            Update,
            Deupdate,
            Idle
        }
        public ESearchStateMachine eSearchSM = new ESearchStateMachine();
        public ushort mbAddr;

        public CT2List()
        {
            eSearchSM = ESearchStateMachine.Idle;
        }

        private string node;
        public string Node
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Node"));
            }
        }
        

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Description"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        
    }


    public class CT2ListAO:CT2List
    {
        public string PlcLogicAddr1
        {
            get { return string.Format("%QB{0:D}-%QB{1:D}", 
                        (mbAddr*2)+200, (mbAddr * 2) + 1+200); }
        }

        public string PlcLogicAddr2
        {
            get { return string.Format("%QW{0:D}", (mbAddr * 2)+200); }
        }
        public string ModbusAddr
        {
            get { return string.Format("{0:D5}", mbAddr+1); }//4
        }
        private ushort val;
        public ushort Val
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Val"));
            }
        }
    }

    public class CT2ListAI : CT2List
    {
        public string PlcLogicAddr1
        {
            get
            {
                return string.Format("%IB{0:D}-%IB{1:D}",
                      (mbAddr * 2) + 200, (mbAddr * 2) + 1 + 200);
             }
        }

        public string PlcLogicAddr2
        {
            get { return string.Format("%IW{0:D}", (mbAddr * 2) + 200); }
        }
        public string ModbusAddr
        {
            get { return string.Format("{0:D5}", mbAddr+1); }//3
        }
        private ushort val;
        public ushort Val
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Val"));
            }
        }
    }

    public class CT2ListDO : CT2List
    {
        public string PlcLogicAddr1
        {
            get
            {
                return string.Format("%QX{0:D}.{1:D}",
                      mbAddr/8, mbAddr%8);
            }
        }
        public string PlcLogicAddr2
        {
            get { return string.Format("%QB{0:D}", mbAddr /8); }
        }
        public string ModbusAddr
        {
            get { return string.Format("{0:D5}", mbAddr+1); }//0
        }

        private bool  val;
        public bool Val
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Val"));
            }
        }

        //hxt
        private string colorname;
        public string ColorName
        {
            get { return colorname; }
            set
            {
                colorname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ColorName"));
            }
        }

        private string contentname;
        public string Contentname
        {
            get { return contentname; }
            set
            {
                contentname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Contentname"));
            }
        }

        private string index;
        public string Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Index"));
            }
        }
        //hxt
    }

    public class CT2ListDI : CT2List
    {
        public string PlcLogicAddr1
        {
            get
            {
                return string.Format("%IX{0:D}.{1:D}",
                      mbAddr / 8, mbAddr % 8);
            }
        }
        public string PlcLogicAddr2
        {
            get { return string.Format("%IB{0:D}", mbAddr / 8); }
        }
        public string ModbusAddr
        {
            get { return string.Format("{0:D5}", mbAddr+1); }//1
        }
        private bool val;
        public bool Val
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Val"));
            }
        }
        //hxt
        private string colorname;
        public string ColorName
        {
            get { return colorname; }
            set
            {
                colorname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ColorName"));
            }
        }

        private string contentname;
        public string Contentname
        {
            get { return contentname; }
            set
            {
                contentname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Contentname"));
            }
        }
        //hxt
    }
    
    public class CT2ListM : CT2List
    {
        
        public string PlcLogicAddr1
        {
            get
            {
                return string.Format("%MB3.{0:D}-%MB3.{1:D}",
                     mbAddr * 2 - 1000, mbAddr * 2 - 1000 + 1);
            }
        }
        public string PlcLogicAddr2
        {
            get { return string.Format("%MW{0:D}", mbAddr*2-1000); }
        }
        public string ModbusAddr
        {
            get { return string.Format("{0:D5}", mbAddr+1); }//
        }
        private ushort val;
        public ushort Val
        {
            get { return val; }
            set
            {
                //if (value==65535)
                //{
                //    value = 0;
                //}
                val = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Val"));
            }
        }
    }
}
