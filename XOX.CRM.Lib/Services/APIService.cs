using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class APIService
    {
        public static bool isAuth(long UserId, string APIKey)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var result = from d in DBContext.XOX_T_IS_KEY
                                 where d.XOX_IS_ID == UserId
                                 select d;

                    if (result.Count() > 0)
                    {
                        result = result.Where(d => d.API_KEY == APIKey);
                        if (result.Count() > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (APIKey == XOXConstants.FirstTimeAPIKey)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateAPIKey(long UserId, string APIKey)
        {
            return "";
            //string tokensalt = Security.RandomString(60);
            //string token = Security.Encrypt(tokensalt, UserId.ToString());

            //using (var DBContext = new CRMDbContext())
            //{
            //    var result = from d in DBContext.XOX_T_IS_KEY
            //                 where d.XOX_IS_ID == UserId
            //                 select d;

            //    if (result.Count() > 0)
            //    {
            //        result = result.Where(d => d.API_KEY == APIKey);
            //        if (result.Count() > 0)
            //        {
            //            result.First().API_KEY = token;
            //            result.First().LAST_UPD = DateTime.Now;
            //            DBContext.SaveChanges();

            //            return token;
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        if (APIKey == XOXConstants.FirstTimeAPIKey)
            //        {
            //            XOX_T_IS_KEY n = new XOX_T_IS_KEY();
            //            n.API_KEY = token;
            //            n.XOX_IS_ID = UserId;
            //            DBContext.XOX_T_IS_KEY.Add(n);
            //            DBContext.SaveChanges();

            //            return token;
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }
            //}
        }

    }
}
