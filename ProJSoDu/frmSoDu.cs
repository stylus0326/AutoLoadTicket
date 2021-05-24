using DataAccessLayer;
using DataTransferObject;
using Microsoft.Win32;
using mshtml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ProJSoDu
{
    public partial class frmSoDu : Form
    {
        public frmSoDu()
        {
            InitializeComponent();
        }

        private void wVJS1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wVJS1.ReadyState == WebBrowserReadyState.Complete && !wVJS1.IsBusy)
            {
                if (hb.ID == 1 || hb.ID == 9 || hb.ID == 16)
                {
                    if (wVJS1.Url.AbsolutePath.Contains("/sitelogin.aspx")) // Trang login
                    {
                        HtmlElement head = wVJS1.Document.GetElementById("wrapper");
                        if (head != null)
                            if (head.InnerText.ToLower().Contains("mật khẩu chưa đúng") || head.InnerText.ToLower().Contains("wrong password input"))
                                this.Close();

                        wVJS1.Document.GetElementById("txtAgentID").SetAttribute("value", hb.TaiKhoan);
                        wVJS1.Document.GetElementById("txtAgentPswd").SetAttribute("value", hb.MatKhau);
                        wVJS1.Document.GetElementById("SiteLogin").InvokeMember("submit");
                    }
                    else
                    {
                        if (wVJS1.Url.AbsolutePath == "/AgentOptions.aspx")
                        {
                            if (hb.ID == 1)
                                wVJS1.Navigate("https://agents.vietjetair.com/SubAgency.aspx?lang=vi&st=sl&sesid=");
                            else
                            {
                                new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_Hang set SoDuThucTe = {0} where NCCID = {1} and convert(date,Ngay) = convert(date,GETDATE())", long.Parse(new String(wVJS1.Document.GetElementById("AgencyCreditAvailable").InnerText.Where(Char.IsDigit).ToArray())) / 100, hb.ID));

                                if (hb.ID == 16)
                                {
                                    hb = listhb.Where(w => w.ID.Equals(5)).First();
                                    wVJS1.Navigate("https://www.bambooairways.com/reservation/ibe/login");
                                }
                                else
                                {
                                    hb = listhb.Where(w => w.ID.Equals(16)).First();
                                    wVJS1.Navigate("https://www.vietjetair.com/Sites/Web/vi-VN/Home");
                                }
                            }
                        }
                        else
                        {
                            if (wVJS1.Url.AbsolutePath == "/SubAgency.aspx")
                            {
                                HtmlElement head = wVJS1.Document.GetElementById("SubAgency");
                                string LayTien0 = head.InnerText;
                                string LayTien1 = LayTien0.Replace("Tổng tín dụng các đại lý hiện có:", "|");
                                string LayTien2 = LayTien1.Split('|')[1];
                                string LayTien3 = LayTien2.Replace("VND", "|");
                                string LayTien4 = LayTien3.Split('|')[0];
                                try
                                {
                                    new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_Hang set SoDuThucTe = {0} where NCCID = 1 and convert(date,Ngay) = convert(date,GETDATE())", long.Parse(new String(LayTien4.Where(Char.IsDigit).ToArray()))));
                                }
                                catch
                                {
                                    new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_Hang set Error = N'{0}' where NCCID = 1 and convert(date,Ngay) = convert(date,GETDATE())", head.InnerText));
                                }
                                finally
                                {
                                    hb = listhb.Where(w => w.ID.Equals(9)).First();
                                    wVJS1.Navigate("https://www.vietjetair.com/Sites/Web/vi-VN/Home");
                                }
                            }
                            else
                            {
                                if (wVJS1.Url.AbsolutePath.Contains("/vi"))
                                {
                                    head = wVJS1.Document.GetElementsByTagName("head")[0];
                                    scriptEl = wVJS1.Document.CreateElement("script");
                                    element = (IHTMLScriptElement)scriptEl.DomElement;
                                    element.text = "function doPost() { location.href = 'https://agents.vietjetair.com/sitelogin.aspx?lang=vi'; }";
                                    head.AppendChild(scriptEl);
                                    wVJS1.Document.InvokeScript("doPost");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Text = wVJS1.Url.ToString();
                    HtmlElement head1 = wVJS1.Document.GetElementsByTagName("head")[0];
                    HtmlElement scriptEl = wVJS1.Document.CreateElement("script");
                    IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;

                    if (wVJS1.Url.AbsolutePath.Contains("/login")) // Trang login
                    {

                        wVJS1.Document.GetElementById("login-agency-code").SetAttribute("value", "3780054");
                        wVJS1.Document.GetElementById("login-agency-id").SetAttribute("value", hb.TaiKhoan);
                        wVJS1.Document.GetElementById("login-password").SetAttribute("value", hb.MatKhau);

                        element.text = @"function doPost() {submitLoginForm('en_US');}";
                        head1.AppendChild(scriptEl);
                        wVJS1.Document.InvokeScript("doPost");
                    }
                    else if (wVJS1.Url.AbsolutePath.Contains("/agent"))
                    {
                        string head = wVJS1.Document.GetElementsByTagName("strong")[16].InnerText;
                        if (head.Contains("VND"))
                        {
                            string LayTien1 = head.Replace("VND", "");
                            string LayTien2 = LayTien1.Replace(",", "");
                            new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_HANG set SoDuThucTe = {0} where NCCID = 5 and convert(date,Ngay) = convert(date,GETDATE())", LayTien2));
                        }
                        VU();
                        new GiaoDichD().ChaySD();
                        Close();
                    }
                }
            }
        }

        HtmlElement head;
        HtmlElement scriptEl;
        IHTMLScriptElement element;
        List<NCCO> listhb = new List<NCCO>();
        NCCO hb = new NCCO();

        void VU()
        {
            ChromeDriver driver;
            IJavaScriptExecutor js;
            WebDriverWait wait;

            NCCO _NCCO = new NCCD().DuLieu().Where(w => w.ID.Equals(21)).ToList()[0];

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            var options = new ChromeOptions();
            //chromeDriverService.HideCommandPromptWindow = true;

            try { driver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(300)); }
            catch { options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; driver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(300)); }

            js = driver as IJavaScriptExecutor;
            wait = new WebDriverWait(driver, TimeSpan.FromMinutes(5));


            driver.Navigate().GoToUrl("https://booking.vietravelairlines.vn/vi/ta");
            wait.Until(ExpectedConditions.ElementExists(By.Id("home-ta-login-button")));
            new Actions(driver).SendKeys(driver.FindElement(By.Id("home-ta-login-username")), _NCCO.TaiKhoan + OpenQA.Selenium.Keys.Tab).SendKeys(_NCCO.MatKhau + OpenQA.Selenium.Keys.Tab + OpenQA.Selenium.Keys.Enter).Build().Perform();
            wait.Until(ExpectedConditions.ElementExists(By.Id("criteria-search-button")));
            js.ExecuteScript("document.getElementsByClassName('icon material-icons arrow-icon')[0].click()");
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='app']/div[2]/header/div/div/div/div[2]/nav/ul/li[3]/div/div[2]/div[2]/div[4]/div/div")));
            long _aa = long.Parse(new String(driver.FindElement(By.XPath("//*[@id='app']/div[2]/header/div/div/div/div[2]/nav/ul/li[3]/div/div[2]/div[2]/div[4]/div/div")).Text.Where(Char.IsDigit).ToArray()));
            new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_Hang set SoDuThucTe = {0} where NCCID = {1} and convert(date,Ngay) = convert(date,GETDATE())", _aa, _NCCO.ID));

            driver.Close();
            driver.Quit();
            Close();
        }

        private void frmSoDu_Load(object sender, EventArgs e)
        {
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true);
            RegKey.SetValue("Display Inline Images", "no");
            wVJS1.Navigate("https://www.vietjetair.com/Sites/Web/vi-VN/Home");
            while (wVJS1.IsBusy)
            {
                Application.DoEvents();
            }
            listhb = new NCCD().DuLieu();
            hb = listhb.Where(w => w.ID.Equals(1)).First();
        }

        private void frmSoDu_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true);
            RegKey.SetValue("Display Inline Images", "yes");
        }
    }
}
