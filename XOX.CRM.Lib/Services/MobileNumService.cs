using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class MobileNumService : IMobileNumService
    {
        public List<MobileNumVO> GetAll()
        {
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_REV_NUM
                           select d;

                if (ett.Count() > 0)
                    return Mapper.MobileNumMapper.Map(ett.ToList());
                else
                    return new List<MobileNumVO>();
            }
        }
        public MobileNumVO Get(long Id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_REV_NUM
                          where d.ROW_ID == Id
                          select d;

                if (ett.Count() > 0)
                    return Mapper.MobileNumMapper.Map(ett.First());
                else
                    return new MobileNumVO();
            }
        }

        public long Add(MobileNumVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_REV_NUM ett = new XOX_T_REV_NUM();
                ett.MAIN_NUM = vo.MSISDN;
                ett.PRICE = vo.Price;
                ett.BATCH_NUM = vo.BatchNum;
                ett.CREATED = DateTime.Now;
                ett.CREATED_BY = UserId;

                DBContext.XOX_T_REV_NUM.Add(ett);
                DBContext.SaveChanges();

                return ett.ROW_ID;
            }
        }

        public bool Edit(MobileNumVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_REV_NUM
                          where d.ROW_ID == vo.MobileNumId
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();
                    v.MAIN_NUM = vo.MSISDN;
                    v.PRICE = vo.Price;
                    v.BATCH_NUM = vo.BatchNum;
                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
        }

        public List<AccountVO> GetBatchAssignedUser(string BatchNum)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_ACCNT_REV
                          join e in DBContext.XOX_T_REV_NUM on d.REV_ID equals e.ROW_ID
                          join f in DBContext.XOX_T_ACCNT on d.ACCNT_ID equals f.ROW_ID
                          where e.BATCH_NUM == BatchNum
                          select f;
                
                var aList = Mapper.Account.Map(ett.Distinct().ToList<XOX_T_ACCNT>());

                return aList;
            }
        }

        //public void BatchUnAssignUser(string BatchNum, long AccountId)
        //{
        //    long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

        //    using (var DBContext = new CRMDbContext())
        //    {
        //        var ett = from d in DBContext.XOX_T_ACCNT_REV
        //                  join e in DBContext.XOX_T_REV_NUM on d.REV_ID equals e.ROW_ID
        //                  join f in DBContext.XOX_T_ACCNT on d.ACCNT_ID equals f.ROW_ID
        //                  where e.BATCH_NUM == BatchNum && d.ACCNT_ID == AccountId
        //                  select d;

        //        foreach (var v in ett)
        //        {
        //            DBContext.XOX_T_ACCNT_REV.Remove(v);
        //        }

        //        DBContext.SaveChanges();
        //    }
        //}

        public void BatchAssignUser(string BatchNum, List<long> AccountIdList)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var acc = from d in DBContext.XOX_T_ACCNT
                          where AccountIdList.Contains(d.ROW_ID)
                          select d;

                var mobilenum = from e in DBContext.XOX_T_REV_NUM
                                where e.BATCH_NUM == BatchNum
                                select e;

                var AssignList = new List<AssignData>();
                foreach (var w in mobilenum)
                {
                    AssignList.Add(new AssignData()
                    {
                        MSISDN = w.MAIN_NUM,
                        Price = w.PRICE ?? 0,
                        Members = string.Join(",", acc.Select(m => m.INTEGRATION_ID))
                    });

                    foreach (var v in acc)
                    {
                        XOX_T_ACCNT_REV n = new XOX_T_ACCNT_REV();
                        n.ACCNT_ID = v.ROW_ID;
                        n.REV_ID = w.ROW_ID;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_ACCNT_REV.Add(n);
                    }
                }

                if (AssignList.Count() > 0)
                {
                    var Result = EAIService.AssignMSISDN(new AssignMSISDN()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name)
                        },
                        List = AssignList
                    });

                    if (Result.ToLower() == "true")
                    {
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        throw new Exception(Result);
                    }
                }
            }
        }

    }
}
