using DataAccessLayer;
using DataTransferObject;
using Microsoft.Win32;
using mshtml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace VietJetProJ
{
    public partial class frmVJ : Form
    {
        GiaoDichD gdD = new GiaoDichD();
        List<SignInO> lstSignIn = new List<SignInO>();
        List<DaiLyO> lstDaiLy = new List<DaiLyO>();
        List<TuyenBayO> lstTuyenBay = new List<TuyenBayO>();
        DateTime NgayChay = new DateTime();
        NCCO _NCCO = new NCCO();
        DaiLyO dl = new DaiLyO();
        public frmVJ(DateTime dtp)
        {
            InitializeComponent();
            NgayChay = dtp;
        }

        private void frmVJ_Load(object sender, EventArgs e)
        {
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true);
            RegKey.SetValue("Display Inline Images", "no");
            lstSignIn = new SignInD().DuLieu();
            lstTuyenBay = new TuyenBayD().DuLieu();
            lstDaiLy = new DaiLyD().All();
            wVJ.Navigate("https://www.vietjetair.com/Sites/Web/vi-VN/Home");
            while (wVJ.IsBusy)
            {
                Application.DoEvents();
            }
            _NCCO = new NCCD().DuLieu().Where(w => w.ID.Equals(1)).ToList()[0];
        }

        int nSearch = 0;
        int StepLoad = 0;
        List<string> MaCho = new List<string>();
        List<GiaoDichO> VeThem = new List<GiaoDichO>();
        List<GiaoDichO> VeThuHoGD = new List<GiaoDichO>();
        List<GiaoDichO> VeThuongGD = new List<GiaoDichO>();
        List<GiaoDichO> VeHoanGD = new List<GiaoDichO>();
        List<GiaoDichO> BanTam = new List<GiaoDichO>();

        private void wVJ_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wVJ.ReadyState == WebBrowserReadyState.Complete && !wVJ.IsBusy)
            {
                Thread.Sleep(100);
                HtmlElement head = wVJ.Document.GetElementsByTagName("head")[0];
                HtmlElement scriptEl = wVJ.Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                if (wVJ.Url.AbsolutePath.Contains("/EWRevisedItinerary.aspx")) // Thông tin chi tiết
                {
                    HtmlElementCollection tables = wVJ.Document.GetElementsByTagName("table");
                    HtmlElement tblThongTinDatCho = tables[1];
                    HtmlElementCollection tblThongTinDatCho_trs = tblThongTinDatCho.GetElementsByTagName("tr");

                    HtmlElement tblGia = tables[tables.Count - 7];
                    HtmlElementCollection tblGiao_trs = tblGia.GetElementsByTagName("tr");

                    for (int i = 0; i < BanTam.Count; i++)
                    {
                        string lienlac = tblThongTinDatCho_trs[0].GetElementsByTagName("td")[1].InnerText;
                        int idx = lienlac.IndexOf(' ');
                        if (idx > 0)
                            BanTam[i].DienThoaiKhachHang = lienlac.Substring(0, idx);
                        BanTam[i].EmailKhachHang = tblThongTinDatCho_trs[1].GetElementsByTagName("td")[1].InnerText;
                        if (BanTam[i].LoaiKhachHang < 1)
                        {
                            List<DaiLyO> Dl = lstDaiLy.Where(w => (w.EmailGiaoDich ?? string.Empty).ToUpper().Contains((BanTam[i].EmailKhachHang ?? string.Empty).ToUpper())
                            && (w.EmailGiaoDich ?? string.Empty).Length > 0 && (BanTam[i].EmailKhachHang ?? string.Empty).Length > 0).ToList();
                            if (Dl.Count > 0)
                            {
                                BanTam[i].IDKhachHang = Dl[0].ID;
                                BanTam[i].LoaiKhachHang = Dl[0].LoaiKhachHang;
                            }
                        }

                        List<TableVJ> tb = new List<TableVJ>();
                        foreach (HtmlElement htmlElement in tblGiao_trs)
                        {
                            HtmlElementCollection g = htmlElement.GetElementsByTagName("td");
                            if (g.Count > 5)
                            {
                                TableVJ tVJ = new TableVJ();
                                tVJ.LEG = int.Parse(new String(g[0].InnerText.Where(Char.IsDigit).ToArray()));
                                tVJ.TEN = g[1].InnerText;
                                tVJ.MOTA = g[2].InnerText;
                                tVJ.SOTIEN = int.Parse(new String(g[3].InnerText.Where(Char.IsDigit).ToArray()));
                                tVJ.VAT = int.Parse(new String(g[4].InnerText.Where(Char.IsDigit).ToArray()));
                                tVJ.TONG = int.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray()));

                                if (tVJ.TEN.Replace(",", string.Empty).Equals(BanTam[i].TenKhach))
                                    tb.Add(tVJ);
                            }
                        }

                        int Net = 0;
                        int Fare = 0;
                        foreach (TableVJ z in tb)
                        {
                            if (z.MOTA.StartsWith("Airport Tax Domestic"))
                                Net += z.TONG;
                            else if (z.MOTA.StartsWith("Admin Fee Domestic"))
                                Net += z.TONG;
                            else if (z.MOTA.StartsWith("Airport Security"))
                                Net += z.TONG;
                            else if (z.MOTA.ToLower().Contains(" - " + BanTam[i].LoaiVeDi.ToLower()) || z.MOTA.ToLower().Contains(" - " + (BanTam[i].LoaiVeVe ?? BanTam[i].LoaiVeDi).ToLower()))
                            {
                                Net += z.TONG;
                                Fare += z.TONG;
                            }
                            else if (z.MOTA.Contains("Add Ons") && z.MOTA.Contains("Bag"))
                            {
                                if (z.TONG == BanTam[i].GiaNet)
                                {
                                    BanTam[i].LoaiGiaoDich = 14;
                                    BanTam[i].Fare = 0;
                                    BanTam[i].BiDanh = "BAG" + int.Parse(new String(z.MOTA.Split('-')[1].Where(Char.IsDigit).ToArray())).ToString();
                                }
                                else
                                {
                                    if (z.LEG == 1)
                                        BanTam[i].HanhLyDi = int.Parse(new String(z.MOTA.Split('-')[1].Where(Char.IsDigit).ToArray())).ToString() + "KG";
                                    else
                                        BanTam[i].HanhLyVe = int.Parse(new String(z.MOTA.Split('-')[1].Where(Char.IsDigit).ToArray())).ToString() + "KG";
                                }
                                break;
                            }
                            else if (z.MOTA.Contains("ROUTING - MODIFICATION") && z.TONG == BanTam[i].GiaNet)
                            {
                                BanTam[i].LoaiGiaoDich = 13;
                                BanTam[i].BiDanh = "RM";
                                BanTam[i].Fare = 0;
                                break;
                            }
                            else if (z.MOTA.Contains("PASSENGER - CHANGE") && z.TONG == BanTam[i].GiaNet)
                            {
                                BanTam[i].LoaiGiaoDich = 13;
                                BanTam[i].Fare = 0;
                                BanTam[i].BiDanh = "PC";
                                break;
                            }
                            else if (z.MOTA.Contains("Seat Assignment") && z.TONG == BanTam[i].GiaNet)
                            {
                                BanTam[i].LoaiGiaoDich = 14;
                                BanTam[i].Fare = 0;
                                BanTam[i].BiDanh = "SEAT";
                                break;
                            }

                        }

                        if (Net <= BanTam[i].GiaNet)
                            BanTam[i].Fare = Fare;
                        else
                        {
                            if (BanTam[i].LoaiGiaoDich == 4)
                                BanTam[i].LoaiGiaoDich = 13;
                            BanTam[i].Fare = 0;
                        }
                        VeThem.Add(BanTam[i]);
                    }

                    Xuli();
                }
                else if (wVJ.Url.AbsolutePath.Contains("/EditRes.aspx"))
                {
                    try
                    {
                        BanTam = VeThuongGD.Where(w => w.MaCho.Equals(MaCho.First())).ToList();

                        HtmlElement CD = wVJ.Document.GetElementById("leg1");
                        HtmlElement CV = wVJ.Document.GetElementById("leg2");

                        for (int i = 0; i < BanTam.Count; i++)
                        {
                            if (CD != null)
                            {
                                if (CD.Document.GetElementById("grdPaxFareDetails") != null)
                                {
                                    HtmlElementCollection CDCellData1 = CD.Document.GetElementById("grdPaxFareDetails").GetElementsByTagName("TD");
                                    BanTam[i].LoaiVeDi = CDCellData1[1].InnerText.Split('-')[1].Replace(" ", string.Empty);
                                }
                                else
                                {
                                    BanTam[i].LoaiVeDi = "Eco";
                                }

                                if (CD.Document.GetElementById("grdFlightInfo") != null)
                                {
                                    HtmlElementCollection CDCellData2 = CD.Document.GetElementById("grdFlightInfo").GetElementsByTagName("TD");
                                    BanTam[i].SoHieuDi = CDCellData2[1].InnerText;
                                    BanTam[i].GioBayDi = DateTime.ParseExact(CDCellData2[0].InnerText.Substring(0, CDCellData2[0].InnerText.IndexOf(' ')) + " " + CDCellData2[2].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                    BanTam[i].GioBayDi_Den = DateTime.ParseExact(CDCellData2[0].InnerText.Substring(0, CDCellData2[0].InnerText.IndexOf(' ')) + " " + CDCellData2[3].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                }
                            }

                            if (CV != null)
                            {
                                HtmlElement a = CV.GetElementsByTagName("tbody")[0];
                                if (a.Document.GetElementById("grdPaxFareDetails") != null)
                                {
                                    HtmlElementCollection CVCellData1 = CV.GetElementsByTagName("tbody")[0].Document.GetElementById("grdPaxFareDetails").GetElementsByTagName("TD");
                                    BanTam[i].LoaiVeVe = CVCellData1[1].InnerText.Split('-')[1].Replace(" ", string.Empty);
                                }

                                if (a.Document.GetElementById("grdFlightInfo") != null)
                                {
                                    HtmlElementCollection CVCellData2 = a.GetElementsByTagName("TD");
                                    if (CVCellData2[2].InnerText != "Departure")
                                    {
                                        BanTam[i].SoHieuVe = CVCellData2[2].InnerText;
                                        BanTam[i].GioBayVe = DateTime.ParseExact(CVCellData2[1].InnerText.Substring(0, CVCellData2[1].InnerText.IndexOf(' ')) + " " + CVCellData2[3].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                        BanTam[i].GioBayVe_Den = DateTime.ParseExact(CVCellData2[1].InnerText.Substring(0, CVCellData2[1].InnerText.IndexOf(' ')) + " " + CVCellData2[4].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                    }
                                    else
                                    {
                                        BanTam[i].SoHieuVe = CVCellData2[5].InnerText;
                                        BanTam[i].GioBayVe = DateTime.ParseExact(CVCellData2[4].InnerText.Substring(0, CVCellData2[4].InnerText.IndexOf(' ')) + " " + CVCellData2[6].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                        BanTam[i].GioBayVe_Den = DateTime.ParseExact(CVCellData2[4].InnerText.Substring(0, CVCellData2[4].InnerText.IndexOf(' ')) + " " + CVCellData2[7].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                    }    
                                }
                            }
                        }
                    }
                    catch
                    { }
                    finally
                    {
                        element.text = "function doPost() { document.forms['Edit'].button.value='newitinerary';SubmitForm(); }";
                        head.AppendChild(scriptEl);
                        wVJ.Document.InvokeScript("doPost");
                    }
                }
                else if (wVJ.Url.AbsolutePath.Contains("/SearchRes.aspx"))
                {
                    if (nSearch == 0)
                    {
                        nSearch++;
                        wVJ.Document.GetElementById("txtSearchResNum").SetAttribute("value", MaCho.First());
                        wVJ.Document.GetElementById("Search").InvokeMember("submit");
                    }
                    else
                    {
                        nSearch = 0;
                        HtmlElement error = GetElementByClass("p", "ErrorCaption");
                        if (error != null)
                        {
                            VeThem.AddRange(VeThuongGD.Where(w => w.MaCho.Equals(MaCho.First())).ToList());
                            MaCho.Remove(MaCho.First());
                            wVJ.Navigate("https://agents.vietjetair.com/SearchRes.aspx?lang=vi&st=sl&sesid=");
                        }
                        else
                        {
                            element.text = "function doPost() { document.forms['Search'].button.value='continue';pop('?lang=vi'); setTimeout('document.forms[\"Search\"].submit()', 100); }";
                            head.AppendChild(scriptEl);
                            wVJ.Document.InvokeScript("doPost");
                        }
                    }
                }
                else if (wVJ.Url.AbsolutePath.Contains("/CompanyPayRpt.aspx"))
                {
                    int isM = 13 - (DateTime.Now.Year - NgayChay.Year) * 12 - (DateTime.Now.Month - NgayChay.Month);
                    string Datestr = string.Format(@"document.getElementById('dlstFromDate_Day').options.item({0}).selected = true; 
                                                              document.getElementById('dlstToDate_Day').options.item({0}).selected = true;
                                                              document.getElementById('dlstFromDate_Month').options.item({1}).selected = true;
                                                              document.getElementById('dlsttoDate_Month').options.item({1}).selected = true;", NgayChay.Day, isM);

                    int a = wVJ.Document.GetElementById("lstCompany").GetElementsByTagName("option").Count - 1;

                    switch (StepLoad)
                    {
                        case 0:
                            element.text = @"function doPost() { " + Datestr + " document.forms['CompanyPayments'].button.value='GetPaymnt';SubmitForm(); }";
                            head.AppendChild(scriptEl);
                            wVJ.Document.InvokeScript("doPost");
                            break;
                        default:
                            HtmlElement table = wVJ.Document.GetElementById("PayDetsGrd");
                            if (table != null)
                            {
                                #region lấy tất cả dòng dữ liệu để xử lí
                                List<string> vs = new List<string>() { "GridPayDetsEven", "GridPayDetsOdd" };
                                HtmlElementCollection eles = wVJ.Document.GetElementsByTagName("tr");
                                foreach (HtmlElement ele in eles)
                                {
                                    if (!vs.Contains(ele.GetAttribute("className")))
                                        continue;

                                    #region thông tin chung
                                    HtmlElementCollection tds = ele.GetElementsByTagName("TD");
                                    GiaoDichO gd = new GiaoDichO();
                                    gd.Hang = "VJ";
                                    gd.LoaiKhachHang = 0;
                                    gd.NhaCungCap = 1;
                                    gd.MaCho = tds[1].InnerText;
                                    gd.NgayCuonChieu = gd.NgayGD = DateTime.ParseExact(tds[3].InnerText.Substring(0, tds[3].InnerText.IndexOf(' ')), "dd/MM/yyyy", null);
                                    gd.Agent = tds[6].InnerText.Split('-')[0].Replace(" ", string.Empty);
                                    gd.TenKhach = tds[2].InnerText.Replace(",", string.Empty);

                                    string[] HanhTrinh = tds[4].InnerText.Replace("CONF", "|").Replace("CANX", "|").Replace(" ", string.Empty).Split('|');

                                    gd.SoLuongVe = HanhTrinh.Length - 1;
                                    gd.SoHieuDi = "VJ";
                                    if (lstTuyenBay.Where(w => HanhTrinh[0].Substring(HanhTrinh[0].Length - 7).Equals(w.Ten)).Count() > 0)
                                        gd.TuyenBayDi = lstTuyenBay.Where(w => HanhTrinh[0].Contains(w.Ten)).ToList()[0].ID;
                                    else
                                        gd.GhiChu = HanhTrinh[0];

                                    if (HanhTrinh.Length > 2)
                                    {
                                        gd.SoHieuVe = "VJ";
                                        if (lstTuyenBay.Where(w => HanhTrinh[1].Substring(HanhTrinh[1].Length - 7).Equals(w.Ten)).Count() > 0)
                                            gd.TuyenBayVe = lstTuyenBay.Where(w => HanhTrinh[1].Contains(w.Ten)).ToList()[0].ID;
                                        else
                                            gd.GhiChu += "-" + HanhTrinh[1];
                                    }
                                    #endregion

                                    if (tds[11].InnerText.Contains("("))
                                    {
                                        gd.GioBayDi = gd.GioBayDi_Den = DateTime.ParseExact(HanhTrinh[0].Substring(0, HanhTrinh[0].Length - 7), "MMMdd,yyyy", null);
                                        if (HanhTrinh.Length > 2)
                                            gd.GioBayVe = gd.GioBayVe_Den = DateTime.ParseExact(HanhTrinh[1].Substring(0, HanhTrinh[1].Length - 7), "MMMdd,yyyy", null);
                                        gd.TenKhach += " /Hoàn vé";
                                        gd.GiaHoan = gd.HangHoan = long.Parse(new String(tds[11].InnerText.Where(Char.IsDigit).ToArray())) / 100;

                                        gd.LoaiGiaoDich = 9;
                                        VeHoanGD.Add(gd);
                                    }//vé hoàn
                                    else
                                    {
                                        List<int> vs1 = lstSignIn.Where(w => w.SignIn.Equals(gd.Agent)).Select(w => w.DaiLy).ToList();
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

                                        gd.GiaThu = gd.GiaHeThong = gd.GiaNet = long.Parse(new String(tds[11].InnerText.Where(Char.IsDigit).ToArray())) / 100;
                                        if (tds[6].InnerText.Contains("-ThuHo"))
                                        {
                                            int index = VeThuongGD.FindIndex(w => w.MaCho.Equals(gd.MaCho));
                                            if (index == -1)
                                            {
                                                gd.TenKhach = tds[6].InnerText;
                                                gd.Agent = tds[6].InnerText;
                                                VeThuHoGD.Add(gd);
                                            }
                                            else
                                                VeThuHoGD[index].GiaHeThong = VeThuongGD[index].GiaThu = VeThuongGD[index].GiaNet += gd.GiaThu;
                                        }// Thu hộ đặt chỗ
                                        else
                                            VeThuongGD.Add(gd);
                                    }
                                }//phân loại giao dịch
                                #endregion
                            }

                            if (StepLoad != a)//chưa lấy hết sub nên chạy tiếp
                            {
                                element.text = @"function doPost() { document.getElementById('lstCompany').options.item(" + (StepLoad + 1) + ").selected = true; document.forms['CompanyPayments'].button.value='GetPaymnt';SubmitForm();}";
                                head.AppendChild(scriptEl);
                                wVJ.Document.InvokeScript("doPost");
                            }
                            else//xử lí dữ liệu sau khi đến sub cuối
                            {
                                List<GiaoDichO> GD = new List<GiaoDichO>();
                                if (VeHoanGD.Count > 0)
                                {
                                    VeHoanGD = VeHoanGD.OrderBy(w => w.MaCho).ToList();
                                    GD = gdD.VeHoan(VeHoanGD, 1);// lấy dữ liệu đã tồn tại

                                    for (int i = 0; i < GD.Count; i++)
                                    {
                                        for (int u = 0; u < VeHoanGD.Count; u++)
                                        {
                                            if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeHoanGD[u].NgayGD.ToString("ddMMyy"))
                                                && GD[i].HangHoan == VeHoanGD[u].HangHoan && GD[i].MaCho == VeHoanGD[u].MaCho
                                                && GD[i].TenKhach == VeHoanGD[u].TenKhach)
                                            {
                                                GD.Remove(GD[i]);
                                                VeHoanGD.Remove(VeHoanGD[u]);
                                                i--;
                                                u--;
                                                break;
                                            }
                                        }
                                    } // xóa dữ liệu đã tồn tại

                                    if (VeHoanGD.Count > 0)
                                    {
                                        GD = gdD.LayDaiLyVeHoan(VeHoanGD, 1);
                                        for (int i = 0; i < VeHoanGD.Count; i++)
                                        {
                                            GiaoDichO gd = VeHoanGD[i];
                                            foreach (GiaoDichO gdo in GD)
                                            {
                                                if (gdo.MaCho.Replace(" ", string.Empty) == gd.MaCho.Replace(" ", string.Empty))
                                                {
                                                    if (gdo.LoaiKhachHang == 1)
                                                    {
                                                        gd.LoaiKhachHang = gdo.LoaiKhachHang;
                                                        gd.IDKhachHang = gdo.IDKhachHang;
                                                    }
                                                    if (gd.HangHoan == gdo.GiaNet && (gdo.BiDanh ?? string.Empty).Length > 1)
                                                        gd.BiDanh = gdo.BiDanh;
                                                    break;
                                                }
                                            }
                                        }
                                        gdD.Them(VeHoanGD);
                                    } //Tìm vé trước đó gán tên đl và thêm
                                }//vé hoàn |thêm ngay

                                if (VeThuHoGD.Count > 0)
                                {
                                    GD = gdD.VeThuong(VeThuHoGD, 1);// lấy dữ liệu đã tồn tại

                                    for (int i = 0; i < GD.Count; i++)
                                    {
                                        for (int u = 0; u < VeThuHoGD.Count; u++)
                                        {
                                            if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeThuHoGD[u].NgayGD.ToString("ddMMyy"))
                                                && GD[i].GiaNet == VeThuHoGD[u].GiaNet && GD[i].MaCho == VeThuHoGD[u].MaCho
                                                && GD[i].TenKhach == VeThuHoGD[u].TenKhach)
                                            {
                                                GD.Remove(GD[i]);
                                                VeThuHoGD.Remove(VeThuHoGD[u]);
                                                i--;
                                                u--;
                                                break;
                                            }
                                        }
                                    } // xóa dữ liệu đã tồn tại
                                    gdD.Them(VeThuHoGD);
                                }//vé thu hộ |thêm ngay

                                if (VeThuongGD.Count > 0)
                                {
                                    List<string> macho = gdD.MaCho(VeThuongGD, 5).Distinct().ToList();

                                    VeThuongGD = VeThuongGD.OrderBy(w => w.MaCho).ToList();
                                    GD = gdD.VeThuong(VeThuongGD, 1).OrderBy(w => w.MaCho).ToList();// lấy dữ liệu đã tồn tại

                                    for (int i = 0; i < GD.Count; i++)
                                    {
                                        for (int u = 0; u < VeThuongGD.Count; u++)
                                        {
                                            if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeThuongGD[u].NgayGD.ToString("ddMMyy"))
                                                && GD[i].GiaNet == VeThuongGD[u].GiaNet && GD[i].MaCho == VeThuongGD[u].MaCho
                                                && GD[i].TenKhach == VeThuongGD[u].TenKhach)
                                            {
                                                GD.Remove(GD[i]);
                                                VeThuongGD.Remove(VeThuongGD[u]);
                                                i--;
                                                break;
                                            }
                                            if (VeThuongGD[u].LoaiGiaoDich == 4 && macho.Contains(VeThuongGD[u].MaCho))
                                                VeThuongGD[u].LoaiGiaoDich = 13;
                                        }
                                    } // xóa dữ liệu đã tồn tại
                                }//vé thường |xử lý thêm chi tiết vào vé

                                if (VeThuongGD.Count == 0) Close();
                                else
                                {
                                    VeThuongGD = VeThuongGD.OrderBy(x => x.MaCho).ToList();
                                    MaCho = VeThuongGD.Select(x => x.MaCho).Distinct().ToList();
                                    wVJ.Navigate("https://agents.vietjetair.com/SearchRes.aspx?lang=vi&st=sl&sesid=");
                                }
                            }
                            break;
                    }
                    StepLoad++;
                }//lấy bản báo cáo
                else if (wVJ.Url.AbsolutePath.Contains("/AgentOptions.aspx"))//Đăng nhập thành công
                    wVJ.Navigate("https://agents.vietjetair.com/CompanyPayRpt.aspx?lang=vi&st=sl&sesid=");
                else if (wVJ.Url.AbsolutePath.Contains("/sitelogin.aspx"))
                {
                    head = wVJ.Document.GetElementById("wrapper");
                    if (head != null)
                        if (head.InnerText.ToLower().Contains("mật khẩu chưa đúng") || head.InnerText.ToLower().Contains("wrong password input"))
                            Close();

                    if (wVJ.Document.Body.InnerHtml == null)
                        return;
                    if (!wVJ.Document.Body.InnerHtml.Contains("txtAgentID"))
                        return;

                    wVJ.Document.GetElementById("txtAgentID").SetAttribute("value", _NCCO.TaiKhoan);
                    wVJ.Document.GetElementById("txtAgentPswd").SetAttribute("value", _NCCO.MatKhau);
                    wVJ.Document.GetElementById("SiteLogin").InvokeMember("submit");
                }//đăng nhập
                else if (wVJ.Url.AbsolutePath.Contains("/Home"))
                {
                    element.text = "function doPost() { location.href = 'https://agents.vietjetair.com/sitelogin.aspx?lang=vi'; }";
                    head.AppendChild(scriptEl);
                    wVJ.Document.InvokeScript("doPost");
                }//Vào trang đăng nhập
            }
        }

        void Xuli()
        {
            MaCho.Remove(MaCho.First());
            if (MaCho.Count > 0)
            {
                gdD.Them(VeThem);
                VeThem.Clear();
                wVJ.Navigate("https://agents.vietjetair.com/SearchRes.aspx?lang=vi&st=sl&sesid=");
            }
            else if (VeThem.Count > 0)
            {
                gdD.Them(VeThem);
                VeThem.Clear();
                RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true);
                RegKey.SetValue("Display Inline Images", "yes");
                Close();
            }
        }

        private HtmlElement GetElementByClass(string tag, string classname)
        {
            HtmlElementCollection eles = wVJ.Document.GetElementsByTagName(tag);
            foreach (HtmlElement ele in eles)
            {
                if (ele.GetAttribute("className") == classname)
                {
                    return ele;
                }
            }
            return null;
        }
    }

    public class TableVJ
    {
        public int LEG { set; get; }
        public string TEN { set; get; }
        public string MOTA { set; get; }
        public int SOTIEN { set; get; }
        public int VAT { set; get; }
        public int TONG { set; get; }
    }
}
