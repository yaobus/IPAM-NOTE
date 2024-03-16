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
			public int Address { get; set; }

			public int AddressStatus { get; set; }

			public string User { get; set; }

			public string Description { get; set; }


			public IpAddressInfo(int address,int addressStatus,string user,string description)
			{
				Address=address;
				AddressStatus=addressStatus;
				User=user;
				Description=description;
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


	}
}
