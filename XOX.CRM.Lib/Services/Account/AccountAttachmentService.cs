using CRM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.Services;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class AccountAttachmentService : IAccountAttachmentService
    {
        public bool AddFiles(List<String> filespath, long AccId, long UserId = 0)
        {
            if(UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_ACCNT
                            where d.ROW_ID == AccId
                            select d;

                if (user.Count() > 0)
                {
                    foreach(var filepath in filespath){
                        XOX_T_ACCNT_ATT n = new XOX_T_ACCNT_ATT();
                        n.ACCNT_ID = AccId;
                        n.FILE_PATH_NAME = filepath;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_ACCNT_ATT.Add(n);
                        DBContext.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<FileVO> GetFiles(long accntId)
        {
            List<FileVO> resultFiles = new List<FileVO>();
            using (var DBContext = new CRMDbContext())
            {
                var files = from d in DBContext.XOX_T_ACCNT_ATT
                            where d.ACCNT_ID == accntId
                            && d.LAST_UPD_BY == null ///
                            select d;
                if (files.Count() > 0)
                    resultFiles = Mapper.Account.Map(files.ToList());
            }
            return resultFiles;
        }

        public bool RemoveFile(long FileId)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var att = from d in DBContext.XOX_T_ACCNT_ATT
                          where d.ROW_ID == FileId
                          select d;

                if (att.Count() > 0)
                {
                    att.First().LAST_UPD = DateTime.Now;
                    att.First().LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();
                }

                return true;
            }
        }
                
    }
}
