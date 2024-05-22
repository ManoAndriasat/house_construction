using System;
using Npgsql;
using Utils.helper;

namespace Models
{
    public class Admin
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Mdp { get; set; }

        public Admin()
        {
        }

        public Admin(string id, string nom, string mdp)
        {
            Id = id;
            Nom = nom;
            Mdp = mdp;
        }

        // public static bool IsAdmin(string id, NpgsqlConnection conn)
        // {
        //     Admin admin = Admin.GetAdminById(id, conn);
        //     return admin != null;
        // }

        public static Admin GetAdminByNom(string nom, string mdp, NpgsqlConnection conn)
        {
            Admin admin = null;
            try
            {
                string sql = "SELECT * FROM admin WHERE nom = @nom and mdp = @mdp";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@mdp", mdp);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string adminId = reader["id"].ToString();
                        string adminNom = reader["nom"].ToString();
                        string adminMdp = reader["mdp"].ToString();
                        admin = new Admin(adminId, adminNom, adminMdp);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return admin;
        }
    }
}
