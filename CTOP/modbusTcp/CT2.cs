using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Converters;
//using System.Text.Encoding;//用于字节数组转字符串
namespace CTOP.CT2
{
    //协议地址，以0开始
    enum ECT2InputReg
    {
        RegionAI = 0,
        deviceInfo = 800,
        systemInfo = 850,
        clientTcp = 870,
        commInfo = 900,
        backplaneInfo = 950,
        cpuInfo = 1000,
        nodeInfo = 1300,
    }

    enum ECT2HoldReg
    {
        RegionAO = 0,
        RtcSetting = 800,
        SelfTest = 810,
        //nodeInfo = 1301,
    }

    enum ECT2Coils
    {
        RegionDO = 0,
    }

    enum ECT2Descrites
    {
        RegionDI = 0,
        //nodeInfo = 1301,
    }

    /// <summary>
    /// 
    /// </summary>
    public class CIoNode
    {
        private UInt16 nodeID;
        private ushort[] info = new ushort[15];

        public UInt16 m_nodeID { get { return nodeID; } }
        public UInt16 m_DO_AddrStart { get { return info[0]; } }
        public UInt16 m_DO_AddrEnd { get { return info[1]; } }
        public UInt16 m_DI_AddrStart { get { return info[2]; } }
        public UInt16 m_DI_AddrEnd { get { return info[3]; } }
        public UInt16 m_AO_AddrStart { get { return info[4]; } }
        public UInt16 m_AO_AddrEnd { get { return info[5]; } }
        public UInt16 m_AI_AddrStart { get { return info[6]; } }
        public UInt16 m_AI_AddrEnd
        {
            get { return info[7]; }
        }

