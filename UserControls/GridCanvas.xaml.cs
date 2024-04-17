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
        private bool isDragging = false;
        private Point lastPosition;
        private bool isDraggingCanvas = false;
        private bool isDraggingContent = false;
        private Point lastCanvasPosition;
        private Point lastContentPosition;

        public GridCanvas()
		{
			InitializeComponent();
			
		}
        private void GridCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawGrid();
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




        private void Canvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右键按下时开始拖动操作
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    // 只拖动内容
                    isDraggingContent = true;
                    lastContentPosition = e.GetPosition(contentContainer);
                    contentContainer.CaptureMouse();
                }
                else
                {
                    // 拖动整个画布
                    isDraggingCanvas = true;
                    lastCanvasPosition = e.GetPosition(canvas);
                    canvas.CaptureMouse();
                }
            }
        }

        private void Canvas_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 右键释放时停止拖动操作
            isDraggingCanvas = false;
            isDraggingContent = false;
            canvas.ReleaseMouseCapture();
            contentContainer.ReleaseMouseCapture();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingCanvas)
            {
                // 如果正在拖动整个画布，则根据鼠标移动的距离调整画布的位置
                Point newPosition = e.GetPosition(canvas);
                double deltaX = newPosition.X - lastCanvasPosition.X;
                double deltaY = newPosition.Y - lastCanvasPosition.Y;

                Canvas.SetLeft(contentContainer, Canvas.GetLeft(contentContainer) + deltaX);
                Canvas.SetTop(contentContainer, Canvas.GetTop(contentContainer) + deltaY);

                lastCanvasPosition = newPosition;
            }
            else if (isDraggingContent)
            {
                // 如果正在拖动内容，则根据鼠标移动的距离调整内容容器的位置
                Point newPosition = e.GetPosition(contentContainer);
                double deltaX = newPosition.X - lastContentPosition.X;
                double deltaY = newPosition.Y - lastContentPosition.Y;

                Canvas.SetLeft(contentContainer, Canvas.GetLeft(contentContainer) + deltaX);
                Canvas.SetTop(contentContainer, Canvas.GetTop(contentContainer) + deltaY);

                lastContentPosition = newPosition;
            }
        }



        private void Canvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取当前鼠标相对于 Canvas 左上角的位置
            Point mousePosition = e.GetPosition(canvas);

            // 获取鼠标滚轮滚动的增量
            int delta = e.Delta;

            // 定义缩放比例的增量，可以根据实际需求进行调整
            double scaleIncrement = 0.1;

            // 获取当前 Canvas 的缩放变换
            ScaleTransform scaleTransform = canvas.RenderTransform as ScaleTransform;
            if (scaleTransform == null)
            {
                // 如果没有缩放变换，则创建一个新的缩放变换，并应用到 Canvas 上
                scaleTransform = new ScaleTransform(1.0, 1.0);
                canvas.RenderTransform = scaleTransform;
            }

            // 计算新的缩放比例
            double newScale = delta > 0 ? scaleTransform.ScaleX + scaleIncrement : scaleTransform.ScaleX - scaleIncrement;

            // 设置缩放比例的上下限，可以根据实际需求进行调整
            double minScale = 1;
            double maxScale = 3.0;
            newScale = Math.Max(minScale, Math.Min(maxScale, newScale));

            // 计算缩放前后画布左上角的偏移量
            double offsetXBeforeScale = Canvas.GetLeft(contentContainer);
            double offsetYBeforeScale = Canvas.GetTop(contentContainer);
            double offsetXAfterScale = offsetXBeforeScale * newScale;
            double offsetYAfterScale = offsetYBeforeScale * newScale;

            // 计算鼠标位置相对于画布左上角的偏移量
            double deltaX = mousePosition.X - offsetXBeforeScale;
            double deltaY = mousePosition.Y - offsetYBeforeScale;

            // 计算缩放后的位置偏移量
            double offsetXDelta = offsetXAfterScale - deltaX;
            double offsetYDelta = offsetYAfterScale - deltaY;

            // 应用新的缩放比例到 Canvas 上
            scaleTransform.ScaleX = newScale;
            scaleTransform.ScaleY = newScale;

            // 同时对内容容器应用相同的缩放变换
            contentContainer.LayoutTransform = new ScaleTransform(newScale, newScale);

            // 根据缩放后的位置调整画布，以保持鼠标位置不变
            Canvas.SetLeft(contentContainer, offsetXDelta);
            Canvas.SetTop(contentContainer, offsetYDelta);
        }

        // 当用户调整窗口大小时，重新绘制网格线等内容以适应新的大小
        private void GridCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGrid();
        }
    }

}


