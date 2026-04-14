using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/reservations")]

public class ReservationsController() : ControllerBase
{
    public static readonly List<Reservation> _reservations = new()
    {
        new()
        {
            Id = 1,RoomId = 1,OrganizerName = "Anna Kowalska", Topic = "Warsztaty z HTTP i REST", Date = new(2026, 4, 16),
            StartTime = new(10, 0, 0), EndTime = new(13, 0, 0), Status = "Confirmed"
        },
        new()
        {
            Id = 2,RoomId = 2, OrganizerName = "Mateusz Ulberg", Topic = "Przyjęcie urodzinowe", Date = new(2026, 6, 10),
            StartTime = new(12, 0, 0), EndTime = new(18, 0, 0), Status = "Confirmed"
        },
        new()
        {
            Id = 3,RoomId = 3, OrganizerName = "Piort Trytytka", Topic = "Impreza firmowa", Date = new(2026, 10, 1),
            StartTime = new(13, 0, 0), EndTime = new(17, 0, 0), Status = "Confirmed"
        },
        new()
        {
            Id = 4,RoomId = 4, OrganizerName = "Anna Kowalenka", Topic = "Warsztaty ze zdrowego odżywiania",
            Date = new(2026, 9, 2), StartTime = new(14, 0, 0), EndTime = new(19, 0, 0), Status = "Confirmed"
        },
        new()
        {
            Id = 5,RoomId = 5, OrganizerName = "Adam Arszen", Topic = "Bal ósmoklasisty", Date = new(2026, 7, 8),
            StartTime = new(15, 0, 0), EndTime = new(20, 0, 0), Status = "Confirmed"
        },
        new()
        {
            Id = 6,RoomId = 6, OrganizerName = "Teodor Opalenka", Topic = "Impreza urodzinowa", Date = new(2026, 8, 7),
            StartTime = new(13, 30, 0), EndTime = new(18, 0, 0), Status = "Confirmed"
        },
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var query = _reservations.AsQueryable();
    
        if (date.HasValue) query = query.Where(r => r.Date == date.Value);
        if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        if (roomId.HasValue) query = query.Where(r => r.RoomId == roomId.Value);
    
        return Ok(query.ToList());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            return NotFound("Nie znaleziono rezerwacji z takim ID");
        }

        return Ok(reservation);
    }


    [HttpPost]
    public IActionResult Add([FromBody] Reservation reservation)
    {
        var room = RoomsController._rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null)
        {
            return NotFound("Sala nie istnieje.");
        }

        if (!room.IsActive)
        {
            return BadRequest("Sala jest nieaktywna.");
        }

        bool overlap = _reservations.Any(r => r.RoomId == reservation.RoomId &&
                                              r.Date == reservation.Date &&
                                              reservation.StartTime < r.EndTime &&
                                              r.StartTime < reservation.EndTime);
        if (overlap)
        {
            return Conflict("Termin jest juz zajety.");
        }

        reservation.Id = _reservations.Any() ? _reservations.Max(r => r.Id) + 1 : 1;
        _reservations.Add(reservation);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Reservation updatedReservation )
    {
        var existingReservation = _reservations.FirstOrDefault(r => r.Id == id);
        if (existingReservation == null)
        {
            return NotFound("Nie znaleziono rezerwacji z takim ID");
        }
        updatedReservation.Id = id;
        var index = _reservations.IndexOf(existingReservation);
        _reservations[index] = updatedReservation;
        return Ok(updatedReservation);
    }
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            return NotFound("Nie znaleziono rezerwacji z takim ID.");
        }
        _reservations.Remove(reservation);
        return NoContent();
    }
}
