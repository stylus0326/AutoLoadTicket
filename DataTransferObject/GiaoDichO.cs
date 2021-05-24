using System;

namespace DataTransferObject
{
    public class GiaoDichO
    {
        //New
        public long ID { set; get; }//
        public int IDKhachHang { set; get; }//
        public DateTime NgayGD { set; get; } = DateTime.Now;//
        public DateTime NgayCuonChieu { set; get; } = DateTime.Now;//
        public string Agent { set; get; }//
        public int NhaCungCap { set; get; }//
        public int LoaiKhachHang { set; get; } = 1;
        public string Hang { set; get; }//
        public string MaCho { set; get; } = string.Empty;//
        public string TenKhach { set; get; }//
        public string SoVeVN { set; get; }//
        public int SoLuongVe { set; get; }//

        public string LoaiVeDi { set; get; }//
        public string SoHieuDi { set; get; }//
        public string HanhLyDi { set; get; }//
        public int TuyenBayDi { set; get; }//
        public DateTime GioBayDi { set; get; } = DateTime.Now;//
        public DateTime GioBayDi_Den { set; get; } = DateTime.Now;//
        public string LoaiVeVe { set; get; }//
        public string SoHieuVe { set; get; }//
        public string HanhLyVe { set; get; }//
        public int TuyenBayVe { set; get; }//
        public DateTime GioBayVe { set; get; } = default(DateTime);
        public DateTime GioBayVe_Den { set; get; } = default(DateTime);//

        public long Fare { set; get; }//
        public long GiaNet { set; get; }//
        public long PhiCK { set; get; }//

        public long HoaHong { set; get; }//
        public long GiaHeThong { set; get; }//
        public long GiaThu { set; get; }//
        public string GhiChu { set; get; }//

        public string EmailKhachHang { set; get; } = string.Empty;//
        public int NVGiaoDich { set; get; }//
        public int NVHoTro { set; get; }//
        public string BiDanh { set; get; }//

        public string DienThoaiKhachHang { set; get; } = string.Empty;//
        public int HTTT { set; get; } = 1;
        public DateTime NgayCapNhatLuyKe { set; get; } = DateTime.Now;//
        public int HoaDon { set; get; } = 0;//
        public long GiaHoan { set; get; }//
        public long HangHoan { set; get; }//
        public int LoaiGiaoDich { set; get; } = 4;
        public bool TinhCongNo { set; get; } = true;
        public bool TheoDoi { set; get; }
        public bool VeTuXuat { set; get; }

        public GiaoDichO()
        {
        }

        public GiaoDichO(GiaoDichO g)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in g.GetType().GetProperties())
            {
                foreach (System.Reflection.PropertyInfo This in GetType().GetProperties())
                {
                    if (This.Name == propertyInfo.Name)
                    {
                        object Vl = propertyInfo.GetValue(g, null);
                        This.SetValue(this, Vl, null);
                        break;
                    }
                }
            }
        }
    }
}
