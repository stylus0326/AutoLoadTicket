using DataTransferObject;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public class DaiLyD : DataProcess
    {
        #region Làm xong

        public DaiLyD()
        {
            TableName = "DAILY";
        }

        public List<DaiLyO> All()
        {
            List<DaiLyO> lst = new List<DaiLyO>();
            lst.AddRange(LayDuLieu<DaiLyO>(false, string.Format("SELECT ID, Ten, MaDL,LoaiKhachHang,EmailGiaoDich,EmailKeToan,MaAGS FROM DAILY where loaikhachhang <3")));
            return lst;
        }
        #endregion
    }

    public class SignInD : DataProcess
    {
        public SignInD()
        {
            TableName = "SIGNIN";
        }

        public List<SignInO> DuLieu()
        {
            return LayDuLieu<SignInO>(true, " ORDER BY DaiLy, HangBay");
        }

        public List<SignInO> TimSignIn(long IDDoiTac)
        {
            return LayDuLieu<SignInO>(true, "WHERE Daily = " + IDDoiTac);
        }
    }
}
