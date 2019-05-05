using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IContactService
    {
        ContactVO Get(long id);
        long Edit(ContactVO vo, long UserId);
        long Add(ContactVO vo, long UserId);
    }
}
