using DataAccessLayer;
using DataTransferObject;
using mshtml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AutoLoadTicket
{
    public partial class frmVJ2 : Form
    {
        List<GiaoDichO> VeThuongGD = new List<GiaoDichO>();
        List<GiaoDichO> _Mail = new List<GiaoDichO>();
        List<string> MaCHo = new List<string>();
        int _NCC = 0;
        public frmVJ2(List<GiaoDichO> a, List<string> n, int _ncc)
        {
            InitializeComponent();
            VeThuongGD = a;
            MaCHo = n;
            _NCC = _ncc;
        }


        NCCO hb = new NCCO();
        GiaoDichD gd = new GiaoDichD();
        DaiLyD dlD = new DaiLyD();
        List<DaiLyO> lstDaiLy = new List<DaiLyO>();
        List<GiaoDichO> VeThem = new List<GiaoDichO>();
        List<GiaoDichO> BanTam = new List<GiaoDichO>();
        int nSearch = 0;
        private void frmVJ2_Load(object sender, EventArgs e)
        {
            _Mail = new GiaoDichD().KhachEMAILVJ2(_NCC);
            lstDaiLy = new DaiLyD().All();
            wVJ.Navigate("https://www.vietjetair.com/Sites/Web/vi-VN/Home");
            while (wVJ.IsBusy)
            {
                Application.DoEvents();
            }
            hb = new NCCD().DuLieu().Where(w => w.ID.Equals(_NCC)).First();
        }

        void Xuli()
        {
            MaCHo.Remove(MaCHo.First());
            if (MaCHo.Count > 0)
            {
                if (VeThem.Count > 5)
                { gd.Them(VeThem); VeThem.Clear(); }
                wVJ.Navigate("https://agents.vietjetair.com/SearchRes.aspx?lang=vi&st=sl&sesid=");
            }
            else if (VeThem.Count > 0)
            { gd.Them(VeThem); VeThem.Clear(); Close(); }
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

        private void wVJ_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wVJ.ReadyState == WebBrowserReadyState.Complete && !wVJ.IsBusy)
            {
                Thread.Sleep(300);
                Text = wVJ.Url.ToString();
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

                        foreach (HtmlElement htmlElement in tblGiao_trs)
                        {
                            HtmlElementCollection g = htmlElement.GetElementsByTagName("td");
                            if (g.Count > 3)
                                if (g[2].InnerText.Contains("Add Ons") && g[2].InnerText.Contains("Bag") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].LoaiGiaoDich = 14;
                                        BanTam[i].Fare = 0;
                                        BanTam[i].BiDanh = g[2].InnerText.Split('-')[1].Replace(" ", string.Empty).ToUpper().Replace("KGS", string.Empty);
                                        break;
                                    }
                                }
                                else if (g[2].InnerText.Contains("ROUTING - MODIFICATION") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].LoaiGiaoDich = 13;
                                        BanTam[i].BiDanh = "RM";
                                        break;
                                    }
                                }
                                else if (g[2].InnerText.Contains("PASSENGER - CHANGE") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].LoaiGiaoDich = 13;
                                        BanTam[i].BiDanh = "PC";
                                        break;
                                    }
                                }
                                else if (g[2].InnerText.Contains("INFANT CHARGE DOM") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].BiDanh = "INFANT";
                                        break;
                                    }
                                }
                                else if (g[2].InnerText.Contains("AirportTax Child Dom") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].BiDanh = "CD";
                                        break;
                                    }
                                }
                                else if (g[2].InnerText.Contains("Seat Assignment") && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) > 0)
                                {
                                    if (g[1].InnerText.Replace(",", string.Empty).Equals(BanTam[i].TenKhach) && long.Parse(new String(g[5].InnerText.Where(Char.IsDigit).ToArray())) == BanTam[i].GiaNet)
                                    {
                                        BanTam[i].BiDanh = "SEAT";
                                        break;
                                    }
                                }
                        }
                        if (BanTam[i].LoaiKhachHang == 0 && !(BanTam[i].EmailKhachHang ?? string.Empty).Contains("@THANHHOANG.VN"))
                        {
                            if (_Mail.Where(w => w.EmailKhachHang.ToLower().Contains((BanTam[i].EmailKhachHang ?? string.Empty).ToLower()) && (BanTam[i].EmailKhachHang ?? string.Empty).ToLower().Length > 5).Count() > 0)
                            {
                                BanTam[i].LoaiKhachHang = 1;
                                BanTam[i].IDKhachHang = _Mail.Where(w => w.EmailKhachHang.ToLower().Contains(BanTam[i].EmailKhachHang.ToLower())).ToList()[0].IDKhachHang;
                            }
                        }

                        VeThem.Add(BanTam[i]);
                    }

                    Xuli();
                }
                else if (wVJ.Url.AbsolutePath.Contains("/EditRes.aspx"))
                {
                    BanTam = VeThuongGD.Where(w => w.MaCho.Equals(MaCHo.First())).ToList();

                    HtmlElement CD = wVJ.Document.GetElementById("leg1");
                    HtmlElement CV = wVJ.Document.GetElementById("leg2");

                    for (int i = 0; i < BanTam.Count; i++)
                    {
                        if (CD != null)
                        {
                            if (CD.Document.GetElementById("grdPaxFareDetails") != null)
                            {
                                HtmlElementCollection CDCellData1 = CD.Document.GetElementById("grdPaxFareDetails").GetElementsByTagName("TD");
                                BanTam[i].Fare = long.Parse(new String(CDCellData1[2].InnerText.Where(Char.IsDigit).ToArray()));
                                BanTam[i].LoaiVeDi = CDCellData1[1].InnerText.Split('-')[1].Replace(" ", string.Empty);
                            }
                            else
                            {
                                BanTam[i].LoaiVeDi = "ECO";
                                BanTam[i].Fare = 0;
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
                            if (CV.GetElementsByTagName("tbody")[0].Document.GetElementById("grdPaxFareDetails") != null)
                            {
                                HtmlElementCollection CVCellData1 = CV.GetElementsByTagName("tbody")[0].Document.GetElementById("grdPaxFareDetails").GetElementsByTagName("TD");
                                BanTam[i].Fare += long.Parse(new String(CVCellData1[2].InnerText.Where(Char.IsDigit).ToArray()));
                                BanTam[i].LoaiVeVe = CVCellData1[1].InnerText.Split('-')[1].Replace(" ", string.Empty);
                            }

                            if (CV.GetElementsByTagName("tbody")[0].Document.GetElementById("grdFlightInfo") != null)
                            {
                                HtmlElementCollection CVCellData2 = CV.GetElementsByTagName("tbody")[0].Document.GetElementById("grdFlightInfo").GetElementsByTagName("TD");
                                BanTam[i].SoHieuVe = CVCellData2[1].InnerText;
                                BanTam[i].GioBayVe = DateTime.ParseExact(CVCellData2[0].InnerText.Substring(0, CVCellData2[0].InnerText.IndexOf(' ')) + " " + CVCellData2[2].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                                BanTam[i].GioBayVe_Den = DateTime.ParseExact(CVCellData2[0].InnerText.Substring(0, CVCellData2[0].InnerText.IndexOf(' ')) + " " + CVCellData2[3].InnerText.Split(' ')[0], "dd/MM/yyyy H:mm", null);
                            }
                        }
                    }

                    element.text = "function doPost() { document.forms['Edit'].button.value='newitinerary';SubmitForm(); }";
                    head.AppendChild(scriptEl);
                    wVJ.Document.InvokeScript("doPost");
                }
                else if (wVJ.Url.AbsolutePath.Contains("/SearchRes.aspx"))
                {
                    if (nSearch == 0)
                    {
                        nSearch++;
                        wVJ.Document.GetElementById("txtSearchResNum").SetAttribute("value", MaCHo.First());
                        wVJ.Document.GetElementById("Search").InvokeMember("submit");
                    }
                    else
                    {
                        nSearch = 0;
                        HtmlElement error = GetElementByClass("p", "ErrorCaption");

                        if (error != null)
                        {
                            VeThem.AddRange(VeThuongGD.Where(w => w.MaCho.Equals(MaCHo.First())).ToList());
                            MaCHo.Remove(MaCHo.First());
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
                else if (wVJ.Url.AbsolutePath.Contains("/AgentOptions.aspx"))//Đăng nhập thành công
                {
                    Text = "Đăng nhập hệ thống Vietjet Air thành công...";
                    wVJ.Navigate("https://agents.vietjetair.com/SearchRes.aspx?lang=vi&st=sl&sesid=");
                }//
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

                    wVJ.Document.GetElementById("txtAgentID").SetAttribute("value", hb.TaiKhoan);
                    wVJ.Document.GetElementById("txtAgentPswd").SetAttribute("value", hb.MatKhau);
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
    }
}
