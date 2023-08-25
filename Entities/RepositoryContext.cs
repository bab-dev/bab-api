using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<VolunteerAvailability> VolunteerAvailabilities { get; set; }
        public DbSet<TransportRequest> TransportRequests { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<MarketSeller> MarketSellers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventVolunteer> EventVolunteers { get; set; }
        public DbSet<BeneficiaryFamily> BeneficiaryFamilies { get; set; }
        public DbSet<BeneficiaryFamilyMember> BeneficiaryFamilyMembers { get; set; }
        public DbSet<BeneficiaryOrganization> BeneficiaryOrganizations { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<Market> Market { get; set; }
        public DbSet<Clocking> Clocking { get; set; }
        public DbSet<VolunteerEventWorkedOn> VolunteerEventsWorkedOn { get; set; }
    }
}
