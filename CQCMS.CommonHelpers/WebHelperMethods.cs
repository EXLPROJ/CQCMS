using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQCMS.CommonHelpers
{
    public class WebHelperMethods
    {
        private static Logger logger = LogManager.GetLogger("EmailTransformation");

        public static string Savebase64image(string base64string, string extension, string ImageDestinationPath)
        {
            byte[] bytes = Convert.FromBase64String(base64string);

            string filename = "ggg" + Guid.NewGuid().ToString().Substring(0, 12) + "." + extension;
            string path = Path.Combine(ImageDestinationPath, filename);

            System.IO.Directory.CreateDirectory(ImageDestinationPath);

            using (var imageFile = new FileStream(path, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
                imageFile.Dispose();

            }
            return path;
        }
    }
}
