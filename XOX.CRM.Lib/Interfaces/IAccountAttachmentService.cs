using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IAccountAttachmentService
    {
        bool AddFiles(List<String> filespath, long AccId, long UserId = 0);
        List<FileVO> GetFiles(long accntId);
        bool RemoveFile(long FileId);
    }
}
