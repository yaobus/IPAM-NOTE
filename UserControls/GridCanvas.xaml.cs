using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IPAM_NOTE.UserControls
{
	/// <summary>
	/// GridCanvas.xaml 的交互逻辑
	/// </summary>
	public partial class GridCanvas : UserControl
	{

		public GridCanvas()
		{
			InitializeComponent();
			
		}


		private void DrawGrid()
		{
			double width = canvas.ActualWidth;
			double height = canvas.ActualHeight;
			double gridSizeInInches = 0.2; // 网格大小（英寸）
			double strongLineInterval = 10; // 每隔多少条线显示一根更明显的线条
			double strongLineThickness = 0.3; // 更明显的线条的粗细

			// 将网格大小转换为像素值
			double gridSizeInPixels = gridSizeInInches * 100; // 1 英寸 = 96 像素

			canvas.Children.Clear(); // 清空之前的网格

			for (double x = 0; x < width; x += gridSizeInPixels)
			{
				Line line = new Line
				{
					X1 = x,
					Y1 = 0,
					X2 = x,
					Y2 = height,
					Stroke = Brushes.LightGray,
					StrokeThickness = (x % (gridSizeInPixels * strongLineInterval) == 0) ? strongLineThickness : 0.1
				};

				canvas.Children.Add(line);
			}

			for (double y = 0; y < height; y += gridSizeInPixels)
			{
				Line line = new Line
				{
					X1 = 0,
					Y1 = y,
					X2 = width,
					Y2 = y,
					Stroke = Brushes.LightGray,
					StrokeThickness = (y % (gridSizeInPixels * strongLineInterval) == 0) ? strongLineThickness : 0.1
				};

				canvas.Children.Add(line);
			}
		}



		private void GridCanvas_OnLoaded(object sender, RoutedEventArgs e)
		{
			DrawGrid();
		}

	}
}

