using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Data.Common;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;
using Ex.Common.Extension;
using Ex.Common.Web;
using Ex.Common.Constant;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;

namespace Ex.DataAccess.Extension
{
    public static class EntityFrameworkExt
    {
        //auditDefaults = context.GetSet<tblAuditDefaults>(a => a.source.EqualsIgnoreCase(AppConstant.AuditWebMainSource));

        public static List<T> GetSet<T>(this DbContext context, Func<T, bool> condition) where T : class
        {
            DbSet<T> dbSetMetadata = context.Set<T>();

            var _set = dbSetMetadata.Where(condition).ToList();

            return _set;
        }

        public static string TableName(this ObjectStateEntry entry)
        {
            string _entity = entry.EntitySet.ToString();
            _entity = _entity.Substring(_entity.LastIndexOf(".") + 1);

            return _entity;
        }

        public static string PrimaryColumn(this ObjectStateEntry entry, List<string> ignoreList = null)
        {
            var primaryFields = entry.EntitySet.ElementType.KeyMembers;
            if (primaryFields.Count <= 0) return null;

            var primaryFieldName = (ignoreList == null) ? primaryFields[0].Name : string.Empty;

            if (ignoreList != null)
            {
                foreach (var primaryField in primaryFields)
                {
                    if (ignoreList.Any(i => i.EqualsIgnoreCase(primaryField.Name))) continue;

                    primaryFieldName = primaryField.Name;
                    break;
                }
            }

            return primaryFieldName;
        }

        public static string PrimaryKeyValueJson(this ObjectStateEntry entry)
        {
            List<string> values = new List<string>();
            if (entry.EntityKey.EntityKeyValues != null)
            {
                foreach (EntityKeyMember member in entry.EntityKey.EntityKeyValues)
                    values.Add(string.Format(" {0} = {1}' ", member.Key.ToUpper(), member.Value));
            }

            string value = values.JoinExt(" AND ");

            return !value.Empty() ? string.Format("{0}", value) : value;
        }

        public static Int32 PrimaryColumnValueFirstInt(this ObjectStateEntry entry, Int32 NoValue = 0)
        {
            List<string> ignoreFields = new List<string>();
            Int32 intValue = 0;

            do
            {
                var primaryFieldName = entry.PrimaryColumn(ignoreFields);
                if (string.IsNullOrWhiteSpace(primaryFieldName)) { intValue = 0; break; }

                try
                {
                    var primaryField = entry.EntityKey.EntityKeyValues.FirstOrDefault(e => e.Key.EqualsIgnoreCase(primaryFieldName));
                    if (primaryField != null)
                        intValue = primaryField.Value.ToString().ToInt();
                }
                catch { }

                if (entry.State == EntityState.Added) break;
                ignoreFields.Add(primaryFieldName);

            } while (intValue == NoValue);

            return intValue;

        }

        public static Int32 PrimaryColumnValue(this ObjectStateEntry entry)
        {
            int value = entry.PrimaryColumnValueFirstInt();

            if (value == 0 && entry.State != EntityState.Added) value = entry.IdentityColumnValue();

            return value;
        }

        public static Int32 IdentityColumnValue(this ObjectStateEntry entry)
        {
            try
            {
                var identityValue = entry.Entity.GetPropertyValueViaAttribute<DatabaseGeneratedAttribute, DatabaseGeneratedOption>(DatabaseGeneratedOption.Identity);

                string strValue = Convert.ToString(identityValue);
                return strValue.ToInt();
            }
            catch { }

            return 0;
        }

        public static List<string> PrimaryKeyTemplate(this Type type, ref string condition, bool isReferenceIdentity = false, bool excludeWhereClause = true) 
        {
            string _condition = "";
            List<string> columns = new List<string>();
            bool _refKeyIdentified = false;

            foreach (PropertyInfo P in type.GetProperties())
            {             
                foreach ( var Attrs in P.CustomAttributes)
                {
                    if ( Attrs.AttributeType.Name.EqualsIgnoreCase("keyAttribute") )
                    {
                        string _name = P.Name.ToUpper();
                        columns.Add(_name);                        
                        _condition += string.Format("{0} = '[{0}]'@",_name);

                        if (P.DataType() == DataTypes.NUMERIC) _refKeyIdentified = true;

                        break;
                    }

                    if (isReferenceIdentity && _refKeyIdentified) break;
                }
            
            };

            if (!_condition.Empty())
            {
                condition = string.Format(" {0}{1}", excludeWhereClause ? "" : " WHERE ", _condition.TrimEx("@").Replace("@", " AND ")).Trim();
                return columns;
            }

            return null;
        }

