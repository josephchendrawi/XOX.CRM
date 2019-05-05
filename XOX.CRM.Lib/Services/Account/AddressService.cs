using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.Services;

namespace XOX.CRM.Lib
{
    public class AddressService : IAddressService
    {
        public AddressInfoVO Get(long id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var addr = from d in DBContext.XOX_T_ADDR
                           where d.ROW_ID == id
                           select d;

                if (addr.Count() > 0)
                    return Mapper.Account.Map(addr.First());
                else
                    return null;
            }
        }

        public long Edit(AddressInfoVO vo, long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var addr = from d in DBContext.XOX_T_ADDR
                           where d.ROW_ID == vo.AddressId
                           select d;

                if (addr.Count() > 0)
                {
                    var a = addr.First();
                    a.ADDR_1 = vo.AddressLine1;
                    a.ADDR_2 = vo.AddressLine2;
                    a.CITY = vo.City;
                    a.COUNTRY = vo.Country;
                    a.POSTAL_CD = vo.Postcode;
                    a.STATE = vo.State;
                    //a.STATUS_CD = vo.Status;
                    //a.ADDR_TYPE = vo.AddressType;

                    a.LAST_UPD = DateTime.Now;
                    a.LAST_UPD_BY = UserId;

                    DBContext.ChangeTracker.DetectChanges();
                    var objectState = ((IObjectContextAdapter)DBContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(a);
                    AuditService auditDal = new AuditService(UserId, XOXConstants.AUDIT_MODULE_ADDRESS, XOXConstants.AUDIT_ACTION_UPDATE);
                    auditDal.Check(objectState, a.ROW_ID);
                    DBContext.SaveChanges();
                    DBContext.SaveChanges();

                    return a.ROW_ID;
                }
                else
                {
                    return Add(vo, UserId);
                }
            }
        }

        public long Add(AddressInfoVO vo, long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_ADDR a = new XOX_T_ADDR();
                a.ADDR_1 = vo.AddressLine1;
                a.ADDR_2 = vo.AddressLine2;
                a.CITY = vo.City;
                a.COUNTRY = vo.Country;
                a.POSTAL_CD = vo.Postcode;
                a.STATE = vo.State;
                //a.STATUS_CD = vo.Status;
                //a.ADDR_TYPE = vo.AddressType;
                a.ADDR_NAME = "TEMP"; ////
                a.CREATED = DateTime.Now;
                a.CREATED_BY = UserId;

                DBContext.XOX_T_ADDR.Add(a);
                DBContext.SaveChanges();

                a.ADDR_NAME = "A" + a.ROW_ID.ToString().PadLeft(8, '0');
                DBContext.SaveChanges();

                return a.ROW_ID;
            }
        }



    }
}
