using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace iccms.Arrows
{
    /// <summary>
    /// 带文本和箭头的两点之间连线
    /// </summary>
    public class ArrowLineWithText : ArrowLine
    {
        #region Properties

        /// <summary>
        /// 文本的依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ArrowLineWithText),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 文本对齐的依赖属性
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment", typeof(TextAlignment), typeof(ArrowLineWithText),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// 文本朝上的依赖属性
        /// </summary>
        public static readonly DependencyProperty IsTextUpProperty = DependencyProperty.Register(
            "IsTextUp", typeof(bool), typeof(ArrowLineWithText),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 文本是否朝上
        /// </summary>
        public bool IsTextUp
        {
            get { return (bool)GetValue(IsTextUpProperty); }
            set { SetValue(IsTextUpProperty, value); }
        }

        /// <summary>
        /// 是否显示文本的依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty = DependencyProperty.Register(
            "ShowText", typeof(bool), typeof(ArrowLineWithText), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public bool ShowText
        {
            get { return (bool)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// 重载渲染事件
        /// </summary>
        /// <param name="drawingContext">绘图上下文</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ShowText && (Text != null))
            {
                var txt = Text.Trim();
                var startPoint = StartPoint;
                if (!string.IsNullOrEmpty(txt))
                {
                    var vec = EndPoint - StartPoint;
                    var angle = GetAngle(StartPoint, EndPoint);

                    //使用旋转变换,使其与线平等
                    var transform = new RotateTransform(angle) { CenterX = StartPoint.X, CenterY = StartPoint.Y };
                    drawingContext.PushTransform(transform);

                    var defaultTypeface = new Typeface(SystemFonts.StatusFontFamily, SystemFonts.StatusFontStyle,
                        SystemFonts.StatusFontWeight, new FontStretch());
                    var formattedText = new FormattedText(txt, CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        defaultTypeface, SystemFonts.StatusFontSize, Brushes.Black)
                    {
                        //文本最大宽度为线的宽度
                        MaxTextWidth = vec.Length,
                        //设置文本对齐方式
                        TextAlignment = TextAlignment
                    };

                    var offsetY = StrokeThickness;
                    if (IsTextUp)
                    {
                        //计算文本的行数
                        double textLineCount = formattedText.Width / formattedText.MaxTextWidth;
                        if (textLineCount < 1)
                        {
                            //怎么也得有一行
                            textLineCount = 1;
                        }
                        //计算朝上的偏移
                        offsetY = -formattedText.Height * textLineCount - StrokeThickness;
                    }
                    startPoint = startPoint + new Vector(0, offsetY);
                    drawingContext.DrawText(formattedText, startPoint);
                    drawingContext.Pop();
                }
            }
        }

        #endregion Overrides

        #region Private Methods

        /// <summary>
        /// 获取两个点的倾角
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>两个点的倾角</returns>
        private double GetAngle(Point start, Point end)
        {
            var vec = end - start;
            //X轴
            var xAxis = new Vector(1, 0);
            return Vector.AngleBetween(xAxis, vec);
        }

        #endregion Private Methods

    }
}