using System;

namespace DataTransferObject
{
    public class NCCO
    {
        public int ID { set; get; }
        public string TenDayDu { set; get; }
        public string Ten { set; get; }
        public long SoDu { set; get; } = 0;
        public string HangBay { set; get; }
        public bool Hang { set; get; }
        public string MaHang { set; get; }
        public string TaiKhoan { set; get; }
        public string MatKhau { set; get; }
    }

    public class NCCGDO
    {
        public int ID { set; get; }
        public int NCC { set; get; }
        public DateTime NgayGD { set; get; } = DateTime.Now;
        public long SoTien { set; get; }
        public int LoaiGiaoDich { set; get; }
        public string GhiChu { set; get; }
    }


    public class HangBayO
    {
        public int ID { set; get; }
        public string TenHang { set; get; }
        public string TenTat { set; get; }
        public string TaiKhoan { set; get; }
        public string MatKhau { set; get; }
        public long SoDu { set; get; }
        public int MauChinh { set; get; }
        public int MauNen { set; get; }
        public int MauChu { set; get; }
        public int HanhLy { set; get; }
        public byte[] LogoHang { set; get; }
        public string LoaiVe { set; get; }
    }

    public class SanBayO
    {
        public int ID { set; get; }
        public string KyHieu { set; get; }
        public string TenDayDu { set; get; }
        public bool NoiDia { set; get; }
    }

    public class TuyenBayO
    {
        public int ID { set; get; }
        public string Ten { set; get; }
        public int KyHieuDi { set; get; }
        public int KyHieuDen { set; get; }
    }
}
