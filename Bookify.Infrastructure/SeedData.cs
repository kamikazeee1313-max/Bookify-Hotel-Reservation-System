using Bookify.Domain.Entities;
using Bookify.Infrastructure.Data.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookifyDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create only the allowed roles
            string[] allowedRoles = { "Customer", "Admin", "Manager" };
            foreach (var roleName in allowedRoles)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminUser = new User
            {
                UserName = "admin@bookify.com",
                Email = "admin@bookify.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var adminExists = await userManager.FindByEmailAsync(adminUser.Email);
            if (adminExists == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create test customer users
            var customer1 = new User
            {
                UserName = "customer1@bookify.com",
                Email = "customer1@bookify.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var customer2 = new User
            {
                UserName = "customer2@bookify.com",
                Email = "customer2@bookify.com",
                FirstName = "Jane",
                LastName = "Smith",
                EmailConfirmed = true
            };

            var customer1Exists = await userManager.FindByEmailAsync(customer1.Email);
            if (customer1Exists == null)
            {
                var createCustomer1 = await userManager.CreateAsync(customer1, "Customer123!");
                if (createCustomer1.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer1, "Customer");
                }
            }

            var customer2Exists = await userManager.FindByEmailAsync(customer2.Email);
            if (customer2Exists == null)
            {
                var createCustomer2 = await userManager.CreateAsync(customer2, "Customer123!");
                if (createCustomer2.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer2, "Customer");
                }
            }

            // Seed room types if none exist - Qeshm Diving School specific
            if (!context.RoomTypes.Any())
            {
                var roomTypes = new List<RoomType>
                {
                    new RoomType
                    {
                        Name = "Diver's Bunk Room",
                        Description = "Budget-friendly shared accommodation for diving enthusiasts with diving gear storage",
                        PricePerNight = 45.00m,
                        Capacity = 4,
                        Amenities = "Gear storage, Shared bathroom, Air conditioning, Wi-Fi, Dive equipment drying area"
                    },
                    new RoomType
                    {
                        Name = "Ocean View Room",
                        Description = "Comfortable private room with stunning views of the Persian Gulf, perfect for divers",
                        PricePerNight = 85.00m,
                        Capacity = 2,
                        Amenities = "Ocean view, Private bathroom, Gear storage, Air conditioning, Mini-fridge, Wi-Fi"
                    },
                    new RoomType
                    {
                        Name = "Dive Master Suite",
                        Description = "Premium accommodation for dive masters and instructors with dedicated gear room",
                        PricePerNight = 120.00m,
                        Capacity = 2,
                        Amenities = "Private gear room, Ocean view balcony, King-size bed, Private bathroom, Work desk, Wi-Fi, Mini-bar"
                    },
                    new RoomType
                    {
                        Name = "Family Diving Package Room",
                        Description = "Spacious family room designed for diving families with children, includes equipment storage",
                        PricePerNight = 150.00m,
                        Capacity = 4,
                        Amenities = "Two bedrooms, Gear storage for family, Private bathroom, Ocean view, Air conditioning, Wi-Fi, Kitchenette"
                    },
                    new RoomType
                    {
                        Name = "Coral Reef Villa",
                        Description = "Luxury villa with private beach access and dedicated dive guide service",
                        PricePerNight = 250.00m,
                        Capacity = 3,
                        Amenities = "Private beach access, Personal dive guide service, Full gear storage, Ocean view terrace, Luxury bathroom, Living area, Wi-Fi, Mini-bar, Breakfast included"
                    }
                };

                context.RoomTypes.AddRange(roomTypes);
                await context.SaveChangesAsync();
            }

            // Seed rooms if none exist - Qeshm Diving School specific room numbering
            if (!context.Rooms.Any())
            {
                var roomTypes = await context.RoomTypes.ToListAsync();
                var rooms = new List<Room>();

                // Create 20 rooms with Qeshm Diving School naming convention (D for Diver, V for Villa)
                for (int i = 1; i <= 20; i++)
                {
                    var roomType = roomTypes[(i - 1) % roomTypes.Count];
                    string roomPrefix = roomType.Name.Contains("Villa") ? "V" : "D";
                    rooms.Add(new Room
                    {
                        RoomNumber = $"{roomPrefix}{100 + i}",
                        RoomTypeId = roomType.Id,
                        IsAvailable = true // All rooms are available for testing
                    });
                }

                // Make a few rooms unavailable for testing (under maintenance for diving equipment)
                rooms[2].IsAvailable = false; // Room D103
                rooms[7].IsAvailable = false; // Room D108
                rooms[15].IsAvailable = false; // Room D116

                context.Rooms.AddRange(rooms);
                await context.SaveChangesAsync();
            }

            // Seed comprehensive test bookings
            if (!context.Bookings.Any())
            {
                var rooms = await context.Rooms.Take(10).ToListAsync();
                var users = await userManager.Users.ToListAsync();
                var adminUserObj = await userManager.FindByEmailAsync("admin@bookify.com");
                var customer1Obj = await userManager.FindByEmailAsync("customer1@bookify.com");
                var customer2Obj = await userManager.FindByEmailAsync("customer2@bookify.com");

                var testBookings = new List<Booking>
                {
					// Pending Bookings (for admin confirmation testing) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[0].Id, // First room (Diver's Bunk)
						CheckInDate = DateTime.Today.AddDays(2),
                        CheckOutDate = DateTime.Today.AddDays(5),
                        TotalCost = 135.00m, // 3 nights * 45.00
						Status = "Pending",
						CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new Booking
                    {
                        UserId = customer2Obj.Id,
                        RoomId = rooms[1].Id, // Second room
						CheckInDate = DateTime.Today.AddDays(3),
                        CheckOutDate = DateTime.Today.AddDays(6),
                        TotalCost = 255.00m, // 3 nights * 85.00
						Status = "Pending",
						CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },

					// Confirmed Bookings (for cancellation testing) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[2].Id, // Third room
						CheckInDate = DateTime.Today.AddDays(7),
                        CheckOutDate = DateTime.Today.AddDays(10),
                        TotalCost = 360.00m, // 3 nights * 120.00
						Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Booking
                    {
                        UserId = customer2Obj.Id,
                        RoomId = rooms[3].Id, // Fourth room
						CheckInDate = DateTime.Today.AddDays(14),
                        CheckOutDate = DateTime.Today.AddDays(17),
                        TotalCost = 450.00m, // 3 nights * 150.00
						Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-2),
                        CreatedAt = DateTime.UtcNow.AddDays(-4)
                    },

					// Active Bookings (currently ongoing) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[4].Id, // Fifth room
						CheckInDate = DateTime.Today.AddDays(-1),
                        CheckOutDate = DateTime.Today.AddDays(2),
                        TotalCost = 750.00m, // 3 nights * 250.00
						Status = "Active",
						ConfirmedAt = DateTime.UtcNow.AddDays(-5),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },

					// Completed Bookings (past bookings) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer2Obj.Id,
                        RoomId = rooms[5].Id, // Sixth room
						CheckInDate = DateTime.Today.AddDays(-10),
                        CheckOutDate = DateTime.Today.AddDays(-7),
                        TotalCost = 135.00m, // 3 nights * 45.00
						Status = "Completed",
						ConfirmedAt = DateTime.Today.AddDays(-15),
                        CreatedAt = DateTime.Today.AddDays(-20)
                    },

					// Cancelled Bookings (for testing cancellation flow) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[6].Id, // Seventh room
						CheckInDate = DateTime.Today.AddDays(5),
                        CheckOutDate = DateTime.Today.AddDays(8),
                        TotalCost = 255.00m, // 3 nights * 85.00
						Status = "Cancelled",
						CancelledAt = DateTime.UtcNow.AddDays(-1),
                        CancellationReason = "Change of diving plans",
                        RefundAmount = 230.00m,
                        CancellationFee = 25.00m,
                        ConfirmedAt = DateTime.UtcNow.AddDays(-3),
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },

					// Rejected Bookings (for admin rejection testing) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer2Obj.Id,
                        RoomId = rooms[7].Id, // Eighth room
						CheckInDate = DateTime.Today.AddDays(1),
                        CheckOutDate = DateTime.Today.AddDays(4),
                        TotalCost = 360.00m, // 3 nights * 120.00
						Status = "Rejected",
						RejectedAt = DateTime.UtcNow.AddDays(-1),
                        RejectionReason = "Room under maintenance for diving equipment",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },

					// Overlapping Bookings (for availability testing) - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[8].Id, // Ninth room
						CheckInDate = DateTime.Today.AddDays(10),
                        CheckOutDate = DateTime.Today.AddDays(15),
                        TotalCost = 1250.00m, // 5 nights * 250.00
						Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-2),
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Booking
                    {
                        UserId = customer2Obj.Id,
                        RoomId = rooms[9].Id, // Tenth room
						CheckInDate = DateTime.Today.AddDays(20),
                        CheckOutDate = DateTime.Today.AddDays(25),
                        TotalCost = 750.00m, // 5 nights * 150.00
						Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                };

                context.Bookings.AddRange(testBookings);
                await context.SaveChangesAsync();
            }

            // Add some bookings that create availability conflicts for testing
            if (context.Bookings.Count() < 15)
            {
                var rooms = await context.Rooms.Skip(10).Take(3).ToListAsync();
                var customer1Obj = await userManager.FindByEmailAsync("customer1@bookify.com");

                var conflictBookings = new List<Booking>
                {
					// Same room, overlapping dates - Updated with Qeshm prices
					new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[0].Id, // First room
						CheckInDate = DateTime.Today.AddDays(5),
                        CheckOutDate = DateTime.Today.AddDays(8),
                        TotalCost = 135.00m, // 3 nights * 45.00
                        Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Booking
                    {
                        UserId = customer1Obj.Id,
                        RoomId = rooms[0].Id, // Same room
						CheckInDate = DateTime.Today.AddDays(7), // Overlaps with previous
						CheckOutDate = DateTime.Today.AddDays(10),
                        TotalCost = 135.00m, // 3 nights * 45.00
                        Status = "Confirmed",
						ConfirmedAt = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                };

                context.Bookings.AddRange(conflictBookings);
                await context.SaveChangesAsync();
            }
        }
    }
}