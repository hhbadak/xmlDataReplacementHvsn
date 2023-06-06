using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace xmlDataReplacementHVSN
{
    public partial class XmlDataHvsn : Form
    {
        private Timer timer;

        #region URL

        private string mvsnXmlUrlVaryasyonsuzKarg10 = "https://karg10.com/disari-al/qff7cm9h/54";
        string mvsnXmlUrlVaryasyonluKarg10 = "https://karg10.com/disari-al/l2h3sdb3/55";
        string xmlCekUrl = "https://www.xmlcek.com/wp-content/uploads/woo-feed/custom/xml/xmllink.xml";
        string teknoTokUrl = "https://teknotok.com/wp-load.php?security_token=b3203d92b4e495eb&export_id=3&action=get_data";
        string xmlTedarikUrl = "https://www.xmltedarik.com/export/1/1488S4586M1488";
        #endregion

        #region PATH

        private string mvsnXmlPathVaryasyonsuzKarg10 = "mvsnVaryasyonsuzKarg10.xml";
        string mvsnXmlPathVaryasyonluKarg10 = "mvsnVaryasyonluKarg10.xml";
        string xmlCekPath = "xmlCek.xml";
        string teknoTokPath = "teknoTok.xml";
        string xmlTedarikPath = "xmlTedarik.xml";
        #endregion

        public XmlDataHvsn()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 45 * 60 * 1000;
            timer.Tick += Timer_Tick;

            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Xml verisi indirme ve kaydetme işlemi
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(mvsnXmlUrlVaryasyonsuzKarg10, mvsnXmlPathVaryasyonsuzKarg10);
                    client.DownloadFile(mvsnXmlUrlVaryasyonluKarg10, mvsnXmlPathVaryasyonluKarg10);
                    client.DownloadFile(xmlCekUrl, xmlCekPath);
                    client.DownloadFile(teknoTokUrl, teknoTokPath);
                    client.DownloadFile(xmlTedarikUrl, xmlTedarikPath);
                }

                // Xml verisini okuma, değiştirme ve kaydetme işlemi

                #region mvsnVaryasyonsuz

                mvsnVaryasyonsuz(mvsnXmlUrlVaryasyonsuzKarg10, mvsnXmlPathVaryasyonsuzKarg10);

                #endregion

                #region mvsnVaryasyonlu

                mvsnVaryasyonlu(mvsnXmlUrlVaryasyonluKarg10, mvsnXmlPathVaryasyonluKarg10);

                #endregion

                #region xmlCek

                xmlCek(xmlCekUrl, xmlCekPath);

                #endregion

                #region Teknotok

                teknoTok(teknoTokUrl, teknoTokPath);
                #endregion

                #region xmlTedarik
                xmlTedarik(xmlTedarikUrl, xmlTedarikPath);
                #endregion

                string ftpUrl = "ftp://ftp.sygstore.com.tr/public_html/XML/";
                string ftpUsername = "u1292730";
                string ftpPassword = "15963VeksiS-";

                // FTP Klasörüne yükleme işlemi

                #region FTP
               
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile(ftpUrl + mvsnXmlPathVaryasyonsuzKarg10, WebRequestMethods.Ftp.UploadFile, mvsnXmlPathVaryasyonsuzKarg10);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile(ftpUrl + mvsnXmlPathVaryasyonluKarg10, WebRequestMethods.Ftp.UploadFile, mvsnXmlPathVaryasyonluKarg10);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile(ftpUrl + xmlCekPath, WebRequestMethods.Ftp.UploadFile, xmlCekPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile(ftpUrl + teknoTokPath, WebRequestMethods.Ftp.UploadFile, teknoTokPath);
                }
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile(ftpUrl + xmlTedarikPath, WebRequestMethods.Ftp.UploadFile, xmlTedarikPath);
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        public static void mvsnVaryasyonlu(string url, string savepath)
        {
            var doc = XDocument.Load(url);
            XElement root = doc.Root;

            foreach (XElement product in root.Elements())
            {
                product.Element("barcode").Value = "MVSN-" + product.Element("barcode").Value;
                product.Element("product_brand").Value = "MVSN-" + product.Element("product_brand").Value;
                foreach (XElement variant in product.Elements("variants").Elements())
                {
                    variant.Element("barcode").Value = "MVSN-" + variant.Element("barcode").Value;
                    variant.Element("product_brand").Value = "MVSN-" + variant.Element("product_brand").Value;
                }
            }

            doc.Save(savepath);
        }
        public static void mvsnVaryasyonsuz(string url, string savepath)
        {
            var doc = XDocument.Load(url);
            XElement root = doc.Root;

            foreach (XElement product in root.Elements())
            {
                product.Element("barcode").Value = "MVSN-" + product.Element("barcode").Value;
                product.Element("product_brand").Value = "MVSN-" + product.Element("product_brand").Value;
            }

            doc.Save(savepath);
        }
        public static void xmlCek(string url, string savepath)
        {
            var doc = XDocument.Load(url);
            XElement root = doc.Root;

            foreach (XElement product in root.Elements())
            {
                product.Element("barcode").Value = $"MVSN- {product.Element("Barcode").Value}";
                product.Element("Marka").Value = $"MVSN- {product.Element("Marka").Value}";
            }

            doc.Save(savepath);
        }
        public static void teknoTok(string url, string savepath)
        {
            var doc = XDocument.Load(url);
            XElement root = doc.Root;

            foreach (XElement product in root.Elements())
            {
                product.Element("Sku").Value = $"MVSN- {product.Element("Sku").Value}";
                product.Element("Markalar").Value = $"MVSN- {product.Element("Markalar").Value}";
            }
            doc.Save(savepath);
        }
        public static void xmlTedarik(string url, string savepath)
        {
            var doc = XDocument.Load(url);
            XElement root = doc.Root;

            foreach (XElement product in root.Elements())
            {
                product.Element("barcode").Value = $"MVSN- {product.Element("barcode").Value}";
                product.Element("brand").Value = $"MVSN- {product.Element("brand").Value}";
            }
            doc.Save(savepath);
        }
    }
}