        //hxt
        public int m_DataDIVal
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < m_DataDI.Count; i++)
                {
                    sum += Convert.ToInt32(m_DataDI[i]) * Convert.ToInt32(Math.Pow(2, i));
                }
                return sum;
            }
        }

        public int m_DataDOVal
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < 15; i++)
                {
                    sum += Convert.ToInt32(m_DataDO[i]) * Convert.ToInt32(Math.Pow(2, i));
                }
                return sum;
            }
        }

        public bool m_isOnline
        {
            get
            {

                return Convert.ToBoolean(info[8]);
            }
            set
            {
                info[8] = 0;
            }
        }

        private List<int> subTypeList;
        private List<int> m_SubTypeList//子类型容器
        {
            get
            {
                subTypeList = new List<int>();
                subTypeList.Add(m_ConfigCH1 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH2 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH3 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH4 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH5 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH6 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH7 & AI8C_LowType);
                subTypeList.Add(m_ConfigCH8 & AI8C_LowType);
                return subTypeList;
            }
        }

        private const int Type_0To20mA = 4;//类似于宏定义
        private const int Type_N20To20mA = 5;//类似于宏定义
        private const int AI8C_LowType = 0x1f;//类似于宏定义
        private List<float> DataAI8C;//中间list容器
        private float TempDataAI8C;//中间变量
        public List<float> m_DataAI8C//属性
        {
            get
            {
                DataAI8C = new List<float>();
                for (int i = 0; i < m_DataAI.Count; i++)//8个AI正好是8个子通道组态类型 
                    //其他情况必须指定通道数目
                {
                    if (m_SubTypeList[i] == Type_0To20mA)
                    {
                        TempDataAI8C = (float)m_DataAI[i] / 3200;
                    }
                    else if (m_SubTypeList[i] == Type_N20To20mA)
                    {
                        if (m_DataAI[i] > 32767)//负数转换
                        {
                            TempDataAI8C = (float)(m_DataAI[i] - 65535) / 1600;
                        }
                        else
                        {
                            TempDataAI8C = (float)m_DataAI[i] / 1600;
                        }
                    }
                    else
                    {
                        TempDataAI8C = 0;
                    }
                    DataAI8C.Add(TempDataAI8C);//数据塞入
                }
                return DataAI8C;
            }
        }

        private string aI8C_FilterType;
        public string AI8C_FilterType
        {
            get
            {
                int typeval = (int)m_ConfigCH1 & 0xe0;
                if (typeval==0)
                {
                    aI8C_FilterType = "无\n1个周期";
                }
                else if (typeval == 32)
                {
                    aI8C_FilterType = "弱\n4个周期";
                }
                else if(typeval == 64)
                {
                    aI8C_FilterType = "中\n16个周期";
                }
                else if(typeval == 96)
                {
                    aI8C_FilterType = "强\n32个周期";
                }
                else
                {
                    aI8C_FilterType = "无\n1个周期";
                }
                //if (typeval == 0)
                //{
                //    aI8C_FilterType = "无\n（1个周期）";
                //}
                //else if (typeval == 32)
                //{
                //    aI8C_FilterType = "弱（4个周期）";
                //}
                //else if (typeval == 64)
                //{
                //    aI8C_FilterType = "中（16个周期）";
                //}
                //else if (typeval == 96)
                //{
                //    aI8C_FilterType = "强\n（32个周期）";
                //}
                //else
                //{
                //    aI8C_FilterType = "无（1个周期）";
                //}
                return aI8C_FilterType;
            }
        }

        //hxt

        public UInt16 m_typeCode { get { return info[9]; } }
        public bool m_isConfigComplete { get { return Convert.ToBoolean(info[10]); } }
        public Byte m_ConfigCH2 { get { return (Byte)(info[11] >> 8); } }
        public Byte m_ConfigCH1 { get { return (Byte)(info[11]); } }
        public Byte m_ConfigCH4 { get { return (Byte)(info[12] >> 8); } }
        public Byte m_ConfigCH3 { get { return (Byte)(info[12]); } }
        public Byte m_ConfigCH6 { get { return (Byte)(info[13] >> 8); } }
        public Byte m_ConfigCH5 { get { return (Byte)(info[13]); } }
        public Byte m_ConfigCH8 { get { return (Byte)(info[14] >> 8); } }
        public Byte m_ConfigCH7 { get { return (Byte)(info[14]); } }

        //public Byte m_ConfigCH1 { get { return (Byte)(info[11] >> 8); } }
        //public Byte m_ConfigCH2 { get { return (Byte)(info[11]); } }
        //public Byte m_ConfigCH3 { get { return (Byte)(info[12] >> 8); } }
        //public Byte m_ConfigCH4 { get { return (Byte)(info[12]); } }
        //public Byte m_ConfigCH5 { get { return (Byte)(info[13] >> 8); } }
        //public Byte m_ConfigCH6 { get { return (Byte)(info[13]); } }
        //public Byte m_ConfigCH7 { get { return (Byte)(info[14] >> 8); } }
        //public Byte m_ConfigCH8 { get { return (Byte)(info[14]); } }

        public string m_stringType { get { return GetNodeModeName(); } }

        public const int NODE_INFO_LENGTH = 15;



        [System.Serializable]
        public class ExceptionIoNode : Exception
        {
            public ExceptionIoNode() { }
            public ExceptionIoNode(string message) : base(message) { }
            public ExceptionIoNode(string message, Exception inner) : base(message, inner) { }
            protected ExceptionIoNode(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        private class m_cIOName
        {
            public ushort typeCode;
            public string Name;
            public m_cIOName(ushort typecode, string name)
            {
                typeCode = typecode;
                Name = name;
            }
        }

        public List<bool> m_DataDO;
        public List<bool> m_DataDI;
        public List<ushort> m_DataAO;
        public List<ushort> m_DataAI;

        private List<string> dataAIStr;
        private float tempval = 0;
        private string tempstr = "";
        public List<string> DataAIStr
        {
            get
            {
                dataAIStr = new List<string>();
                for (int i = 0; i < m_DataAI.Count; i++)
                {
                    if (m_DataAI[i]>32767)
                    {
                        tempval = m_DataAI[i] - 65535;
                    }
                    else
                    {
                        tempval = m_DataAI[i];
                    }
                    if (Tool.AI8ITypeVal==0)//AD
                    {
                        tempstr = tempval.ToString();
                    }
                    else if (Tool.AI8ITypeVal == 1)//mA
                    {
                        tempval /= 800;
                        tempstr = tempval.ToString("#0.000");
                    }
                    else if (Tool.AI8ITypeVal == 2)//Vol
                    {
                        tempval /= 3200;
                        tempstr = tempval.ToString("#0.000");
                    }
                    dataAIStr.Add(tempstr);
                }
                return dataAIStr;
            }
        }




        m_cIOName[] tableIOName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="id"></param>
        public CIoNode(ushort[] inputs, ushort id)
        {
            nodeID = id;



            tableIOName = new m_cIOName[]{
                new m_cIOName( 0x01,   "IOType_DI_16"),
                new m_cIOName( 0x02,   "IOType_DO_15"),
                new m_cIOName( 0x03,   "IOType_AI_8"),
                new m_cIOName( 0x04,   "IOType_AO_8"),
                new m_cIOName( 0x05,   "IOType_AI_4"),
                new m_cIOName( 0x06,   "IOType_AO_4"),
                new m_cIOName( 0x07,   "IOType_AI_2"),
                new m_cIOName( 0x08,   "IOType_AO_2"),
                new m_cIOName( 0x010,   "IOType_AI_TP_4"),
                new m_cIOName( 0x011,   "IOType_AI_RTD_4"),
                new m_cIOName( 0x012,   "IOType_AI_TC_4"),
                new m_cIOName( 0x013,   "IOType_AI_8C"),
                new m_cIOName( 0x014,   "IOType_DI_8"),
                new m_cIOName( 0x015,   "IOType_FI_4"),
                new m_cIOName( 0x016,   "IOType_DO_8R"),
                new m_cIOName( 0x017,   "IOType_DO_8"),
                new m_cIOName( 0x018,   "IOType_DO_12"),
                new m_cIOName( 0x020,   "IOType_LJDI2"),
            };

            UpdateInfo(inputs);
            m_DataDO = new List<bool>();
            m_DataDI = new List<bool>();
            m_DataAO = new List<ushort>();
            m_DataAI = new List<ushort>();
            ushort len;
            if (CheckIfExistDO())
            {
                len = (ushort)((m_DO_AddrEnd + 1 - m_DO_AddrStart) * 8);
                for (int i = 0; i < len; i++)
                {
                    m_DataDO.Add(false);
                }
            }
            if (CheckIfExistDI())
            {
                len = (ushort)((m_DI_AddrEnd + 1 - m_DI_AddrStart) * 8);
                for (int i = 0; i < len; i++)
                {
                    m_DataDI.Add(false);
                }
            }
            if (CheckIfExistAO())
            {
                len = (ushort)((m_AO_AddrEnd + 1 - m_AO_AddrStart) / 2);
                for (int i = 0; i < len; i++)
                {
                    m_DataAO.Add(0);
                }
            }
            if (CheckIfExistAI())
            {
                len = (ushort)((m_AI_AddrEnd + 1 - m_AI_AddrStart) / 2);
                for (int i = 0; i < len; i++)
                {
                    m_DataAI.Add(0);
                }
            }
        }


        public void UpdateInfo(ushort[] inputs)
        {
            inputs.CopyTo(info, 0);
            CheckAddrIfLegal();

        }
        private void CheckAddrIfLegal()
        {
            if (m_DO_AddrStart >= 200)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: DO_AddrStart ≥ 200.");
            }
            if (m_DO_AddrEnd >= 200)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: m_DO_AddrEnd ≥ 200.");
            }
            if (m_DO_AddrStart > m_DO_AddrEnd)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: DO_AddrStart ＞ DO_AddrEnd.");
            }

            if (m_DI_AddrStart >= 200)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: m_DI_AddrStart ≥ 200.");
            }
            if (m_DI_AddrEnd >= 200)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: m_DI_AddrEnd ≥ 200.");
            }
            if (m_DI_AddrStart > m_DI_AddrEnd)
            {
                throw new ExceptionIoNode(
                    "Wrong node info: m_DI_AddrStart ＞ m_DI_AddrEnd.");
            }

            if (CheckIfExistAO())
            {
                if ((m_AO_AddrStart < 200) || (m_AO_AddrStart >= 1800))
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: (m_AO_AddrStart<200)|| (m_AO_AddrStart >=1800).");
                }
                if ((m_AO_AddrEnd < 200) || (m_AO_AddrEnd >= 1800))
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: (m_AO_AddrEnd<200)|| (m_AO_AddrEnd >=1800).");
                }
                if (m_AO_AddrStart > m_AO_AddrEnd)
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: m_AO_AddrStart ＞ m_AO_AddrEnd.");
                }
            }


            if (CheckIfExistAI())
            {
                if ((m_AI_AddrStart < 200) || (m_AI_AddrStart >= 1800))
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: (m_AI_AddrStart<200)|| (m_AI_AddrStart >=1800).");
                }
                if ((m_AI_AddrEnd < 200) || (m_AI_AddrEnd >= 1800))
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: (m_AI_AddrEnd<200)|| (m_AI_AddrEnd >=1800).");
                }
                if (m_AI_AddrStart > m_AI_AddrEnd)
                {
                    throw new ExceptionIoNode(
                        "Wrong node info: m_AI_AddrStart ＞ m_AI_AddrEnd.");
                }
            }

        }
        public bool CheckIfExistDO()
        {
            if ((m_DO_AddrStart == 0) &&
                (m_DO_AddrEnd == 0))
            {
                return false;
            }
            return true;
        }
        public bool CheckIfExistDI()
        {
            if ((m_DI_AddrStart == 0) &&
                (m_DI_AddrEnd == 0))
            {
                return false;
            }
            return true;
        }
        public bool CheckIfExistAO()
        {
            if ((m_AO_AddrStart == 0) &&
                (m_AO_AddrEnd == 0))
            {
                return false;
            }
            return true;
        }
        public bool CheckIfExistAI()
        {
            if ((m_AI_AddrStart == 0) &&
                (m_AI_AddrEnd == 0))
            {
                return false;
            }
            return true;
        }
        public bool CheckIfExist()
        {
            if (CheckIfExistDO() == true)
            {
                return true;
            }
            if (CheckIfExistAO() == true)
            {
                return true;
            }
            if (CheckIfExistDI() == true)
            {
                return true;
            }
            if (CheckIfExistAI() == true)
            {
                return true;
            }
            return false;
        }
        private string GetNodeModeName()
        {
            for (int i = 0; i < tableIOName.Length; i++)
            {
                if (tableIOName[i].typeCode == m_typeCode)
                {
                    return tableIOName[i].Name;
                }
            }
            return "unknown";
        }

        public void ClearData()
        {
            m_isOnline = false;
            for (int i = 0; i < m_DataDO.Count; i++)
            {
                m_DataDO[i] = false;
            }
            for (int i = 0; i < m_DataDI.Count; i++)
            {
                m_DataDI[i] = false;
            }
            for (int i = 0; i < m_DataAO.Count; i++)
            {
                m_DataAO[i] = 0;
            }
            for (int i = 0; i < m_DataAI.Count; i++)
            {
                m_DataAI[i] = 0;
            }
        }
    };

    public class CInfo
    {
        public enum EInputOrHold
        {
            Input,
            Holding
        }
        public ushort DATA_LENGTH;
        public ushort AddressStart;
        public ushort[] data;
        public EInputOrHold eInputOrHold = new EInputOrHold();
        public void UpdateInfo(ushort[] array)
        {
            data = new ushort[array.Length];
            array.CopyTo(data, 0);
        }

    }

    public class CInputInfo : CInfo
    {
        public CInputInfo()
        {
            eInputOrHold = EInputOrHold.Input;
        }

    }

    public class CHoldInfo : CInfo
    {
        public CHoldInfo()
        {
            eInputOrHold = EInputOrHold.Holding;
        }
    }
    public class CDeviceInfo : CInputInfo
    {
        public string m_FirmwareVer
        {
            get
            {
                return string.Format("{0:F3}", data[0] / 1000.0f);
            }
        }
        public string m_HardwareVer
        {
            get { return string.Format("{0:F3}", data[1] / 1000.0f); }
        }

        public string m_DeviceModel
        {
            get
            {
                byte[] modelBytes = new byte[32];
                for (int i = 0; i < 14; i++)
                {
                    modelBytes[2 * i] = (Byte)(data[2 + i]);
                    modelBytes[(2 * i) + 1] = (Byte)(data[2 + i] >> 8);
                }
                string strModel = System.Text.Encoding.Default.GetString(modelBytes);
                //strModel.Replace("\0","");
                strModel = strModel.Trim("\0".ToCharArray());
                return strModel;
            }
        }

        public string m_strSerialNum
        {
            get
            {
                return string.Format(
                    "{0:X4}H-{1:X4}H-{2:X4}H-" +
                    "{3:X4}H-{4:X4}H-{5:X4}H",
                    data[18], data[19], data[20],
                    data[21], data[22], data[23]);
            }
        }

        public CDeviceInfo()
        {
            DATA_LENGTH = 24;
            AddressStart = Convert.ToUInt16(ECT2InputReg.deviceInfo);
        }
    }

    public class CCpuInfo : CInputInfo
    {
        /*0x00  最大锁调度器时间*/
        public UInt32 u32MaxTime_SchedLock
        {
            get
            {
                return (UInt32)data[0] * 65536 + (UInt32)data[1];
            }
        }
        /*0x02  最大中断时间*/
        public Int32 i32MaxTime_interrupt
        {
            get
            {
                return (Int32)((UInt32)data[2] * 65536 + (UInt32)data[3]);
            }
        }
        /*0x04  CPU主频*/
        public string CpuClikFreq
        {
            get
            {
                float Freq = ((UInt32)data[4] * 65536 + (UInt32)data[5]) / 1000000;
                return String.Format("{0:F}M", Freq);
            }
        }
        /*0x06  任务切换总次数*/
        public Int32 i32TaskCtxSwCtr
        {
            get
            {
                return (Int32)((UInt32)data[6] * 65536 + (UInt32)data[7]);
            }
        }

        /*0x08  ucos版本,格式：##.##.##(*10000)     */
        public string ucosVer
        {
            get
            {
                int ver3 = data[8] % 100;
                int ver2 = (data[8] % 10000) / 100;
                int ver1 = data[8] / 10000;
                return string.Format("{0:D2}.{1:D2}.{2:D2}.",
                    ver1, ver2, ver3);
            }
        }

        /*0x09  Cpu使用率 格式：##.##%(*100)  	   */
        public float CPU_Usage
        {
            get { return (float)(data[9] / 100.0); }
        }

        /*0x0A  Cpu最大使用率  格式：##.##%(*100)   */
        public float CPU_MaxUsage
        {
            get { return (float)(data[10] / 100.0); }
        }

        public CCpuInfo()
        {
            DATA_LENGTH = 12;
            AddressStart = Convert.ToUInt16(ECT2InputReg.cpuInfo);
        }

    }

    public class CClientTcpInfo : CInputInfo
    {
        public string clientIP1
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[0], data[1], data[2], data[3]);
            }
        }
        public ushort clientPort1
        { get { return data[4]; } }

        public string clientIP2
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[6], data[7], data[8], data[9]);
            }
        }
        public ushort clientPort2
        { get { return data[10]; } }

        public string clientIP3
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[12], data[13], data[14], data[15]);
            }
        }
        public ushort clientPort3
        { get { return data[16]; } }

        public string clientIP4
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[18], data[19], data[20], data[21]);
            }
        }
        public ushort clientPort4
        { get { return data[22]; } }


        public CClientTcpInfo()
        {
            DATA_LENGTH = 24;
            AddressStart = Convert.ToUInt16(ECT2InputReg.clientTcp);
        }

    }
    public class CCommInfo : CInputInfo
    {
        public bool bIfIpFromCfg
        { get { return Convert.ToBoolean(data[0]); } }
        public string LocalIP
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[1], data[2], data[3], data[4]);
            }
        }
        public string RemoteIP
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[5], data[6], data[7], data[8]);
            }
        }
        public string Netmask
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[9], data[10], data[11], data[12]);
            }
        }
        public string Gateway
        {
            get
            {
                return string.Format("{0:D}.{1:D}.{2:D}.{3:D}",
                    data[13], data[14], data[15], data[16]);
            }
        }
        public string MacAddress
        {
            get
            {
                return string.Format("{0:X2}.{1:X2}.{2:X2}.{3:X2}.{4:X2}.{5:X2}",
                    data[17], data[18], data[19], data[20], data[21], data[22]);
            }
        }
        public bool bIfRtuFromCfg
        { get { return Convert.ToBoolean(data[23]); } }

        public ushort RTUslaveAddress
        { get { return data[24]; } }
        public UInt32 BaudRS232
        { get { return ((UInt32)data[25]) * 100U; } }
        public string ParityRS232
        {
            get
            {
                switch (data[26])
                {
                    case 0: return "无校验";
                    case 1: return "偶校验";
                    case 2: return "奇校验";
                    default:
                        return "未知";
                }
            }
        }
        public ushort DatabitRS232
        { get { return data[27]; } }
        public ushort StopbitRS232
        { get { return data[28]; } }

        public UInt32 BaudRS485
        { get { return ((UInt32)data[29]) * 100U; } }
        public string ParityRS485
        {
            get
            {
                switch (data[30])
                {
                    case 0: return "无校验";
                    case 1: return "偶校验";
                    case 2: return "奇校验";
                    default:
                        return "未知";
                }
            }
        }
        public ushort DatabitRS485
        { get { return data[31]; } }
        public ushort StopbitRS485
        { get { return data[32]; } }


        public CCommInfo()
        {
            DATA_LENGTH = 33;
            AddressStart = Convert.ToUInt16(ECT2InputReg.commInfo);
        }

    }

    public class CRtcInfo : CInputInfo
    {
        public string CpuCoreTemprature
        {
            get
            {
                return string.Format("{0:F}℃", data[0] / 10.0);
            }
        }
        public string CpuCoreVoltage
        {
            get
            {
                return string.Format("{0:F}V", data[1] / 1000.0);
            }
        }
        public string BatteryVoltage
        {
            get
            {
                return string.Format("{0:F}V", data[3] / 1000.0);
            }
        }
        public string RtcCurrentTime
        {
            get
            {
                int year = data[9];
                int month = data[8];
                int day = data[7];
                int hour = data[6];
                int minute = data[5];
                int second = data[4];
                DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
                string strDateTime = dateTime.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm:ss");
                //("yyyy/MM/dd")+" "+ DateTime.Now.ToString("HH:mm:ss");
                return strDateTime;
            }
        }

        public CRtcInfo()
        {
            DATA_LENGTH = 11;
            AddressStart = Convert.ToUInt16(ECT2InputReg.systemInfo);
        }
    }

    public class CBackplaneInfo : CInputInfo
    {
        public bool bIfErr
        {
            get
            {
                return Convert.ToBoolean(data[0]);
            }
        }
        public string BpStatus
        {
            get
            {
                switch (data[0])
                {
                    case 0: return "无故障";
                    case 1: return "被动错误";
                    case 2: return "主动错误";
                    case 3: return "bus off";
                    default: return "未知";
                }
            }
        }
        public string BpErrCode
        {
            get
            {
                switch (data[1])
                {
                    case 0: return "无故障";
                    case 1: return "填充错误";
                    case 2: return "格式错误";
                    case 3: return "应答错误";
                    case 4: return "隐性位错误";
                    case 5: return "显性位错误";
                    case 6: return "CRC错误";
                    case 7: return "软件置错";
                    default: return "未知";
                }
            }
        }
        public ushort ErrCountRecv
        { get { return data[2]; } }

        public ushort ErrCountSend
        { get { return data[3]; } }

        public CBackplaneInfo()
        {
            DATA_LENGTH = 4;
            AddressStart = Convert.ToUInt16(ECT2InputReg.backplaneInfo);
        }
    }

    public class CRtcSettingInfo : CHoldInfo
    {
        public bool bIfSetting
        {
            get
            {
                return Convert.ToBoolean(data[0]);
            }
            set
            {
                data[0] = Convert.ToUInt16(value);
            }
        }
        public string RtcSettingTime
        {
            get
            {
                int year = data[6];
                int month = data[5];
                int day = data[4];
                int hour = data[3];
                int minute = data[2];
                int second = data[1];
                DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
                string strDateTime = dateTime.ToString();
                return strDateTime;
            }
            set
            {
                DateTime dateTime = new DateTime();
                if (DateTime.TryParse(value, out dateTime))
                {//转换成功
                    data[1] = (ushort)dateTime.Second;
                    data[2] = (ushort)dateTime.Minute;
                    data[3] = (ushort)dateTime.Hour;
                    data[4] = (ushort)dateTime.Day;
                    data[5] = (ushort)dateTime.Month;
                    data[6] = (ushort)dateTime.Year;
                    data[7] = (ushort)dateTime.DayOfWeek;
                }
            }
        }

        public CRtcSettingInfo()
        {
            DATA_LENGTH = 8;
            data = new ushort[] { 0, 1, 1, 12, 1, 1, 2000, 1 };
            AddressStart = Convert.ToUInt16(ECT2HoldReg.RtcSetting);
        }
    }

    public class CSelfTestInfo : CHoldInfo
    {
        public bool bSwitchSetTest
        {
            get
            {
                return Convert.ToBoolean(data[0]);
            }
            set
            {
                data[0] = Convert.ToUInt16(value);
            }
        }
        public string RamTestRes
        {
            get
            {
                switch (data[1])
                {
                    case 0: return "正常";
                    case 1: return "外部SRAM故障";
                    case 2: return "外部MRAM故障";
                    case 3: return "外部SRAM与MRAM均故障";
                    default: return "未知";
                }
            }
        }

        public string TFCardTestRes
        {
            get
            {
                switch (data[2])
                {
                    case 0: return "正常";
                    case 1: return "故障";
                    default: return "未知";
                }
            }
        }

        public UInt16 TFCardTotal
        { get { return data[3]; } }

        public UInt16 TFCardUsed
        { get { return data[4]; } }

        public string CfgFileTestRes
        {
            get
            {
                switch (data[5])
                {
                    case 0: return "正常";
                    case 1: return "usb线连接状态，无法检查";
                    case 2: return "配置文件打开失败";
                    default: return "未知";
                }
            }
        }
        public CSelfTestInfo()
        {
            DATA_LENGTH = 6;
            data = new ushort[DATA_LENGTH];
            AddressStart = Convert.ToUInt16(ECT2HoldReg.SelfTest);
        }
    }
}



