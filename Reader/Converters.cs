using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reader
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class Not : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool)) throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool)) throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value;
        }

        #endregion
    }

    public class HtmlToXaml : IValueConverter
    {
        /// <summary>
        /// device-independent units (1/96th inch per unit)
        /// </summary>
        private const double maxImgHeight = 200;
        private static Dictionary<string, TextBlock> cache = new Dictionary<string, TextBlock>();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if (cache.ContainsKey(value.ToString())) return cache[value.ToString()];

            txtBlock = new TextBlock();
            var xml = new XmlDocument();
            string html = $"<html>{Escape(value.ToString())}</html>";
            xml.LoadXml(html);
            ProcessNode(xml.DocumentElement);
            cache[value.ToString()] = txtBlock;
            return txtBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private TextBlock txtBlock;

        private void ProcessNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                var r = new Run(node.InnerText);
                txtBlock.Inlines.Add(r);
                return;
            }
            else if (node.Name.ToLower() == "p")
            {
                foreach (XmlNode item in node.ChildNodes)
                {
                    ProcessNode(item);
                }
                txtBlock.Inlines.Add(new LineBreak());
                return;
            }
            else if (node.Name.ToLower() == "img")
            {
                var image = new Image();
                string imageUrl = node.Attributes["src"].Value;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(string.Format(imageUrl), UriKind.Absolute);
                bitmap.CreateOptions = BitmapCreateOptions.None;
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.EndInit();

                bitmap.DownloadCompleted += new EventHandler(
                (object sender, EventArgs e) =>
                {
                    BitmapImage initBitmap = (BitmapImage)sender;
                    double adjust = maxImgHeight / initBitmap.Height;
                    if (adjust > 1) adjust = 1;
                    image.Height = adjust * initBitmap.Height;
                    image.Width = adjust * initBitmap.Width;
                    image.Source = initBitmap;
                    Debug.WriteLine($"Image dowloaded: {imageUrl}, height adjust: {initBitmap.Height} >> {image.Height}, width adjust: {initBitmap.Width} >> {image.Width}");
                });

                txtBlock.Inlines.Add(image);
                txtBlock.Inlines.Add(new LineBreak());
                Debug.WriteLine($"Image added: {imageUrl}");
            }
            else if (node.Name.ToLower() == "a")
            {
                var l = new Hyperlink(new Run(node.InnerText));
                txtBlock.Inlines.Add(l);
            }

            foreach (XmlNode item in node.ChildNodes)
            {
                ProcessNode(item);
            }

        }

        private string Escape(string html)
        {
            return html.Replace("&", "&amp;");
        }
    }
}
