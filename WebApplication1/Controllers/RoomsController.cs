using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/rooms")]

public class RoomsController() : ControllerBase
{
    public static readonly List<Room> _rooms = new()
    {
        new()
        {
            Id = 1, Name = "Lab 204", BuildingCode = "B", Floor = 3, Capacity = 20, HasProjector = true, IsActive = true
        },
        new()
        {
            Id = 2, Name = "04", BuildingCode = "A", Floor = 1, Capacity = 10, HasProjector = false, IsActive = false
        },
        new()
        {
            Id = 3, Name = "306", BuildingCode = "C", Floor = 4, Capacity = 15, HasProjector = true, IsActive = true
        },
        new()
        {
            Id = 4, Name = "Lab 214", BuildingCode = "A", Floor = 2, Capacity = 21, HasProjector = false,
            IsActive = true
        },
        new()
        {
            Id = 5, Name = "Lab 222", BuildingCode = "B", Floor = 3, Capacity = 12, HasProjector = true, IsActive = true
        },


    };

    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        if (room == null)
        {
            return NotFound("Nie znaleziono pokoju z takim ID");
        }

        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public IActionResult GetByBuildingCode(string buildingCode)
    {
        var rooms = _rooms.Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.CurrentCultureIgnoreCase));
        return Ok(rooms);
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        var query = _rooms.AsQueryable();
        if (minCapacity.HasValue) query = query.Where(r => r.Capacity >= minCapacity.Value);
        if (hasProjector.HasValue) query = query.Where(r => r.HasProjector == hasProjector.Value);
        if (activeOnly.HasValue && activeOnly.Value) query = query.Where(r => r.IsActive == activeOnly.Value);
        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Add([FromBody] Room room)
    {
        room.Id = _rooms.Any() ? _rooms.Max(r => r.Id) + 1 : 1;
        _rooms.Add(room);

        return CreatedAtAction(
            nameof(GetById),
            new { id = room.Id },
            room);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Room updatedRoom)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null)
        {
            return NotFound("Nie znaleziono pokoju z takim ID");
        }

        updatedRoom.Id = id;
        var index = _rooms.IndexOf(existingRoom);
        _rooms[index] = updatedRoom;

        return Ok(updatedRoom);
    }
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var room = _rooms.FirstOrDefault(r=> r.Id == id);
        if (room == null)
        {
            return NotFound("Nie znaleziono pokoju z takim ID");
        }

        bool hasReservations = ReservationsController._reservations.Any(r => r.RoomId == id);
        if (hasReservations)
        {
            return Conflict("Nie mozna usunac sali, poniewaz istnieja dla nie przypisane rezerwacje.");
        }
        _rooms.Remove(room);
        return NoContent();
    }
}