﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		/// <summary>
		/// 搜索框搜索后被选中的IP地址索引
		/// </summary>
		public static int SelectIndex;

		/// <summary>
		/// 搜索框搜索的网段INDEX
		/// </summary>
		public static int SelectNetwork;

		/// <summary>
		/// 正常加载=0，搜索加载=1
		/// </summary>
		public static int LoadType;

		/// <summary>
		/// 搜索时所选表格名字
		/// </summary>
		public static string SelectTableName;


		public static List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();


		//搜索区网段列表
		public static ObservableCollection<ComBoxAddressInfo> ComBoxAddressInfos = new ObservableCollection<ComBoxAddressInfo>();

		/// <summary>
		/// 数据导入界面，网段ComboBox数据源
		/// </summary>
		public static List<string> ComboBoxAddressList = new List<string>();

		/// <summary>
		/// 被选择的按钮的标记
		/// </summary>
		public static int SelectButtonTag;

		/// <summary>
		/// 搜索的类型，0为全局搜索，1为指定搜索
		/// </summary>
		public static int SearchType;


		/// <summary>
		/// 全局搜索时点击搜索结果被选中的表项
		/// </summary>
		public static ViewMode.SearchInfo SelectSearchInfo = new ViewMode.SearchInfo();


		/// <summary>
		/// 全局搜索时候获取的结果列表
		/// </summary>
		public static List<ViewMode.SearchInfo> searchInfos = new List<SearchInfo>();


	}
}
