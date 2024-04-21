using System;
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

        public static ViewMode.IpAddressInfo IpAddress = new ViewMode.IpAddressInfo(0, 0, "", "", System.Net.NetworkInformation.IPStatus.Unknown, -1, "", "", "", "");

        public static List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();

        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public static string Ver = "1.22";

        /// <summary>
        /// 最新版本号
        /// </summary>
        public static string LatestVersion = "0";

        /// <summary>
        /// 最新下载地址
        /// </summary>
        public static String DownloadUrl = "";

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


        //public static List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();




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


        /// <summary>
        /// 分配或者释放模式，0为未选择状态、1为选择未分配地址准备分配，2为选择已分配地址准备释放
        /// </summary>
        public static int SelectMode = 0;


        /// <summary>
        /// 多选模式下一并被选择的IP
        /// </summary>
        public static List<int> SelectedIpAddress = new List<int>();


        /// <summary>
        /// 更新信息
        /// </summary>
        public static string UpdateInfos = "";


        //----------------------------DevicesPage-----------------------------------

        //获取到的设备信息列表
        public static List<DeviceInfo> DeviceInfos = new List<DeviceInfo>();


        /// <summary>
        /// 从数据库获取到的设备端口信息列表
        /// </summary>
        public static List<ViewMode.DevicePortInfo> DevicePortInfos = new List<ViewMode.DevicePortInfo>();

        /// <summary>
        /// 所选设备端口的标签
        /// </summary>
        public static string SelectDeviceButtonTag;



        /// <summary>
        /// 点击设备列表时临时存储当前所选设备的对应信息
        /// </summary>
        public static ViewMode.DeviceInfo SelectDeviceInfo;


        /// <summary>
        /// 点击端口按钮后临时存储所选端口对应的信息
        /// </summary>
        public static ViewMode.DevicePortInfo SelectDevicePortInfo;


        /// <summary>
        /// 0为添加设备，1为编辑设备
        /// </summary>
        public static int DeviceAddStatus = 0;


        /// <summary>
        /// 设备端口的选择模式，0为未选择状态，1为已选择未分配端口准备分配，2为已选择已分配端口准备释放
        /// </summary>
        public static int SelectDevicePortMode = 0;

        /// <summary>
        /// 多选模式下首次选择的端口类型，E,F,M,D
        /// </summary>
        public static string SelectDevicePortType = "";


        /// <summary>
        /// 多选模式下首次选择的端口状态，0未分配，1已分配
        /// </summary>
        public static string SelectDevicePortStatus = "-1";

        /// <summary>
        /// 多选模式下，一次选择的多个端口清单
        /// </summary>
        public static List<string> portList = new List<string>();//批量选择的端口号

        /// <summary>
        /// 设备端口列表显示还是图形显示，0为图形，1为列表
        /// </summary>
        public static int GraphicsMode = 0;


        /// <summary>
        /// 搜索的设备表名
        /// </summary>
        public static string SearchDeviceTableName;

        /// <summary>
        /// 点击设备列表时候的设备表名
        /// </summary>
        public static string SelectDeviceTableName;


		/// <summary>
		/// 全局搜索到的设备信息清单
		/// </summary>
		public static List<SearchDeviceInfo> SearchDeviceInfos = new List<SearchDeviceInfo>();


        /// <summary>
        /// 设备信息加载方式，0为正常加载，1为搜索加载
        /// </summary>
        public static int DeviceLoadType;


		/// <summary>
		/// 设备端口信息搜索方式,0为全局搜索，1为指定搜索
		/// </summary>
		public static int DeviceSearchType = -1;



        /// <summary>
        /// 设备列表，显示在搜索筛选框
        /// </summary>
		public static List<string> DevicesList = new List<string>();


        /// <summary>
        /// 表示点击标签是编辑还是访问，0为访问，1为编辑
        /// </summary>
        public static int EditMode = 0;


        /// <summary>
        /// 表示当前选中的导航标签，-1为未选择
        /// </summary>
        public static int SelectIndexTag = -1;

        //----------------------------ModelPreset-----------------------------------

        /// <summary>
        /// 型号预设列表
        /// </summary>
        public static List<ViewMode.ModelPresetInfo> ModelPresetInfos = new List<ViewMode.ModelPresetInfo>();
    }
}