        public static string ReferenceKeyTemplate(this Type type)
        {
            string column = "";
            List<string> columns = type.PrimaryKeyTemplate(ref column, true );
            
            return (columns != null  && columns.Count > 0 ) ? columns[0] : null;
        }

        public static string ProfileKeyColumn(this Type type, List<string> colNames)
        {
            foreach (PropertyInfo P in type.GetProperties())
            {
                var _col = colNames.Find(C => C.EqualsIgnoreCase(P.Name));
                if (_col != null) return _col;
            }
                        
            return null;
        }


        public static string PrimaryKeyCondition(this ObjectStateEntry entry, bool excludeWhereClause = false)
        {

            string condition = "";
            if (entry.EntityKey.EntityKeyValues != null)
            {
                foreach (EntityKeyMember member in entry.EntityKey.EntityKeyValues)
                {
                    condition += (condition.Length > 0 ? " AND " : "") + string.Format(" {0} = '{1}' ", member.Key, member.Value);
                }
            }
            return string.IsNullOrWhiteSpace(condition) ? "" : (excludeWhereClause ? "" : " Where ") + condition;
        }


        public static string PrimaryKeyCondition(this IEnumerable<ObjectStateEntry> entries, bool excludeWhereClause = false)
        {
            //if (entries.Count() <= 1) return entries.ElementAt(0).PrimaryKeyCondition();

            string condition = "";
            foreach (var entry in entries)
            {
                string innerCondition = "";
                if (!(innerCondition = entry.PrimaryKeyCondition(true)).Empty())
                {
                    condition += string.Format("|( {0} ) ", innerCondition);
                }
            }

            condition = condition.Trim().Trim(new char[] { '|' }).Replace("|", " OR ");
            return condition.Empty() ? "" : (excludeWhereClause ? "" : " Where ") + condition;
        }

        public static string GetColumnValue(this ObjectStateEntry entry, string field, bool returnNull = false)
        {
            try
            {
                CurrentValueRecord current = entry.CurrentValues;
                string value = current.GetValue(current.GetOrdinal(field)).ToStringExt().ToEmpty();

                return value;
            }
            catch
            {
                return returnNull ? null : string.Empty;
            }
        }

        public static string GetKnownColumnName(this ObjectStateEntry entry, List<string> fieldNames)
        {
            foreach (var field in fieldNames)
            {
                try
                {
                    CurrentValueRecord current = entry.CurrentValues;
                    int fieldIndex = current.GetOrdinal(field);

                    if (fieldIndex >= 0) return field;
                }
                catch { }
            }

            return null;
        }

        public static string CurrentUserName(this List<ObjectStateEntry> entries)
        {
            string value = string.Empty; //WebIdentity.CurrentIdentityUserName;

            //value = value.EqualsIgnoreCase(Defaults.SystemUser) ? string.Empty : value;

            foreach (ObjectStateEntry entry in entries)
            {
                IList<string> columnsToIgnore = new List<string>();
                while (value.Empty())
                {
                    string userField = entry.Entity.UserField(entry.State == EntityState.Added, columnsToIgnore);

                    if (userField.Empty()) break;

                    columnsToIgnore.Add(userField);
                    value = entry.GetColumnValue(userField, true);
                }
                if (!value.Empty()) break;
            }

            return value.ToEmpty();
        }


        /// <summary>
        /// Sets entity state to Modified
        /// </summary>
        /// <param name="db">The Context Object</param>
        /// <param name="entity">For the entity to set</param>
        public static void SetEntityStateToModified(this DbContext db, object entity)
        {
            ObjectContext ctx = ((IObjectContextAdapter)db).ObjectContext;
            var documentEntry = ctx.ObjectStateManager.GetObjectStateEntry(entity);
            documentEntry.ChangeState(EntityState.Modified);
        }

    }
}