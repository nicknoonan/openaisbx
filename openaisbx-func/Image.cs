using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace openaisbx
{
    class Image
    {
        public Guid image_id;
        public string blob_link;
        public string prompt;
        public string date_created;
        public Image(Guid image_id, string blob_link, string prompt, string date_created)
        {
            this.image_id = image_id;
            this.blob_link = blob_link;
            this.prompt = prompt;
            this.date_created = date_created;
        }

        public static List<Image> MapReaderToList(SqlDataReader reader)
        {
            List<Image> image_list = new List<Image>();
            while(reader.Read())
            {
                Guid guid = new Guid(reader[0].ToString());
                string blob_link = reader[1].ToString();
                string prompt = reader[2].ToString();
                string date_created = reader[3].ToString();
                Image image = new Image(guid,blob_link, prompt, date_created);
                image_list.Add(image);
            }
            return image_list;
        }
    }
}
