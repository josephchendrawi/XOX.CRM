using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class ContactService : IContactService
    {
        public ContactVO Get(long AccountId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_CONTACT
                           join e in DBContext.XOX_T_CON_ACCNT on d.ROW_ID equals e.CON_ID
                           where e.ACCNT_ID == AccountId
                           select d;

                if (result.Count() > 0)
                {
                    return new ContactVO()
                    {
                        ContactId = result.First().ROW_ID,
                        ContactNumber = result.First().MOBILE_NO
                    };
                }
                else
                    return null;
            }
        }

        public long Edit(ContactVO vo, long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_CONTACT
                             join e in DBContext.XOX_T_CON_ACCNT on d.ROW_ID equals e.CON_ID
                             where e.ACCNT_ID == vo.AccountId
                             select d;

                if (result.Count() > 0)
                {
                    var a = result.First();
                    a.MOBILE_NO = vo.ContactNumber;

                    a.LAST_UPD = DateTime.Now;
                    a.LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();

                    return a.ROW_ID;
                }
                else
                {
                    return Add(vo, UserId);
                }
            }
        }

        public long Add(ContactVO vo, long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_CONTACT a = new XOX_T_CONTACT();
                a.MOBILE_NO = vo.ContactNumber;
                a.CREATED = DateTime.Now;
                a.CREATED_BY = UserId;
                DBContext.XOX_T_CONTACT.Add(a);
                DBContext.SaveChanges();

                XOX_T_CON_ACCNT b = new XOX_T_CON_ACCNT();
                b.ACCNT_ID = vo.AccountId;
                b.CON_ID = a.ROW_ID;
                a.CREATED = DateTime.Now;
                a.CREATED_BY = UserId;
                DBContext.XOX_T_CON_ACCNT.Add(b);
                DBContext.SaveChanges();                

                return a.ROW_ID;
            }
        }



    }
}
