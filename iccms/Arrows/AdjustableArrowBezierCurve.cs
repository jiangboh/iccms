using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace iccms.Arrows
{
    /// <summary>
    /// 可调整的贝塞尔曲线
    /// </summary>
    public class AdjustableArrowBezierCurve : ArrowBezierCurve
    {
        #region Fields

        /// <summary>
        /// 连线画笔
        /// </summary>
        private readonly Pen _linePen = new Pen(Brushes.Black, 1);

        /// <summary>
        /// 控制点画刷
        /// </summary>
        private readonly Brush _ellipseBrush = Brushes.Black;

        /// <summary>
        /// 控制点椭圆画笔
        /// </summary>
        private readonly Pen _ellipsePen = new Pen(Brushes.Black, 1);

        /// <summary>
        /// 控制点椭圆半径
        /// </summary>
        private const double EllipseRadius = 5;

        /// <summary>
        /// 是否按下控制点1
        /// </summary>
        private bool _isPressedControlPoint1;

        /// <summary>
        /// 是否按下控制点2
        /// </summary>
        private bool _isPressedControlPoint2;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 是否显示控制的依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowControlProperty = DependencyProperty.Register(
            "ShowControl", typeof(bool), typeof(AdjustableArrowBezierCurve),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 是否显示控制
        /// </summary>
        public bool ShowControl
        {
            get { return (bool)GetValue(ShowControlProperty); }
            set { SetValue(ShowControlProperty, value); }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// 当未处理的 <see cref="E:System.Windows.Input.Mouse.MouseDown"/> 附加事件在其路由中到达派生自此类的元素时，调用该方法。实现此方法可为此事件添加类处理。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>。此事件数据报告有关按下的鼠标按钮和已处理状态的详细信息。
        ///                 </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if ((ShowControl) && (e.LeftButton == MouseButtonState.Pressed))
            {
                CaptureMouse();
                Point pt = e.GetPosition(this);
                Vector slide = pt - ControlPoint1;
                if (slide.Length < EllipseRadius)
                {
                    _isPressedControlPoint1 = true;
                }

                slide = pt - ControlPoint2;
                if (slide.Length < EllipseRadius)
                {
                    _isPressedControlPoint2 = true;
                }
            }
        }

        /// <summary>
        /// 当未处理的 <see cref="E:System.Windows.Input.Mouse.MouseUp"/> 路由事件在其路由中到达派生自此类的元素时，调用该方法。实现此方法可为此事件添加类处理。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>。事件数据将报告已释放了鼠标按钮。
        ///                 </param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouseCapture();
            _isPressedControlPoint1 = false;
            _isPressedControlPoint2 = false;
        }

        /// <summary>
        /// 当未处理的 <see cref="E:System.Windows.Input.Mouse.MouseMove"/> 附加事件在其路由中到达派生自此类的元素时，调用该方法。实现此方法可为此事件添加类处理。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="T:System.Windows.Input.MouseEventArgs"/>。
        ///                 </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ShowControl && (e.LeftButton == MouseButtonState.Pressed))
            {
                var currentPt = e.GetPosition(this);
                if (_isPressedControlPoint1)
                {
                    ControlPoint1 = currentPt;
                }

                if (_isPressedControlPoint2)
                {
                    ControlPoint2 = currentPt;
                }
            }
        }

        /// <summary>
        /// 在派生类中重写时，会参与由布局系统控制的呈现操作。调用此方法时，不直接使用此元素的呈现指令，而是将其保留供布局和绘制在以后异步使用。
        /// </summary>
        /// <param name="drawingContext">特定元素的绘制指令。此上下文是为布局系统提供的。
        ///                 </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ShowControl)
            {
                drawingContext.DrawLine(_linePen, StartPoint, ControlPoint1);
                drawingContext.DrawEllipse(_ellipseBrush, _ellipsePen, ControlPoint1, EllipseRadius, EllipseRadius);

                drawingContext.DrawLine(_linePen, EndPoint, ControlPoint2);
                drawingContext.DrawEllipse(_ellipseBrush, _ellipsePen, ControlPoint2, EllipseRadius, EllipseRadius);
            }
        }

        #endregion Overrides
    }
}