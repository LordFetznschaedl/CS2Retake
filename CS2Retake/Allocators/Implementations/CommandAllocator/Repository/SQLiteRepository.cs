using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using CS2Retake.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Interfaces;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Repository
{
    public class SQLiteRepository : IDisposable, IRetakeRepository
    {
        private SQLiteConnection _connection;

        public SQLiteRepository(string path)
        {
            this._connection = new SQLiteConnection($"Data Source={path}/cs2retake.db;Version=3;");
            this.Init();
        }

        private void OpenConnection()
        {
            try
            {
                this._connection.Open();
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, $"Error while creating a connection to the cs2retake.db - Message: {ex.Message}");
            }
        }

        public void Init()
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuyPrimary (UserId UNSIGNED BIG INT, WeaponString VARCHAR(255), Team INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuySecondary (UserId UNSIGNED BIG INT, WeaponString VARCHAR(255), Team INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuyAWPChance (UserId UNSIGNED BIG INT, AWPChance INT, Team INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS MidPrimary (UserId UNSIGNED BIG INT, WeaponString VARCHAR(255), Team INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS MidSecondary (UserId UNSIGNED BIG INT, WeaponString VARCHAR(255), Team INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS Pistol (UserId UNSIGNED BIG INT, WeaponString VARCHAR(255), Team INT)";
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        public void Dispose() 
        { 
            if(this._connection.State != System.Data.ConnectionState.Closed)
            {
                this._connection.Close();
            }
        }

        public bool InsertOrUpdateFullBuyPrimaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@weapon", weaponString);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM FullBuyPrimary WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE FullBuyPrimary SET WeaponString = @weapon WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO FullBuyPrimary (UserId, WeaponString, Team) VALUES (@id, @weapon, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool InsertOrUpdateFullBuySecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@weapon", weaponString);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM FullBuySecondary WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE FullBuySecondary SET WeaponString = @weapon WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO FullBuySecondary (UserId, WeaponString, Team) VALUES (@id, @weapon, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool InsertOrUpdateFullBuyAWPChance(ulong userId, int chance, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@chance", chance);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM FullBuyAWPChance WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE FullBuyAWPChance SET AWPChance = @chance WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO FullBuyAWPChance (UserId, AWPChance, Team) VALUES (@id, @chance, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool InsertOrUpdateMidPrimaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@weapon", weaponString);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM MidPrimary WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE MidPrimary SET WeaponString = @weapon WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO MidPrimary (UserId, WeaponString, Team) VALUES (@id, @weapon, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool InsertOrUpdateMidSecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@weapon", weaponString);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM MidSecondary WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE MidSecondary SET WeaponString = @weapon WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO MidSecondary (UserId, WeaponString, Team) VALUES (@id, @weapon, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public bool InsertOrUpdatePistolWeaponString(ulong userId, string weaponString, int team)
        {
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@weapon", weaponString);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT COUNT(*) FROM Pistol WHERE UserId = @id AND Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            var stmt = string.Empty;
            while (reader.Read()) 
            {
                if (reader.GetInt32(0) > 0)
                {
                    stmt = $"UPDATE Pistol SET WeaponString = @weapon WHERE UserId = @id AND Team = @team";
                }
                else
                {

                    stmt = $"INSERT INTO Pistol (UserId, WeaponString, Team) VALUES (@id, @weapon, @team)";
                }
            }

            reader.Close();

            cmd.CommandText = stmt;

            cmd.Prepare();

            return cmd.ExecuteNonQuery() == 1;
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetFullBuyWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (null, null, null);

            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT DISTINCT fp.WeaponString, fs.WeaponString, fa.AWPChance FROM FullBuyPrimary AS fp INNER JOIN FullBuySecondary AS fs ON fp.UserId = fs.UserId INNER JOIN FullBuyAWPChance AS fa ON fp.UserId = fa.UserId WHERE fp.UserId = @id AND fp.Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            if(reader.Read()) 
            {
                returnValue.primaryWeapon = reader.GetString(0);
                returnValue.secondaryWeapon = reader.GetString(1);
                returnValue.awpChance = reader.GetInt32(2);
            }

            return returnValue;
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetMidWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (null, null, 0);

            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT DISTINCT mp.WeaponString, ms.WeaponString FROM MidPrimary AS mp INNER JOIN MidSecondary AS ms ON mp.UserId = ms.UserId WHERE mp.UserId = @id AND mp.Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                returnValue.primaryWeapon = reader.GetString(0);
                returnValue.secondaryWeapon = reader.GetString(1);
            }

            return returnValue;
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetPistolWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (string.Empty, null, 0);

            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this.OpenConnection();
            }

            var cmd = this._connection.CreateCommand();

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@team", team);

            cmd.CommandText = $"SELECT DISTINCT p.WeaponString FROM Pistol AS p WHERE p.UserId = @id AND p.Team = @team";

            cmd.Prepare();

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                returnValue.secondaryWeapon = reader.GetString(0);
            }

            return returnValue;
        }
    }
}
