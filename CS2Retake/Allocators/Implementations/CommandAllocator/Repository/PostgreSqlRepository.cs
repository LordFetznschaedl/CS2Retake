using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Interfaces;
using CS2Retake.Utils;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Repository
{
    public class PostgreSqlRepository : IDisposable, IRetakeRepository
    {
        private List<NpgsqlConnection> _connectionPool = new List<NpgsqlConnection>();

        private string _connectionString = string.Empty;


        public PostgreSqlRepository(string connectionString)
        {
            this._connectionString = connectionString;
            this.Init();
        }

        private void OpenConnection()
        {
            try
            {
                this.GetConnectionFromPool();
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, $"Error while creating a connection to the cs2retake.db - Message: {ex.Message}");
            }
        }

        private NpgsqlConnection GetConnectionFromPool()
        {
            var openConnections = this._connectionPool.Where(x => x.FullState == System.Data.ConnectionState.Closed);

            var connection = openConnections.FirstOrDefault() ?? null;

            if (!openConnections.Any() || connection == null) 
            {
                connection = new NpgsqlConnection(this._connectionString);

                this._connectionPool.Add(connection);
            }

            connection.Open();
            return connection;
        }

        public void Dispose()
        {
            foreach(var connection in this._connectionPool) 
            {
                connection.Close();
            }
            this._connectionPool.Clear();
        }

        public void Init()
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuyPrimary (UserId VARCHAR(64), WeaponString VARCHAR(255), Team INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuySecondary (UserId VARCHAR(64), WeaponString VARCHAR(255), Team INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FullBuyAWPChance (UserId VARCHAR(64), AWPChance INT, Team INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS MidPrimary (UserId VARCHAR(64), WeaponString VARCHAR(255), Team INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS MidSecondary (UserId VARCHAR(64), WeaponString VARCHAR(255), Team INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS Pistol (UserId VARCHAR(64), WeaponString VARCHAR(255), Team INT)";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if(connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public bool InsertOrUpdateFullBuyPrimaryWeaponString(ulong userId, string weaponString, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }

        public bool InsertOrUpdateFullBuySecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }

        public bool InsertOrUpdateFullBuyAWPChance(ulong userId, int chance, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }

        public bool InsertOrUpdateMidPrimaryWeaponString(ulong userId, string weaponString, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }

        public bool InsertOrUpdateMidSecondaryWeaponString(ulong userId, string weaponString, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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

                return cmd.ExecuteNonQuery() == 1; ;
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }

        public bool InsertOrUpdatePistolWeaponString(ulong userId, string weaponString, int team)
        {
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
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
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return false;
        }


        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetFullBuyWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (null, null, null);

            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
                cmd.Parameters.AddWithValue("@team", team);

                cmd.CommandText = $"SELECT DISTINCT fp.WeaponString, fs.WeaponString, fa.AWPChance FROM FullBuyPrimary AS fp LEFT JOIN FullBuySecondary AS fs ON fp.UserId = fs.UserId LEFT JOIN FullBuyAWPChance AS fa ON fp.UserId = fa.UserId WHERE fp.UserId = @id AND fp.Team = @team";

                cmd.Prepare();

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnValue.primaryWeapon = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    returnValue.secondaryWeapon = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    returnValue.awpChance = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return returnValue;
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetMidWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (null, null, 0);
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
                cmd.Parameters.AddWithValue("@team", team);

                cmd.CommandText = $"SELECT DISTINCT mp.WeaponString, ms.WeaponString FROM MidPrimary AS mp LEFT JOIN MidSecondary AS ms ON mp.UserId = ms.UserId WHERE mp.UserId = @id AND mp.Team = @team";

                cmd.Prepare();

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnValue.primaryWeapon = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    returnValue.secondaryWeapon = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return returnValue;
        }

        public (string? primaryWeapon, string? secondaryWeapon, int? awpChance) GetPistolWeapons(ulong userId, int team)
        {
            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) returnValue = (string.Empty, null, 0);
            NpgsqlCommand? cmd = null;

            var connection = this.GetConnectionFromPool();

            try
            {
                cmd = connection.CreateCommand();

                cmd.Parameters.AddWithValue("@id", userId.ToString());
                cmd.Parameters.AddWithValue("@team", team);

                cmd.CommandText = $"SELECT DISTINCT p.WeaponString FROM Pistol AS p WHERE p.UserId = @id AND p.Team = @team";

                cmd.Prepare();

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnValue.secondaryWeapon = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }

                cmd.Dispose();

                return returnValue;
            }
            catch (Exception ex)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return returnValue;
        }
    }
}
