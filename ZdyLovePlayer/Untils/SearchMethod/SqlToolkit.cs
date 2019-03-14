using Infrastructure.SearchModel.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZdyLovePlayer.Untils.SearchMethod
{
    public class SqlToolkit
    {
        /// <summary>
        /// 获取数据条数Sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string GetRowCountSql(string sql)
        {
            return $"select count(1) from ({sql}) rc";
        }

        /// <summary>
        /// 获取数据分页sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数据条数</param>
        /// <returns></returns>
        public static string GetDataPagedSql(string sql, string orderField, int pageIndex, int pageSize)
        {
            return $"SELECT * FROM ({sql}) ORDER BY {orderField} LIMIT {pageSize} OFFSET ({pageIndex} - 1)*{pageSize}";
        }

        /// <summary>
        /// 获取数据分页sql语句
        /// </summary>
        /// <returns></returns>
        public static string GetDataPagedSql(string sql, string orderField, bool isAsc, int pageIndex, int pageSize)
        {
            string desc = isAsc ? "" : " desc";
            string order = orderField + desc;
            return GetDataPagedSql(sql, order, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取查询条件sql语句
        /// </summary>
        /// <param name="queryModel">查询对象</param>
        /// <returns></returns>
        public static string GetQueryWhere(QueryModel queryModel)
        {
            StringBuilder sb = new StringBuilder();
            if (queryModel.Items.Count > 0)
            {
                List<KeyValuePair<string, string>> searchItems = new List<KeyValuePair<string, string>>();
                for (var i = 0; i < queryModel.Items.Count; i++)
                {
                    var item = queryModel.Items[i];
                    //以“NS__”开头的不是查询条件
                    if (!item.Field.StartsWith("NS_") && item.Value != null && item.Value.ToString() != "")
                    {
                        if (item.Field.EndsWith("_Start") || item.Field.EndsWith("_End"))
                        {
                            item.Field = item.Field.Substring(0, item.Field.LastIndexOf('_'));
                        }
                        var format = "";
                        switch ((int)item.Method)
                        {
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.Equal:
                                format = " {0}='{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.LessThan:
                                format = " {0}<'{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.GreaterThan:
                                format = " {0}>'{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.LessThanOrEqual:
                                format = " {0}<='{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.GreaterThanOrEqual:
                                format = " {0}>='{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.Like:
                                format = " {0} like '%{1}%' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.Contains:
                                format = " {0} like '%{1}%' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.NotEqual:
                                format = " {0} <> '{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.StartsWith:
                                format = " {0} like '{1}%' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.EndsWith:
                                format = " {0} like '%{1}' ";
                                break;
                            case (int)Infrastructure.SearchModel.Model.QueryMethod.In:
                                format = " {0} in ({1}) ";
                                break;
                            default:
                                format = " {0} ='{1}' ";
                                break;
                        }

                        searchItems.Add(new KeyValuePair<string, string>(string.IsNullOrEmpty(item.OrGroup) ? i.ToString() : item.OrGroup, string.Format(format, item.Field, item.Value)));
                    }
                }

                searchItems.Select(t => t.Key).Distinct().ToList().ForEach(key =>
                {
                    string orGroupItem = "";
                    searchItems.Where(item => item.Key == key).ToList().ForEach(item =>
                    {
                        orGroupItem = string.IsNullOrEmpty(orGroupItem) ? item.Value : string.Format(" {0} or {1} ", orGroupItem, item.Value);
                    });
                    orGroupItem = string.Format("({0})", orGroupItem);
                    if (string.IsNullOrEmpty(sb.ToString()))
                    {
                        sb.Append(orGroupItem);
                    }
                    else
                    {
                        sb.Append(" and " + orGroupItem);
                    }
                });
            }
            return sb.ToString();
        }
    }
}
