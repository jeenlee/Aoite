using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace System
{
    /// <summary>
    /// 表示图像验证码的生成器。
    /// </summary>
    public static class ValidCodeGenerator
    {
        /// <summary>
        /// 生成指定宽、高、验证码的 GIF 格式图片。
        /// </summary>
        /// <param name="code">验证码。</param>
        /// <param name="width">图像的宽度，小于 1 则自动计算。</param>
        /// <param name="height">图像的高度，小于 1 则自动计算。默认为 22。</param>
        /// <param name="familyName">字体名称。</param>
        /// <returns>返回一个图像对象。</returns>
        public static Image Create(string code, int width = 0, int height = 0, string familyName = "Arial")
        {
            if(string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if(string.IsNullOrEmpty(familyName)) throw new ArgumentNullException("familyName");

            Bitmap image = new Bitmap(width < 1 ? (int)(code.Length * 21.5) : width, height < 1 ? 22 : height);
            using(Graphics g = Graphics.FromImage(image))
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.White);

                // 画图片的背景噪音线
                for(int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new System.Drawing.Font(familyName, 12, (FontStyle.Bold));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2F, true);
                g.DrawString(code, font, brush, 2, 2);

                //画图片的前景噪音点
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                return image;
            }

        }

        /// <summary>
        /// 将指定的图片转换成内存流。
        /// </summary>
        /// <param name="image">转换的图像。</param>
        /// <param name="format">转换的图像格式。</param>
        /// <returns>返回一个内存流对象。</returns>
        public static MemoryStream ToStream(this Image image, ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            if(image != null)
            {
                image.Save(ms, format);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
            }
            return ms;
        }
    }

}