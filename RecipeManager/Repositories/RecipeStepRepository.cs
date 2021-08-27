using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public class RecipeStepRepository : IRecipeStepRepository {
        private readonly SqlConnection _connection;

        public RecipeStepRepository(IConfiguration Config) {
            _connection = new SqlConnection(Config.GetConnectionString("DefaultConnection"));
        }

        public async Task<Guid> CreateAsync( Step item ) {
            //TODO:: check that the RecipeId is valid (record w/ id exists already) before saving otherwise throw invalidInput or something.
            await _connection.OpenAsync();
            var queryString = @"INSERT INTO RecipeManager.dbo.StepTbl (Id, RecipeId, Name, SortOrder, Detail, Duration, DurationUnit) 
                                  VALUES(@Id, @RecipeId, @Name, @SortOrder, @Detail, @Duration, @DurationUnit)";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@RecipeId", item.RecipeId);
            command.Parameters.AddWithValue("@Name", item.Name);
            command.Parameters.AddWithValue("@SortOrder", item.SortOrder ?? 0);
            command.Parameters.AddWithValue("@Detail", item.Detail ?? "");
            if (item.Duration == null) {
                command.Parameters.AddWithValue("@Duration", DBNull.Value); 
            } else {
                command.Parameters.AddWithValue("@Duration", item.Duration); 
            }
            command.Parameters.AddWithValue("@DurationUnit", item.DurationUnit ?? "");
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            return item.Id;
        }

        public async Task<IEnumerable<Step>> ReadAllAsync() {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, SortOrder, Detail, Duration, DurationUnit FROM RecipeManager.dbo.StepTbl";
            SqlCommand command = new SqlCommand(queryString, _connection);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyStepsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Step>> ReadRecipeStepsAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, SortOrder, Detail, Duration, DurationUnit FROM RecipeManager.dbo.StepTbl WHERE RecipeId = @RecipeId";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyStepsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Step>> ReadByQueryAsync( string queryString ) {
            if (!queryString.ToLower().StartsWith("select"))
                throw new InvalidOperationException("Only Select queries are supported as a queryString parameter by this method.");
            if (!queryString.ToLower().Contains("recipemanager.dbo.steptbl"))
                throw new InvalidOperationException("Only queries against 'RecipeManager.dbo.StepTbl' are supported as a queryString parameter by this method.");

            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand( queryString, _connection );
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyStepsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Step> ReadByIdAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, SortOrder, Detail, Duration, DurationUnit FROM RecipeManager.dbo.StepTbl WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadStepAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Step> UpdateAsync( Step item ) {
            await _connection.OpenAsync();
            var queryString = "Update RecipeManager.dbo.StepTbl SET Name = @Name, SortOrder=@SortOrder, Detail = @Detail, Duration = @Duration, DurationUnit = @DurationUnit WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@Name", item.Name ?? "");
            command.Parameters.AddWithValue("@SortOrder", item.SortOrder ?? 0);
            command.Parameters.AddWithValue("@Detail", item.Detail ?? "");
            command.Parameters.Add("@Duration", SqlDbType.Decimal).Value = item.Duration;
            if (item.Duration == null) {
                command.Parameters.Add("@Duration", SqlDbType.Decimal).Value = DBNull.Value;
            } else {
                command.Parameters.Add("@Duration", SqlDbType.Decimal).Value = item.Duration;
            }
            command.Parameters.AddWithValue("@DurationUnit", item.DurationUnit ?? "");
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadStepAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Step> SoftUpdateAsync( Step item ) {
            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand();
            var queryString = "Update RecipeManager.dbo.StepTbl SET";
            if (!string.IsNullOrEmpty(item.Name)) {
                queryString += " Name = @Name,";
                command.Parameters.AddWithValue("@Name", item.Name);
            }
            if (item.SortOrder.HasValue) {
                queryString += " SortOrder = @SortOrder,";
                command.Parameters.AddWithValue("@SortOrder", item.SortOrder);
            }
            if (!string.IsNullOrEmpty(item.Detail)) {
                queryString += " Detail = @Detail,";
                command.Parameters.AddWithValue("@Detail", item.Detail);
            }
            if (item.Duration.HasValue) {
                queryString += " Duration = @Duration,";
                if (item.Duration == null) {
                    command.Parameters.AddWithValue("@Duration", DBNull.Value);
                } else {
                    command.Parameters.AddWithValue("@Duration", item.Duration);
                }
            }
            if (item.Duration != null && !string.IsNullOrEmpty(item.DurationUnit)) {
                queryString += " DurationUnit = @DurationUnit,";
                command.Parameters.AddWithValue("@DurationUnit", item.DurationUnit ?? "");
            }
            queryString.TrimEnd(',');
            queryString += " WHERE Id = @Id";
            command.CommandText = queryString;
            command.Connection = _connection; 
            command.Parameters.AddWithValue("@Id", item.Id);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadStepAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task DeleteAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "DELETE FROM RecipeManager.dbo.StepTbl WHERE Id = @Id";
            var command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task DeleteRecipeStepsAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "DELETE FROM RecipeManager.dbo.StepTbl WHERE RecipeId = @RecipeId";
            var command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task<int> CurrentCountAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "SELECT COUNT(1) FROM RecipeManager.dbo.StepTbl WHERE RecipeId = @RecipeId";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            var reader = await command.ExecuteReaderAsync();
            await _connection.CloseAsync();
            await reader.ReadAsync();
            var count = reader.GetInt32(0);
            await reader.CloseAsync();            
            return count;
        }

        private async Task<Step> ReadStepAsync(SqlDataReader reader) {
            var result = new Step();
            if (reader.HasRows) {
                await reader.ReadAsync();
                result.Id = reader.GetGuid(0);
                result.RecipeId = reader.GetGuid(1);
                result.Name = reader.GetString(2);
                result.SortOrder = reader.GetInt32(3);
                result.Detail = reader.GetString(4);
                result.Duration = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5);
                result.DurationUnit = reader.IsDBNull(5) ? "" : reader.GetString(6);
                await reader.CloseAsync();
            }
            return result;
        }

        private async Task<IEnumerable<Step>> ReadManyStepsAsync(SqlDataReader reader) {
            var result = new List<Step>();
            if (reader.HasRows) {
                while (await reader.ReadAsync()) {
                    result.Add(new Step {
                        Id = reader.GetGuid(0),
                        RecipeId = reader.GetGuid(1),
                        Name = reader.GetString(2),
                        SortOrder = reader.GetInt32(3),
                        Detail = reader.GetString(4),
                        Duration = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                        DurationUnit = reader.IsDBNull(5) ? "" : reader.GetString(6)
                    });
                }
            }
            await reader.CloseAsync();
            return result;
        }
    }
}
