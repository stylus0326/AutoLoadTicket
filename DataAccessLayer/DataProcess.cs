using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccessLayer
{
    public class DataProcess
    {
        public string TableName { set; get; }
        string CauTruyVan { set; get; }

        #region Thêm sửa xóa nhiều dòng
        public long ThemNhieu1Ban(List<Dictionary<string, object>> lstDuLieu)
        {
            return DataProvider.ThemNhieu1Ban(lstDuLieu, TableName);
        }

        public long TaoChungTu()
        {
            return DataProvider.TaoChungTu();
        }

        public long SuaNhieu1Ban(List<Dictionary<string, object>> lstDuLieu, List<string> LstCauDieuKien, string TenBan = "")
        {
            if (TenBan.Length == 0)
                TenBan = TableName;
            if (lstDuLieu.Count < 30)
                return DataProvider.SuaNhieu1Ban(lstDuLieu, LstCauDieuKien, TenBan);
            else
            {
                long b = 0;
                List<Dictionary<string, object>> G = new List<Dictionary<string, object>>();
                List<string> LstCauDieuKien1 = new List<string>();

                for (int i = 1; i < lstDuLieu.Count + 1; i++)
                {
                    G.Add(lstDuLieu[i - 1]);
                    LstCauDieuKien1.Add(LstCauDieuKien[i - 1]);
                    if (i % 30 == 0 || i == lstDuLieu.Count)
                    {
                        b += DataProvider.SuaNhieu1Ban(G, LstCauDieuKien1, TenBan);
                        G.Clear();
                        LstCauDieuKien1.Clear();
                    }
                }
                return b;
            }
        }

        public long XoaNhieu1Ban(List<object> LstCauDieuKien, string TenBan = "")
        {
            if (TenBan.Length > 0)
                return DataProvider.XoaNhieu1Ban(LstCauDieuKien, TenBan);
            else
                return DataProvider.XoaNhieu1Ban(LstCauDieuKien, TableName);
        }

        public long XoaNhieu(List<string> LstTenBan, List<string> LstCauDieuKien)
        {
            return DataProvider.XoaNhieu(LstTenBan, LstCauDieuKien);
        }
        #endregion

        public bool DaTonTai(string CauDieuKien)
        {
            string CauTruyVan = string.Format("SELECT COUNT(*) FROM {0} {1}", TableName, CauDieuKien);
            return (int)DataProvider.LayGiaTri(CauTruyVan) > 0;
        }

        public bool DaTonTai(string TenCot, object DuLieu, object ID, string DieuKienThem = "")
        {
            string CauTruyVan = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = N'{2}' AND ID <> {3} {4}", TableName, TenCot, DuLieu, ID, DieuKienThem);
            return (int)DataProvider.LayGiaTri(CauTruyVan) > 0;
        }

        public List<T> LayDuLieu<T>(bool TuTaoCauTruyVan = true, string CauDieuKien = "", string CauTimKiemThem = "") where T : new()
        {
            T t = new T();
            if (TuTaoCauTruyVan)
            {
                string TenCot = string.Empty;
                foreach (System.Reflection.PropertyInfo propertyInfo in t.GetType().GetProperties())
                {
                    if (propertyInfo.Name == "End")
                        break;
                    TenCot += propertyInfo.Name + " ,";
                }
                CauTruyVan = string.Format("SELECT {0}{2} FROM {1} ", TenCot.Substring(0, TenCot.Length - 1), TableName.ToUpper(), CauTimKiemThem) + CauDieuKien;

            }
            else
                CauTruyVan = CauDieuKien;
            return DataProvider.DataTableToListOfObject<T>(DataProvider.LayBanDuLieu(CauTruyVan));
        }

        public long EXECUP(string name, Dictionary<string, string> Dc = null)
        {
            return DataProvider.RunUpStoredProcedure(name, Dc);
        }

        public long EXECSE(string name)
        {
            return DataProvider.RunSeStoredProcedure(name);
        }

        public T LayMotDongDonGian<T>(string CauDieuKien = "") where T : new()
        {
            string TenCot = string.Empty;
            T t = new T();
            foreach (System.Reflection.PropertyInfo propertyInfo in t.GetType().GetProperties())
            {
                if (propertyInfo.Name == "End")
                    break;
                TenCot += propertyInfo.Name + " ,";
            }
            CauTruyVan = string.Format("SELECT TOP 1 {0} FROM {1} ", TenCot.Substring(0, TenCot.Length - 1), TableName.ToUpper()) + CauDieuKien;
            DataTable data = DataProvider.LayBanDuLieu(CauTruyVan);
            if (data.Rows.Count > 0)
                return DataProvider.DataTableToListOfObject<T>(data)[0];
            else
                return new T();
        }

        public T LayMotDongTuTao<T>(string CauDieuKien = "") where T : new()
        {
            DataTable data = DataProvider.LayBanDuLieu(CauDieuKien);
            if (data.Rows.Count > 0)
                return DataProvider.DataTableToListOfObject<T>(data)[0];
            else
                return new T();
        }
        public DateTime LayGioServer()
        {
            return (DateTime)DataProvider.LayGiaTri("SELECT GETDATE()");
        }

        //Dùng cho Công nợ
        public List<T> LayBanCongNo<T>(DataTable data, List<int> lstDaiLyID, List<long> lstDaiLyLuyKe, List<long> lstDaiLyLuyKeTong) where T : new()
        {
            return DataProvider.DataTableToListOfObject<T>(data, lstDaiLyID, lstDaiLyLuyKe, lstDaiLyLuyKeTong);
        }

        public bool KiemTraDaTonTai(string CauDieuKien)
        {
            string CauTruyVan = string.Format("SELECT COUNT(*) FROM {0} {1}", TableName, CauDieuKien);
            return (int)DataProvider.LayGiaTri(CauTruyVan) > 0;
        }


        public int ChayCauTruyVan(string query)
        {
            return (int)DataProvider.RunOrderQuery(query);
        }


        public DataTable LayDataTable(string Query)
        {
            return DataProvider.LayBanDuLieu(Query);
        }


        public object GetOneVale(string Query)
        {
            return DataProvider.LayGiaTri(Query);
        }

        public long ThemMoi(Dictionary<string, object> data, bool insIdentity = false)
        {
            return DataProvider.TaoData(data, TableName, insIdentity);
        }

        public long CapNhat(Dictionary<string, object> data, object id, string ChuoiGhep = " WHERE ID = {0}")
        {
            return DataProvider.SuaData(data, TableName, string.Format(ChuoiGhep, id));
        }


        public long Xoa(object GiaTri, string CauDieuKien = "WHERE ID = {0}")
        {
            return DataProvider.XoaData(TableName, string.Format(CauDieuKien, GiaTri));
        }

        public long ThemMoiTuExcel<T>(List<T> lstDuLieu, string TbName) where T : new()
        {
            return DataProvider.TaoDataTuEx(lstDuLieu, TbName);
        }

        public long ThemNhieuNhieuBan(List<Dictionary<string, object>> lstDuLieu, List<string> LstTenBan)
        {
            List<string> LstCauDieuKien = new List<string>();
            List<string> lstThem = new List<string>();
            foreach (string a in LstTenBan)
            {
                LstCauDieuKien.Add("A");
                lstThem.Add("T");
            }
            return DataProvider.TaoSuaNhieu(lstDuLieu, LstTenBan, LstCauDieuKien, lstThem);
        }



        public long CapNhatNhieu(List<Dictionary<string, object>> lstDuLieu, List<string> LstTenBan, List<string> LstCauDieuKien)
        {
            return DataProvider.SuaNhieu(lstDuLieu, LstTenBan, LstCauDieuKien);
        }

        public long CapNhat_ThemNhieu(List<Dictionary<string, object>> lstDuLieu, List<string> LstTenBan, List<string> LstCauDieuKien, List<string> lstThem)
        {
            return DataProvider.TaoSuaNhieu(lstDuLieu, LstTenBan, LstCauDieuKien, lstThem);
        }



    }
}
