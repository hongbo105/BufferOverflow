using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyFixIt3.Persistence
{
    public interface IPhotoService
    {
       Task<string> UploadPhotoAsync(HttpPostedFileBase photo);
    }
}
