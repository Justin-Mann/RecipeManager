using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public class RecipeIngredientRepository : IRecipeIngredientRepository {
        private readonly SqlConnection _connection;

        public RecipeIngredientRepository(IConfiguration Config) {
            _connection = new SqlConnection(Config.GetConnectionString("DefaultConnection"));
        }

        public async Task<Guid> CreateAsync( Ingredient item ) {
            //TODO:: check that the RecipeId is valid (record w/ id exists already) before saving otherwise throw invalidInput or something.
            await _connection.OpenAsync();
            var queryString = @"INSERT INTO RecipeManager.dbo.IngredientTbl (Id, RecipeId, Name, Units, UnitOfMeasure, Description) 
                                  VALUES(@Id, @RecipeId, @Name, @Units, @UnitOfMeasure, @Description)";
            SqlCommand command = new SqlCommand(queryString, _connection);
            item.Id = Guid.NewGuid();
            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@RecipeId", item.RecipeId);
            command.Parameters.AddWithValue("@Name", item.Name);
            //command.Parameters.AddWithValue("@Units", item.Units);
            if (item.Units == null)
            {
                command.Parameters.AddWithValue("@Units", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Units", item.Units);
            }
            command.Parameters.AddWithValue("@UnitOfMeasure", item.UnitOfMeasure ?? "");
            command.Parameters.AddWithValue("@Description", item.Description ?? "");
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            return item.Id;
        }

        public async Task<IEnumerable<Ingredient>> ReadAllAsync() {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, Units, UnitOfMeasure, Description FROM RecipeManager.dbo.IngredientTbl";
            SqlCommand command = new SqlCommand(queryString, _connection);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyIngredientsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Ingredient>> ReadRecipeIngredientsAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, Units, UnitOfMeasure, Description FROM RecipeManager.dbo.IngredientTbl WHERE RecipeId = @RecipeId";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyIngredientsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<IEnumerable<Ingredient>> ReadByQueryAsync( string queryString ) {
            if (!queryString.ToLower().StartsWith("select"))
                throw new InvalidOperationException("Only Select queries are supported as a queryString parameter by this method.");
            if (!queryString.ToLower().Contains("recipemanager.dbo.ingredienttbl"))
                throw new InvalidOperationException("Only queries against 'RecipeManager.dbo.IngredientTbl' are supported as a queryString parameter by this method.");

            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand( queryString, _connection );
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadManyIngredientsAsync(reader);
            await _connection.CloseAsync();
            return result;
        }
        
        public async Task<Ingredient> ReadByIdAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "SELECT Id, RecipeId, Name, Units, UnitOfMeasure, Description FROM RecipeManager.dbo.IngredientTbl WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadIngredientAsync(reader);
            await _connection.CloseAsync();
            return result;
        }
        //Id, RecipeId, Name, Units, UnitOfMeasure, Description
        public async Task<Ingredient> UpdateAsync( Ingredient item ) {
            await _connection.OpenAsync();
            var queryString = "Update RecipeManager.dbo.IngredientTbl SET Name = @Name, Units = @Units, UnitOfMeasure = @UnitOfMeasure, Description = @Description WHERE Id = @Id";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@Name", item.Name ?? "");
            //command.Parameters.AddWithValue("@Units", item.Units);
            if (item.Units == null)
            {
                command.Parameters.AddWithValue("@Units", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Units", item.Units);
            }
            command.Parameters.AddWithValue("@UnitOfMeasure", item.UnitOfMeasure ?? "");
            command.Parameters.AddWithValue("@Description", item.Description ?? "");
            var reader = await command.ExecuteReaderAsync();
            var result = await ReadIngredientAsync(reader);
            await _connection.CloseAsync();
            return result;
        }

        public async Task<Ingredient> SoftUpdateAsync( Ingredient item ) {
            await _connection.OpenAsync();
            SqlCommand command = new SqlCommand();
            var queryString = "Update RecipeManager.dbo.IngredientTbl SET";
            if (item.Name != null) {
                queryString += " Name = @Name,";
                command.Parameters.AddWithValue("@Name", item.Name);
            }
            if (item.Units.HasValue) {
                queryString += " Units = @Units,";
                //command.Parameters.AddWithValue("@Units", item.Units);
                if (item.Units == null)
                {
                    command.Parameters.AddWithValue("@Units", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@Units", item.Units);
                }
            }
            if (item.Units.HasValue && item.UnitOfMeasure != null) {
                queryString += " UnitOfMeasure = @UnitOfMeasure,";
                command.Parameters.AddWithValue("@UnitOfMeasure", item.UnitOfMeasure ?? "");
            }
            if (item.Description != null) {
                queryString += " Description = @Description,";
                command.Parameters.AddWithValue("@Description", item.Description ?? "");
            }
            queryString.TrimEnd(',');
            queryString += " WHERE Id = @Id";
            command.CommandText = queryString;
            command.Connection = _connection; 
            command.Parameters.AddWithValue("@Id", item.Id);
            var reader = command.ExecuteReaderAsync();
            var result = await ReadIngredientAsync(reader.Result);
            await _connection.CloseAsync();
            return result;
        }

        public async Task DeleteAsync( Guid id ) {
            await _connection.OpenAsync();
            var queryString = "DELETE FROM RecipeManager.dbo.IngredientTbl WHERE Id = @Id";
            var command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task DeleteRecipeIngredientsAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "DELETE FROM RecipeManager.dbo.IngredientTbl WHERE RecipeId = @RecipeId";
            var command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task<int> CurrentCountAsync(Guid recipeId) {
            await _connection.OpenAsync();
            var queryString = "SELECT COUNT(1) FROM RecipeManager.dbo.IngredientTbl WHERE RecipeId = @RecipeId";
            SqlCommand command = new SqlCommand(queryString, _connection);
            command.Parameters.AddWithValue("@RecipeId", recipeId);
            var reader = await command.ExecuteReaderAsync();
            await _connection.CloseAsync();
            await reader.ReadAsync();
            var count = reader.GetInt32(0);
            await reader.CloseAsync();            
            return count;
        }

        private async Task<Ingredient> ReadIngredientAsync(SqlDataReader reader) {
            var result = new Ingredient();
            if (reader.HasRows) {
                await reader.ReadAsync();
                result.Id = reader.GetGuid(0);
                result.RecipeId = reader.GetGuid(1);
                result.Name = reader.GetString(2);
                result.Units = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3);
                result.UnitOfMeasure = reader.IsDBNull(3) ? "" : reader.GetString(4);
                result.Description = reader.GetString(5) ?? "";
                await reader.CloseAsync();
            }
            return result;
        }

        private async Task<IEnumerable<Ingredient>> ReadManyIngredientsAsync(SqlDataReader reader) {
            var result = new List<Ingredient>();
            if (reader.HasRows) {
                while (await reader.ReadAsync()) {
                    result.Add(new Ingredient {
                        Id = reader.GetGuid(0),
                        RecipeId = reader.GetGuid(1),
                        Name = reader.GetString(2),
                        Units = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
                        UnitOfMeasure = reader.IsDBNull(3) ? "" : reader.GetString(4),
                        Description = reader.GetString(5) ?? ""
                    });
                }
            }
            await reader.CloseAsync();
            return result;
        }
    }
}
