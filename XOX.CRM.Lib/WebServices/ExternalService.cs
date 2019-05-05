using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib.WebServices
{
    public class ExternalService
    {
        public static string CreateNewSubscriber(AddSubscriberProfile request)
        {
            var result = EAIService.AddSubscriberProfile(request);

            return result;
        }
        public static string CreateMNPSubscriber(AddSubscriberProfileMNP request)
        {
            var result = EAIService.AddSubscriberProfileMNP(request);

            return result;
        }
        public static string CreateMNPSubscriberSubs(AddSubscriberProfileMNPSubs request)
        {
            var result = EAIService.AddSubscriberProfileMNPSubs(request);

            return result;
        }
        public static string CreateCOBPSubsciber(AddSubscriberProfile request)
        {
            var result = EAIService.AddSubscriberProfileCOBP(request);

            return result;
        }
    }
}
