using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAM_NOTE
{
	internal class DataBrige
	{
		public static ViewMode.AddressInfo TempAddress = new ViewMode.AddressInfo(0, "", "", "", "", "");

		public static string SelectIp;

		public static ViewMode.IpAddressInfo IpAddress = new ViewMode.IpAddressInfo(0, 0, "", "");
	}
}
