﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Household_Budgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Household_Budgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty(nameof(Households.Owner))]
        public virtual List<Households> OwnerHouseHold { get; set; }
        [InverseProperty(nameof(Households.Users))]
        public virtual List<Households> ParticipateHouseHold { get; set; }
        [InverseProperty(nameof(Households.InviteUsers))]
        public virtual List<Households> InviteUsers { get; set; }

        public virtual List<Transaction> CreateTransactions { get; set; }

        public ApplicationUser()
        {
            OwnerHouseHold = new List<Households>();
            ParticipateHouseHold = new List<Households>();
            InviteUsers = new List<Households>();
            CreateTransactions = new List<Transaction>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Households> Allhouseholds { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Households>()
             .HasMany<Categories>(s => s.Categories)
             .WithRequired(s => s.Households)
             .WillCascadeOnDelete(false);
        }
    }
}