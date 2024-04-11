using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPAM_NOTE
{
	public class ViewMode
	{


		/// <summary>
		/// ip地址信息
		/// </summary>
		public class IpAddressInfo
		{
			//IP地址
			public int Address { get; set; }

			public int AddressStatus { get; set; }

			public string User { get; set; }

			public string Description { get; set; }

			public IPStatus PingStatus { get; set; }

			public long PingTime { get; set; }

			public string HostName { get; set; }

			public string MacAddress { get; set; }

			public string OnlineHostName { get; set; }//在线的主机名

			public string OnlineMacAddress { get; set; }//在线的MAC地址


			public IpAddressInfo(int address,int addressStatus,string user,string description, IPStatus pingStatus, long pingTime, string hostName, string macAddress,string onlineHostName,string onlineMacAddress)
			{
				Address = address;
				AddressStatus = addressStatus;
				User = user;
				Description = description;
				PingStatus = pingStatus;
				PingTime = pingTime;
				HostName = hostName;
				MacAddress = macAddress;
				OnlineHostName = onlineHostName;
				OnlineMacAddress = onlineMacAddress;
			}
		}




		/// <summary>
		/// ip地址信息
		/// </summary>
		public class IpAddressInfoFromCsv
		{
			//IP地址
			public int Address { get; set; }

			public int AddressStatus { get; set; }

			public string User { get; set; }

			public string Description { get; set; }

			public string PingStatus { get; set; }

			public long PingTime { get; set; }

			public string HostName { get; set; }

			public string MacAddress { get; set; }


		}





		/// <summary>
		/// 网段信息
		/// </summary>
		public class AddressInfo
		{
			public int Id { get; set; }

			public string TableName { get; set; }

			public string Network { get; set; }

			public string NetMask { get; set; }

			public string Description { get; set; }

			public string Del { get; set; }




			public AddressInfo(int id,string tableName,string network,string netMask,string description,string del)
			{
				Id = id;
				TableName = tableName;
				Network = network;
				NetMask = netMask;
				Description = description;
				Del=del;

			}

		}

		/// <summary>
		/// 检测状态
		/// </summary>
		public class IPInfo
		{
			public int IPAddress { get; set; }
			public IPStatus PingStatus { get; set; }
			public long PingTime { get; set; }
			public string HostName { get; set; }
			public string MACAddress { get; set; }

			public IPInfo(int ipAddress, IPStatus pingStatus, long pingTime, string hostName, string macAddress)
			{
				IPAddress = ipAddress;
				PingStatus = pingStatus;
				PingTime = pingTime;
				HostName = hostName;
				MACAddress = macAddress;
			}
		}


		/// <summary>
		/// 搜索复选框网段信息
		/// </summary>
		public class ComBoxAddressInfo
		{

			public string TableName { get; set; }

			public string Network { get; set; }

			public string NetMask { get; set; }


			public ComBoxAddressInfo(string tableName, string network,string netMask)
			{

				TableName = tableName;
				Network = network;
				NetMask=netMask;
				
			}

		}


		/// <summary>
		/// 备份文件信息
		/// </summary>
		public class BackupInfo
		{
			public int Index { get; set; }
			public string FileName { get; set; }
			public string BackupTime { get; set; }
		}


		/// <summary>
		/// 通过全局搜索获取到的数据类型
		/// </summary>
		public  class SearchInfo
		{
			// SearchInfo类的定义
			public string TableName { get; set; }
			public string Network { get; set; }
			public string Netmask { get; set; }
			public int Address { get; set; }
			public int AddressStatus { get; set; }
			public string User { get; set; }
			public string Description { get; set; }
			public string HostName { get; set; }
			public string MacAddress { get; set; }
		}

		/// <summary>
		/// 需要导出的通过全局搜索获取到的数据类型
		/// </summary>
		public class SearchExportInfo
		{
			// SearchInfo类的定义
			public string TableName { get; set; }
			public string Network { get; set; }
			public string Netmask { get; set; }
			public string Address { get; set; }
			public string AddressStatus { get; set; }
			public string User { get; set; }
			public string Description { get; set; }
			public string PingStatus { get; set; }
			public string PingTime { get; set; }
			public string HostName { get; set; }
			public string MacAddress { get; set; }
		}


        //-------------------------------------Devices-------------------------------------------

        /// <summary>
        /// 设备总表设备信息数据类型
        /// </summary>
        public class DeviceInfo
        {
            // 设备对应表名称
            public int Id
            {
                get; set;
            }

            // 设备对应表名称
            public string TableName
            {
                get; set;
            }

            // 设备名称
            public string Name
            {
                get; set;
            }
            public string Model
            {
                get; set;
            }
            public string Number
            {
                get; set;
            }
            public string People
            {
                get; set;
            }
            public string Date
            {
                get; set;
            }
            public string Description
            {
                get; set;
            }
            public int Eport
            {
                get; set;
            }
            public string EportTag
            {
                get; set;
            }
            public int Fport
            {
                get; set;
            }
            public string FportTag
            {
                get; set;
            }
            public int Mport
            {
                get; set;
            }
            public string MportTag
            {
                get; set;
            }
            public int Dport
            {
                get; set;
            }
            public string DportTag
            {
                get; set;
            }
            public DeviceInfo(int id, string tableName, string name, string model, string number, string people, string date, string description, int ePort,string ePortTag, int fPort, string fPortTag, int mPort, string mPortTag, int dPort, string dPortTag)
            {
                Id = id;
                TableName = tableName;
                Name = name;
                Model = model;
                Number = number;
                People = people;
                Date = date;
                Description = description;
                Eport = ePort;
                EportTag = ePortTag;
                Fport = fPort;
                FportTag=fPortTag;
                Mport = mPort;
                MportTag=mPortTag;
                Dport = dPort;
                DportTag = dPortTag;


            }
        }


        
        /// <summary>
        /// 设备信息数据类型
        /// </summary>
        public class ModelPresetInfo
        {
            // 设备对应表名称
            public string Model
            {
                get; set;
            }

            public int Eport
            {
                get; set;
            }

            public int Fport
            {
                get; set;
            }
            
            public int Mport
            {
                get; set;
            }

            public ModelPresetInfo(string model,int eport,int fport,int mport)
            {

                Model = model;
                Eport = eport;
                Fport = fport;
                Mport = mport;

            }
        }


        /// <summary>
        /// 单个设备端口信息，含网口和硬盘位
        /// </summary>
        public class DevicePortInfo
        {
            // 设备端口类型
            public string PortType
            {
                get; set;
            }

            //端口号
            public string PortNumber
            {
                get; set;
            }
            //端口状态，0为未使用，1为已使用
            public string PortStatus
            {
                get; set;
            }

            public string PortTag1
            {
                get; set;
            }

            public string PortTag2
            {
                get; set;
            }

            public string PortTag3
            {
                get; set;
            }


            public string Description
            {
                get; set;
            }


            public DevicePortInfo(string portType,string portNumber,int portStatus,string portTag1,string portTag2,string portTag3,string description)
            {
                PortType=portType;
                PortNumber=portNumber;
                PortStatus=portStatus.ToString();
                PortTag1=portTag1;
                PortTag2=portTag2;
                PortTag3=portTag3;
                Description=description;

                

            }
        }


        /// <summary>
        /// 通过全局搜索获取到的设备数据类型
        /// </summary>
        public class SearchDeviceInfo
        {
            // SearchInfo类的定义
            public string TableName
            {
                get; set;
            }
            public string Name
            {//设备名
                get; set;
            }
            public string Model
            {//设备型号
                get; set;
            }
            public string Number
            {//设备编号
                get; set;
            }
            public string PortType
            {//端口类型
                get; set;
            }
            public string PortNumber
            {
                get; set;
            }

            public string PortStatus
            {//端口状态
                get; set;
            }
            public string PortTag1
            {
                get; set;
            }
            public string PortTag2
            {
                get; set;
            }
            public string PortTag3
            {
                get; set;
            }

            public string Description
            {
                get; set;
            }

        }

    }
}
