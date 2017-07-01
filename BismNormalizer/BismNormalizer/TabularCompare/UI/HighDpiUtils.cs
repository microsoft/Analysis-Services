using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    /// <summary>
    /// Utilities class for handling high DPI scenarios
    /// </summary>
    public class HighDPIUtils
    {
        // Since the move to .Net4.5.2 we should use this const to scale context menus or they will be scaled twice
        public static Size ContextMenuDefaultImageScalingSize = new Size(16, 16);

        public static void ScaleByDpi(ref int height, ref int width)
        {
            float dpiFactor = GetDpiFactor();
            height = (int)(height * dpiFactor);
            width = (int)(width * dpiFactor);
        }

        public static int ScaleByDpi(int size)
        {
            float dpiFactor = GetDpiFactor();
            return (int)(size * dpiFactor);
        }

        public static Size ScaleByDpi(Size size)
        {
            float dpiFactorFudged = GetDpiFactor() * HighDPIUtils.PrimaryFudgeFactor;
            return new Size((int)(size.Width * dpiFactorFudged), (int)(size.Height * dpiFactorFudged));
        }

        /// <summary>
        /// Scales a Size object based on a given factor
        /// </summary>
        /// <param name="size">Size to scale</param>
        /// <param name="factor">Factor to scale with</param>
        /// <returns>Scaked Size object</returns>
        public static Size ScaleByFactor(Size size, float factor)
        {
            return new Size(
                (int)(size.Width * factor),
                (int)(size.Height * factor));
        }

        /// <summary>
        /// Scales an ImageList which is already populated with an ImageStream. Scaling occurs on the object iteself.
        /// </summary>
        /// <param name="imageList">Target ImageList</param>
        public static void ScaleStreamedImageListByDpi(ImageList imageList)
        {
            // Need to store the images before setting ImageSize as this completely wipes the Images collection
            Dictionary<string, Image> originalImages = new Dictionary<string, Image>();
            for (int i = 0; i < imageList.Images.Count; i++)
            {
                originalImages.Add(imageList.Images.Keys[i], imageList.Images[i]);
            }

            // According to MSDN, setting a new size resets the Images collection
            imageList.ImageSize = HighDPIUtils.ScaleByDpi(imageList.ImageSize);
            foreach (KeyValuePair<string, Image> entry in originalImages)
            {
                imageList.Images.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Check whether a string with a preferred font fits a rectangle. If if does, return the preferred font. If it doesn't, scale down to fit.
        /// </summary>
        /// <param name="g">Graphics control used to draw</param>
        /// <param name="str">String to draw</param>
        /// <param name="rectangle">Rectangle to draw the string on</param>
        /// <param name="preferedFont">Preferred font to draw</param>
        /// <param name="minFontSize">Minumum font size to be used if scaling is required</param>
        /// <returns></returns>
        public static Font ScaleFontToFit(System.Drawing.Graphics g, string str, RectangleF rectangle, Font preferredFont, float minFontSize)
        {
            SizeF actualSize = g.MeasureString(str, preferredFont);
            if (actualSize.Width <= rectangle.Width)
            {
                return preferredFont;
            }

            float scaleRatio = rectangle.Width / actualSize.Width;
            float scaledFontSize = preferredFont.Size * scaleRatio;
            scaledFontSize = Math.Max(scaledFontSize, minFontSize);
            return new Font(preferredFont.FontFamily, (float)Math.Floor(scaledFontSize));
        }

        public static float PrimaryFudgeFactor = 0.72f; // 0.68f; // 0.54f;
        public static float SecondaryFudgeFactor = 1.6f; //1.2f;

        public static float GetDpiFactor()
        {
            float factor = 1.0F;
            try
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    factor = (float)graphics.DpiX / 96F; //96 is the default windows DPI
                    if (factor > 1.3 && factor < 1.7)
                    {
                        HighDPIUtils.PrimaryFudgeFactor = 0.66f;
                        HighDPIUtils.SecondaryFudgeFactor = 1.1f;
                    }
                    else if (factor >= 1.7)
                    {
                        if (!Settings.Default.OptionHighDpiLocal)
                        {
                            HighDPIUtils.PrimaryFudgeFactor = 0.72f;
                            HighDPIUtils.SecondaryFudgeFactor = 1.6f;
                        }
                        else
                        {
                            HighDPIUtils.PrimaryFudgeFactor = 0.54f;
                            HighDPIUtils.SecondaryFudgeFactor = 1.2f;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Swallow any exceptions related to system DPI and use the default
            }

            return factor;
        }

        public static float GetDpiWindowMonitor()
        {
            float dpi = 0;

            try
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpi = (float)graphics.DpiX;
                }
            }
            catch (Exception)
            {
                // Swallow any exceptions related to system DPI and use the default
            }

            return dpi;
        }

        public static Bitmap ScaleByDpi(Bitmap bmp)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            ScaleByDpi(ref height, ref width);

            if (height == bmp.Height && width == bmp.Width)
            {
                // No scaling is done
                return bmp;
            }

            return new Bitmap(bmp, new Size(width, height));
        }

        /// <summary>
        /// Scales an Image object if its of type Bitmap. Otherwise doesn't scale.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>Scaled Image</returns>
        public static Image ScaleByDpi(Image bmp)
        {
            if (bmp is Bitmap)
            {
                return ScaleByDpi((Bitmap)bmp);
            }

            // We can't scale if we don't know the type of Image sent
            return bmp;
        }

        // Get child Controls in a specified Control.
        internal static List<Control> GetChildInControl(Control parent)
        {
            List<Control> controlList = new List<Control>();

            foreach (Control child in parent.Controls)
            {
                controlList.Add(child);
                controlList.AddRange(GetChildInControl(child));
            }

            return controlList;
        }
    }

}

