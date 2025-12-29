using Bookify.Domain.Entities;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Data.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bookify.Tests
{
	public class QeshmDivingSchoolDataTests : IDisposable
	{
		private readonly ServiceProvider _serviceProvider;
		private readonly BookifyDbContext _context;

		public QeshmDivingSchoolDataTests()
		{
			var services = new ServiceCollection();

			// Configure in-memory database
			services.AddDbContext<BookifyDbContext>(options =>
				options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

			// Configure logging
			services.AddLogging();

			// Configure Identity
			services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<BookifyDbContext>()
				.AddDefaultTokenProviders();

			_serviceProvider = services.BuildServiceProvider();
			_context = _serviceProvider.GetRequiredService<BookifyDbContext>();
		}

		[Fact]
		public async Task RoomType_SupportsAmenitiesProperty()
		{
			// Arrange
			var roomType = new RoomType
			{
				Name = "Test Room",
				Description = "Test Description",
				PricePerNight = 100.00m,
				Capacity = 2,
				Amenities = "Test amenity 1, Test amenity 2"
			};

			// Act
			_context.RoomTypes.Add(roomType);
			await _context.SaveChangesAsync();

			// Assert
			var savedRoomType = await _context.RoomTypes.FirstAsync();
			Assert.NotNull(savedRoomType.Amenities);
			Assert.Equal("Test amenity 1, Test amenity 2", savedRoomType.Amenities);
		}

		[Fact]
		public async Task QeshmDivingSchoolRoomTypes_CanBeCreated()
		{
			// Arrange - Create Qeshm Diving School specific room types
			var roomTypes = new[]
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

			// Act
			await _context.RoomTypes.AddRangeAsync(roomTypes);
			await _context.SaveChangesAsync();

			// Assert
			var savedRoomTypes = await _context.RoomTypes.ToListAsync();
			Assert.Equal(5, savedRoomTypes.Count);
			Assert.Contains(savedRoomTypes, rt => rt.Name == "Diver's Bunk Room");
			Assert.Contains(savedRoomTypes, rt => rt.Name == "Ocean View Room");
			Assert.Contains(savedRoomTypes, rt => rt.Name == "Dive Master Suite");
			Assert.Contains(savedRoomTypes, rt => rt.Name == "Family Diving Package Room");
			Assert.Contains(savedRoomTypes, rt => rt.Name == "Coral Reef Villa");
		}

		[Fact]
		public async Task QeshmDivingSchoolRoomTypes_HaveDivingAmenities()
		{
			// Arrange - Create Qeshm Diving School specific room types
			var roomTypes = new[]
			{
				new RoomType
				{
					Name = "Diver's Bunk Room",
					Description = "Budget-friendly shared accommodation for diving enthusiasts",
					PricePerNight = 45.00m,
					Capacity = 4,
					Amenities = "Gear storage, Shared bathroom, Dive equipment drying area"
				}
			};

			// Act
			await _context.RoomTypes.AddRangeAsync(roomTypes);
			await _context.SaveChangesAsync();

			// Assert
			var bunkRoom = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Diver's Bunk Room");
			Assert.NotNull(bunkRoom.Amenities);
			Assert.Contains("Gear storage", bunkRoom.Amenities);
			Assert.Contains("Dive equipment drying area", bunkRoom.Amenities);
		}

		[Fact]
		public async Task QeshmDivingSchoolRoomTypes_HaveCorrectPricing()
		{
			// Arrange
			var roomTypes = new[]
			{
				new RoomType { Name = "Diver's Bunk Room", Description = "Test", PricePerNight = 45.00m, Capacity = 4, Amenities = "Test" },
				new RoomType { Name = "Ocean View Room", Description = "Test", PricePerNight = 85.00m, Capacity = 2, Amenities = "Test" },
				new RoomType { Name = "Dive Master Suite", Description = "Test", PricePerNight = 120.00m, Capacity = 2, Amenities = "Test" },
				new RoomType { Name = "Family Diving Package Room", Description = "Test", PricePerNight = 150.00m, Capacity = 4, Amenities = "Test" },
				new RoomType { Name = "Coral Reef Villa", Description = "Test", PricePerNight = 250.00m, Capacity = 3, Amenities = "Test" }
			};

			// Act
			await _context.RoomTypes.AddRangeAsync(roomTypes);
			await _context.SaveChangesAsync();

			// Assert
			var bunkRoom = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Diver's Bunk Room");
			Assert.Equal(45.00m, bunkRoom.PricePerNight);

			var oceanView = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Ocean View Room");
			Assert.Equal(85.00m, oceanView.PricePerNight);

			var diveMaster = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Dive Master Suite");
			Assert.Equal(120.00m, diveMaster.PricePerNight);

			var familyDiving = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Family Diving Package Room");
			Assert.Equal(150.00m, familyDiving.PricePerNight);

			var coralVilla = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Coral Reef Villa");
			Assert.Equal(250.00m, coralVilla.PricePerNight);
		}

		[Fact]
		public async Task QeshmDivingSchoolRoomTypes_HaveCorrectCapacity()
		{
			// Arrange
			var roomTypes = new[]
			{
				new RoomType { Name = "Diver's Bunk Room", Description = "Test", PricePerNight = 45.00m, Capacity = 4, Amenities = "Test" },
				new RoomType { Name = "Ocean View Room", Description = "Test", PricePerNight = 85.00m, Capacity = 2, Amenities = "Test" },
				new RoomType { Name = "Dive Master Suite", Description = "Test", PricePerNight = 120.00m, Capacity = 2, Amenities = "Test" },
				new RoomType { Name = "Family Diving Package Room", Description = "Test", PricePerNight = 150.00m, Capacity = 4, Amenities = "Test" },
				new RoomType { Name = "Coral Reef Villa", Description = "Test", PricePerNight = 250.00m, Capacity = 3, Amenities = "Test" }
			};

			// Act
			await _context.RoomTypes.AddRangeAsync(roomTypes);
			await _context.SaveChangesAsync();

			// Assert
			var bunkRoom = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Diver's Bunk Room");
			Assert.Equal(4, bunkRoom.Capacity);

			var oceanView = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Ocean View Room");
			Assert.Equal(2, oceanView.Capacity);

			var diveMaster = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Dive Master Suite");
			Assert.Equal(2, diveMaster.Capacity);

			var familyDiving = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Family Diving Package Room");
			Assert.Equal(4, familyDiving.Capacity);

			var coralVilla = await _context.RoomTypes.FirstAsync(rt => rt.Name == "Coral Reef Villa");
			Assert.Equal(3, coralVilla.Capacity);
		}

		[Fact]
		public async Task QeshmDivingSchoolRooms_UseCorrectNamingConvention()
		{
			// Arrange
			var roomType = new RoomType
			{
				Name = "Test Room Type",
				Description = "Test",
				PricePerNight = 100.00m,
				Capacity = 2,
				Amenities = "Test"
			};
			await _context.RoomTypes.AddAsync(roomType);
			await _context.SaveChangesAsync();

			var rooms = new[]
			{
				new Room { RoomNumber = "D101", RoomTypeId = roomType.Id, IsAvailable = true },
				new Room { RoomNumber = "D102", RoomTypeId = roomType.Id, IsAvailable = true },
				new Room { RoomNumber = "V501", RoomTypeId = roomType.Id, IsAvailable = true }
			};

			// Act
			await _context.Rooms.AddRangeAsync(rooms);
			await _context.SaveChangesAsync();

			// Assert
			var savedRooms = await _context.Rooms.ToListAsync();
			Assert.Equal(3, savedRooms.Count);
			Assert.All(savedRooms, room =>
			{
				Assert.True(room.RoomNumber.StartsWith("D") || room.RoomNumber.StartsWith("V"),
					$"Room {room.RoomNumber} should start with D or V prefix");
			});
		}

		[Fact]
		public async Task QeshmBookings_CalculateCostCorrectly()
		{
			// Arrange
			var roomType = new RoomType
			{
				Name = "Ocean View Room",
				Description = "Test",
				PricePerNight = 85.00m,
				Capacity = 2,
				Amenities = "Ocean view, Gear storage"
			};
			await _context.RoomTypes.AddAsync(roomType);
			await _context.SaveChangesAsync();

			var room = new Room
			{
				RoomNumber = "D201",
				RoomTypeId = roomType.Id,
				IsAvailable = true
			};
			await _context.Rooms.AddAsync(room);
			await _context.SaveChangesAsync();

			// Create user
			var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
			var user = new User
			{
				UserName = "test@test.com",
				Email = "test@test.com",
				FirstName = "Test",
				LastName = "User",
				EmailConfirmed = true
			};
			await userManager.CreateAsync(user, "Test123!");

			var booking = new Booking
			{
				UserId = user.Id,
				RoomId = room.Id,
				CheckInDate = DateTime.Today.AddDays(1),
				CheckOutDate = DateTime.Today.AddDays(4),
				TotalCost = 255.00m, // 3 nights * 85.00
				Status = "Confirmed",
				NumberOfNights = 3
			};

			// Act
			await _context.Bookings.AddAsync(booking);
			await _context.SaveChangesAsync();

			// Assert
			var savedBooking = await _context.Bookings
				.Include(b => b.Room)
				.ThenInclude(r => r.RoomType)
				.FirstAsync();

			var expectedCost = savedBooking.Room.RoomType.PricePerNight * 3;
			Assert.Equal(255.00m, savedBooking.TotalCost);
			Assert.Equal(expectedCost, savedBooking.TotalCost);
		}

		[Fact]
		public async Task QeshmBookings_SupportDivingRelatedCancellationReasons()
		{
			// Arrange
			var roomType = new RoomType { Name = "Test", Description = "Test", PricePerNight = 100m, Capacity = 2, Amenities = "Test" };
			await _context.RoomTypes.AddAsync(roomType);
			await _context.SaveChangesAsync();

			var room = new Room { RoomNumber = "D101", RoomTypeId = roomType.Id, IsAvailable = true };
			await _context.Rooms.AddAsync(room);
			await _context.SaveChangesAsync();

			var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
			var user = new User { UserName = "test@test.com", Email = "test@test.com", FirstName = "Test", LastName = "User", EmailConfirmed = true };
			await userManager.CreateAsync(user, "Test123!");

			var booking = new Booking
			{
				UserId = user.Id,
				RoomId = room.Id,
				CheckInDate = DateTime.Today.AddDays(1),
				CheckOutDate = DateTime.Today.AddDays(4),
				TotalCost = 300m,
				Status = "Cancelled",
				NumberOfNights = 3,
				CancellationReason = "Change of diving plans"
			};

			// Act
			await _context.Bookings.AddAsync(booking);
			await _context.SaveChangesAsync();

			// Assert
			var savedBooking = await _context.Bookings.FirstAsync();
			Assert.Equal("Cancelled", savedBooking.Status);
			Assert.NotNull(savedBooking.CancellationReason);
			Assert.Contains("diving", savedBooking.CancellationReason, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task QeshmBookings_SupportDivingEquipmentRejectionReasons()
		{
			// Arrange
			var roomType = new RoomType { Name = "Test", Description = "Test", PricePerNight = 100m, Capacity = 2, Amenities = "Test" };
			await _context.RoomTypes.AddAsync(roomType);
			await _context.SaveChangesAsync();

			var room = new Room { RoomNumber = "D101", RoomTypeId = roomType.Id, IsAvailable = true };
			await _context.Rooms.AddAsync(room);
			await _context.SaveChangesAsync();

			var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
			var user = new User { UserName = "test@test.com", Email = "test@test.com", FirstName = "Test", LastName = "User", EmailConfirmed = true };
			await userManager.CreateAsync(user, "Test123!");

			var booking = new Booking
			{
				UserId = user.Id,
				RoomId = room.Id,
				CheckInDate = DateTime.Today.AddDays(1),
				CheckOutDate = DateTime.Today.AddDays(4),
				TotalCost = 300m,
				Status = "Rejected",
				NumberOfNights = 3,
				RejectionReason = "Room under maintenance for diving equipment"
			};

			// Act
			await _context.Bookings.AddAsync(booking);
			await _context.SaveChangesAsync();

			// Assert
			var savedBooking = await _context.Bookings.FirstAsync();
			Assert.Equal("Rejected", savedBooking.Status);
			Assert.NotNull(savedBooking.RejectionReason);
			Assert.Contains("equipment", savedBooking.RejectionReason, StringComparison.OrdinalIgnoreCase);
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			_serviceProvider.Dispose();
		}
	}
}
