using System;
using Npgsql;
using Utils.helper;

namespace Models
{
    public class Users
    {
        public string numero { get; set; }

        public Users()
        {
        }

        public Users(string numero)
        {
            numero = numero;
        }

        public static Users GetUsersByNumero(string numero, NpgsqlConnection conn)
        {
            Users users = null;

            string sql = "SELECT * FROM users WHERE numero = @Numero";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Numero", numero);

            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    users = new Users(reader["numero"].ToString());
                }
            }
            return users;
        }
    }
}

