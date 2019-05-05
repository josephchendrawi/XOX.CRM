using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IAddressService
    {
        AddressInfoVO Get(long id);
        long Edit(AddressInfoVO vo, long UserId);
        long Add(AddressInfoVO vo, long UserId);
    }
}
