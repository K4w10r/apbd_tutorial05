using Microsoft.AspNetCore.Mvc;
using apbd05.Properties.Models;
using Microsoft.Data.SqlClient;
namespace apbd05.Properties.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string group)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        connection.Open();
        
        // Create command
        string commandText = "";
        switch (group.ToLower())
        {
            case "description":
            {
                commandText = "SELECT * FROM Animal ORDER BY DESCRIPTION ASC;";
                break;
            }
            case "category":
            {
                commandText = "SELECT * FROM Animal ORDER BY CATEGORY ASC;";
                break;
            }
            case "area":
            {
                commandText = "SELECT * FROM Animal ORDER BY AREA ASC;";
                break;
            }
            default: commandText = "SELECT * FROM Animal ORDER BY NAME ASC;";
                break;
        }
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = commandText;
        
        // Execute command
        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal)
            });
        }
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AnimalDTO animal)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        connection.Open();
        
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName,'','','')";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        
        // Execute command
        command.ExecuteNonQuery();

        return Created("", null);
    }
}