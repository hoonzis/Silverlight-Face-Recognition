using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;


namespace TestingSilverlightApp.Web.Technical
{
    public class FileAccessUtil
    {
        public static String GetFolder()
        {
            
            //if (RoleEnvironment.IsAvailable)
            //{
            //    LocalResource photos = RoleEnvironment.GetLocalResource("photos");
            //    return photos.RootPath;

            //}
            //else
            //{
                String basePath = AppDomain.CurrentDomain.BaseDirectory;
                String photosFolder = basePath + "App_Data\\photos";
                return photosFolder;
                
            //}
            
        }

        public static String GetHaarCascade()
        {
            String basePath = AppDomain.CurrentDomain.BaseDirectory;
            return basePath + "\\App_Data\\haarcascade_frontalface_alt.xml";
        }
    }
}