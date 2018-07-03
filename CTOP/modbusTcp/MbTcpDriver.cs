using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using System.Threading;

//using namespace Modbus;
namespace CTOP.modbusTcp
{
    public class TcpClientWithTimeout
    {
        protected string _hostname;
        protected int _port;
        protected int _timeout_milliseconds;
        protected TcpClient connection;
        protected bool connected;
        protected Exception exception;

        public TcpClientWithTimeout(string hostname, int port, int timeout_milliseconds)
        {
            _hostname = hostname;
            _port = port;
            _timeout_milliseconds = timeout_milliseconds;
        }
        public TcpClient Connect()
        {
            // kick off the thread that tries to connect
            connected = false;
            exception = null;
            Thread thread = new Thread(new ThreadStart(BeginConnect));
            thread.IsBackground = true; // 作为后台线程处理
                                        // 不会占用机器太长的时间
            thread.Start();

            // 等待如下的时间
            thread.Join(_timeout_milliseconds);

            if (connected == true)
            {
                // 如果成功就返回TcpClient对象
                thread.Abort();
                return connection;
            }
            if (exception != null)
            {
                // 如果失败就抛出错误
                thread.Abort();
                throw exception;
            }
            else
            {
                // 同样地抛出错误
                thread.Abort();
                string message = string.Format("TcpClient connection to {0}:{1} timed out",
                  _hostname, _port);
                throw new TimeoutException(message);
            }
        }
        protected void BeginConnect()
        {
            try
            {
                connection = new TcpClient(_hostname, _port);
                // 标记成功，返回调用者
                connected = true;
            }
            catch (Exception ex)
            {
                // 标记失败
                exception = ex;
            }
        }
    }

    public class MbTcpDriver
    {
        enum eComm
        {
            TCP,
            UDP,
            SERIAL,
        }

