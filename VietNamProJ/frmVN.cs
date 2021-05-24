using DataAccessLayer;
using DataTransferObject;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace VietNamProJ
{
    public partial class frmVN : Form
    {

        DateTime _dtp;
        bool AGS = false;
        public frmVN(DateTime dtp, bool ags = false)
        {
            InitializeComponent();
            _dtp = dtp;
            AGS = ags;
        }

        private void frmVN_Load(object sender, EventArgs e)
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("chromedriver"))
            { process.Kill(); }

            lstSignIn = new SignInD().DuLieu();
            _lstDaiLyALL = new DaiLyD().All();
            _lstDaiLy = _lstDaiLyALL.Where(w => w.LoaiKhachHang.Equals(1)).ToList();
            //options.AddUserProfilePreference("download.default_directory", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (AGS)
                ChayVN(true);
            else
                ChayVN(false);
        }

        GiaoDichD gd = new GiaoDichD();
        List<DaiLyO> _lstDaiLy = new List<DaiLyO>();
        List<DaiLyO> _lstDaiLyALL = new List<DaiLyO>();

        void ChayVN(bool HideVN)
        {
            WebDriverWait wait;
            IJavaScriptExecutor js;
            ChromeOptions options;
            ChromeDriverService chromeDriverService;
            options = new ChromeOptions();
            //options.AddExtension("CSS-Block_v1.0.0.crx");
            chromeDriverService = ChromeDriverService.CreateDefaultService();

            //chromeDriverService.HideCommandPromptWindow = true;
            ChromeDriver chromeDriver;
            try
            {
                chromeDriver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(10000));
            }
            catch
            {
                options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeDriver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(10000));
            }
            js = chromeDriver as IJavaScriptExecutor;
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromMinutes(5));
            if (!HideVN)
            {
                TuyenBayD Tb = new TuyenBayD();
                SanBayD Sb = new SanBayD();
                NCCO _NCCO = new NCCD().DuLieu().Where(w => w.ID.Equals(2)).ToList()[0];

                List<string> lstStr = new List<string>(new string[] { "MR", "MRS", "MS", "MISS", "MSTR" });
                List<string> LuuSoVeThuongVoid = new List<string>();
                List<string> LuuSoVeHoanVoid = new List<string>();
                List<string> SoVeThuong = new List<string>();
                List<string> SoVeHoan = new List<string>();
                List<string> SoVeThuongVoid = new List<string>();
                List<string> SoVeHoanVoid = new List<string>();
                List<string> LuuDuLieuVe1ban = new List<string>();

                int M = DateTime.Now.Month;
                int Y = DateTime.Now.Year;
                int D = DateTime.Now.Day;
                string URLS = string.Empty;

                chromeDriver.Navigate().GoToUrl("https://agency.Vietnamairlines.com/SSW2010/VNAG/agency.html");
                wait.Until(driver => !driver.PageSource.Contains("progressContainer"));
                wait.Until(driver => ExpectedConditions.ElementExists(By.Id("username")));
                Thread.Sleep(500);
                chromeDriver.FindElement(By.Id("username")).SendKeys(_NCCO.TaiKhoan);
                Thread.Sleep(500);
                chromeDriver.FindElement(By.Id("password")).SendKeys(_NCCO.MatKhau);
                Thread.Sleep(500);
                js.ExecuteScript(string.Format(@"var ID4 = document.getElementById('btn-login');ID4.click();"));
                wait.Until(driver => !driver.PageSource.Contains("progressContainer"));
                wait.Until(driver => ExpectedConditions.ElementExists(By.Id("loginComponentTraveBankViewDetailsLink")));

                Thread.Sleep(500);
                js.ExecuteScript(string.Format(@"var ID4 = document.getElementById('loginComponentTraveBankViewDetailsLink');ID4.click();"));

                wait.Until(driver => driver.WindowHandles.Count() == 2);
                chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles[1]);
                wait.Until(driver => driver.PageSource.Contains("transactionsHistory.html"));


                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                for (DateTime i = DateTime.Now; _dtp.Date.Subtract(i.Date).Days < 1; i = i.AddDays(-1))
                {
                    URLS = "https://agencymanagement.havail.sabre.com/ctva/user/agency/rest/monthly-statement/" + i.ToString("ddMMMyyyy") + "/";
                    URLS += i.ToString("ddMMMyyyy") + "/details.json?sEcho=3&iDisplayStart=0&iDisplayLength=5000";
                    chromeDriver.Navigate().GoToUrl(URLS);

                    string innerText = string.Empty;
                    innerText = chromeDriver.FindElement(By.TagName("body")).Text.Replace("\"", string.Empty).Replace("</a>", string.Empty);
                    innerText = innerText.Replace(@")\>", ",");
                    innerText = innerText.Replace(@"[738,<a href=\javascript:;\ onclick=\", string.Empty);
                    innerText = innerText.Replace(@"submit_link(", "|");
                    List<string> ListinnerText = innerText.Split('|').ToList();
                    ListinnerText.RemoveAt(0);
                    LuuDuLieuVe1ban.AddRange(ListinnerText.AsEnumerable().Reverse());
                }

                foreach (string za in LuuDuLieuVe1ban)//lấy từ dưới lên trên từ ngày gần nhất đến ngày xa nhất
                {
                    string[] G = za.Split(',');

                    if ((G[4] ?? string.Empty).Contains("VOID"))//số vé sau cùng là 1 giao dịch không phải void hay hoặc đã ref rồi
                    {
                        if (G[4].Contains("REF"))
                        {
                            LuuSoVeHoanVoid.Add("738" + G[1]);
                            SoVeHoanVoid.Add(za);
                        }
                        else
                        {
                            LuuSoVeThuongVoid.Add("738" + G[1]);
                            SoVeThuongVoid.Add(za);
                        }
                    }
                    else
                    {
                        if (G[4].Contains("REF"))
                        {
                            if (LuuSoVeHoanVoid.Where(w => w.Equals("738" + G[1])).Count() < 1)
                                SoVeHoan.Add(za);
                        }
                        else if (LuuSoVeThuongVoid.Where(w => w.Equals("738" + G[1])).Count() < 1)
                        {
                            SoVeThuong.Add(za);
                        }
                    }

                }

                #region Xóa void
                List<GiaoDichO> lstVoid = gd.VeThuong(SoVeThuongVoid, 4);
                lstVoid.AddRange(gd.VeThuong(SoVeThuongVoid, 9));

                string CauTruyVan = String.Join(",", lstVoid.Select(w => w.ID).ToArray());

                if (gd.Xoa(CauTruyVan, "WHERE ID IN ({0})") > 0)
                {
                    string NoiDung = string.Format("{0}: {1}", "Xóa void", String.Join(",", lstVoid.Select(w => w.SoVeVN).ToArray()));
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("FormName", "Hệ thống");
                    dic.Add("MaCho", string.Empty);
                    dic.Add("NoiDung", NoiDung);
                    dic.Add("LoaiKhachHang", 0);
                    dic.Add("Ma", 0);
                    if (NoiDung.Length > 10)
                        new LichSuD().ThemMoi(dic);
                }
                #endregion

                #region Vé Hoàn
                List<GiaoDichO> lstHoan = gd.VeThuong(SoVeHoan, 9);

                for (int i = 0; i < SoVeHoan.Count; i++)
                {
                    if (lstHoan.Where(w => SoVeHoan[i].Split(',')[1].Equals(w.SoVeVN.Substring(3))).Count() > 0)
                    {
                        SoVeHoan.Remove(SoVeHoan[i]);
                        i--;
                    }
                }//xóa đã tồn tại

                if (SoVeHoan.Count > 0)
                {
                    lstHoan = gd.VeThuong(SoVeHoan, 4);
                    if (lstHoan.Count > 0)
                    {
                        for (int i = 0; i < lstHoan.Count; i++)
                        {
                            List<string> aZ = SoVeHoan.Where(w => w.Split(',')[1].Equals(lstHoan[i].SoVeVN.Substring(3))).ToList();
                            if (aZ.Count > 0)
                            {
                                string[] G = aZ[0].Split(',');
                                lstHoan[i].Agent = G[5];
                                lstHoan[i].NgayGD = DateTime.Now;
                                lstHoan[i].TheoDoi = true;
                                lstHoan[i].TinhCongNo = false;
                                lstHoan[i].TenKhach += " /Hoàn vé";
                                lstHoan[i].GiaHoan = 0;
                                lstHoan[i].GiaNet = lstHoan[i].GiaHeThong = lstHoan[i].GiaThu = long.Parse(G[9]);
                                lstHoan[i].HangHoan = 0 - long.Parse(G[11]) + lstHoan[i].GiaNet;
                                lstHoan[i].PhiCK = lstHoan[i].HoaHong = lstHoan[i].Fare = 0;
                                lstHoan[i].LoaiGiaoDich = 9;
                                lstHoan[i].NVHoTro = lstHoan[i].NVGiaoDich;
                                lstHoan[i].NVGiaoDich = 0;
                                SoVeHoan.Remove(aZ[0]);
                            }
                        }
                        gd.Them(lstHoan);
                    }
                }
                #endregion

                #region Vé Thường
                List<GiaoDichO> lstThuong = gd.VeThuong(SoVeThuong, 4);
                List<string> macho = gd.MaCho(SoVeThuong).Distinct().ToList();

                string[] vs = new string[] { "HANFJN", "DAD10D", "SGN0P5", "HANNGE", "HANNGG", "GYXT3N", "SGNN4U", "SGNLD8" };

                for (int i = 0; i < SoVeThuong.Count; i++)
                {
                    string[] asd = SoVeThuong[i].Split(',');
                    if (lstThuong.Where(w => asd[1].Equals(w.SoVeVN.Substring(3))).Count() > 0 || vs.Contains(asd[5]) || (asd[4].Contains("EXC") && long.Parse(asd[11]) == 0))
                    {
                        SoVeThuong.Remove(SoVeThuong[i]);
                        i--;
                    }
                }//xóa đã tồn tại
                #endregion

                if (SoVeThuong.Count > 0)
                {
                    lstThuong = new List<GiaoDichO>();
                    for (int i = 0; i < SoVeThuong.Count; i++)
                    {
                        GiaoDichO giaoDichO = new GiaoDichO();
                        string[] Liststr = SoVeThuong[i].Split(',');
                        giaoDichO.SoVeVN = "738" + Liststr[1];


                        giaoDichO.LoaiKhachHang = 0;
                        giaoDichO.Agent = Liststr[5];
                        if (Liststr[4].Contains("EXC"))
                            giaoDichO.LoaiGiaoDich = 13;


                        List<int> vs1 = lstSignIn.Where(w => w.SignIn.Equals(giaoDichO.Agent)).Select(w => w.DaiLy).ToList();
                        if (vs1.Count > 0)
                        {
                            DaiLyO dl = _lstDaiLyALL.First(w => w.ID.Equals(vs1[0]));
                            if (dl.LoaiKhachHang == 0)
                            {
                                giaoDichO.NVGiaoDich = dl.ID;
                                giaoDichO.VeTuXuat = false;
                            }
                            giaoDichO.IDKhachHang = dl.ID;
                            giaoDichO.LoaiKhachHang = dl.LoaiKhachHang;
                        }

                        giaoDichO.Hang = "VN";
                        DateTime[] dta = new DateTime[] { DateTime.ParseExact(Liststr[3], "MM/dd/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(Liststr[2], "MM/dd/yyyy", CultureInfo.InvariantCulture) };

                        giaoDichO.NgayCuonChieu = giaoDichO.NgayGD = dta.Max();

                        string Str = "https://agencymanagement.havail.sabre.com/ctva/user/include/txnTKTDetails.html?txnId=" + Liststr[0].ToString().Replace("'", string.Empty) + "&txnType=TRANS_TYPE_TKT";
                        try
                        {
                            chromeDriver.Navigate().GoToUrl(Str);
                        }
                        catch { chromeDriver.Navigate().GoToUrl(Str); }

                        List<string> lstStrTicket = chromeDriver.FindElement(By.TagName("body")).Text.Replace("\r\n", "|").Split('|').ToList();

                        bool HanhTrinhDi = false;
                        bool HanhTrinhVe = false;
                        for (int y = 0; y < lstStrTicket.Count(); y++)
                        {
                            string[] TTVe = lstStrTicket[y].Split(' ');
                            switch (TTVe[0])
                            {
                                case "Document":
                                    giaoDichO.GiaThu = giaoDichO.GiaNet = giaoDichO.GiaHeThong = long.Parse(TTVe.Last());
                                    break;
                                case "PNR":
                                    giaoDichO.MaCho = TTVe[1];
                                    break;
                                case "Tour":
                                    giaoDichO.Fare = long.Parse(TTVe.Last());
                                    break;
                                case "Transaction":
                                    HanhTrinhDi = true;
                                    break;
                                case "Miscellaneous":
                                    y += 3;
                                    break;
                                default:
                                    if (HanhTrinhVe)
                                    {
                                        DateTime NgayBay = DateTime.ParseExact(TTVe[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                        giaoDichO.SoLuongVe = 2;
                                        giaoDichO.GioBayVe = giaoDichO.GioBayVe_Den = NgayBay;
                                        giaoDichO.SoHieuVe = TTVe[1] + TTVe[2];
                                        giaoDichO.LoaiVeVe = TTVe[6];
                                        giaoDichO.TuyenBayVe = Tb.TuyenBay(Sb.SanBay(TTVe[3]).ID, Sb.SanBay(TTVe[4]).ID).ID;
                                        if (giaoDichO.TuyenBayVe == 0)
                                            giaoDichO.GhiChu += "|" + TTVe[3] + "-" + TTVe[4];
                                    }
                                    else if (HanhTrinhDi)
                                    {
                                        DateTime NgayBay = DateTime.Now;
                                        try
                                        {
                                            NgayBay = DateTime.ParseExact(TTVe[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                        }
                                        catch { }

                                        giaoDichO.SoLuongVe = 1;
                                        giaoDichO.GioBayDi = giaoDichO.GioBayDi_Den = NgayBay;
                                        giaoDichO.SoHieuDi = TTVe[1] + TTVe[2];
                                        giaoDichO.LoaiVeDi = TTVe[6];

                                        if (lstStr.Contains(TTVe[TTVe.Length - 2]))
                                            giaoDichO.BiDanh = TTVe[TTVe.Length - 2];
                                        else
                                            giaoDichO.BiDanh = "-";

                                        giaoDichO.TuyenBayDi = Tb.TuyenBay(Sb.SanBay(TTVe[3]).ID, Sb.SanBay(TTVe[4]).ID).ID;
                                        if (giaoDichO.TuyenBayDi == 0)
                                            giaoDichO.GhiChu = TTVe[3] + "-" + TTVe[4];
                                        for (int u = 7; u < TTVe.Length; u++)
                                        {
                                            giaoDichO.TenKhach += TTVe[u] + " ";
                                        }
                                        if (giaoDichO.TenKhach != null)
                                        {
                                            giaoDichO.TenKhach = giaoDichO.TenKhach.Replace(giaoDichO.BiDanh, string.Empty);
                                            if (Liststr[4].Contains("EMD"))
                                            {
                                                giaoDichO.TenKhach += "-HL";
                                                giaoDichO.LoaiGiaoDich = 14;
                                            }
                                        }
                                        else
                                            giaoDichO.TenKhach = "-VC";
                                        giaoDichO.PhiCK = 0;
                                        HanhTrinhVe = true;
                                    }
                                    break;
                            }
                        }

                        if (giaoDichO.LoaiGiaoDich == 4 && macho.Contains(giaoDichO.MaCho))
                            giaoDichO.LoaiGiaoDich = 13;

                        giaoDichO.NhaCungCap = 2;
                        lstThuong.Add(giaoDichO);
                        if (lstThuong.Count > 0)
                        {
                            gd.Them(lstThuong);
                            lstThuong.Clear();
                        }
                    }
                }

                if (SoVeHoan.Count > 0)
                {
                    lstThuong = new List<GiaoDichO>();
                    for (int i = 0; i < SoVeHoan.Count; i++)
                    {
                        GiaoDichO giaoDichO = new GiaoDichO();
                        string[] Liststr = SoVeHoan[i].Split(',');
                        giaoDichO.SoVeVN = "738" + Liststr[1];
                        giaoDichO.GiaNet = giaoDichO.GiaHeThong = long.Parse(Liststr[9]);
                        giaoDichO.HangHoan = 0 - long.Parse(Liststr[11]) + giaoDichO.GiaNet;
                        giaoDichO.Agent = Liststr[5];
                        giaoDichO.LoaiGiaoDich = 9;


                        List<int> vs1 = lstSignIn.Where(w => w.SignIn.Equals(giaoDichO.Agent)).Select(w => w.DaiLy).ToList();
                        if (vs1.Count > 0)
                        {
                            DaiLyO dl = _lstDaiLyALL.First(w => w.ID.Equals(vs1[0]));
                            if (dl.LoaiKhachHang == 0)
                            {
                                giaoDichO.NVGiaoDich = dl.ID;
                                giaoDichO.VeTuXuat = false;
                            }
                            giaoDichO.IDKhachHang = dl.ID;
                            giaoDichO.LoaiKhachHang = dl.LoaiKhachHang;
                        }

                        giaoDichO.TinhCongNo = false;
                        giaoDichO.Hang = "VN";
                        DateTime[] dta = new DateTime[] { DateTime.ParseExact(Liststr[3], "MM/dd/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(Liststr[2], "MM/dd/yyyy", CultureInfo.InvariantCulture) };

                        giaoDichO.NgayCuonChieu = giaoDichO.NgayGD = dta.Max();

                        string Str = "https://agencymanagement.havail.sabre.com/ctva/user/include/txnTKTDetails.html?txnId=" + Liststr[0].ToString().Replace("'", string.Empty) + "&txnType=TRANS_TYPE_TKT";
                        try
                        {
                            chromeDriver.Navigate().GoToUrl(Str);
                        }
                        catch { chromeDriver.Navigate().GoToUrl(Str); }

                        List<string> lstStrTicket = chromeDriver.FindElement(By.TagName("body")).Text.Replace("\r\n", "|").Split('|').ToList();

                        bool HanhTrinhDi = false;
                        bool HanhTrinhVe = false;
                        for (int y = 0; y < lstStrTicket.Count(); y++)
                        {
                            string[] TTVe = lstStrTicket[y].Split(' ');
                            switch (TTVe[0])
                            {
                                case "PNR":
                                    giaoDichO.MaCho = TTVe[1];
                                    break;
                                case "Tour":
                                    giaoDichO.Fare = long.Parse(TTVe.Last());
                                    break;
                                case "Transaction":
                                    HanhTrinhDi = true;
                                    break;
                                case "Miscellaneous":
                                    y += 3;
                                    break;
                                default:
                                    if (HanhTrinhVe)
                                    {
                                        DateTime NgayBay = DateTime.ParseExact(TTVe[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                        giaoDichO.SoLuongVe = 2;
                                        giaoDichO.GioBayVe = giaoDichO.GioBayVe_Den = NgayBay;
                                        giaoDichO.SoHieuVe = TTVe[1] + TTVe[2];
                                        giaoDichO.LoaiVeVe = TTVe[6];
                                        giaoDichO.TuyenBayVe = Tb.TuyenBay(Sb.SanBay(TTVe[3]).ID, Sb.SanBay(TTVe[4]).ID).ID;
                                        if (giaoDichO.TuyenBayVe == 0)
                                            giaoDichO.GhiChu += "|" + TTVe[3] + "-" + TTVe[4];
                                    }
                                    else if (HanhTrinhDi)
                                    {
                                        DateTime NgayBay = DateTime.Now;
                                        try
                                        {
                                            NgayBay = DateTime.ParseExact(TTVe[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                        }
                                        catch { }

                                        giaoDichO.SoLuongVe = 1;
                                        giaoDichO.GioBayDi = giaoDichO.GioBayDi_Den = NgayBay;
                                        giaoDichO.SoHieuDi = TTVe[1] + TTVe[2];
                                        giaoDichO.LoaiVeDi = TTVe[6];

                                        if (lstStr.Contains(TTVe[TTVe.Length - 2]))
                                            giaoDichO.BiDanh = TTVe[TTVe.Length - 2];
                                        else
                                            giaoDichO.BiDanh = "-";

                                        giaoDichO.TuyenBayDi = Tb.TuyenBay(Sb.SanBay(TTVe[3]).ID, Sb.SanBay(TTVe[4]).ID).ID;
                                        if (giaoDichO.TuyenBayDi == 0)
                                            giaoDichO.GhiChu = TTVe[3] + "-" + TTVe[4];
                                        for (int u = 7; u < TTVe.Length; u++)
                                        {
                                            giaoDichO.TenKhach += TTVe[u] + " ";
                                        }
                                        if (giaoDichO.TenKhach != null)
                                        {
                                            giaoDichO.TenKhach = giaoDichO.TenKhach.Replace(giaoDichO.BiDanh, string.Empty);
                                            if (Liststr[4].Contains("EMD"))
                                                giaoDichO.TenKhach += "-HL";
                                        }
                                        else
                                            giaoDichO.TenKhach = "-VC";
                                        giaoDichO.PhiCK = 0;
                                        HanhTrinhVe = true;
                                    }
                                    break;
                            }
                        }

                        giaoDichO.TheoDoi = true;
                        giaoDichO.TinhCongNo = false;
                        giaoDichO.TenKhach += " /Hoàn vé";
                        giaoDichO.GiaHoan = 0;
                        giaoDichO.PhiCK = giaoDichO.HoaHong = giaoDichO.Fare = 0;
                        giaoDichO.NVGiaoDich = giaoDichO.NVHoTro = 0;

                        giaoDichO.NhaCungCap = 2;
                        lstThuong.Add(giaoDichO);
                        if (lstThuong.Count > 5 || lstThuong.Count == SoVeHoan.Count)
                        {
                            gd.Them(lstThuong);
                            lstThuong.Clear();
                        }
                    }
                    gd.Them(lstThuong);
                }
            }

            {
                GiaoDichD giaoDichD = new GiaoDichD();
                List<GiaoDichO> lstGiaoDichOLD = giaoDichD.DuLieu(" LoaiKhachHang = 0 and LEN(SoVeVN) > 4 and LoaiGiaoDich in (4,13,14) and Convert(date,NgayGD) > Convert(date,getdate()-10) and ( NhaCungCap = 2 or Agent = 'USERAPI')");
            Step1:
                chromeDriver.Navigate().GoToUrl("http://ags.thanhhoang.vn/Login.aspx");
                wait.Until(driver => driver.PageSource.Contains("imgImageValidate"));
                IWebElement ele = chromeDriver.FindElement(By.CssSelector("#imgImageValidate"));
                string res = "";
                try
                {
                    using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
                    {
                        using (var page = engine.Process(TakeScreenshot(chromeDriver, ele), PageSegMode.AutoOnly))
                            res = page.GetText();
                    }
                }
                catch { chromeDriver.Navigate().GoToUrl("http://ags.thanhhoang.vn/Login.aspx"); }
                chromeDriver.FindElement(By.Id("txtUsernameVNiSC")).SendKeys("admin");
                chromeDriver.FindElement(By.Id("txtMatKhau")).SendKeys("11223399");
                chromeDriver.FindElement(By.Id("txtAgentCode")).SendKeys("THD");
                chromeDriver.FindElement(By.Id("txtImageValidate")).SendKeys(res);
                Thread.Sleep(1000);
                if (!chromeDriver.Url.Equals("http://ags.thanhhoang.vn/Booking.aspx"))
                    goto Step1;

                List<string> MaCho = lstGiaoDichOLD.Select(w => w.MaCho.ToUpper().Replace(" ", "")).Distinct().ToList();
                int DemSoLanThatBai = 0;
                foreach (string gdO in MaCho)
                {
                    Thread.Sleep(500);
                    chromeDriver.Navigate().GoToUrl($"http://ags.thanhhoang.vn/Manage.aspx?Do=ListBooking&PNRCode={gdO}");
                    Thread.Sleep(500);
                    wait.Until(driver => driver.PageSource.Contains("Total Booking:"));
                    string TonTai = chromeDriver.PageSource.Substring(chromeDriver.PageSource.IndexOf("Total Booking:"), 16).Split(':')[1];
                    if (TonTai.Contains("\r"))
                        continue;

                    chromeDriver.Navigate().GoToUrl(chromeDriver.FindElement(By.XPath("//*[@id='form1']/div[3]/div[2]/div/div[6]/div[2]/div[9]/a")).GetAttribute("href"));

                    GiaoDichO giaoDichO = new GiaoDichO();
                    int _Index = chromeDriver.FindElements(By.ClassName("line")).Count() - 1;
                    giaoDichO.EmailKhachHang = chromeDriver.FindElements(By.ClassName("line"))[_Index - 8].Text.Replace("Email\r\n", "");
                    giaoDichO.DienThoaiKhachHang = chromeDriver.FindElements(By.ClassName("line"))[_Index - 9].Text.Replace("Telephone\r\n", "");
                    string[] Ag = chromeDriver.FindElements(By.ClassName("line"))[_Index].Text.Replace("\r\nXuất vé :", "|").Replace("\r\nVoid vé :", "|").Replace("\r\nBook vé :", "|").Split('|');
                    DaiLyO daiLy = new DaiLyO();
                    if (daiLy.ID == 0)
                    {
                        giaoDichO.Agent = Ag[2].Replace(":", ".").Split('.')[1].Trim();
                        List<int> vs1 = lstSignIn.Where(w => w.SignIn.Equals(giaoDichO.Agent)).Select(w => w.DaiLy).ToList();
                        if (vs1.Count > 0)
                        {
                            try
                            {
                                daiLy = _lstDaiLy.First(w => w.ID.Equals(vs1[0]));
                                DemSoLanThatBai = 0;
                            }
                            catch { }
                        }
                    }

                    if (daiLy.ID == 0)
                    {
                        giaoDichO.Agent = Ag[1].Replace(":", ".").Split('.')[1].Trim();
                        List<int> vs1 = lstSignIn.Where(w => w.SignIn.Equals(giaoDichO.Agent)).Select(w => w.DaiLy).ToList();
                        if (vs1.Count > 0)
                        {
                            try
                            {
                                daiLy = _lstDaiLy.First(w => w.ID.Equals(vs1[0]));
                                DemSoLanThatBai = 0;
                            }
                            catch { }
                        }
                    }

                    if (daiLy.ID == 0)
                    {
                        List<DaiLyO> Dl = _lstDaiLy.Where(w => (w.EmailGiaoDich ?? string.Empty).ToUpper().Contains((giaoDichO.EmailKhachHang ?? string.Empty).ToUpper())
                               && (w.EmailGiaoDich ?? string.Empty).Length > 0 && (giaoDichO.EmailKhachHang ?? string.Empty).Length > 0).ToList();
                        if (Dl.Count > 0)
                        {
                            daiLy = Dl[0];
                            DemSoLanThatBai = 0;
                        }
                    }

                    if (daiLy.ID == 0)
                    {
                        DemSoLanThatBai++;
                        if (DemSoLanThatBai == 30)
                            goto _end;
                        continue;
                    }

                    List<GiaoDichO> lstGiaoDichNew = lstGiaoDichOLD.Where(w => w.MaCho.ToUpper().Replace(" ", "").Equals(gdO)).ToList();

                    string TenCu = _lstDaiLyALL.Where(w => w.ID.Equals(lstGiaoDichNew[0].IDKhachHang)).Count() > 0 ? _lstDaiLyALL.Where(w => w.ID.Equals(lstGiaoDichNew[0].IDKhachHang)).ToList()[0].Ten : "";


                    for (int i = 0; i < lstGiaoDichNew.Count; i++)
                    {
                        lstGiaoDichNew[i].Agent = giaoDichO.Agent;
                        lstGiaoDichNew[i].DienThoaiKhachHang = giaoDichO.DienThoaiKhachHang;
                        lstGiaoDichNew[i].EmailKhachHang = giaoDichO.EmailKhachHang;
                        lstGiaoDichNew[i].IDKhachHang = daiLy.ID;
                        lstGiaoDichNew[i].LoaiKhachHang = daiLy.LoaiKhachHang;
                    }

                    string TenMoi = _lstDaiLy.Where(w => w.ID.Equals(lstGiaoDichNew[0].IDKhachHang)).ToList()[0].Ten;

                    if (giaoDichD.ThucThiSua(lstGiaoDichNew) > 0)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        if (TenCu != TenMoi)
                        {
                            dic.Add("FormName", "Hệ thống");
                            dic.Add("MaCho", string.Empty);
                            dic.Add("NoiDung", string.Format("{0}: [{2} => {1}]", lstGiaoDichNew[0].MaCho, TenMoi, TenCu));
                            dic.Add("LoaiKhachHang", 0);
                            dic.Add("Ma", 0);
                            new LichSuD().ThemMoi(dic);
                        }
                    }
                }
            }

        _end:
            chromeDriver.Close();
            chromeDriver.Quit();
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("chromedriver"))
            { process.Kill(); }

        }

        List<SignInO> lstSignIn = new List<SignInO>();

        Bitmap TakeScreenshot(IWebDriver driver, IWebElement element)
        {
            Byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
            Bitmap screenshot = new Bitmap(new System.IO.MemoryStream(byteArray));
            Rectangle croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);
            return screenshot;
        }

    }
}
