﻿using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) :base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Worker> Workers { get; set; }
    }
}
