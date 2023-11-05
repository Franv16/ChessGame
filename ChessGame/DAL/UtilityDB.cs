﻿using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Nez;
using Nez.Persistence;
using Nez.UI;
using Npgsql;

namespace ChessGame.DAL;

public class UtilityDB
{
    
    public static bool ValidatePlayer(string username, string password)
    {
        using var conn = GetConnection();
        using var cmd =
            new NpgsqlCommand("Select Count(*) from players where Username = (@pUsername) and Password = (@pPassword)", conn)
            {
                Parameters =
                {
                    new("@pUsername", username),
                    new("@pPassword", password)
                }
            };

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                if (Convert.ToInt32(reader["count"].ToString()) == 1)
                {
                    conn.Close();
                    return true;
                } 
            }
        }
        
        conn.Close();
        return false;
    }
    
    public static bool UserExist(string username)
    {
        using var conn = GetConnection();
        using var cmd =
            new NpgsqlCommand("Select Count(*) from players where Username = (@pUsername)", conn)
            {
                Parameters =
                {
                    new("@pUsername", username)
                }
            };

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                if (Convert.ToInt32(reader["count"].ToString()) == 1)
                {
                    conn.Close();
                    return true;
                } 
            }
        }
        
        conn.Close();
        return false;
    }

    private static NpgsqlConnection GetConnection()
    {
        // add config to the game pipeline.
        // load config and read content
        
        var jsonString = File.ReadAllText("Content/Config.json");
        var MyConfig = Json.FromJson<DBConfig>(jsonString);
        using var dataSource = NpgsqlDataSource.Create(MyConfig.ToString());
        
        return dataSource.OpenConnection();
    }


    public static void CreateUser(string userName, string password)
    {
        using var conn = GetConnection();
        using var cmd =
            new NpgsqlCommand("Insert into players values (@pUsername , @pPassword, 0)", conn)
            {
                Parameters =
                {
                    new("@pUsername", userName),
                    new("@pPassword", password)
                }
            };

        cmd.ExecuteNonQuery();
        
        conn.Close();
    }
}