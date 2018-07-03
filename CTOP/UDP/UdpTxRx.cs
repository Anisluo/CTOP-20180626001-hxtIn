using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Net.Sockets;


using System.ComponentModel;
using System.Collections.ObjectModel;
namespace CTOP.UDP
{
    public class JsonTool: INotifyPropertyChanged
    {
        public JsonTool()
        {
            MName = "CT3-CTOP";
            CMD = "Read";
            MAC = "";
            UName = "";
            SNum = "";
            HWVer = "";
            FMVer = "";
            LIP = "";
            RIP = "";
            NetM = "";
            GateW = "";
            Key = "";
        }

        private string mname;
        public string MName
        {
            get
            {
                return mname;
            }
            set
            {
                mname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MName"));
            }
        }

        private string cmd;
        public string CMD
        {
            get
            {
                return cmd;
            }
            set
            {
                cmd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CMD"));
            }
        }


        private string uname;
        public string UName
        {
            get
            {
                return uname;
            }
            set
            {
                uname = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UName"));
            }
        }

        private string snum;
        public string SNum
        { set; get; }
        private string hw;
        public string HWVer
        { set; get; }
        private string fm;
        public string FMVer
        { set; get; }
        private string mac;
        public string MAC
        {
            get
            {
                return mac;
            }
            set
            {
                mac = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MAC"));
            }
        }
        private string lip;
        public string LIP
        {
            get
            {
                return lip;
            }
            set
            {
                lip = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LIP"));
            }
        }
        private string rip;
        public string RIP
        { set; get; }
        private string netm;
        public string NetM
        { set; get; }
        private string gateW;
        public string GateW
        { set; get; }
        private string key;
        public string Key
        { set; get; }

        public string Index
        { set; get; }

        public int GetJsonString(string Jsontext, ref JsonTool json)
        {
            try
            {
                json = JsonConvert.DeserializeObject<JsonTool>(Jsontext);
                //报文收发
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }

        public int SetJsonString(ref string JsonText,JsonTool json)
        {
            try
            {
                JsonText = JsonConvert.SerializeObject(json);
            }
            catch (Exception)
            {
                JsonText = "";
                return -1;
            }
            return 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

    }

    public class UdpTool
    {
        private UdpClient myudpclient;
        private UdpClient receiveudpclient;
        private Thread thread;
        private bool IsInit = false;
        private JsonTool json = new JsonTool();

        private int RemotePort;
        private int ListenPort;
        private string RemoteIP;

        public string Recstring;
        public ObservableCollection<JsonTool> jsonlist = new ObservableCollection<JsonTool>();
        
        public bool IsRecOK
        {
            get;set;
        }

        private IPEndPoint connetcip;//远程连接到自己上的IP地址

        public int Init(int remotePort, string remoteIP, int listenPort)
        {
            try
            {
                if (!IsInit)
                {
                    IsInit = true;
                    RemotePort = remotePort;
                    RemoteIP = remoteIP;
                    ListenPort = listenPort;

                    myudpclient = new UdpClient();
                    myudpclient.Connect(IPAddress.Parse(RemoteIP), RemotePort);

                    connetcip = new IPEndPoint(IPAddress.Any, ListenPort);
                    receiveudpclient = new UdpClient(ListenPort);
                    thread = new Thread(Rec);
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }

        public int Send(string msg)
        {
            try
            {
                Byte[] SendBytes = Encoding.ASCII.GetBytes(msg);
                myudpclient.Send(SendBytes, SendBytes.Length);
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }

        public int StartRec()
        {
            try
            {
                if (IsInit)
                {
                    thread.Start();
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }

        public int EndRec()
        {
            try
            {
                if (IsInit)
                {
                    thread.Abort();
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return 1;
        }

        private void Rec()
        {
            Byte[] receivebyte;
            bool IsMacDuplicate;
            while (true)
            {
                try
                {
                    
                    receivebyte = receiveudpclient.Receive(ref connetcip);
                    Recstring = Encoding.ASCII.GetString(receivebyte);

                    if (json.GetJsonString(Recstring, ref json)==1)//解析报文
                    {
                        if (jsonlist.Count==0)//第一条报文直接保存
                        {
                            json.Index = jsonlist.Count.ToString();
                            jsonlist.Add(json);
                        }
                        else//从第二条报文起，检查MAC地址是否不同 相同就不保存
                        {
                            IsMacDuplicate = false;
                            for (int i = 0; i < jsonlist.Count; i++)
                            {
                                if (jsonlist[i].MAC == json.MAC)
                                {
                                    IsMacDuplicate = true;
                                    break;
                                }
                            }
                            if (!IsMacDuplicate)
                            {
                                json.Index = jsonlist.Count.ToString();
                                jsonlist.Add(json);
                            }
                        }
                        
                        if (jsonlist.Count == 2)
                        {
                            IsRecOK = true;
                        }
                    }

                }
                catch (Exception e)
                {
                    if (jsonlist.Count == 2)
                    {
                        IsRecOK = true;
                    }
                    //break;
                }
            }
        }

        public int ClcJsonListData()
        {
            jsonlist.Clear();
            return 1;
        }
    }

}
