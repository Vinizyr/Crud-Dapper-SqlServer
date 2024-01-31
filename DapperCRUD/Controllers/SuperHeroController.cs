using Dapper;
using DapperCRUD.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SuperHeroController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHeroes>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var heroes = await connection.QueryAsync<SuperHeroes>("select * from superheroes");
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHeroes>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var heroe = await connection.QueryFirstAsync<SuperHeroes>("select * from superheroes where id = @Id", 
                new { Id = heroId});
            return Ok(heroe);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHeroes>>> CreateHero(SuperHeroes hero)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into superheroes (nome, firstname, lastname, place) values (@Nome, @FirstName, @LastName, @Place)", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHeroes>>> UpdateHero(SuperHeroes hero)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set nome = @Nome, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", 
                hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("heroId")]
        public async Task<ActionResult<List<SuperHeroes>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from superheroes where id = @Id",
                new {Id = heroId});
            return Ok(await SelectAllHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHeroes>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHeroes>("select * from superheroes");
        }
    }
}
