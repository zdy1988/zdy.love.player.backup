using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Untils
{
    public static class NetWorkHelper
    {
        public static bool IsUrlExist(string Url)
        {
            bool IsExist = false;


            try
            {
                var url = new Uri(Url);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.Expect100Continue = false;
                ((HttpWebResponse)request.GetResponse()).Close();
                IsExist = true;
            }
            catch (UriFormatException ex)
            {
                IsExist = false;
            }
            catch (WebException ex)
            {
                //if (exception.Status != WebExceptionStatus.ProtocolError)
                //{
                //    return num;
                //}
                //if (exception.Message.IndexOf("500 ") > 0)
                //{
                //    return 500;
                //}
                //if (exception.Message.IndexOf("401 ") > 0)
                //{
                //    return 401;
                //}
                //if (exception.Message.IndexOf("404") > 0)
                //{
                //    num = 404;
                //}
                IsExist = false;
            }

            return IsExist;
        }
    }
}
