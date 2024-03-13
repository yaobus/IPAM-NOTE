using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAM_NOTE
{
	internal class ViewMode
	{


		/// <summary>
		/// ip地址信息
		/// </summary>
		public class IpAddressInfo
		{
			//IP地址
			public int Id { get; set; }

			public string Network { get; set; }

			public IpAddressInfo(int id,string network)
			{
				Id = id;
				Network=network;
			}
		}



	}
}
