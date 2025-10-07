using Microsoft.AspNetCore.Mvc;
using DentistQueue.Shared.DTOs;

namespace DentistQueue.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly List<AppointmentDto> _mockAppointments;

    public AppointmentsController()
    {
        _mockAppointments = GetMockAppointments();
    }

    [HttpGet]
    public ActionResult<List<AppointmentDto>> GetAppointments()
    {
        return Ok(_mockAppointments);
    }

    [HttpGet("{id}")]
    public ActionResult<AppointmentDto> GetAppointment(Guid id)
    {
        var appointment = _mockAppointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
    }

    [HttpPost]
    public ActionResult<AppointmentDto> CreateAppointment([FromBody] CreateAppointmentDto createDto)
    {
        var appointment = new AppointmentDto
        {
            Id = Guid.NewGuid(),
            PatientId = createDto.PatientId,
            DoctorId = createDto.DoctorId,
            ServiceId = createDto.ServiceId,
            StartTime = createDto.StartTime,
            EndTime = createDto.StartTime.AddHours(1), // Предполагаем 1 час на прием
            Status = "Requested",
            PatientNotes = createDto.PatientNotes,
            CreatedAt = DateTime.UtcNow
        };

        _mockAppointments.Add(appointment);
        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}/status")]
    public ActionResult UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusDto updateDto)
    {
        var appointment = _mockAppointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }

        appointment.Status = updateDto.Status;
        appointment.DoctorNotes = updateDto.DoctorNotes;

        return Ok(appointment);
    }

    [HttpGet("patient/{patientId}")]
    public ActionResult<List<AppointmentDto>> GetPatientAppointments(Guid patientId)
    {
        var appointments = _mockAppointments.Where(a => a.PatientId == patientId).ToList();
        return Ok(appointments);
    }

    private List<AppointmentDto> GetMockAppointments()
    {
        return new List<AppointmentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Иван Иванов",
                PatientPhone = "+7 (999) 123-45-67",
                DoctorId = Guid.NewGuid(),
                DoctorName = "Иванов Иван Иванович",
                ServiceId = Guid.NewGuid(),
                ServiceTitle = "Консультация",
                ServicePrice = 1500,
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                Status = "Confirmed",
                PatientNotes = "Плановый осмотр",
                CreatedAt = DateTime.Now.AddDays(-2)
            }
        };
    }
}
