using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using workshop.wwwapi.Models;
using workshop.wwwapi.Repository;
using workshop.wwwapi.DTO.Responses;
using Microsoft.EntityFrameworkCore;

namespace workshop.wwwapi.Endpoints
{
    public static class SurgeryEndpoint
    {
        //TODO:  add additional endpoints in here according to the requirements in the README.md 
        public static void ConfigurePatientEndpoint(this WebApplication app)
        {
            var surgeryGroup = app.MapGroup("surgery");

            surgeryGroup.MapGet("/patients", GetPatients);
            //surgeryGroup.MapGet("/patient{id}", GetPatienById);
            surgeryGroup.MapGet("/doctors", GetDoctors);
            surgeryGroup.MapGet("/appointmentsbydoctor/{doctor_id}/{patien_id}", GetAppointmentsByDoctor);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetPatients(IRepository<Patient> repository, IMapper mapper)
        {
            var patients = await repository.GetWithNestedIncludes(query =>
                query.Include(p => p.Appointments)
                     .ThenInclude(a => a.Doctor)
            );

            var response = mapper.Map<List<PatientDTO>>(patients);
            
            return TypedResults.Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        public static async Task<IResult> GetDoctors(IRepository<Doctor> repository, IMapper mapper)
        {
            var doctors = await repository.GetWithNestedIncludes(query =>
                query.Include(d => d.Appointments)
                     .ThenInclude(a => a.Patient)
            );

            var response = mapper.Map<List<DoctorDTO>>(doctors);
            return TypedResults.Ok(response);
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public static async Task<IResult> GetPatienById(IRepository<Patient> repository, int id, IMapper mapper)
        //{
        //    var patient = await repository.GetById(id);
        //    if (patient == null) 
        //    {
        //        return TypedResults.NotFound();
        //    }
        //    var response = mapper.Map<List<PatientDTO>>(patient);
        //    return TypedResults.Ok(response);
        //}


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetAppointmentsByDoctor(IRepository<Appointment> repository, int doctor_id, int patient_id)
        {
            var appointment = await repository.GetByCompositKey(doctor_id, patient_id);
            if (appointment != null)
            {
                return TypedResults.Ok(appointment);
            }
            return TypedResults.NotFound();
        }


    }
}
