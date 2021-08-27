using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public class RecipeRepository : IRecipeRepository {
        private readonly SqlConnection _connection;

        public RecipeRepository(IConfiguration Config) {
            var connectionString = Config.GetConnectionString("DefaultConnection");
            _connection = new SqlConnection(connectionString);
        }

        public async Task<Guid> CreateAsync( Recipe item ) {
            await _connection.OpenAsync();
            item.Id = Guid.NewGuid();
            var queryString = "INSERT INTO RecipeManager.dbo.RecipeTbl (Id, OwnerId, Name, Description, IsPublic) VALUES(@Id, @OwnerId, @Name, @Description, @IsPublic)";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@OwnerId", item.OwnerId);
            command.Parameters.AddWithValue("@Name", item.Name);
            command.Parameters.AddWithValue("@Description", item.Description);
            command.Parameters.AddWithValue("@IsPublic", item.IsPublic);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            return item.Id;
        }

        public async Task<IEnumerable<Recipe>> ReadAllAsync() {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, Name, Description, IsPublic FROM RecipeManager.dbo.RecipeTbl";
            SqlCommand command = new SqlCommand(queryString, _connection);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyRecipesAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Recipe>> ReadPublicAsync() {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, Name, Description, IsPublic FROM RecipeManager.dbo.RecipeTbl WHERE IsPublic = 1";
            SqlCommand command = new SqlCommand(queryString, _connection);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyRecipesAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Recipe>> ReadPrivateAsync(Guid userId) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, Name, Description, IsPublic FROM RecipeManager.dbo.RecipeTbl WHERE OwnerId = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", userId);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyRecipesAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Recipe>> ReadByQueryAsync( string queryString ) {
            if(!queryString.ToLower().StartsWith("select"))
                throw new InvalidOperationException("Only Select queries are supported as a queryString parameter by this method.");
            if (!queryString.ToLower().Contains("recipemanager.dbo.recipetbl"))
                throw new InvalidOperationException("Only queries against 'RecipeManager.dbo.RecipeTbl' are supported as a queryString parameter by this method.");
            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand( queryString, _connection );
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyRecipesAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Recipe> ReadByIdAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, Name, Description, IsPublic FROM RecipeManager.dbo.RecipeTbl WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadRecipeAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Recipe> UpdateAsync( Recipe item ) {
            await _connection.OpenAsync();
            var queryString = "Update RecipeManager.dbo.RecipeTbl SET Name = @Name, Description = @Description, IsPublic = @IsPublic WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@Name", item.Name ?? "");
            command.Parameters.AddWithValue("@Description", item.Description ?? "");
            command.Parameters.AddWithValue("@IsPublic", item.IsPublic);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadRecipeAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Recipe> SoftUpdateAsync( Recipe item ) {
            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand();
            var queryString = "Update RecipeManager.dbo.RecipeTbl SET";
            if (!string.IsNullOrEmpty(item.Name)) {
                queryString += " Name = @Name,";
                command.Parameters.AddWithValue("@Name", item.Name);
            }
            if (!string.IsNullOrEmpty(item.Description)) {
                queryString += " Description = @Description";
                command.Parameters.AddWithValue("@Description", item.Description);
            }
            queryString.TrimEnd(',');
            queryString += " WHERE Id = @Id";
            command.CommandText = queryString;
            command.Connection = _connection; 
            command.Parameters.AddWithValue("@Id", item.Id);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadRecipeAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task DeleteAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "DELETE FROM RecipeManager.dbo.RecipeTbl WHERE Id = @Id";
            var command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        private async Task<Recipe> ReadRecipeAsync(SqlDataReader reader) {
            var result = new Recipe();
            if (reader.HasRows) {
                await reader.ReadAsync();
                result.Id = reader.GetGuid(0);
                result.Name = reader.GetString(1);
                result.Description = reader.GetString(2);
                result.IsPublic = reader.GetBoolean(3);
            }
            await reader.CloseAsync();
            return result;
        }

        private async Task<IEnumerable<Recipe>> ReadManyRecipesAsync(SqlDataReader reader) {
            var result = new List<Recipe>();
            if (reader.HasRows) {
                while (await reader.ReadAsync()) {
                    result.Add(new Recipe { Id = reader.GetGuid(0), Name = reader.GetString(1), Description = reader.GetString(2), IsPublic = reader.GetBoolean(3) });
                }
            }
            await reader.CloseAsync();
            return result;
        }
    }
}
