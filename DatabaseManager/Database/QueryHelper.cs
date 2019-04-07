using DatabaseManager.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Database
{
    public static class QueryHelper
    {
        public static IList<Artist> GetAllArtists()
        {
            if(!File.Exists(DB_Constants.DB_Artist_Path))
            {
                return null;
            }

            var lines = File.ReadAllText(DB_Constants.DB_Artist_Path);
            IList<Artist> result = JsonConvert.DeserializeObject<List<Artist>>(lines);

            return result;
        }
    }
}
