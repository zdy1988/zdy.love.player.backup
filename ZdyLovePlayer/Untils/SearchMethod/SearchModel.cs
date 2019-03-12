using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Untils.SearchMethod
{
    public class SearchModel
    {
        public SearchModel() {
            if (this.fields == null)
            {
                this.fields = new List<ConditionItem>();
            }
        }

        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public string orderField { get; set; }
        public bool isAsc { get; set; }

        public List<ConditionItem> fields;

        public QueryModel GetQueryModel()
        {
            QueryModel queryModel = new QueryModel();
            if (this.fields.Count() > 0)
            {
                foreach (var item in this.fields)
                {
                    if (item.Value != null && item.Value.ToString().Trim() != "")
                    {
                        if (item.Field.EndsWith("_Start") || item.Field.EndsWith("_End"))
                        {
                            item.Field = item.Field.Substring(0, item.Field.LastIndexOf('_'));
                        }
                        queryModel.Items.Add(item);
                    }
                }
            }
            return queryModel;
        }
    }
}
