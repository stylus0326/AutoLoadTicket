using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DataAccessLayer
{
    class DataProvider
    {
        public static string ConnectionString = Ultis.ChuoiKetNoi;
        public static long TaoDataTuEx<T>(List<T> lstDuLieu, string TypeName) where T : new()
        {
            T obj = new T();
            DataTable dt = new DataTable();
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name.Equals("End"))
                    break;
                else if (propertyInfo.Name != "ID")
                    dt.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));

            }

            foreach (T tinfo in lstDuLieu)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in tinfo.GetType().GetProperties())
                {
                    if (propertyInfo.Name.Equals("End"))
                        break;
                    else if (propertyInfo.Name != "ID")
                    {
                        object oj = propertyInfo.GetValue(tinfo, null);
                        if (oj != null)
                        {
                            if (propertyInfo.PropertyType == typeof(DateTime))
                            {
                                if ((DateTime)oj == default(DateTime))
                                    dataRow[propertyInfo.Name] = DBNull.Value;
                                else
                                    dataRow[propertyInfo.Name] = (DateTime)oj;
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                if (string.IsNullOrEmpty(oj.ToString()))
                                    dataRow[propertyInfo.Name] = DBNull.Value;
                                else
                                    dataRow[propertyInfo.Name] = oj.ToString();
                            }
                            else if (propertyInfo.PropertyType == typeof(long))
                                dataRow[propertyInfo.Name] = long.Parse(oj.ToString());
                            else if (propertyInfo.PropertyType == typeof(int))
                                dataRow[propertyInfo.Name] = int.Parse(oj.ToString());
                            else if (propertyInfo.PropertyType == typeof(bool))
                                dataRow[propertyInfo.Name] = (bool)oj;
                        }
                        else
                            dataRow[propertyInfo.Name] = DBNull.Value;
                    }
                }
                dt.Rows.Add(dataRow);
            }

            SqlConnection Connection = new SqlConnection(ConnectionString);
            SqlCommand sqlcom = new SqlCommand(TypeName, Connection);
            sqlcom.CommandType = CommandType.StoredProcedure;
            sqlcom.Parameters.AddWithValue("@tableproducts", dt);
            Connection.Open();
            long Num = sqlcom.ExecuteNonQuery();
            sqlcom.Connection.Close();
            return Num;
        }

        public static long RunUpStoredProcedure(string Query, Dictionary<string, string> pA = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand Command = new SqlCommand(Query, conn);
                Command.CommandType = CommandType.StoredProcedure;
                if (pA != null)
                    foreach (KeyValuePair<string, string> val in pA)
                    {
                        Command.Parameters.Add(new SqlParameter(val.Key, val.Value));
                    }
                Command.CommandTimeout = 0;
                long Num = Command.ExecuteNonQuery();
                return Num;
            }
        }

        public static long RunSeStoredProcedure(string Query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                object data = 0;
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    data = command.ExecuteScalar();
                }
                catch
                {
                    throw new Exception("Lỗi truy vấn cơ sở dữ liệu...");
                }
                connection.Close();
                return long.Parse(data.ToString());
            }
        }


        public static DataTable LayBanDuLieu(string Query)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(Query, ConnectionString);
                da.Fill(dt);
                return dt;
            }
            catch
            {
                throw new Exception("LỖI: Tải dữ liệu...");
            }
        }

        public static long TaoData(Dictionary<string, object> DuLieu, string TenBan, bool LayID)
        {
            string ClmName = string.Empty;
            string ClmValue = string.Empty;
            foreach (KeyValuePair<string, object> val in DuLieu)
            {
                ClmName += val.Key + ",";
                if (val.Value == null)
                    ClmValue += "@" + val.Key + ",";
                else if (!val.Value.ToString().ToLower().Contains("getdate()"))
                    ClmValue += "@" + val.Key + ",";
                else
                    ClmValue += val.Value + ",";
            }
            string QueRy = "INSERT INTO " + TenBan + "(" + ClmName.Substring(0, ClmName.Count() - 1) + ") VALUES (" + ClmValue.Substring(0, ClmValue.Count() - 1) + ")";

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            if (LayID)
                QueRy += "SELECT @@IDENTITY;";
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);

            foreach (KeyValuePair<string, object> val in DuLieu)
            {
                if (val.Value != null)
                {
                    if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), val.Value));
                    else
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), DBNull.Value));
                }
                else
                    Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), DBNull.Value));
            }

            long Num = 0;
            try
            {
                Num = (LayID ? long.Parse(Command.ExecuteScalar().ToString()) : Command.ExecuteNonQuery());
                Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long SuaData(Dictionary<string, object> DuLieu, string TenBan, string CauDieuKien)
        {
            string ClmName = string.Empty;
            foreach (KeyValuePair<string, object> val in DuLieu)
            {
                if (val.Value == null)
                    ClmName += val.Key + " = @" + val.Key + ",";
                else if (val.Value.ToString().ToLower().Contains("getdate()"))
                    ClmName += val.Key + " = " + val.Value + ",";
                else
                    ClmName += val.Key + " = @" + val.Key + ",";
            }
            string QueRy = "UPDATE " + TenBan + " SET " + ClmName.Substring(0, ClmName.Count() - 1) + " " + CauDieuKien;

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);
            foreach (KeyValuePair<string, object> val in DuLieu)
            {
                if (val.Value != null)
                {
                    if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), val.Value));
                    else
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), DBNull.Value));
                }
                else
                    Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString(), DBNull.Value));
            }

            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long XoaData(string TenBan, string CauDieuKien)
        {
            CauDieuKien = "DELETE FROM " + TenBan + " " + CauDieuKien;
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(CauDieuKien, Connection, Transaction);
            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }

        public static long SuaNhieu(List<Dictionary<string, object>> lstDuLieu, List<string> LstTenBan, List<string> LstCauDieuKien)
        {
            if (lstDuLieu.Count != LstTenBan.Count || LstTenBan.Count != LstCauDieuKien.Count)
                return 0;
            string QueRy = string.Empty;
            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                string ClmName = string.Empty;
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value == null)
                        ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                    else if (val.Value.ToString().ToLower().Contains("getdate()"))
                        ClmName += val.Key + " = " + val.Value + ",";
                    else
                        ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                }
                QueRy += "UPDATE " + LstTenBan[i] + " SET " + ClmName.Substring(0, ClmName.Count() - 1) + " " + LstCauDieuKien[i] + "; ";
            }

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);

            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value != null)
                    {
                        if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), val.Value));
                        else
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                    }
                    else
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                }
            }

            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != lstDuLieu.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long XoaNhieu(List<string> LstTenBan, List<string> LstCauDieuKien)
        {
            if (LstTenBan.Count != LstCauDieuKien.Count)
                return 0;

            string CauDieuKien = string.Empty;
            for (int i = 0; i < LstTenBan.Count; i++)
            {
                CauDieuKien += "DELETE FROM " + LstTenBan[i] + " " + LstCauDieuKien[i] + ";";
            }
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(CauDieuKien, Connection, Transaction);
            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != LstTenBan.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long TaoSuaNhieu(List<Dictionary<string, object>> lstDuLieu, List<string> LstTenBan, List<string> LstCauDieuKien, List<string> lstThem)
        {
            if (lstDuLieu.Count != LstTenBan.Count || LstTenBan.Count != LstCauDieuKien.Count || LstCauDieuKien.Count != lstThem.Count)
                return 0;

            string QueRy = string.Empty;
            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                string ClmName = string.Empty;
                string ClmValue = string.Empty;
                switch (lstThem[i])
                {
                    case "S":
                        foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                        {
                            if (val.Value == null)
                                ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                            else if (val.Value.ToString().ToLower().Contains("getdate()"))
                                ClmName += val.Key + " = " + val.Value + ",";
                            else
                                ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                        }
                        QueRy += "UPDATE " + LstTenBan[i] + " SET " + ClmName.Substring(0, ClmName.Count() - 1) + " " + LstCauDieuKien[i] + "; ";
                        break;
                    case "T":
                        foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                        {
                            ClmName += val.Key + ",";
                            if (val.Value == null)
                                ClmValue += "@" + val.Key + i.ToString("000#") + ",";
                            else if (val.Value.ToString().ToLower().Contains("getdate()"))
                                ClmValue += val.Value + ",";
                            else if (val.Value.ToString().Equals("@@IDENTITY"))
                                ClmValue += "@@IDENTITY,";
                            else
                                ClmValue += "@" + val.Key + i.ToString("000#") + ",";
                        }
                        QueRy += "INSERT INTO " + LstTenBan[i] + "(" + ClmName.Substring(0, ClmName.Count() - 1) + ") VALUES (" + ClmValue.Substring(0, ClmValue.Count() - 1) + ");";
                        break;
                    default:
                        QueRy += "DELETE FROM " + LstTenBan[i] + " " + LstCauDieuKien[i] + ";";
                        break;
                }
            }

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);

            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value != null)
                    {
                        if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), val.Value));
                        else
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                    }
                    else
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                }
            }

            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != lstDuLieu.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }

        public static long TaoChungTu()
        {
            string QueRy = "INSERT INTO SOCHUNGTU(DaDung) VALUES (1); SELECT @@IDENTITY;";

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);

            long Num = 0;
            try
            {
                Num = long.Parse(Command.ExecuteScalar().ToString());
                Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }

        public static long ThemNhieu1Ban(List<Dictionary<string, object>> lstDuLieu, string TenBan)
        {
            string QueRy = string.Empty;
            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                string ClmName = string.Empty;
                string ClmValue = string.Empty;

                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    ClmName += val.Key + ",";
                    if (val.Value == null)
                        ClmValue += "@" + val.Key + i.ToString("000#") + ",";
                    else if (val.Value.ToString().ToLower().Contains("getdate()"))
                        ClmValue += val.Value + ",";
                    else if (val.Value.ToString().Equals("@@IDENTITY"))
                        ClmValue += "@@IDENTITY,";
                    else
                        ClmValue += "@" + val.Key + i.ToString("000#") + ",";
                }
                QueRy += "INSERT INTO " + TenBan + "(" + ClmName.Substring(0, ClmName.Count() - 1) + ") VALUES (" + ClmValue.Substring(0, ClmValue.Count() - 1) + ");";
            }

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);
            string a = QueRy;
            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value != null)
                    {
                        if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                        {
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), val.Value));
                            a = a.Replace("@" + val.Key.ToString() + i.ToString("000#"), $"'{val.Value.ToString()}'");
                        }
                        else
                        {
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                            a = a.Replace("@" + val.Key.ToString() + i.ToString("000#"), "null");
                        }
                    }
                    else
                    {
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                        a = a.Replace("@" + val.Key.ToString() + i.ToString("000#"), "null");
                    }
                }
            }

            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != lstDuLieu.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long SuaNhieu1Ban(List<Dictionary<string, object>> lstDuLieu, List<string> LstCauDieuKien, string TenBan)
        {
            if (lstDuLieu.Count != LstCauDieuKien.Count)
                return 0;
            string QueRy = string.Empty;
            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                string ClmName = string.Empty;
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value == null)
                        ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                    else if (val.Value.ToString().ToLower().Contains("getdate()"))
                        ClmName += val.Key + " = " + val.Value + ",";
                    else
                        ClmName += val.Key + " = @" + val.Key + i.ToString("000#") + ",";
                }
                QueRy += "UPDATE " + TenBan + " SET " + ClmName.Substring(0, ClmName.Count() - 1) + " " + LstCauDieuKien[i] + "; ";
            }

            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(QueRy, Connection, Transaction);

            for (int i = 0; i < lstDuLieu.Count; i++)
            {
                foreach (KeyValuePair<string, object> val in lstDuLieu[i])
                {
                    if (val.Value != null)
                    {
                        if (val.Value.ToString() != string.Empty && !val.Value.ToString().Contains("01/01/0001"))
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), val.Value));
                        else
                            Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                    }
                    else
                        Command.Parameters.Add(new SqlParameter("@" + val.Key.ToString() + i.ToString("000#"), DBNull.Value));
                }
            }

            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != lstDuLieu.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }
        public static long XoaNhieu1Ban(List<object> ID, string TenBan)
        {
            string CauDieuKien = string.Empty;
            for (int i = 0; i < ID.Count; i++)
            {
                CauDieuKien += "DELETE FROM " + TenBan + " WHERE ID = " + ID[i] + ";";
            }
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(CauDieuKien, Connection, Transaction);
            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                if (Num != ID.Count)
                {
                    Transaction.Rollback();
                    Num = 0;
                }
                else
                    Transaction.Commit();
            }
            catch { Transaction.Rollback(); }
            finally { Command.Connection.Close(); }
            return Num;
        }

        public static object LayGiaTri(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                object data = 0;
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    data = command.ExecuteScalar();
                }
                catch
                {
                    throw new Exception("Lỗi truy vấn cơ sở dữ liệu...");
                }
                connection.Close();
                return data;
            }
        }

        public static List<T> DataTableToListOfObject<T>(DataTable tbl) where T : new()
        {
            var columnNames = tbl.Columns.Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .ToList();
            var properties = typeof(T).GetProperties();
            return tbl.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType), null);
                    }
                }
                return objT;
            }).ToList();
        }

        public static List<T> DataTableToListOfObject<T>(DataTable tbl, List<int> lstDaiLyID, List<long> lstDaiLyLuyKe, List<long> lstDaiLyLuyKeTong) where T : new()
        {
            int IDKH1 = int.Parse(tbl.Rows[0]["IDKhachHang"].ToString());
            int IDKH2 = IDKH1;
            int Mo1 = int.Parse(((DateTime)tbl.Rows[0]["NgayGD"]).ToString("MM"));
            int Mo2 = Mo1;
            List<T> list = new List<T>();
            foreach (DataRow row in tbl.Rows)
            {
                object ob = row["NgayGD"];
                IDKH1 = int.Parse(row["IDKhachHang"].ToString());
                if (IDKH2 != IDKH1)
                {
                    IDKH2 = IDKH1;
                    Mo1 = Mo2 = int.Parse(((DateTime)ob).ToString("MM"));
                }
                else
                {
                    Mo1 = int.Parse(((DateTime)ob).ToString("MM"));
                    if (Mo2 != Mo1)// tháng mới
                    {
                        lstDaiLyLuyKeTong[lstDaiLyID.IndexOf(IDKH2)] = 0;
                        Mo2 = Mo1;
                    }
                }

                list.Add(DataRowToObjectCN<T>(row, lstDaiLyID, lstDaiLyLuyKe, lstDaiLyLuyKeTong));
            }
            return list;
        }

        public static T DataRowToObjectCN<T>(DataRow row, List<int> lstDaiLyID, List<long> lstDaiLyLuyKe, List<long> lstDaiLyLuyKeTong) where T : new()
        {
            T obj = new T();
            int index = 0;

            foreach (DataColumn c in row.Table.Columns)
            {
                System.Reflection.PropertyInfo p = obj.GetType().GetProperty(c.ColumnName);
                if (p != null && row[c] != DBNull.Value)
                {
                    switch (p.Name)
                    {
                        case "IDKhachHang":
                            index = lstDaiLyID.IndexOf(int.Parse(row[c].ToString()));
                            break;
                        case "GiaThu":
                            lstDaiLyLuyKe[index] -= long.Parse(row[c].ToString());
                            break;
                        case "TaiKhoanCo":
                            lstDaiLyLuyKe[index] += long.Parse(row[c].ToString());
                            break;
                        case "GiaHoan":
                            lstDaiLyLuyKe[index] += long.Parse(row[c].ToString());
                            lstDaiLyLuyKeTong[index] -= long.Parse(row[c].ToString());
                            break;
                        case "GiaHeThong":
                            lstDaiLyLuyKeTong[index] += long.Parse(row[c].ToString());
                            break;
                    }

                    if (p.Name == "LuyKe")
                        p.SetValue(obj, lstDaiLyLuyKe[index], null);
                    else if (p.Name == "LuyKeTong")
                        p.SetValue(obj, lstDaiLyLuyKeTong[index], null);
                    else
                        p.SetValue(obj, row[c], null);
                }
            }
            return obj;
        }

        public DataTable ConvertListOjbectToDataTable<T>(List<T> objectClass, string table_name = "Table")
        {
            DataTable dt = new DataTable();
            try
            {
                dt.TableName = table_name;

                foreach (PropertyInfo property in objectClass[0].GetType().GetProperties())
                {
                    dt.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var vehicle in objectClass)
                {
                    DataRow newRow = dt.NewRow();
                    foreach (PropertyInfo property in vehicle.GetType().GetProperties())
                    {
                        newRow[property.Name] = vehicle.GetType().GetProperty(property.Name).GetValue(vehicle, null);
                    }
                    dt.Rows.Add(newRow);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static long RunOrderQuery(string Query)
        {
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction Transaction = Connection.BeginTransaction();
            SqlCommand Command = new SqlCommand(Query, Connection, Transaction);
            long Num = 0;
            try
            {
                Num = Command.ExecuteNonQuery();
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
            }
            finally
            {
                Command.Connection.Close();
            }
            return Num;
        }
    }
}
