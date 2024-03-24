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




			public IpAddressInfo(int address,int addressStatus,string user,string description, IPStatus pingStatus, long pingTime, string hostName, string macAddress)
			{
				Address = address;
				AddressStatus = addressStatus;
				User = user;
				Description = description;
				PingStatus = pingStatus;
				PingTime = pingTime;
				HostName = hostName;
				MacAddress = macAddress;
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



			public ComBoxAddressInfo(string tableName, string network)
			{

				TableName = tableName;
				Network = network;
				
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
	}
}
