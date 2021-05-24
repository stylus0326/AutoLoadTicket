using DataAccessLayer;
using DataTransferObject;
using Microsoft.Win32;
using ProJBamBoo;
using ProJSoDu;
using ProJVietTravel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VietJetProJ;
using VietNamProJ;

namespace AutoLoadTicket
{
    public partial class frmAutoLoadTicket : Form
    {
        public frmAutoLoadTicket()
        {
            InitializeComponent();
        }

        private void frmAutoLoadTicket_Load(object sender, EventArgs e)
        {
            Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            foreach (var process in Process.GetProcessesByName("ProJSoDu"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("ProJBamBoo"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("VietJetProJ"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("VietNamProJ"))
            { process.Kill(); }
        }

        private void btnVJ_Click(object sender, EventArgs e)
        {
            new frmVJ(dtpVJ.Value).Show();
        }

        private void btnQH_Click(object sender, EventArgs e)
        {
            new frmQH(dtpQH.Value).Show();
        }

        private void btnVN_Click(object sender, EventArgs e)
        {
            new frmVN(dtpVN.Value).Show();
        }

        private void chkAuto_CheckedChanged(object sender, EventArgs e)
        {
            TimerLayDuLieu.Enabled = true;
        }

        void VIetJetO(int _NCCC)
        {
            List<GiaoDichO> VeThuongGD = new List<GiaoDichO>();
            List<GiaoDichO> VeHoanGD = new List<GiaoDichO>();
            List<TuyenBayO> lstTuyenBay = new List<TuyenBayO>();
            lstTuyenBay = new TuyenBayD().DuLieu();
            string sysUIFormat = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Mở File";
            ofd.Filter = " Excel File (*.xls, *.xlsx) | *.xls; *.xlsx";
            ofd.DefaultExt = ".xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                string ChuoiKetNoi = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "; Extended Properties='Excel 12.0 Macro;HDR=YES';";

                using (OleDbConnection conn = new OleDbConnection(ChuoiKetNoi))
                {
                    conn.Open();
                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string CauTruyVan = "SELECT * FROM [" + dbSchema.Rows[0].Field<string>("TABLE_NAME").Replace("'", string.Empty) + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(CauTruyVan, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row[0].ToString().Length < 3)
                            continue;
                        #region thông tin chung
                        GiaoDichO gdb = new GiaoDichO();
                        gdb.Hang = "VJ";
                        gdb.NhaCungCap = _NCCC;
                        gdb.LoaiKhachHang = 0;
                        gdb.MaCho = row[0].ToString();
                        gdb.NgayCuonChieu = gdb.NgayGD = DateTime.ParseExact(row[2].ToString().Substring(0, row[2].ToString().IndexOf(' ')), "dd/MM/yyyy", null);
                        gdb.TenKhach = row[1].ToString().Replace(",", string.Empty);

                        string[] HanhTrinh = row[3].ToString().Replace("CONF", "|").Replace("CANX", "|").Replace(" ", string.Empty).Split('|');

                        gdb.SoLuongVe = HanhTrinh.Length - 1;
                        gdb.SoHieuDi = "VJ";
                        if (lstTuyenBay.Where(w => HanhTrinh[0].Substring(HanhTrinh[0].Length - 7).Equals(w.Ten)).Count() > 0)
                            gdb.TuyenBayDi = lstTuyenBay.Where(w => HanhTrinh[0].Contains(w.Ten)).ToList()[0].ID;
                        else
                            gdb.GhiChu = HanhTrinh[0];

                        if (HanhTrinh.Length > 2)
                        {
                            gdb.SoHieuVe = "VJ";
                            if (lstTuyenBay.Where(w => HanhTrinh[1].Substring(HanhTrinh[1].Length - 7).Equals(w.Ten)).Count() > 0)
                                gdb.TuyenBayVe = lstTuyenBay.Where(w => HanhTrinh[1].Contains(w.Ten)).ToList()[0].ID;
                            else
                                gdb.GhiChu += "-" + HanhTrinh[1];
                        }
                        #endregion

                        if (row[6].ToString().Contains("($"))
                        {
                            gdb.TenKhach += " /Hoàn vé";
                            gdb.GiaHoan = gdb.HangHoan = long.Parse(new String(row[6].ToString().Where(Char.IsDigit).ToArray()));

                            gdb.LoaiGiaoDich = 9;
                            VeHoanGD.Add(gdb);
                        }//vé hoàn
                        else
                        {
                            gdb.GiaThu = gdb.GiaHeThong = gdb.GiaNet = long.Parse(new String(row[6].ToString().Where(Char.IsDigit).ToArray()));
                            VeThuongGD.Add(gdb);
                        }
                    }

                    GiaoDichD gd = new GiaoDichD();
                    List<GiaoDichO> GD = new List<GiaoDichO>();

                    if (VeHoanGD.Count > 0)
                    {
                        VeHoanGD = VeHoanGD.OrderBy(w => w.MaCho).ToList();
                        GD = gd.VeHoan(VeHoanGD, _NCCC);// lấy dữ liệu đã tồn tại

                        for (int i = 0; i < GD.Count; i++)
                        {
                            for (int u = 0; u < VeHoanGD.Count; u++)
                            {
                                if (GD[i].NgayGD.ToString("ddMMyy").Equals(VeHoanGD[u].NgayGD.ToString("ddMMyy"))
                                    && GD[i].GiaHoan == VeHoanGD[u].GiaHoan && GD[i].MaCho == VeHoanGD[u].MaCho
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
                            GD = gd.LayDaiLyVeHoan(VeHoanGD, _NCCC);
                            for (int i = 0; i < VeHoanGD.Count; i++)
                            {
                                GiaoDichO gda = VeHoanGD[i];
                                foreach (GiaoDichO gdo in GD)
                                {
                                    if (gdo.MaCho.Replace(" ", string.Empty) == gda.MaCho.Replace(" ", string.Empty))
                                    {
                                        if (gdo.LoaiKhachHang == 1)
                                        {
                                            gda.LoaiKhachHang = gdo.LoaiKhachHang;
                                            gda.IDKhachHang = gdo.IDKhachHang;
                                        }
                                        if (gda.HangHoan == gdo.GiaNet && (gdo.BiDanh ?? string.Empty).Length > 1)
                                            gda.BiDanh = gdo.BiDanh;
                                        break;
                                    }
                                }
                            }
                            gd.Them(VeHoanGD);
                        } //Tìm vé trước đó gán tên đl và thêm
                    }//vé hoàn |thêm ngay

                    if (VeThuongGD.Count > 0)
                    {
                        List<string> macho = gd.MaCho(VeThuongGD, 5).Distinct().ToList();
                        VeThuongGD = VeThuongGD.OrderBy(w => w.MaCho).ToList();
                        GD = gd.VeThuong(VeThuongGD, _NCCC).OrderBy(w => w.MaCho).ToList();// lấy dữ liệu đã tồn tại

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

                    List<string> MaCHo = new List<string>();
                    if (VeThuongGD.Count == 0) Close();
                    else
                    {
                        VeThuongGD = VeThuongGD.OrderBy(x => x.MaCho).ToList();
                        MaCHo = VeThuongGD.Select(x => x.MaCho).Distinct().ToList();
                        new frmVJ2(VeThuongGD, MaCHo, _NCCC).Show();
                    }
                }
            }
        }

        private void VJ2_Click(object sender, EventArgs e)
        {
            VIetJetO(9);
        }

        private void VJ3_Click(object sender, EventArgs e)
        {
            VIetJetO(16);
        }

        private void TimerLayDuLieu_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;

            if (dt.Hour < 4)
                TimerLayDuLieu.Interval = 3 * 60 * 60 * 1000;
            if (dt.Hour > 5)
                TimerLayDuLieu.Interval = 60 * 1000;


            if (dt.Minute == 15 || dt.Minute == 30 || dt.Minute == 45 || dt.Minute == 59)
            {
                if (dt.ToString("HH:mm") == "23:59")
                {
                    if (new GiaoDichD().GetOneVale("SELECT count(*) FROM KHOANGAY WHERE CONVERT(DATE,TuNgay) = CONVERT(DATE,GETDATE()-1)").ToString() == "0")
                    {
                        string CauTruyVan = string.Format(@"INSERT INTO SODU_DAILY(DaiLyID, SoDuCuoi, Ngay, ChinhSachID, LoaiKhachHangSD)  SELECT ID, 0 , GETDATE()+1, ChinhSach, LoaiKhachHang FROM DAILY WHERE LoaiKhachHang <> 3;
                                            insert into SODU_HANG select ID, 0, GETDATE()+1,0,'' from NHACUNGCAP;
                                            insert into SODU_NGANHANG select id, SoDu, GETDATE()+1 from NGANHANG;
                                            insert into khoangay(TuNgay,HoatDong,Them,Sua,Xoa,KhoaHangBay,Code,nganhang,ThemNH,SuaNH,XoaNH) values (GETDATE()-1,1,1,1,1,'','',1,1,1,1);
                                            INSERT INTO [dbo].[BANBAOCAO] ([NgaySDKH],[SDC_Duong_NV] ,[SDC_Am_NV] ,[SDC_Duong_DL] ,[SDC_Am_DL] ,[SDC_Duong_CTV] ,[SDC_Am_CTV] ,[SDC_Duong_HB] ,[SDC_Am_HB] ,[SDC_Duong_NH] ,[SDC_Am_NH] ,[BienDong] ,[BienDongVon] ,[KhachLe] ,[QuyCoc])
                                                 VALUES (CONVERT(DATE,GETDATE()) ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0);

                                            INSERT INTO [CRMF1].[dbo].[SODU_NGANHANG]([NganHangID],[SoDuCuoi],[Ngay],[IDThanhHoang]) SELECT ID,0,GETDATE()+1,IDThanhHoang FROM [CRMF1].[dbo].NGANHANG
                                            INSERT INTO [CRMF1].[dbo].[SODU_KHACHHANG] ([Ngay],[DaiLy],[SoDuCuoi] ,[ChinhSach] ,[IDThanhHoang]) SELECT GETDATE()+1,ID,0,ChinhSach,IDThanhHoang FROM [CRMF1].[dbo].KHACHHANG WHERE LoaiKhachHang <> 3;
                                            INSERT INTO [CRMF1].[dbo].[SODU_HANG]([NCCID] ,[SoDuCuoi] ,[Ngay] ,[SoDuThucTe] ,[Error] ,[IDThanhHoang])SELECT ID,0,GETDATE()+1,0,'',IDThanhHoang FROM  [CRMF1].[dbo].NHACUNGCAP;

");
                        new GiaoDichD().ChayCauTruyVan(CauTruyVan);
                    }
                }
                foreach (var process in Process.GetProcessesByName("ProJSoDu"))
                { process.Kill(); }
                Process.Start("ProJSoDu");
            }
            else if (dt.Minute == 58 || dt.Minute == 29 || dt.Hour == 3)
            {
                if (chkQH.Checked)
                {
                    foreach (var process in Process.GetProcessesByName("ProJBamBoo"))
                    { process.Kill(); }
                    Process.Start("ProJBamBoo");
                }

                if (chkVJ.Checked)
                {
                    foreach (var process in Process.GetProcessesByName("VietJetProJ"))
                    { process.Kill(); }
                    Process.Start("VietJetProJ");
                }

                if (chkVN.Checked)
                {
                    foreach (var process in Process.GetProcessesByName("chrome"))
                    { process.Kill(); }
                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    { process.Kill(); }
                    foreach (var process in Process.GetProcessesByName("VietNamProJ"))
                    { process.Kill(); }
                    Process.Start("VietNamProJ");
                }
            }

        }

        private void frmAutoLoadTicket_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("ProJSoDu"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("ProJBamBoo"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("VietJetProJ"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            { process.Kill(); }
            foreach (var process in Process.GetProcessesByName("VietNamProJ"))
            { process.Kill(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new frmVN(dtpVN.Value, true).Show();
        }

        private void btnVU_Click(object sender, EventArgs e)
        {
            new frmVU(dtpVU.Value).Show();
        }
    }
}
