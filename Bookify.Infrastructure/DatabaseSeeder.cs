using Bookify.Domain.Entities;
using Bookify.Infrastructure.Data.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Data
{
	public class DatabaseSeeder : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<DatabaseSeeder> _logger;

		public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<BookifyDbContext>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			// Apply pending migrations
			await context.Database.MigrateAsync(cancellationToken);

			// Seed data
			await SeedRolesAsync(roleManager);
			await SeedRoomTypesAsync(context);
			await SeedRoomsAsync(context);
			var admin = await SeedAdminUserAsync(userManager);
			await SeedBookingsAsync(context, admin);

			_logger.LogInformation("Database seeding completed successfully.");
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			var roles = new[] { "Admin", "Customer", "Manager" };

			foreach (var roleName in roles)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
					_logger.LogInformation("Created role: {RoleName}", roleName);
				}
			}
		}

		private async Task SeedRoomTypesAsync(BookifyDbContext context)
		{
			if (await context.RoomTypes.AnyAsync()) return;

			var roomTypes = new[]
			{
				new RoomType
		{
			Name = "Diver's Bunk Room",
			Description = "Budget-friendly shared accommodation for diving enthusiasts with diving gear storage",
			PricePerNight = 45.00m,
			Capacity = 4,
			ImageUrl = "/images/rooms/divers-bunk.jpg",
			Amenities = "Gear storage, Shared bathroom, Air conditioning, Wi-Fi, Dive equipment drying area"
		},
		new RoomType
		{
			Name = "Ocean View Room",
			Description = "Comfortable private room with stunning views of the Persian Gulf, perfect for divers",
			PricePerNight = 85.00m,
			Capacity = 2,
			ImageUrl = "/images/rooms/ocean-view.jpg",
			Amenities = "Ocean view, Private bathroom, Gear storage, Air conditioning, Mini-fridge, Wi-Fi"
		},
		new RoomType
		{
			Name = "Dive Master Suite",
			Description = "Premium accommodation for dive masters and instructors with dedicated gear room",
			PricePerNight = 120.00m,
			Capacity = 2,
			ImageUrl = "/images/rooms/divemaster-suite.jpg",
			Amenities = "Private gear room, Ocean view balcony, King-size bed, Private bathroom, Work desk, Wi-Fi, Mini-bar"
		},
		new RoomType
		{
			Name = "Family Diving Package Room",
			Description = "Spacious family room designed for diving families with children, includes equipment storage",
			PricePerNight = 150.00m,
			Capacity = 4,
			ImageUrl = "/images/rooms/family-diving.jpg",
			Amenities = "Two bedrooms, Gear storage for family, Private bathroom, Ocean view, Air conditioning, Wi-Fi, Kitchenette"
		},
		new RoomType
		{
			Name = "Coral Reef Villa",
			Description = "Luxury villa with private beach access and dedicated dive guide service",
			PricePerNight = 250.00m,
			Capacity = 3,
			ImageUrl = "/images/rooms/coral-villa.jpg",
			Amenities = "Private beach access, Personal dive guide service, Full gear storage, Ocean view terrace, Luxury bathroom, Living area, Wi-Fi, Mini-bar, Breakfast included"
		}
			};

			await context.RoomTypes.AddRangeAsync(roomTypes);
			await context.SaveChangesAsync();
			_logger.LogInformation("Seeded {Count} Qeshm Diving School room types with images.", roomTypes.Length);
		}

		private async Task SeedRoomsAsync(BookifyDbContext context)
		{
			if (await context.Rooms.AnyAsync()) return;

			var roomTypes = await context.RoomTypes.ToListAsync();

			var bunkRoom = roomTypes.First(rt => rt.Name == "Diver's Bunk Room").Id;
			var oceanView = roomTypes.First(rt => rt.Name == "Ocean View Room").Id;
			var diveMaster = roomTypes.First(rt => rt.Name == "Dive Master Suite").Id;
			var familyDiving = roomTypes.First(rt => rt.Name == "Family Diving Package Room").Id;
			var coralVilla = roomTypes.First(rt => rt.Name == "Coral Reef Villa").Id;

			var rooms = new[]
			{
				// Diver's Bunk Rooms (D prefix for diver rooms, first floor)
				new Room { RoomNumber = "D101", RoomTypeId = bunkRoom, IsAvailable = true },
				new Room { RoomNumber = "D102", RoomTypeId = bunkRoom, IsAvailable = true },
				new Room { RoomNumber = "D103", RoomTypeId = bunkRoom, IsAvailable = true },
				new Room { RoomNumber = "D104", RoomTypeId = bunkRoom, IsAvailable = true },

				// Ocean View Rooms (second floor)
				new Room { RoomNumber = "D201", RoomTypeId = oceanView, IsAvailable = true },
				new Room { RoomNumber = "D202", RoomTypeId = oceanView, IsAvailable = true },
				new Room { RoomNumber = "D203", RoomTypeId = oceanView, IsAvailable = true },

				// Dive Master Suites (third floor)
				new Room { RoomNumber = "D301", RoomTypeId = diveMaster, IsAvailable = true },
				new Room { RoomNumber = "D302", RoomTypeId = diveMaster, IsAvailable = true },

				// Family Diving Package Rooms (fourth floor)
				new Room { RoomNumber = "D401", RoomTypeId = familyDiving, IsAvailable = true },
				new Room { RoomNumber = "D402", RoomTypeId = familyDiving, IsAvailable = true },

				// Coral Reef Villas (V prefix, beachfront separate buildings)
				new Room { RoomNumber = "V501", RoomTypeId = coralVilla, IsAvailable = true },
				new Room { RoomNumber = "V502", RoomTypeId = coralVilla, IsAvailable = true }
			};

			await context.Rooms.AddRangeAsync(rooms);
			await context.SaveChangesAsync();
			_logger.LogInformation("Seeded {Count} Qeshm Diving School rooms.", rooms.Length);
		}

		private async Task<User> SeedAdminUserAsync(UserManager<User> userManager)
		{
			var admin = await userManager.FindByEmailAsync("admin@bookify.com");
			if (admin != null) return admin;

			var adminUser = new User
			{
				UserName = "admin",
				Email = "admin@bookify.com",
				FirstName = "System",
				LastName = "Administrator",
				EmailConfirmed = true
			};

			var result = await userManager.CreateAsync(adminUser, "Admin123!");

			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Admin");
				_logger.LogInformation("Created admin user: admin@bookify.com");

				// fetch persisted user again to ensure it exists in DB
				return await userManager.FindByEmailAsync("admin@bookify.com");
			}
			else
			{
				_logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
				throw new Exception("Admin user seeding failed."); // fail early
			}
		}

		private async Task SeedBookingsAsync(BookifyDbContext context, User admin)
		{
			if (await context.Bookings.AnyAsync()) return;

			var room = await context.Rooms.FirstOrDefaultAsync(); // get any existing room
			if (room == null) return; // no rooms exist

			var booking = new Booking
			{
				RoomId = room.Id,  // use the actual Id from DB
				UserId = admin.Id,
				CheckInDate = DateTime.UtcNow.Date.AddDays(1),
				CheckOutDate = DateTime.UtcNow.Date.AddDays(3),
				NumberOfNights = 2,
				Status = "Confirmed",
				TotalCost = 199.98m,
				CreatedAt = DateTime.UtcNow
			};

			await context.Bookings.AddAsync(booking);
			await context.SaveChangesAsync();
		}

	}
}
