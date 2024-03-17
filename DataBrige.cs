using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IPAM_NOTE.ViewMode;

namespace IPAM_NOTE
{
	internal class DataBrige
	{
		public static ViewMode.AddressInfo TempAddress = new ViewMode.AddressInfo(0, "", "", "", "", "");

		public static string SelectIp;

		public static ViewMode.IpAddressInfo IpAddress = new ViewMode.IpAddressInfo(0, 0, "", "", System.Net.NetworkInformation.IPStatus.Unknown, -1, "", "");

		public static List<IpAddressInfo> IpAddressInfos = new List<IpAddressInfo>();

		/// <summary>
		/// 0为新建网段，1为编辑网段
		/// </summary>
		public static int AddStatus = 0;



		public static List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();
	}
}
