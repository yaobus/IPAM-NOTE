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


		/// <summary>
		/// 网段信息
		/// </summary>
		public class AddressInfo
		{
			public int Id { get; set; }

			public string TableName { get; set; }

			public string Network { get; set; }

			public string Netmask { get; set; }

			public string Description { get; set; }

			public string Del { get; set; }




			public AddressInfo(int id,string tableName,string network,string netmask,string description,string del)
			{
				Id = id;
				TableName = tableName;
				Network = network;
				Netmask = netmask;
				Description = description;
				Del=del;

			}

		}


	}
}
