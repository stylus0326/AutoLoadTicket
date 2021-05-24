using DataTransferObject;
using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public class HangBayD : DataProcess
    {
        public HangBayD()
        {
            TableName = "HANGBAY";
        }

        public List<HangBayO> DuLieu()
        {
            return LayDuLieu<HangBayO>(true, " ORDER BY SapXep DESC");
        }

        public HangBayO LayHangBay(string tentat)
        {
            return LayMotDongDonGian<HangBayO>(string.Format(@"WHERE TenTat = '{0}' AND Xoa = '0'", tentat));
        }
    }
    public class NCCGDD : DataProcess
    {
        public NCCGDD()
        {
            TableName = "NHACUNGCAP_GIAODICHPHATSINH";
        }

        public List<NCCGDO> DuLieu()
        {
            return LayDuLieu<NCCGDO>(true);
        }
    }

    public class NCCD : DataProcess
    {
        public NCCD()
        {
            TableName = "NHACUNGCAP";
        }

        public List<NCCO> DuLieu()
        {
            return LayDuLieu<NCCO>(false, @"SELECT NC.[ID], [Ten] ,SD.SoDuCuoi [SoDu] ,[HangBay], [TenDayDu], [TaiKhoan], [MatKhau] , [MaHang], [Hang]
FROM NHACUNGCAP NC
LEFT JOIN (SELECT * FROM SODU_HANG WHERE CONVERT(date,Ngay) =CONVERT(date,GETDATE())) SD ON NC.ID = SD.NCCID");
        }

        public List<NCCO> DuLieu_GiaoDich()
        {
            return LayDuLieu<NCCO>(false, "SELECT [ID], (case when Hang = 1 then N'Hãng' else Ten end) Ten ,[SoDu] ,[HangBay] FROM NHACUNGCAP");
        }
    }

    public class TuyenBayD : DataProcess
    {
        public TuyenBayD()
        {
            TableName = "TUYENBAY";
        }

        public List<TuyenBayO> DuLieu()
        {
            return LayDuLieu<TuyenBayO>();
        }

        public TuyenBayO LayTuyenBay(int id)
        {
            return LayMotDongDonGian<TuyenBayO>(string.Format(@"WHERE ID = {0}", id));
        }

        public TuyenBayO TuyenBay(int Di, int Den)
        {
            return LayMotDongDonGian<TuyenBayO>(string.Format(@"WHERE KyHieuDi = {0} AND KyHieuDen = {1}", Di, Den));
        }
    }

    public class LichSuD : DataProcess
    {
        public LichSuD()
        {
            TableName = "LS_GIAODICH";
        }
    }

    public class SanBayD : DataProcess
    {
        public SanBayD()
        {
            TableName = "SANBAY";
        }

        public List<SanBayO> DuLieu()
        {
            return LayDuLieu<SanBayO>();
        }

        public SanBayO SanBay(string ID)
        {
            return LayMotDongDonGian<SanBayO>(string.Format(@"WHERE KyHieu = '{0}'", ID));
        }
    }
}
