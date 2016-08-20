using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences._ExtensionMethods
{
    public static class DataReaderExtensions
    {
        public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
        {
            if (dr != null && dr.HasRows)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

                while (dr.Read())
                {
                    T newObject = new T();
                    for (int index = 0; index < dr.FieldCount; index++)
                    {
                        if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
                        {
                            var info = propDict[dr.GetName(index).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                var val = dr.GetValue(index);
                                info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                            }
                        }
                    }
                    entities.Add(newObject);
                }
                return entities;
            }
            return null;
        }
    }
}


//how to use it...
//using (DataContext)
//{
//    //Had to go this route since EF Code First doesn't support output parameters 
//    //returned from sprocs very well at this point
//    using (DataContext.Database.Connection)
//    {
//        DataContext.Database.Connection.Open();
//        DbCommand cmd = DataContext.Database.Connection.CreateCommand();
//        cmd.CommandText = "MySproc";
//        cmd.CommandType = CommandType.StoredProcedure;
//        cmd.Parameters.Add(new SqlParameter("UserID", userID));
//        cmd.Parameters.Add(new SqlParameter("SubID", subdivisionID));
//        cmd.Parameters.Add(new SqlParameter("FromDueDate", fromDueDate));
//        cmd.Parameters.Add(new SqlParameter("ToDueDate", toDueDate));
//        cmd.Parameters.Add(new SqlParameter("ShowHistory", showHistory));
//        cmd.Parameters.Add(new SqlParameter("CurrentPage", currentPage));
//        cmd.Parameters.Add(new SqlParameter("PageSize", pageSize));
//        var totalCountParam = new SqlParameter ("TotalCount", 0) { Direction = ParameterDirection.Output};
//        cmd.Parameters.Add(totalCountParam);

//        List<Task> tasks;
//        using (var reader = cmd.ExecuteReader())
//        {
//            tasks = reader.MapToList<MyItem>();
//        }
//        //Access output variable after reader is closed
//        totalCount = (totalCountParam.Value == null) ? 0 : Convert.ToInt32(totalCountParam.Value);
//        return tasks;
//    }
//}