        private ModbusIpMaster master;
        private int IntervalTimeMs;
        //private eComm comm;
        public string Init(string ipAddress, int port)
        {
            try
            {
                TcpClient client = new TcpClientWithTimeout(ipAddress, port, 1000).Connect();
                master = ModbusIpMaster.CreateIp(client);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";

        }

        public string SetParam(int readTimeout = 2000,
                            int retries = 3,
                            int WaitToRetryMilliseconds = 1000,
                            int WriteTimeout = 2000,
                            int IntervalTime = 50)
        {
            if (master == default(ModbusIpMaster))
            {
                return "UnInit modbus master driver";
            }
            master.Transport.ReadTimeout = readTimeout;
            master.Transport.Retries = retries;
            master.Transport.WaitToRetryMilliseconds = WaitToRetryMilliseconds;
            master.Transport.WriteTimeout = WriteTimeout;
            IntervalTimeMs = IntervalTime;
            return "NO_ERR";
        }


        /// <summary>
        /// 获取IO节点地址
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string GetIoNode(ref List<CT2.CIoNode> list)
        {
            list.Clear();
            for (int i = 0; i < 19; i++)
            {
                //5个节点上传一次
                ushort[] inputs5;
                try
                {
                    ushort addrStart = (ushort)(CT2.ECT2InputReg.nodeInfo +
                        i * 5 * CT2.CIoNode.NODE_INFO_LENGTH);
                    Thread.Sleep(5);
                    inputs5 = master.ReadInputRegisters(
                              addrStart, CT2.CIoNode.NODE_INFO_LENGTH * 5);
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                for (int j = 0; j < 5; j++)
                {
                    ushort[] inputs = new ushort[CT2.CIoNode.NODE_INFO_LENGTH];
                    for (int k = j * CT2.CIoNode.NODE_INFO_LENGTH;
                        k < (j + 1) * CT2.CIoNode.NODE_INFO_LENGTH;
                        k++)
                    {
                        inputs[k - j * CT2.CIoNode.NODE_INFO_LENGTH] = inputs5[k];
                    }
                    CT2.CIoNode cIoNode = new CT2.CIoNode(inputs, (ushort)(i * 5 + j + 1));
                    if (cIoNode.CheckIfExist() == true)
                    {
                        list.Add(cIoNode);
                    }
                }
            }
            return "NO_ERR";
        }

        public string RefreshIoNodeInfo(ref List<CT2.CIoNode> list)
        {
            ushort[] table = new ushort[100];
            foreach (var item in list)
            {
                table[item.m_nodeID] = item.m_nodeID;
            }
            List<ushort[]> listInfo = new List<ushort[]>();
            listInfo.Clear();

            List<List<ushort>> listlistRegion = FindCommRegion(table);
            foreach (var listRegion in listlistRegion)
            {
                for (int i = 0; i < listRegion.Count; i = i + 5)
                {
                    int num;
                    if ((listRegion.Count - i) > 5)
                    {
                        num = 5;
                    }
                    else
                    {
                        num = listRegion.Count - i;
                    }
                    ushort[] inputs5;
                    try
                    {
                        ushort addrStart = (ushort)(CT2.ECT2InputReg.nodeInfo +
                            (listRegion[i] - 1) * CT2.CIoNode.NODE_INFO_LENGTH);
                        Thread.Sleep(5);
                        inputs5 = master.ReadInputRegisters(
                                  addrStart, (ushort)(num * CT2.CIoNode.NODE_INFO_LENGTH));
                        for (int j = 0;
                            j < num;
                            j++)
                        {
                            ushort[] inputs = new ushort[CT2.CIoNode.NODE_INFO_LENGTH];
                            for (int k = j * CT2.CIoNode.NODE_INFO_LENGTH;
                                k < (j + 1) * CT2.CIoNode.NODE_INFO_LENGTH;
                                k++)
                            {
                                inputs[k - j * CT2.CIoNode.NODE_INFO_LENGTH] = inputs5[k];
                            }
                            listInfo.Add(inputs);
                        }
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].UpdateInfo(listInfo[i]);
            }
            return "NO_ERR";
        }

        private enum ERegionType
        {
            Region_DI,
            Region_AI,
            Region_DO,
            Region_AO,
        }
        public string GetIoData(ref List<CT2.CIoNode> list)
        {
            ushort[] tableAO = new ushort[800];
            ushort[] tableDO = new ushort[100];
            ushort[] tableAI = new ushort[800];
            ushort[] tableDI = new ushort[100];
            ushort[] RegionDataAO = new ushort[800];
            ushort[] RegionDataAI = new ushort[800];
            bool[] RegionDataDO = new bool[1600];
            bool[] RegionDataDI = new bool[1600];

            for (int i = 0; i < tableAO.Length; i++) { tableAO[i] = 0; }
            for (int i = 0; i < tableAI.Length; i++) { tableAI[i] = 0; }
            for (int i = 0; i < RegionDataAO.Length; i++) { RegionDataAO[i] = 0; }
            for (int i = 0; i < RegionDataAI.Length; i++) { RegionDataAI[i] = 0; }
            for (int i = 0; i < tableDO.Length; i++) { tableDO[i] = 0; }
            for (int i = 0; i < tableDI.Length; i++) { tableDO[i] = 0; }
            for (int i = 0; i < RegionDataDO.Length; i++) { RegionDataDO[i] = false; }
            for (int i = 0; i < RegionDataDI.Length; i++) { RegionDataDI[i] = false; }


            foreach (var IoNode in list)
            {
                if ((IoNode.m_DO_AddrStart != 0) ||
                    (IoNode.m_DO_AddrEnd != 0))
                {
                    for (int j = (IoNode.m_DO_AddrStart / 2);
                            j <= ((IoNode.m_DO_AddrEnd) / 2);
                            j++)
                    {
                        tableDO[j] = IoNode.m_nodeID;
                    }
                }
                if ((IoNode.m_AO_AddrStart != 0) ||
                    (IoNode.m_AO_AddrEnd != 0))
                {
                    for (int j = (IoNode.m_AO_AddrStart - 200) / 2;
                            j <= (IoNode.m_AO_AddrEnd - 200) / 2;
                            j++)
                    {
                        tableAO[j] = IoNode.m_nodeID;
                    }
                }
                if ((IoNode.m_DI_AddrStart != 0) ||
                    (IoNode.m_DI_AddrEnd != 0))
                {
                    for (int j = IoNode.m_DI_AddrStart / 2;
                            j <= IoNode.m_DI_AddrEnd / 2;
                            j++)
                    {
                        tableDI[j] = IoNode.m_nodeID;
                    }
                }
                if ((IoNode.m_AI_AddrStart != 0) ||
                    (IoNode.m_AI_AddrEnd != 0))
                {
                    for (int j = (IoNode.m_AI_AddrStart - 200) / 2;
                            j <= (IoNode.m_AI_AddrEnd - 200) / 2;
                            j++)
                    {
                        tableAI[j] = IoNode.m_nodeID;
                    }
                }
            }

            List<List<ushort>> listlistRegionDO = FindCommRegion(tableDO);
            List<List<ushort>> listlistRegionDI = FindCommRegion(tableDI);
            List<List<ushort>> listlistRegionAO = FindCommRegion(tableAO);
            List<List<ushort>> listlistRegionAI = FindCommRegion(tableAI);

            string strRes = "";
            strRes = GetDataArrayFromRegion(ERegionType.Region_DO,
                            listlistRegionDO,
                            ref RegionDataDO);
            if ("NO_ERR" != strRes)
            {
                return strRes;
            }

            strRes = GetDataArrayFromRegion(ERegionType.Region_DI,
                            listlistRegionDI,
                            ref RegionDataDI);
            if ("NO_ERR" != strRes)
            {
                return strRes;
            }

            strRes = GetDataArrayFromRegion(ERegionType.Region_AO,
                            listlistRegionAO,
                            ref RegionDataAO);
            if ("NO_ERR" != strRes)
            {
                return strRes;
            }

            strRes = GetDataArrayFromRegion(ERegionType.Region_AI,
                                        listlistRegionAI,
                                        ref RegionDataAI);
            if ("NO_ERR" != strRes)
            {
                return strRes;
            }

            WriteDataFromRegionToNode(ERegionType.Region_DO,
                                        ref list,
                                        RegionDataDO);
            WriteDataFromRegionToNode(ERegionType.Region_DI,
                                        ref list,
                                        RegionDataDI);
            WriteDataFromRegionToNode(ERegionType.Region_AO,
                                        ref list,
                                        RegionDataAO);
            WriteDataFromRegionToNode(ERegionType.Region_AI,
                                        ref list,
                                        RegionDataAI);
            return "NO_ERR";
        }

        private enum ESearchStateMachine
        {
            FindStart,
            Finded
        }
        private List<List<ushort>> FindCommRegion(ushort[] tableRegion)
        {
            ESearchStateMachine eSearchSM = (int)ESearchStateMachine.FindStart;

            List<List<ushort>> listlistTable = new List<List<ushort>>();
            List<ushort> listTable = new List<ushort>();
            listTable.Clear();
            listlistTable.Clear();
            for (int i = 0; i < tableRegion.Length; i++)
            {
                switch (eSearchSM)
                {
                    case ESearchStateMachine.FindStart:
                        if (tableRegion[i] != 0)
                        {
                            listTable.Add((ushort)i);
                            eSearchSM = ESearchStateMachine.Finded;
                        }
                        break;
                    case ESearchStateMachine.Finded:
                        if (tableRegion[i] == 0)
                        {//结束
                            eSearchSM = ESearchStateMachine.FindStart;
                            listlistTable.Add(listTable);
                            listTable = new List<ushort>();
                            listTable.Clear();
                            //comm
                        }
                        else
                        {
                            listTable.Add((ushort)i);
                        }
                        break;
                    default:
                        break;
                }
            }
            if (eSearchSM == ESearchStateMachine.Finded)
            {
                listlistTable.Add(listTable);
            }
            return listlistTable;
        }

        private string GetDataArrayFromRegion(ERegionType eType,
                                        List<List<ushort>> listlistRegion,
                                        ref ushort[] regionData)
        {
            List<short[]> listData = new List<short[]>();
            foreach (var listT in listlistRegion)
            {
                for (int j = 0; j < listT.Count; j = j + 100)
                {
                    ushort num;
                    if ((listT.Count - j) > 100)
                    {
                        num = 100;
                    }
                    else
                    {
                        num = (ushort)(listT.Count - j);
                    }
                    ushort[] regs;
                    try
                    {
                        switch (eType)
                        {
                            case ERegionType.Region_AO:
                                {
                                    Thread.Sleep(10);
                                    regs = master.ReadHoldingRegisters(
                                        (ushort)(CT2.ECT2HoldReg.RegionAO + listT[j] * 1),
                                        (ushort)num);
                                    regs.CopyTo(regionData, listT[j]);
                                    break;
                                }
                            case ERegionType.Region_AI:
                                {
                                    Thread.Sleep(10);
                                    regs = master.ReadInputRegisters(
                                        (ushort)(CT2.ECT2InputReg.RegionAI + listT[j] * 1),
                                        (ushort)num);
                                    regs.CopyTo(regionData, listT[j]);
                                    break;
                                }
                            case ERegionType.Region_DI:
                            case ERegionType.Region_DO:
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
            }
            return "NO_ERR";
        }

        private string GetDataArrayFromRegion(ERegionType eType,
                                List<List<ushort>> listlistRegion,
                                ref bool[] regionData)
        {
            List<short[]> listData = new List<short[]>();
            foreach (var listT in listlistRegion)
            {
                for (int j = 0; j < listT.Count; j = j + 5)
                {
                    ushort num;
                    if (((listT.Count - j) * 16) > 80)
                    {
                        num = 80;
                    }
                    else
                    {
                        num = (ushort)((listT.Count - j) * 16);
                    }
                    bool[] bData;
                    try
                    {
                        switch (eType)
                        {
                            case ERegionType.Region_DI:
                                {
                                    Thread.Sleep(5);
                                    bData = master.ReadInputs(
                                        (ushort)(CT2.ECT2Descrites.RegionDI + (listT[j]) * 16),
                                        (ushort)num);
                                    bData.CopyTo(regionData, listT[j] * 16);
                                    break;
                                }
                            case ERegionType.Region_DO:
                                {
                                    Thread.Sleep(5);
                                    bData = master.ReadCoils(
                                    (ushort)(CT2.ECT2Coils.RegionDO + (listT[j]) * 16),
                                    (ushort)num);
                                    bData.CopyTo(regionData, listT[j] * 16);
                                    break;
                                }
                            case ERegionType.Region_AO:
                            case ERegionType.Region_AI:
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                        //throw;
                    }
                }
            }
            return "NO_ERR";
        }

        private void WriteDataFromRegionToNode(ERegionType eType,
                                ref List<CT2.CIoNode> list,
                                ushort[] regionData)
        {
            foreach (var IoNode in list)
            {
                switch (eType)
                {
                    case ERegionType.Region_DI:
                        break;
                    case ERegionType.Region_AI:
                        if (IoNode.CheckIfExistAI())
                        {
                            IoNode.m_DataAI.Clear();
                            for (int i = ((IoNode.m_AI_AddrStart - 200) / 2);
                                i <= ((IoNode.m_AI_AddrEnd - 200) / 2); i++)
                            {
                                IoNode.m_DataAI.Add(regionData[i]);
                            }
                        }
                        break;
                    case ERegionType.Region_DO:
                        break;
                    case ERegionType.Region_AO:
                        if (IoNode.CheckIfExistAO())
                        {
                            IoNode.m_DataAO.Clear();
                            for (int i = ((IoNode.m_AO_AddrStart - 200) / 2);
                                i <= ((IoNode.m_AO_AddrEnd - 200) / 2); i++)
                            {
                                IoNode.m_DataAO.Add(regionData[i]);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void WriteDataFromRegionToNode(ERegionType eType,
                                ref List<CT2.CIoNode> list,
                                bool[] regionData)
        {
            foreach (var IoNode in list)
            {
                switch (eType)
                {
                    case ERegionType.Region_DI:
                        {
                            if (IoNode.CheckIfExistDI())
                            {
                                IoNode.m_DataDI.Clear();
                                for (int i = IoNode.m_DI_AddrStart * 8;
                                    i < (IoNode.m_DI_AddrEnd + 1) * 8; i++)
                                {
                                    IoNode.m_DataDI.Add(regionData[i]);
                                }
                            }
                            break;
                        }

                    case ERegionType.Region_DO:
                        {
                            if (IoNode.CheckIfExistDO())
                            {
                                IoNode.m_DataDO.Clear();
                                for (int i = IoNode.m_DO_AddrStart * 8;
                                    i < (IoNode.m_DO_AddrEnd + 1) * 8; i++)
                                {
                                    IoNode.m_DataDO.Add(regionData[i]);
                                }
                            }
                            break;
                        }
                    case ERegionType.Region_AO:
                    case ERegionType.Region_AI:
                    default:
                        break;
                }
            }
        }


        public string WriteIoData(CT2.CIoNode node)
        {
            ushort len;
            if (node.CheckIfExistDO())
            {
                len = (ushort)((node.m_DO_AddrEnd + 1 - node.m_DO_AddrStart) * 8);
                if (len != node.m_DataDO.Count)
                {
                    return "ERR_illegalDataLength_DO";
                }
                try
                {
                    Thread.Sleep(5);
                    ushort addrStart = (ushort)(CT2.ECT2Coils.RegionDO + node.m_DO_AddrStart * 8);
                    master.WriteMultipleCoils(addrStart, node.m_DataDO.ToArray());
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            if (node.CheckIfExistAO())
            {
                len = (ushort)((node.m_AO_AddrEnd + 1 - node.m_AO_AddrStart) / 2);
                if (len != node.m_DataAO.Count)
                {
                    return "ERR_illegalDataLength_DO";
                }
                try
                {
                    Thread.Sleep(5);
                    ushort addrStart = (ushort)(CT2.ECT2HoldReg.RegionAO + (node.m_AO_AddrStart - 200) / 2);
                    master.WriteMultipleRegisters(addrStart, node.m_DataAO.ToArray());
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return "NO_ERR";
        }

        public string GetInfo(CT2.CInfo cInfo)
        {
            switch (cInfo.eInputOrHold)
            {
                case CT2.CInfo.EInputOrHold.Input:
                    {
                        ushort[] inputs;
                        try
                        {
                            Thread.Sleep(5);
                            inputs = master.ReadInputRegisters(cInfo.AddressStart,
                                    cInfo.DATA_LENGTH);
                            cInfo.UpdateInfo(inputs);
                        }
                        catch (Exception e)
                        {
                            return e.Message;
                        }
                        return "NO_ERR";
                    }
                case CT2.CInfo.EInputOrHold.Holding:
                    {
                        ushort[] holds;
                        try
                        {
                            //Thread.Sleep(5);
                            holds = master.ReadHoldingRegisters(cInfo.AddressStart,
                                    cInfo.DATA_LENGTH);
                            cInfo.UpdateInfo(holds);
                        }
                        catch (Exception e)
                        {
                            return e.Message;
                        }
                        return "NO_ERR";
                    }
                default:
                    return "unknown";
            }
        }

        public string WriteInfo(CT2.CHoldInfo cHoldInfo)
        {
            try
            {
                Thread.Sleep(5);
                master.WriteMultipleRegisters(cHoldInfo.AddressStart,
                        cHoldInfo.data);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";

        }

        public string GetDataByAddressAO(ushort addrStart,
                                        ushort length,
                                        ref List<ushort> listData)
        {
            ushort[] regs;
            try
            {
                Thread.Sleep(5);
                regs = master.ReadHoldingRegisters(
                    (ushort)(CT2.ECT2HoldReg.RegionAO + addrStart),
                    (ushort)length);
                listData = regs.ToList();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }
        public string GetDataByAddressAI(ushort addrStart,
                        ushort length,
                        ref List<ushort> listData)
        {
            ushort[] regs;
            try
            {
                Thread.Sleep(5);
                regs = master.ReadInputRegisters(
                    (ushort)(CT2.ECT2InputReg.RegionAI + addrStart),
                    (ushort)length);
                listData = regs.ToList();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }

        public string GetDataByAddressDO(ushort addrStart,
                                ushort length,
                                ref List<bool> listData)
        {
            bool[] regs;
            try
            {
                Thread.Sleep(5);
                regs = master.ReadCoils(
                    (ushort)(CT2.ECT2Coils.RegionDO + addrStart),
                    (ushort)length);
                listData = regs.ToList();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }
        public string GetDataByAddressDI(ushort addrStart,
                        ushort length,
                        ref List<bool> listData)
        {
            bool[] regs;
            try
            {
                Thread.Sleep(5);
                regs = master.ReadInputs(
                    (ushort)(CT2.ECT2Descrites.RegionDI + addrStart),
                    (ushort)length);
                listData = regs.ToList();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }

        public string UpdateTable(ref ICollection<CT2ListAO> listAO)
        {
            ushort[] table = new ushort[800];
            ushort[] data = new ushort[800];
            for (int i = 0; i < table.Length; i++) { table[i] = 0; }
            for (int i = 0; i < data.Length; i++) { data[i] = 0; }

            ushort index = 0;
            foreach (var item in listAO)
            {

                if (item.eSearchSM == CT2ListAO.ESearchStateMachine.Update)
                {
                    table[index] = (ushort)(index + 1);
                }
                else
                {
                    table[index] = 0;
                }
                index++;
            }
            List<List<ushort>> listlistRegion = FindCommRegion(table);
            List<ushort> listData = new List<ushort>();
            foreach (var listT in listlistRegion)
            {
                ushort addrStart = listT.First();
                ushort length = (ushort)(listT.Last() - listT.First() + 1);
                GetDataByAddressAO(addrStart, length,
                                        ref listData);
                listData.CopyTo(data, addrStart);
            }

            index = 0;
            foreach (var item in listAO)
            {
                if (table[index] != 0)
                {
                    item.Val = data[index];
                }
                index++;
            }

            return "NO_ERR";
        }

        public string DeupdateTable(ref ICollection<CT2ListAO> listAO)
        {
            try
            {
                foreach (var item in listAO)
                {
                    if (item.eSearchSM == CT2ListAO.ESearchStateMachine.Deupdate)
                    {
                        ushort addrStart = item.mbAddr;
                        ushort value = item.Val;
                        Thread.Sleep(5);
                        master.WriteSingleRegister(
                            (ushort)(CT2.ECT2HoldReg.RegionAO + addrStart),
                            value);
                        item.eSearchSM = CT2ListAO.ESearchStateMachine.Update;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }

        public string UpdateTable(ref ICollection<CT2ListAI> listAI)
        {
            ushort[] table = new ushort[800];
            ushort[] data = new ushort[800];
            for (int i = 0; i < table.Length; i++) { table[i] = 0; }
            for (int i = 0; i < data.Length; i++) { data[i] = 0; }

            ushort index = 0;
            foreach (var item in listAI)
            {

                if (item.eSearchSM == CT2ListAI.ESearchStateMachine.Update)
                {
                    table[index] = (ushort)(index + 1);
                }
                else
                {
                    table[index] = 0;
                }
                index++;
            }
            List<List<ushort>> listlistRegion = FindCommRegion(table);
            List<ushort> listData = new List<ushort>();
            foreach (var listT in listlistRegion)
            {
                ushort addrStart = listT.First();
                ushort length = (ushort)(listT.Last() - listT.First() + 1);
                GetDataByAddressAI(addrStart, length,
                                        ref listData);
                listData.CopyTo(data, addrStart);
            }

            index = 0;
            foreach (var item in listAI)
            {
                if (table[index] != 0)
                {
                    item.Val = data[index];
                }
                index++;
            }

            return "NO_ERR";
        }

        public string UpdateTable(ref ICollection<CT2ListDO> listDO)
        {
            ushort[] table = new ushort[1600];
            bool[] data = new bool[1600];
            for (int i = 0; i < table.Length; i++) { table[i] = 0; }
            for (int i = 0; i < data.Length; i++) { data[i] = false; }

            ushort index = 0;
            foreach (var item in listDO)
            {

                if (item.eSearchSM == CT2ListAI.ESearchStateMachine.Update)
                {
                    table[index] = (ushort)(index + 1);
                }
                else
                {
                    table[index] = 0;
                }
                index++;
            }
            List<List<ushort>> listlistRegion = FindCommRegion(table);
            List<bool> listData = new List<bool>();
            foreach (var listT in listlistRegion)
            {
                ushort addrStart = listT.First();
                ushort length = (ushort)(listT.Last() - listT.First() + 1);
                GetDataByAddressDO(addrStart, length,
                                        ref listData);
                listData.CopyTo(data, addrStart);
            }

            index = 0;
            foreach (var item in listDO)
            {
                if (table[index] != 0)
                {
                    item.Val = data[index];
                    //hxt
                    if (item.Val)
                    {
                        item.ColorName = "Green";
                        item.Contentname = "1";
                    }
                    else
                    {
                        item.ColorName = "Gray";
                        item.Contentname = "0";
                    }
                    //hxt
                }
                index++;
            }

            return "NO_ERR";
        }
        public string DeupdateTable(ref ICollection<CT2ListDO> listDO)
        {
            try
            {
                foreach (var item in listDO)
                {
                    if (item.eSearchSM == CT2ListAO.ESearchStateMachine.Deupdate)
                    {
                        ushort addrStart = item.mbAddr;
                        bool value = item.Val;
                        Thread.Sleep(5);
                        master.WriteSingleCoil(
                            (ushort)(CT2.ECT2HoldReg.RegionAO + addrStart),
                            value);
                        item.eSearchSM = CT2ListAO.ESearchStateMachine.Update;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
                //throw;
            }
            return "NO_ERR";
        }

        public string UpdateTable(ref ICollection<CT2ListDI> listDI)
        {
            ushort[] table = new ushort[1600];
            bool[] data = new bool[1600];
            for (int i = 0; i < table.Length; i++) { table[i] = 0; }
            for (int i = 0; i < data.Length; i++) { data[i] = false; }

            ushort index = 0;
            foreach (var item in listDI)
            {

                if (item.eSearchSM == CT2ListAI.ESearchStateMachine.Update)
                {
                    table[index] = (ushort)(index + 1);
                }
                else
                {
                    table[index] = 0;
                }
                index++;
            }
            List<List<ushort>> listlistRegion = FindCommRegion(table);
            List<bool> listData = new List<bool>();
            foreach (var listT in listlistRegion)
            {
                ushort addrStart = listT.First();
                ushort length = (ushort)(listT.Last() - listT.First() + 1);
                GetDataByAddressDI(addrStart, length,
                                        ref listData);//hxt 18.1.22
                listData.CopyTo(data, addrStart);
            }

            index = 0;
            foreach (var item in listDI)
            {
                if (table[index] != 0)
                {
                    item.Val = data[index];
                    //hxt
                    if (item.Val)
                    {
                        item.ColorName = "Green";
                        item.Contentname = "1";
                    }
                    else
                    {
                        item.ColorName = "Gray";
                        item.Contentname = "0";
                    }
                    //hxt
                }
                index++;
            }

            return "NO_ERR";
        }

        public string UpdateTable(ref ICollection<CT2ListM> listM)
        {
            ushort[] table = new ushort[19000];
            ushort[] data = new ushort[19000];
            for (int i = 0; i < table.Length; i++) { table[i] = 0; }
            for (int i = 0; i < data.Length; i++) { data[i] = 0; }

            ushort index = 0;
            foreach (var item in listM)
            {

                if (item.eSearchSM == CT2ListAO.ESearchStateMachine.Update)
                {
                    table[index] = (ushort)(index + 1);
                }
                else
                {
                    table[index] = 0;
                }
                index++;
            }
            List<List<ushort>> listlistRegion = FindCommRegion(table);
            List<ushort> listData = new List<ushort>();
            foreach (var listT in listlistRegion)
            {
                ushort addrStart = Convert.ToUInt16(listT.First() + 1000);
                ushort length = (ushort)(listT.Last() - listT.First() + 1);
                GetDataByAddressAO(addrStart, length,
                                        ref listData);
                listData.CopyTo(data, addrStart - 1000);
            }

            index = 0;
            foreach (var item in listM)
            {
                if (table[index] != 0)
                {
                    item.Val = data[index];
                }
                index++;
            }

            return "NO_ERR";
        }

        public string DeupdateTable(ref ICollection<CT2ListM> listM)
        {
            try
            {
                foreach (var item in listM)
                {
                    if (item.eSearchSM == CT2ListAO.ESearchStateMachine.Deupdate)
                    {
                        ushort addrStart = item.mbAddr;
                        ushort value = item.Val;
                        Thread.Sleep(5);
                        master.WriteSingleRegister(
                            (ushort)(CT2.ECT2HoldReg.RegionAO + addrStart),
                            value);
                        item.eSearchSM = CT2ListAO.ESearchStateMachine.Update;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }

        public string ClcAllofMData(ref ICollection<CT2ListM> listM)
        {
            try
            {
                ushort[] inputs100=new ushort[100];
                for (int i = 0; i < 100; i++)
                {
                    inputs100[i] = 2;
                }
                for (int i = 0; i < 95; i++)
                {
                    //1000 ~10499 (1000,10500)
                    master.WriteMultipleRegisters((ushort)(CT2.ECT2HoldReg.RegionAO + 1000 + i*100), inputs100);
                    Thread.Sleep(5);
                }

            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "NO_ERR";
        }
    }
}
