using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using MyFixIt3.Persistence;

namespace MyFixIt3.Controllers
{
    public class TasksController : Controller
    {
        private IFixItQueueManager queueManager = new FixItQueueManager();

        // GET: Tasks
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FixItTaskId,CreatedBy,Owner,Title,Notes,PhotoUrl,IsDone")]FixItTask fixittask, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                fixittask.CreatedBy = User.Identity.Name;
                // fixittask.PhotoUrl = await photoService.UploadPhotoAsync(photo);
                fixittask.PhotoUrl = "http://justafakeurl";

                await queueManager.SendMessageAsync(fixittask);

                //if (ConfigurationManager.AppSettings["UseQueues"] == "true")
                //{
                //    await queueManager.SendMessageAsync(fixittask);
                //}
                //else
                //{
                //    await fixItRepository.CreateAsync(fixittask);
                //}

                // return RedirectToAction("Success");

            }

            return View();
            // return View(fixittask);
        }
    }
}