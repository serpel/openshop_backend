using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenshopBackend.BussinessLogic
{
    public class Uploader
    {
        private static Uploader instance;

        public static Uploader GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Uploader();
                }

                return instance;
            }
        }

        private Uploader(){}

        public string GenerateUrlPath(string fullPath, string baseUrl, HttpPostedFileBase file)
        {
            CreateIfMissing(fullPath);

            var imagePath = Path.Combine(fullPath, file.FileName);
            file.SaveAs(imagePath);

            return Path.Combine(baseUrl, file.FileName);         
        }

        public void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
        }
    }
}