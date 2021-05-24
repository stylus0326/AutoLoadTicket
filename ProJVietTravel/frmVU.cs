using DataAccessLayer;
using DataTransferObject;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ProJVietTravel
{
    public partial class frmVU : Form
    {
        GiaoDichD gdD = new GiaoDichD();
        List<SignInO> lstSignIn = new List<SignInO>();
        List<DaiLyO> lstDaiLy = new List<DaiLyO>();
        DateTime NgayChay = new DateTime();
        DaiLyO dl = new DaiLyO();
        ChromeDriver driver;
        IJavaScriptExecutor js;
        WebDriverWait wait;
        TuyenBayD Tb = new TuyenBayD();
        SanBayD Sb = new SanBayD();

        public frmVU(DateTime dtp)
        {
            InitializeComponent();
            NgayChay = dtp;
        }


        private void frmVU_Load(object sender, EventArgs e)
        {
            lstSignIn = new SignInD().DuLieu();
            lstDaiLy = new DaiLyD().All();
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
            //new DataProcess().ChayCauTruyVan(string.Format(@"update SODU_Hang set SoDuThucTe = {0} where NCCID = {1} and convert(date,Ngay) = convert(date,GETDATE())", _aa, _NCCO.ID));

            driver.Navigate().GoToUrl("https://booking.vietravelairlines.vn/vi/ta/reports"); 
            new SelectElement(driver.FindElement(By.Id("ta-report-type-dropdown"))).SelectByIndex(3);

            driver.FindElement(By.Id("ta-reports-datepicker-from")).Click();
            IList<IWebElement> webElements = driver.FindElements(By.ClassName("asd__day-button")).Where(w => w.GetAttribute("date").Equals(NgayChay.ToString("yyyy-MM-dd"))).ToList();
            webElements[0].Click();
            driver.FindElement(By.Id("ta-reports-datepicker-to")).Click();
            if (NgayChay.Month < DateTime.Now.Month)
                driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div/div[4]/div/div[2]/div[1]/div[1]/button")).Click();
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(getAbsoluteXPath(webElements[1]))));
            webElements[1].Click();
            Thread.Sleep(1000);
            wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div/div[6]/button")));
            driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div/div[6]/button")).Click();

            wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div[2]/table/tbody")));
            IList<IWebElement> lstTR = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div[2]/table/tbody")).FindElements(By.TagName("tr"));

            DataTable dt = new DataTable();
            IList<IWebElement> lstTH = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div/div[2]/table/thead/tr")).FindElements(By.TagName("th"));
            foreach (IWebElement iw in lstTH)
            {
                dt.Columns.Add(iw.Text, typeof(string));
            }

            foreach (IWebElement iw in lstTR)
            {
                List<string> _Lis = iw.FindElements(By.TagName("td")).Select(w => w.Text).ToList();
                if (_Lis[0] == "-")
                    continue;
                dt.Rows.Add(_Lis.ToArray());
            }

            List<string> GD = new List<string>();
            List<string> macho = gdD.MaCho(dt).Distinct().ToList();
            GD = gdD.VeThuong(dt).Select(w => w.MaCho).Distinct().ToList(); ;// lấy dữ liệu đã tồn tại

            List<string> VeDaThem = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                if (GD.Contains(dr["ConfirmationNum"].ToString()))
                    continue;
                if (VeDaThem.Contains(dr["ConfirmationNum"].ToString()))
                    continue;
                if (dr["Bookedamount"].ToString() == "0")
                    continue;
                VeDaThem.Add(dr["ConfirmationNum"].ToString());
                driver.Navigate().GoToUrl($"https://booking.vietravelairlines.vn/vi/manage?confirmationNumber={dr["ConfirmationNum"].ToString()}&bookingLastName={dr["LastName"].ToString()}");
                //driver.Navigate().GoToUrl($"https://booking.vietravelairlines.vn/vi/manage?confirmationNumber=DZ6LR9&bookingLastName=TRAN");
                wait.Until(ExpectedConditions.ElementExists(By.ClassName("mmb-section")));
                wait.Until(d => d.FindElements(By.XPath("/html/body/div[1]/div[2]/main/div[1]/div/div/div[2]/div[2]/div[1]")).Count > 0);
                if (driver.PageSource.Contains("Đặt chỗ này đã được hủy."))
                    continue;
                IList<IWebElement> ThongTinKhach = driver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div[1]/div/div/div[2]/div[2]/div[1]")).FindElements(By.ClassName("flight"));

                List<GiaoDichO> lstgd = new List<GiaoDichO>();
                GiaoDichO gd = new GiaoDichO();
                gd.Hang = "VU";
                gd.NhaCungCap = 21;
                gd.MaCho = dr["ConfirmationNum"].ToString();
                gd.NgayGD = gd.NgayCuonChieu = DateTime.ParseExact(dr["TransactionDate"].ToString(), "dd MMM yyyy", new System.Globalization.CultureInfo("en-US"));
                gd.Agent = dr["UserId"].ToString();

                if (macho.Contains(gd.MaCho))
                    gd.LoaiGiaoDich = 13;
                else
                    gd.LoaiGiaoDich = 4;

                List<int> vs1 = lstSignIn.Where(w => w.SignIn.ToUpper().Equals(gd.Agent.ToUpper())).Select(w => w.DaiLy).ToList();
                if (vs1.Count > 0)
                {
                    dl = lstDaiLy.First(w => w.ID.Equals(vs1[0]));
                    gd.VeTuXuat = true;
                    if (dl.LoaiKhachHang == 0)
                    {
                        gd.NVGiaoDich = dl.ID;
                        gd.VeTuXuat = false;
                    }
                    gd.IDKhachHang = dl.ID;
                    gd.LoaiKhachHang = dl.LoaiKhachHang;
                }

                for (int u = 0; u < ThongTinKhach.Count; u++)
                {
                a1:
                    string[] a = ThongTinKhach[u].Text.Replace("\r\n", "|").Split('|');
                    string[] b = a[0].Split(' ');
                    if (b.Count() < 4)
                        goto a1;
                    if (u == 0)
                    {
                        gd.GioBayDi = new DateTime(int.Parse(b[5]), int.Parse(b[4]), int.Parse(b[2]), int.Parse(a[3].Split(':')[0]), int.Parse(a[3].Split(':')[1]), 0);
                        gd.GioBayDi_Den = new DateTime(int.Parse(b[5]), int.Parse(b[4]), int.Parse(b[2]), int.Parse(a[5].Split(':')[0]), int.Parse(a[5].Split(':')[1]), 0);
                        gd.SoHieuDi = a[1];
                        gd.TuyenBayDi = Tb.TuyenBay(Sb.SanBay(a[2]).ID, Sb.SanBay(a[4]).ID).ID;
                        gd.LoaiVeDi = "Khác";
                        gd.SoLuongVe = 1;
                    }
                    else
                    {
                        gd.GioBayVe = new DateTime(int.Parse(b[5]), int.Parse(b[4]), int.Parse(b[2]), int.Parse(a[3].Split(':')[0]), int.Parse(a[3].Split(':')[1]), 0);
                        gd.GioBayVe_Den = new DateTime(int.Parse(b[5]), int.Parse(b[4]), int.Parse(b[2]), int.Parse(a[5].Split(':')[0]), int.Parse(a[5].Split(':')[1]), 0);
                        gd.SoHieuVe = a[1];
                        gd.TuyenBayVe = Tb.TuyenBay(Sb.SanBay(a[2]).ID, Sb.SanBay(a[4]).ID).ID;
                        gd.LoaiVeVe = "Khác";
                        gd.SoLuongVe = 2;
                    }
                }

                js.ExecuteScript("document.getElementsByClassName('button normal')[document.getElementsByClassName('button normal').length-2].click();");

                wait.Until(d => d.WindowHandles.Count == 3);
                Thread.Sleep(1000);
                driver.SwitchTo().Window(driver.WindowHandles[1]);
                try
                {
                    js.ExecuteScript("document.querySelector('print-preview-app').shadowRoot.querySelector('print-preview-sidebar').shadowRoot.querySelector('print-preview-button-strip').shadowRoot.querySelectorAll('cr-button')[1].click()");
                }
                catch
                {
                    driver.SwitchTo().Window(driver.WindowHandles[2]);
                    js.ExecuteScript("document.querySelector('print-preview-app').shadowRoot.querySelector('print-preview-sidebar').shadowRoot.querySelector('print-preview-button-strip').shadowRoot.querySelectorAll('cr-button')[1].click()");
                }
                wait.Until(d => d.WindowHandles.Count == 2);
                driver.SwitchTo().Window(driver.WindowHandles[1]);

                #region hành lý
                int intS = 0;
                if (driver.PageSource.Contains("Đặt chỗ của Quý khách sẽ được giữ đến"))
                    intS = 4;

                ThongTinKhach = driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody/tr[" + (6 + intS) + "]/td/table/tbody")).FindElements(By.TagName("tr"));
                for (int u = 1; u < ThongTinKhach.Count; u++)
                {
                    GiaoDichO dichO = new GiaoDichO(gd);
                    IList<IWebElement> elements = ThongTinKhach[u].FindElements(By.TagName("td"));
                    dichO.Fare = long.Parse(new String(elements[3].Text.Where(Char.IsDigit).ToArray())) / 100;
                    dichO.GiaNet = dichO.GiaHeThong = dichO.GiaThu = dichO.Fare + (dichO.Fare / 10);
                    string[] _Name = elements[0].Text.Replace("\r\n", "|").Split('|');
                    dichO.BiDanh = _Name[0].Split(' ')[0];
                    dichO.TenKhach = _Name[0].Replace(dichO.BiDanh + " ", "") + "-HL";
                    dichO.LoaiVeDi = _Name[2].Replace("(", "").Replace(")", "").Replace(" ", "");
                    dichO.LoaiGiaoDich = 14;

                    if (driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody")).FindElements(By.TagName("tbody")).Count == 15)
                    {
                        IList<IWebElement> ThongTinKhach2 = driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody/tr[" + (11 + intS) + "]/td/table/tbody")).FindElements(By.TagName("tr"));
                        for (int uu = 1; uu < ThongTinKhach2.Count; uu++)
                        {
                            elements = ThongTinKhach2[uu].FindElements(By.TagName("td"));
                            _Name = elements[0].Text.Replace("\r\n", "|").Split('|');
                            if (dichO.BiDanh + " " + dichO.TenKhach == _Name[0] + "-HL")
                            {
                                dichO.LoaiVeVe = _Name[2].Replace("(", "").Replace(")", "").Replace(" ", "");
                                dichO.GiaNet += dichO.Fare + (dichO.Fare / 10);
                                dichO.GiaHeThong = dichO.GiaThu = dichO.GiaNet;
                                dichO.Fare = long.Parse(new String(elements[3].Text.Where(Char.IsDigit).ToArray())) / 100;
                            }

                        }
                    }

                    if (dichO.Fare > 0)
                        lstgd.Add(dichO);
                }
                #endregion

                ThongTinKhach = driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody/tr[" + (4 + intS) + "]/td/table/tbody")).FindElements(By.TagName("tr"));

                for (int u = 1; u < ThongTinKhach.Count; u++)
                {
                    GiaoDichO dichO = new GiaoDichO(gd);
                    IList<IWebElement> elements = ThongTinKhach[u].FindElements(By.TagName("td"));
                    dichO.GiaNet = dichO.GiaHeThong = dichO.GiaThu = long.Parse(new String(elements[2].Text.Where(Char.IsDigit).ToArray())) / 100;
                    string[] _Name = elements[0].Text.Replace("\r\n", "|").Split('|');
                    dichO.BiDanh = _Name[0].Split(' ')[0];
                    dichO.TenKhach = _Name[0].Replace(dichO.BiDanh + " ", "");
                    dichO.LoaiVeDi = _Name[4].Replace("Loại vé: ", "").Replace(" FARE CLASS", "").Replace(")", "").TrimStart();
                    _Name = elements[1].Text.Replace("\r\n", "|").Split('|');
                    dichO.Fare = long.Parse(new String(_Name[0].Where(Char.IsDigit).ToArray())) / 100;

                    if (driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody")).FindElements(By.TagName("tbody")).Count == 15)
                    {
                        IList<IWebElement> ThongTinKhach2 = driver.FindElement(By.XPath("/html/body/center/table/tbody/tr/td/table/tbody/tr[" + (9 + intS) + "]/td/table/tbody")).FindElements(By.TagName("tr"));
                        for (int uu = 1; uu < ThongTinKhach2.Count; uu++)
                        {
                            elements = ThongTinKhach2[uu].FindElements(By.TagName("td"));
                            _Name = elements[0].Text.Replace("\r\n", "|").Split('|');
                            if (dichO.BiDanh + " " + dichO.TenKhach == _Name[0])
                            {
                                dichO.LoaiVeVe = _Name[4].Replace("Loại vé: ", "").Replace(" FARE CLASS", "").Replace(")", "").Replace(" ", "");
                                dichO.GiaNet += long.Parse(new String(elements[2].Text.Where(Char.IsDigit).ToArray())) / 100;
                                dichO.GiaHeThong = dichO.GiaThu = dichO.GiaNet;
                                _Name = elements[1].Text.Replace("\r\n", "|").Split('|');
                                dichO.Fare += long.Parse(new String(_Name[0].Where(Char.IsDigit).ToArray())) / 100;
                            }

                        }
                    }

                    if (dichO.Fare < 10000)
                        dichO.Fare = 0;
                    lstgd.Add(dichO);
                }



                gdD.Them(lstgd);
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[0]);
            }
        }

        IWebElement ChromeFindElementByClassName(string _TagName, string _ClassName, int _ViTri = 0)
        {
            IList<IWebElement> lst = driver.FindElements(By.TagName(_TagName));
            int _viTri = 0;
            foreach (IWebElement ele in lst)
            {
                if (ele.GetAttribute("className") == _ClassName)
                {
                    if (_viTri == _ViTri)
                        return ele;
                    _viTri++;
                }
            }
            return null;
        }

        public String getAbsoluteXPath(IWebElement element)
        {
            return (String)js.ExecuteScript(
                    "function absoluteXPath(element) {" +
                            "var comp, comps = [];" +
                            "var parent = null;" +
                            "var xpath = '';" +
                            "var getPos = function(element) {" +
                            "var position = 1, curNode;" +
                            "if (element.nodeType == Node.ATTRIBUTE_NODE) {" +
                            "return null;" +
                            "}" +
                            "for (curNode = element.previousSibling; curNode; curNode = curNode.previousSibling) {" +
                            "if (curNode.nodeName == element.nodeName) {" +
                            "++position;" +
                            "}" +
                            "}" +
                            "return position;" +
                            "};" +

                            "if (element instanceof Document) {" +
                            "return '/';" +
                            "}" +

                            "for (; element && !(element instanceof Document); element = element.nodeType == Node.ATTRIBUTE_NODE ? element.ownerElement : element.parentNode) {" +
                            "comp = comps[comps.length] = {};" +
                            "switch (element.nodeType) {" +
                            "case Node.TEXT_NODE:" +
                            "comp.name = 'text()';" +
                            "break;" +
                            "case Node.ATTRIBUTE_NODE:" +
                            "comp.name = '@' + element.nodeName;" +
                            "break;" +
                            "case Node.PROCESSING_INSTRUCTION_NODE:" +
                            "comp.name = 'processing-instruction()';" +
                            "break;" +
                            "case Node.COMMENT_NODE:" +
                            "comp.name = 'comment()';" +
                            "break;" +
                            "case Node.ELEMENT_NODE:" +
                            "comp.name = element.nodeName;" +
                            "break;" +
                            "}" +
                            "comp.position = getPos(element);" +
                            "}" +

                            "for (var i = comps.length - 1; i >= 0; i--) {" +
                            "comp = comps[i];" +
                            "xpath += '/' + comp.name.toLowerCase();" +
                            "if (comp.position !== null) {" +
                            "xpath += '[' + comp.position + ']';" +
                            "}" +
                            "}" +

                            "return xpath;" +

                            "} return absoluteXPath(arguments[0]);", element);
        }
    }
}
