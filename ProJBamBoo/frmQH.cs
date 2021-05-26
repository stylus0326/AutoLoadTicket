using DataAccessLayer;
using DataTransferObject;
using mshtml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ProJBamBoo
{
    public partial class frmQH : Form
    {
        GiaoDichD gd = new GiaoDichD();
        List<SignInO> lstSignIn = new List<SignInO>();
        List<DaiLyO> lstDaiLy = new List<DaiLyO>();
        List<TuyenBayO> lstTuyenBay = new List<TuyenBayO>();
        DateTime NgayChay = new DateTime();
        NCCO _NCCO = new NCCO();
        DaiLyO dl = new DaiLyO();
        public frmQH(DateTime dtp)
        {
            InitializeComponent();
            NgayChay = dtp;
        }

        private void frmQH_Load(object sender, EventArgs e)
        {
            lstSignIn = new SignInD().DuLieu();
            lstTuyenBay = new TuyenBayD().DuLieu();
            lstDaiLy = new DaiLyD().All();
            wQH.Navigate("https://www.bambooairways.com/reservation/ibe/login");
            while (wQH.IsBusy)
            {
                Application.DoEvents();
            }
            _NCCO = new NCCD().DuLieu().Where(w => w.ID.Equals(5)).ToList()[0];
        }

        int StepLoad = 0;
        GiaoDichO ClassTamLuu = new GiaoDichO();
        List<GiaoDichO> VeThem = new List<GiaoDichO>();
        List<GiaoDichO> VeThuHoGD = new List<GiaoDichO>();
        List<GiaoDichO> VeThuongGD = new List<GiaoDichO>();
        List<GiaoDichO> VeHoanGD = new List<GiaoDichO>();
        List<GiaoDichO> BanTam = new List<GiaoDichO>();
        List<string> MaCHo = new List<string>();

        private void wQH_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wQH.ReadyState == WebBrowserReadyState.Complete && !wQH.IsBusy)
            {
                Text = wQH.Url.ToString();
                HtmlElement head1 = wQH.Document.GetElementsByTagName("head")[0];
                HtmlElement scriptEl = wQH.Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                if (wQH.Document.Body.InnerHtml.Contains("502") && wQH.Document.Body.InnerHtml.Contains("Bad gateway"))
                {
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 8");//Temporary Internet Files
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2");//Cookies
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 1");//History
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 11");//Form Data
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 32");//Passwords
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");//Delete All
                    Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 4351");//Also delete files and settings stored by add-ons}
                    wQH.Navigate("https://www.bambooairways.com/reservation/ibe/login");
                }
                else if (wQH.Url.AbsolutePath.Contains("/modify"))
                {
                    Thread.Sleep(2000);
                    HtmlElement ele = wQH.Document.GetElementById("form-flights");
                    HtmlElementCollection elec = ele.GetElementsByTagName("p");
                    foreach (HtmlElement eles in elec)
                    {
                        if (eles.InnerText.Contains("Phone Number"))
                            ClassTamLuu.DienThoaiKhachHang = eles.InnerText.Split(':')[1];
                        if (eles.InnerText.Contains("E-Mail"))
                            ClassTamLuu.EmailKhachHang = eles.InnerText.Split(':')[1];
                    }

                    List<DaiLyO> Dl = lstDaiLy.Where(w => (w.EmailGiaoDich ?? string.Empty).ToUpper().Contains((ClassTamLuu.EmailKhachHang ?? string.Empty).ToUpper())
                           && (w.EmailGiaoDich ?? string.Empty).Length > 0 && (ClassTamLuu.EmailKhachHang ?? string.Empty).Length > 0).ToList();
                    if (Dl.Count > 0)
                    {
                        ClassTamLuu.IDKhachHang = Dl[0].ID;
                        ClassTamLuu.LoaiKhachHang = Dl[0].LoaiKhachHang;
                    }

                    for (int i = 0; i < BanTam.Count; i++)
                    {
                        if (BanTam[i].LoaiKhachHang != 1)
                        {
                            BanTam[i].LoaiKhachHang = ClassTamLuu.LoaiKhachHang;
                            BanTam[i].IDKhachHang = ClassTamLuu.IDKhachHang;
                        }

                        BanTam[i].LoaiVeDi = ClassTamLuu.LoaiVeDi;
                        BanTam[i].GioBayDi_Den = ClassTamLuu.GioBayDi_Den;
                        BanTam[i].GioBayDi = ClassTamLuu.GioBayDi;
                        BanTam[i].HanhLyDi = "0KG";

                        if ((BanTam[i].BiDanh ?? string.Empty).Contains("Reissue"))
                            BanTam[i].LoaiGiaoDich = 13;
                        else if ((BanTam[i].BiDanh ?? string.Empty).Contains("BAG"))
                            BanTam[i].LoaiGiaoDich = 14;

                        BanTam[i].EmailKhachHang = ClassTamLuu.EmailKhachHang;
                        if (ClassTamLuu.DienThoaiKhachHang.Length > 8)
                            BanTam[i].DienThoaiKhachHang = "0" + ClassTamLuu.DienThoaiKhachHang.Substring(0, 9);

                        VeThem.Add(BanTam[i]);
                        break;
                    }

                    Xuli(head1, scriptEl, element);
                }
                else if (wQH.Url.AbsolutePath.Contains("/recap"))
                {
                    Thread.Sleep(2000);

                    if (!wQH.DocumentText.Contains("Back to PNR"))
                    {
                        if ((ClassTamLuu.EmailKhachHang ?? string.Empty).Length == 0) // Lấy Email
                            timer.Start();
                        else
                        {
                            List<string> lstStr = new List<string>();
                            try
                            {
                                int Faredi = 0;
                                int FareVe = 0;
                                int SoNguoi = BanTam.Where(w => !(w.BiDanh ?? "").Contains("INFANT")).Count();
                                HtmlElementCollection ele = wQH.Document.GetElementsByTagName("tbody");
                                for (int i = 1; i < ele.Count; i++)
                                {
                                    int row = 0;
                                    foreach (HtmlElement RowEle in ele[i].GetElementsByTagName("tr"))
                                    {
                                        HtmlElementCollection RowHEle = RowEle.GetElementsByTagName("td");
                                        switch (i)
                                        {
                                            case 1:
                                                lstStr.Add(RowHEle[1].InnerText + " " + RowHEle[0].InnerText + "|" + RowHEle[2].InnerText.Replace(" ", string.Empty));
                                                break;
                                            case 2:
                                                try
                                                {
                                                    if (row == 2)
                                                    {
                                                        if (BanTam[0].SoLuongVe == 2)
                                                            if (RowHEle[4] != null)
                                                                if (RowHEle[4].InnerText != null)
                                                                {
                                                                    FareVe = int.Parse(new String((RowHEle[7].InnerText ?? "0").Where(Char.IsDigit).ToArray())) / SoNguoi;
                                                                    ClassTamLuu.LoaiVeVe = RowHEle[4].InnerText.Replace("Bamboo ", string.Empty);
                                                                    ClassTamLuu.GioBayVe = DateTime.ParseExact(RowHEle[9].InnerText + RowHEle[10].InnerText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                                                    ClassTamLuu.GioBayVe_Den = DateTime.ParseExact(RowHEle[9].InnerText + RowHEle[11].InnerText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                                                }
                                                    }
                                                    else if (row == 0)
                                                    {
                                                        if (RowHEle[4] != null)
                                                            if (RowHEle[4].InnerText != null)
                                                            {
                                                                Faredi = int.Parse(new String((RowHEle[7].InnerText ?? "0").Where(Char.IsDigit).ToArray())) / SoNguoi;
                                                                ClassTamLuu.LoaiVeDi = RowHEle[4].InnerText.Replace("Bamboo ", string.Empty);
                                                                ClassTamLuu.GioBayDi = DateTime.ParseExact(RowHEle[9].InnerText + RowHEle[10].InnerText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                                                ClassTamLuu.GioBayDi_Den = DateTime.ParseExact(RowHEle[9].InnerText + RowHEle[11].InnerText, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                                            }
                                                    }
                                                }
                                                catch { }
                                                row++;
                                                break;
                                            case 4:
                                                if (RowHEle[3].InnerText.Contains("Free "))
                                                {
                                                    string[] ten = RowHEle[1].InnerText.Split(',');
                                                    string TenKhach = ten[1] + ten[0];
                                                    int index = lstStr.FindIndex(a => a.Replace(" ", string.Empty).Contains(TenKhach.Replace(" ", string.Empty)));
                                                    lstStr[index] += "|" + RowHEle[0].InnerText;
                                                }
                                                break;
                                        }
                                    }
                                }

                                for (int i = 0; i < BanTam.Count; i++)
                                {
                                    foreach (string str in lstStr)
                                    {
                                        string[] str1 = str.Split('|');
                                        if (BanTam[i].LoaiKhachHang != 1)
                                        {
                                            BanTam[i].LoaiKhachHang = ClassTamLuu.LoaiKhachHang;
                                            BanTam[i].IDKhachHang = ClassTamLuu.IDKhachHang;
                                        }

                                        BanTam[i].LoaiVeDi = ClassTamLuu.LoaiVeDi;
                                        BanTam[i].GioBayDi_Den = ClassTamLuu.GioBayDi_Den;
                                        BanTam[i].GioBayDi = ClassTamLuu.GioBayDi;

                                        if (BanTam[i].SoLuongVe == 2)
                                        {
                                            BanTam[i].LoaiVeVe = ClassTamLuu.LoaiVeVe;
                                            BanTam[i].GioBayVe_Den = ClassTamLuu.GioBayVe_Den;
                                            BanTam[i].GioBayVe = ClassTamLuu.GioBayVe;
                                            if (str1.Length > 3)
                                                BanTam[i].HanhLyVe = str1[3];
                                            else
                                                BanTam[i].HanhLyVe = "0KG";
                                        }

                                        BanTam[i].EmailKhachHang = ClassTamLuu.EmailKhachHang;
                                        if (ClassTamLuu.DienThoaiKhachHang.Length > 8)
                                            BanTam[i].DienThoaiKhachHang = "0" + ClassTamLuu.DienThoaiKhachHang.Substring(0, 9);

                                        if (str1[0].Replace(" ", string.Empty) == BanTam[i].TenKhach.Replace(" ", string.Empty) || (lstStr.Count == 1 && BanTam.Count == 1))
                                        {
                                            if (BanTam[i].BiDanh == null)
                                                BanTam[i].BiDanh = str1[1];

                                            if (str1.Length > 3)
                                                BanTam[i].HanhLyDi = str1[2];
                                            else
                                                BanTam[i].HanhLyDi = "0KG";

                                            if (!(BanTam[i].BiDanh ?? "").Equals("INFANT"))
                                                BanTam[i].Fare = BanTam[i].Fare - Faredi - FareVe;

                                            if (BanTam[i].Fare < 0)
                                                BanTam[i].Fare = 0;

                                            VeThem.Add(BanTam[i]);
                                            break;
                                        }

                                    }
                                }
                            }
                            catch { VeThem.AddRange(BanTam); }
                            finally { Xuli(head1, scriptEl, element); }

                        }
                    }
                    else
                    {
                        List<List<string>> lststr = new List<List<string>>();
                        HtmlElementCollection eles = wQH.Document.GetElementsByTagName("tr");
                        foreach (HtmlElement ele in eles)
                        {
                            if (ele.InnerText.Contains("PAX_HOME_CONTACT"))
                            {
                                List<string> lstStrA = ele.InnerText.Split(' ').ToList();
                                for (int o = 0; o < lstStrA.Count; o++)
                                {
                                    if (lstStrA[o].Contains("@"))
                                    {
                                        ClassTamLuu.EmailKhachHang = lstStrA[o];
                                        ClassTamLuu.DienThoaiKhachHang = lstStrA[o + 1];
                                    }
                                }
                            }
                        }// lấy mail

                        List<DaiLyO> Dl = lstDaiLy.Where(w => (w.EmailGiaoDich ?? string.Empty).Contains(ClassTamLuu.EmailKhachHang ?? string.Empty)
                           && (w.EmailGiaoDich ?? string.Empty).Length > 0 && (ClassTamLuu.EmailKhachHang ?? string.Empty).Length > 0).ToList();
                        if (Dl.Count > 0)
                        {
                            ClassTamLuu.IDKhachHang = Dl[0].ID;
                            ClassTamLuu.LoaiKhachHang = Dl[0].LoaiKhachHang;
                        }

                        timer.Start();
                    }

                }//Chi tiết
                else if (wQH.Url.AbsolutePath.Contains("/agent"))
                {
                    switch (StepLoad)
                    {
                        case 0:
                            element.text = @"function doPost() {createInvoice(event);}";
                            head1.AppendChild(scriptEl);
                            wQH.Document.InvokeScript("doPost");
                            break;
                        case 1:
                            wQH.Document.GetElementById("ir-start-date").SetAttribute("value", NgayChay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            wQH.Document.GetElementById("ir-end-date").SetAttribute("value", NgayChay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            element.text = @"function doPost() {doInvoice('/reservation','vi',true);}";
                            head1.AppendChild(scriptEl);
                            wQH.Document.InvokeScript("doPost");
                            break;
                        case 2:
                            if (NgayChay.Subtract(DateTime.Now).Days == 0)
                            {
                                element.text = @"function doPost() {viewdetails('0', 'VND', 'AG', '3780054');}";
                                head1.AppendChild(scriptEl);
                                wQH.Document.InvokeScript("doPost");
                                StepLoad++;
                            }
                            else
                                wQH.Document.GetElementById("invoicedetails").InvokeMember("click");
                            break;
                        case 4:
                            #region lấy tất cả dòng dữ liệu để xử lí
                            HtmlElementCollection ele = wQH.Document.GetElementsByTagName("tr");
                            foreach (HtmlElement eles in ele)
                            {
                                HtmlElementCollection elez = eles.GetElementsByTagName("td");
                                if (elez.Count == 0)
                                    continue;

                                #region thông tin chung
                                GiaoDichO gd = new GiaoDichO();
                                gd.Hang = "QH";
                                gd.NhaCungCap = 5;
                                gd.LoaiKhachHang = 0;
                                gd.SoVeVN = elez[6].InnerText;
                                gd.MaCho = elez[4].InnerText;
                                gd.NgayGD = gd.NgayCuonChieu = DateTime.ParseExact(elez[0].InnerText, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                gd.Agent = elez[3].InnerText.Split('@')[0];
                                string[] ten = elez[7].InnerText.Split('/');
                                gd.TenKhach = ten[1] + " " + ten[0];

                                if (elez[8].InnerText.Equals("INFANT"))
                                    gd.BiDanh = elez[8].InnerText;
                                else
                                    gd.BiDanh = elez[12].InnerText;

                                List<string> HanhTrinh = (elez[23].InnerText + ";").Replace(" ", string.Empty).Split(';').ToList();

                                if (HanhTrinh.Count > 3)
                                    HanhTrinh.Remove(HanhTrinh[1]);

                                if (HanhTrinh[0].Length > 7)
                                {
                                    if (gd.MaCho == "EG66DA")
                                        gd.SoLuongVe = 0;
                                    gd.SoLuongVe = HanhTrinh.Count - 1;
                                    if (lstTuyenBay.Where(w => HanhTrinh[0].Substring(HanhTrinh[0].Length - 7).Equals(w.Ten)).Count() > 0)
                                    {
                                        gd.SoHieuDi = "QH" + long.Parse(new String(HanhTrinh[0].Substring(10, 7).Where(Char.IsDigit).ToArray()));
                                        gd.GioBayDi_Den = gd.GioBayDi = DateTime.ParseExact(HanhTrinh[0].Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        gd.TuyenBayDi = lstTuyenBay.Where(w => HanhTrinh[0].Contains(w.Ten)).ToList()[0].ID;
                                    }
                                    else
                                        gd.GhiChu = HanhTrinh[0];

                                    if (HanhTrinh.Count > 2)
                                    {
                                        if (lstTuyenBay.Where(w => HanhTrinh[1].Substring(HanhTrinh[1].Length - 7).Equals(w.Ten)).Count() > 0)
                                        {
                                            gd.SoHieuVe = "QH" + long.Parse(new String(HanhTrinh[1].Substring(10, 7).Where(Char.IsDigit).ToArray()));
                                            gd.GioBayVe_Den = gd.GioBayVe = DateTime.ParseExact(HanhTrinh[1].Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            gd.TuyenBayVe = lstTuyenBay.Where(w => HanhTrinh[1].Contains(w.Ten)).ToList()[0].ID;
                                        }
                                        else
                                            gd.GhiChu += "-" + HanhTrinh[1];
                                    }
                                }
                                #endregion

                                if (long.Parse(elez[15].InnerText.Replace(",", string.Empty)) < 0)
                                {
                                    gd.TenKhach += " /Hoàn vé";
                                    gd.GiaHoan = gd.HangHoan = long.Parse(new String(elez[15].InnerText.Where(Char.IsDigit).ToArray()));

                                    gd.LoaiGiaoDich = 9;
                                    VeHoanGD.Add(gd);
                                }
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

                                    gd.Fare = long.Parse(new String(elez[9].InnerText.Where(Char.IsDigit).ToArray()));
                                    gd.GiaThu = gd.GiaHeThong = gd.GiaNet = long.Parse(new String(elez[15].InnerText.Where(Char.IsDigit).ToArray()));
                                    if (elez[12].InnerText != null)//Hành lí
                                    {
                                        switch (elez[12].InnerText)
                                        {
                                            case "XBAG":
                                            case "Bas":
                                            case "SEAT":
                                                gd.LoaiGiaoDich = 14;
                                                break;
                                            case "Reissue":
                                                gd.LoaiGiaoDich = 13;
                                                break;
                                        }
                                        VeThuHoGD.Add(gd);
                                    }
                                    else
                                        VeThuongGD.Add(gd);
                                }
                            }
                            #endregion

                            List<GiaoDichO> GD = new List<GiaoDichO>();
                            if (VeHoanGD.Count > 0)
                            {
                                GD = gd.VeHoan(VeHoanGD, 5);// lấy dữ liệu đã tồn tại

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
                                    GD = gd.LayDaiLyVeHoan(VeHoanGD, 5);
                                    for (int i = 0; i < VeHoanGD.Count; i++)
                                    {
                                        GiaoDichO gd = VeHoanGD[i];
                                        foreach (GiaoDichO gdo in GD)
                                        {
                                            if (gdo.MaCho == gd.MaCho)
                                            {
                                                if (gdo.LoaiKhachHang == 1)
                                                {
                                                    gd.LoaiKhachHang = gdo.LoaiKhachHang;
                                                    gd.IDKhachHang = gdo.IDKhachHang;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    gd.Them(VeHoanGD);
                                } //Tìm vé trước đó gán tên đl và thêm
                            }//vé hoàn |thêm ngay

                            if (VeThuHoGD.Count > 0)
                            {
                                GD = gd.VeThuong(VeThuHoGD, 5);// lấy dữ liệu đã tồn tại

                                for (int i = 0; i < GD.Count; i++)
                                {
                                    for (int u = 0; u < VeThuHoGD.Count; u++)
                                    {
                                        if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeThuHoGD[u].NgayGD.ToString("ddMMyy"))
                                            && GD[i].GiaNet == VeThuHoGD[u].GiaNet && GD[i].MaCho == VeThuHoGD[u].MaCho
                                            && GD[i].TenKhach == VeThuHoGD[u].TenKhach && GD[i].BiDanh == VeThuHoGD[u].BiDanh)
                                        {
                                            GD.Remove(GD[i]);
                                            VeThuHoGD.Remove(VeThuHoGD[u]);
                                            i--;
                                            u--;
                                            break;
                                        }
                                    }
                                } // xóa dữ liệu đã tồn tại
                                if (VeThuHoGD.Count > 0)
                                    gd.Them(VeThuHoGD);
                            }//hành lí |thêm ngay

                            if (VeThuongGD.Count > 0)
                            {
                                List<string> macho = gd.MaCho(VeThuongGD, 5).Distinct().ToList();

                                VeThuongGD = VeThuongGD.OrderBy(w => w.SoVeVN).ToList();
                                GD = gd.VeThuong(VeThuongGD, 5).OrderBy(w => w.SoVeVN).ToList();// lấy dữ liệu đã tồn tại

                                for (int i = 0; i < GD.Count; i++)
                                {
                                    for (int u = 0; u < VeThuongGD.Count; u++)
                                    {
                                        if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeThuongGD[u].NgayGD.ToString("ddMMyy"))
                                            && GD[i].GiaNet == VeThuongGD[u].GiaNet && GD[i].SoVeVN == VeThuongGD[u].SoVeVN
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
                                MaCHo = VeThuongGD.Select(x => x.MaCho).Distinct().ToList();
                                element.text = @"function doPost() {bookingList(event);}";
                                head1.AppendChild(scriptEl);
                                wQH.Document.InvokeScript("doPost");
                            }

                            break;
                        case 5:
                            wQH.Document.GetElementById("bl-pnr-number").SetAttribute("value", MaCHo.First());
                            wQH.Document.GetElementById("proceed").InvokeMember("click");
                            break;
                        case 6:
                            element.text = @"function doPost() { doRetrievePnr('" + MaCHo.First() + "') }";
                            head1.AppendChild(scriptEl);
                            BanTam = VeThuongGD.Where(w => w.MaCho.Equals(MaCHo.First())).ToList();
                            wQH.Document.InvokeScript("doPost");
                            break;
                    }

                    StepLoad++;
                }//lấy bản báo cáo
                else if (wQH.Url.AbsolutePath.Contains("/login"))
                {

                    wQH.Document.GetElementById("login-agency-code").SetAttribute("value", _NCCO.MaHang);
                    wQH.Document.GetElementById("login-agency-id").SetAttribute("value", _NCCO.TaiKhoan);
                    wQH.Document.GetElementById("login-password").SetAttribute("value", _NCCO.MatKhau);
                    element.text = @"function doPost() {submitLoginForm('en_US');}";
                    head1.AppendChild(scriptEl);
                    wQH.Document.InvokeScript("doPost");
                }//đăng nhập
            }
        }

        int Secon = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            HtmlElement head1 = wQH.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = wQH.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            Secon++;
            if (Secon == 2)
            {
                if (GetElementByClass("a", "pnrHistory hideSection-Awings") != null)
                    element.text = "function doPost2() {document.getElementsByClassName('pnrHistory hideSection-Awings')[0].click();}";
                else
                    element.text = "function doPost2() {document.getElementsByClassName('pnrHistory')[0].click();}";
                head1.AppendChild(scriptEl);
                wQH.Document.InvokeScript("doPost2");
                timer.Stop();
            }
            else if (Secon == 4)
            {
                try
                {
                    if (!wQH.DocumentText.Contains("Single Booking Lists"))
                    {
                        wQH.Document.GetElementById("pnrHistory").InvokeMember("click");
                        timer.Stop();
                        Secon = 0;
                    }
                    else if (wQH.DocumentText.Contains("An error has occurred. Sorry for the inconvenience caused. Please try after some time."))
                    {
                        VeThem.AddRange(BanTam);
                        Xuli(head1, scriptEl, element);
                        Secon = 0;
                    }
                }
                catch
                { }
            }
        }

        void Xuli(HtmlElement head1, HtmlElement scriptEl, IHTMLScriptElement element)
        {
            MaCHo.Remove(MaCHo.First());
            ClassTamLuu = new GiaoDichO();
            if (VeThem.Count == 0 && BanTam.Count > 0)
                VeThem.AddRange(BanTam);
            gd.Them(VeThem); VeThem.Clear();
            if (MaCHo.Count > 0)
            {
                BanTam = VeThuongGD.Where(w => w.MaCho.Equals(MaCHo.First())).ToList();
                element.text = @"function doPost1() {goToPnr('" + MaCHo.First() + "')}";
                head1.AppendChild(scriptEl);
                wQH.Document.InvokeScript("doPost1");
            }
            else
                Close();
        }

        private HtmlElement GetElementByClass(string tag, string classname)
        {
            HtmlElementCollection eles = wQH.Document.GetElementsByTagName(tag);
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
}
