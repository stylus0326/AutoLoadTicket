
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DataAccessLayer
{
    public class GiaoDichD : DataProcess
    {
        public GiaoDichD()
        { TableName = "GIAODICH"; }

        #region Lấy dữ liệu tích hợp
        public long ThucThiSua(List<GiaoDichO> gd)
        {
            GiaoDichD gdD = new GiaoDichD();
            List<int> lstID = gd.Where(w => w.LoaiKhachHang != 3).Select(w => w.IDKhachHang).Distinct().ToList();
            List<string> lststr = new List<string>();
            List<Dictionary<string, object>> lstDic = new List<Dictionary<string, object>>();
            for (int i = 0; i < gd.Count; i++)
            {
                if (gd[i].IDKhachHang < 1)
                {
                    gd[i].IDKhachHang = 104841;
                    gd[i].LoaiKhachHang = 0;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("IDKhachHang", gd[i].IDKhachHang);
                dic.Add("HTTT", 1);
                dic.Add("NVGiaoDich", gd[i].NVGiaoDich);
                dic.Add("GhiChu", gd[i].GhiChu);
                dic.Add("LoaiKhachHang", gd[i].LoaiKhachHang);
                if (!(gd[i].EmailKhachHang ?? "").Contains("Email"))
                    dic.Add("EmailKhachHang", gd[i].EmailKhachHang);
                if (!(gd[i].DienThoaiKhachHang ?? "").Contains("Telephone"))
                    dic.Add("DienThoaiKhachHang", gd[i].DienThoaiKhachHang);
                dic.Add("Agent", gd[i].Agent);
                lstDic.Add(dic);
                lststr.Add("WHERE ID = " + gd[i].ID);
            }
            return gdD.SuaNhieu1Ban(lstDic, lststr);
        }

        public void ChaySD()
        {
            EXECUP("RSSoDuHB");
            EXECUP("UpdateNopQuyALL");
            EXECUP("RSSoDuNH");
            EXECUP("RSSoDu");
        }

        public void Them(List<GiaoDichO> gd)
        {
            //    var query = gd.Where(w => w.LoaiKhachHang != 3)
            //          .GroupBy(cm => cm.IDKhachHang)
            //          .Select(g => new GiaoDichO
            //          {
            //              IDKhachHang = g.Key,
            //              NgayGD = g.Min(cm => cm.NgayGD)
            //          });

            long a = 0;
            int LGD = 0;
            string SoVe = String.Join(" ,", gd.Select(w => (w.SoVeVN ?? "")).Distinct().ToArray());
            string MaCho = String.Join(" ,", gd.Select(w => (w.MaCho ?? "")).Distinct().ToArray());
            List<Dictionary<string, object>> lstDic = new List<Dictionary<string, object>>();

            for (int i = 0; i < gd.Count; i++)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (gd[i].IDKhachHang < 1)
                {
                    gd[i].IDKhachHang = 104841;
                    gd[i].LoaiKhachHang = 0;
                }

                if (gd[i].NVGiaoDich < 1 && gd[i].Hang == "VN")
                    gd[i].VeTuXuat = true;
                LGD = gd[i].LoaiGiaoDich;
                foreach (PropertyInfo propertyInfo in gd[i].GetType().GetProperties())
                {
                    if (propertyInfo.Name.ToLower() == "end")
                        break;
                    if (propertyInfo.GetValue(gd[i], null) != null)
                    {
                        if (propertyInfo.PropertyType == typeof(DateTime))
                            dic.Add(propertyInfo.Name, ((DateTime)propertyInfo.GetValue(gd[i], null)).ToString("yyyyMMdd HH:mm"));
                        else
                            dic.Add(propertyInfo.Name, propertyInfo.GetValue(gd[i], null));
                    }
                }
                dic.Remove("ID");
                if (gd[i].SoLuongVe != 2 || gd[i].GioBayVe.ToString().Contains("0001"))
                {
                    dic.Remove("GioBayVe");
                    dic.Remove("GioBayVe_Den");
                    dic.Remove("TuyenBayVe");
                    dic.Remove("SoHieuVe");
                    dic.Remove("LoaiVeVe");
                    dic.Remove("HanhLyVe");
                }
                lstDic.Add(dic);
                if (lstDic.Count > 5)
                {
                    a += ThemNhieu1Ban(lstDic);
                    dic = new Dictionary<string, object>();
                    dic.Add("FormName", "Hệ thống");
                    dic.Add("MaCho", string.Empty);
                    dic.Add("NoiDung", string.Format("{0}: {1}", (LGD == 9) ? "Thêm Vé hoàn" : "Thêm Vé", (SoVe.Length > 10) ? SoVe : MaCho));
                    dic.Add("LoaiKhachHang", 0);
                    dic.Add("Ma", 0);
                    new LichSuD().ThemMoi(dic);
                    lstDic.Clear();
                }
            }

            if (lstDic.Count > 0)
            {
                a += ThemNhieu1Ban(lstDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("FormName", "Hệ thống");
                dic.Add("MaCho", string.Empty);
                dic.Add("NoiDung", string.Format("{0}: {1}", (LGD == 9) ? "Thêm Vé hoàn" : "Thêm Vé", (SoVe.Length > 10) ? SoVe : MaCho));
                dic.Add("LoaiKhachHang", 0);
                dic.Add("Ma", 0);
                new LichSuD().ThemMoi(dic);
                lstDic.Clear();
            }
        }

        public List<GiaoDichO> VeHoan(List<GiaoDichO> lst, int NCC)
        {

            string CauTruyVan = @"SELECT MaCho,GiaHoan,NgayGD,TenKhach FROM GIAODICH WHERE NhaCungCap = " + NCC + " AND LoaiGiaoDich = 9 AND (";
            foreach (GiaoDichO gd in lst)
            {
                CauTruyVan += string.Format("(Macho = '{0}' and GiaHoan = {1} and CONVERT(DATE,NgayGD) = '{2}' and TenKhach = N'{3}') OR ", gd.MaCho, gd.GiaHoan, gd.NgayGD.ToString("yyyyMMdd"), gd.TenKhach);
            }

            CauTruyVan = CauTruyVan.Substring(0, CauTruyVan.Length - 3) + ")";
            return LayDuLieu<GiaoDichO>(false, CauTruyVan);
        }

        public List<GiaoDichO> LayDaiLyVeHoan(List<GiaoDichO> lst, int NCC)
        {
            List<string> a = lst.Select(w => w.MaCho).Distinct().ToList();
            string CauTruyVan = $"SELECT MaCho,LoaiKhachHang,IDKhachHang,TenKhach,GiaThu,GiaNet FROM GIAODICH WHERE NhaCungCap = {NCC} AND LoaiGiaoDich in (4,13,14) AND MaCho in ('{string.Join("','", a.ToArray())}') ORDER BY MaCho";
            return LayDuLieu<GiaoDichO>(false, CauTruyVan);
        }

        public List<GiaoDichO> VeThuong(List<GiaoDichO> lst, int NCC)
        {
            string CauTruyVan = "";
            if (NCC == 1 || NCC == 9 || NCC == 16)
            {
                CauTruyVan = @"SELECT MaCho,GiaNet,NgayGD,TenKhach,BiDanh,SoVeVN FROM GIAODICH WHERE NhaCungCap = " + NCC + " AND LoaiGiaoDich in (4,13,14) AND (";
                foreach (GiaoDichO gd in lst)
                {
                    CauTruyVan += string.Format("(Macho = '{0}' and GiaNet = {1} and CONVERT(DATE,NgayGD) = '{2}' and REPLACE(TenKhach,',','') = N'{3}') OR ", gd.MaCho, gd.GiaNet, gd.NgayGD.ToString("yyyyMMdd"), gd.TenKhach.Replace(",", ""));
                }
            }
            else
            {
                CauTruyVan = @"SELECT MaCho,GiaNet,NgayGD,TenKhach,BiDanh,SoVeVN FROM GIAODICH WHERE NhaCungCap = " + NCC + " AND LoaiGiaoDich in (4,13,14) AND LEN(SoVeVN)>7 AND (";
                foreach (GiaoDichO gd in lst)
                {
                    CauTruyVan += string.Format("(SoVeVN = '{0}' and GiaNet = {1} and CONVERT(DATE,NgayGD) = '{2}' and REPLACE(TenKhach,',','') = N'{3}') OR ", gd.SoVeVN, gd.GiaNet, gd.NgayGD.ToString("yyyyMMdd"), gd.TenKhach.Replace(",", ""));
                }
            }
            CauTruyVan = CauTruyVan.Substring(0, CauTruyVan.Length - 3) + ")";
            return LayDuLieu<GiaoDichO>(false, CauTruyVan);
        }

        public List<string> MaCho(List<GiaoDichO> lst, int NCC)
        {
            List<string> a = new List<string>();
            string CauTruyVan = string.Format(@"SELECT distinct MaCho FROM GIAODICH WHERE NhaCungCap = {1} AND LoaiGiaoDich = 4 AND Macho in('{0}') ", String.Join("','", lst.Select(w => w.MaCho).ToArray()),NCC);
            return LayDuLieu<GiaoDichO>(false, CauTruyVan).Select(w => w.MaCho).ToList();
        }

        public List<GiaoDichO> VeThuong(DataTable XulyStr)
        {
            string CauTruyVan = "";
            CauTruyVan = @"SELECT MaCho,NgayGD FROM GIAODICH WHERE NhaCungCap = 21 AND LoaiGiaoDich in (4,13,14) AND (";
            foreach (DataRow dr in XulyStr.Rows)
            {
                CauTruyVan += string.Format("(Macho = '{0}' and CONVERT(DATE,NgayGD) = '{1}') OR ", dr["ConfirmationNum"].ToString(), DateTime.ParseExact(dr["TransactionDate"].ToString(), "dd MMM yyyy", new CultureInfo("en-US")).ToString("yyyyMMdd"));
            }
            CauTruyVan = CauTruyVan.Substring(0, CauTruyVan.Length - 3) + ")";
            return LayDuLieu<GiaoDichO>(false, CauTruyVan);
        }

        public List<string> MaCho(DataTable XulyStr)
        {
            List<string> a = new List<string>();
            string CauTruyVan = "";

            for (int i = 0; i < XulyStr.Rows.Count; i++)
            {
                CauTruyVan += XulyStr.Rows[i]["ConfirmationNum"].ToString();
                if (i < XulyStr.Rows.Count - 1)
                    CauTruyVan += "','";
            }
            CauTruyVan += string.Format(@"SELECT distinct MaCho FROM GIAODICH WHERE NhaCungCap = 21 AND LoaiGiaoDich = 4 Macho in('{0}') ");
            return LayDuLieu<GiaoDichO>(false, CauTruyVan).Select(w => w.MaCho).ToList();
        }

        public List<GiaoDichO> VeThuong(List<string> lst, int IsHoan)
        {
            lst = lst.Select(s => "738" + s.Split(',')[1]).Distinct().ToList();
            string CauTruyVan = String.Join("' ,'", lst.ToArray());
            if (IsHoan == 9)
                CauTruyVan = "WHERE NhaCungCap = 2 AND LoaiGiaoDich = 9 AND SoVeVN IN ('" + CauTruyVan + "')  and convert(date,ngaygd) >= convert(date,GETDATE() -400)";
            else
                CauTruyVan = "WHERE NhaCungCap = 2 AND LoaiGiaoDich in (4,13,14) AND SoVeVN IN ('" + CauTruyVan + "')  and convert(date,ngaygd) >= convert(date,GETDATE() -400)";
            return LayDuLieu<GiaoDichO>(true, CauTruyVan);
        }

        public List<string> MaCho(List<string> lst)
        {
            lst = lst.Select(s => "738" + s.Split(',')[1]).Distinct().ToList();
            string CauTruyVan = "SELECT distinct MaCho FROM GIAODICH WHERE NhaCungCap = 2 AND LoaiGiaoDich = 4 AND SoVeVN IN ('" + String.Join("' ,'", lst.ToArray()) + "')";
            return LayDuLieu<GiaoDichO>(false, CauTruyVan).Select(w => w.MaCho).ToList();
        }


        #endregion

        public List<GiaoDichO> KhachEMAILVJ2(int _NCC)
        {
            string CauTruyVan = string.Format("select distinct EmailKhachHang,IDKhachHang from GIAODICH where NhaCungCap = " + _NCC + " and IDKhachHang > 1 and EmailKhachHang is not null and LoaiKhachHang = 1");
            return LayDuLieu<GiaoDichO>(false, CauTruyVan);
        }

        public List<GiaoDichO> DuLieu(string CauTV)
        {
            return LayDuLieu<GiaoDichO>(true, string.Format(@"WHERE {0} order by ID desc", CauTV), ",coalesce(GiaHeThong,0) + coalesce(PhiCK,0) + coalesce(HoaHong,0) - coalesce(GiaNet,0) - coalesce(GiaHoan,0) + coalesce(HangHoan,0) 'LoiNhuan'");
        }

        public List<GiaoDichO> GDCongTy()
        {
            return LayDuLieu<GiaoDichO>(true, string.Format(@"WHERE NhaCungCap = 2 and convert(date,NgayGD) = '{0}' and len(coalesce(Agent,''))>0 and LoaiGiaoDich in (4,13,14)", DateTime.Now.AddDays(-1).ToString("yyyyMMdd")));
        }
    }
}