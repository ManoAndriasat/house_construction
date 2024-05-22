﻿using System;
using Npgsql;

namespace Utils.helper
{
    public class Connection
    {
        private string connectionString;
        private NpgsqlConnection connection;
        string server = "localhost";
        string database = "eval";
        string username = "postgres";
        string password = "Mano-123";

        public Connection()
        {
            connectionString = $"Server={server};Database={database};User Id={username};Password={password};";
        }

        public NpgsqlConnection GetConnection()
        {
            try
            {
                connection = new NpgsqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (NpgsqlException ex)
            {
                return null;
            }
        }
    }
}