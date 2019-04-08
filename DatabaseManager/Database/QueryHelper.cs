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

        public static IList<Album> GetAllAlbums()
        {
            if (!File.Exists(DB_Constants.DB_Album_Path))
            {
                return null;
            }

            var lines = File.ReadAllText(DB_Constants.DB_Album_Path);
            IList<Album> result = JsonConvert.DeserializeObject<List<Album>>(lines);

            return result;
        }

        public static IList<Collaboration> GetAllCollaborations()
        {
            if (!File.Exists(DB_Constants.DB_Collaboration_Path))
            {
                return null;
            }

            var lines = File.ReadAllText(DB_Constants.DB_Collaboration_Path);
            IList<Collaboration> result = JsonConvert.DeserializeObject<List<Collaboration>>(lines);

            return result;
        }

        public static Artist GetArtistById(int p_ArtistId)
        {
            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {

                }
            }
            return null;
        }
    }
}
