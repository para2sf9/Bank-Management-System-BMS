using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementSystem
{
    public class UserInfo
    {
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public string? EmployeeId { get; set; }
    }

    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string DepartmentId { get; set; } = "";
        public string Position { get; set; } = "";
        public double Salary { get; set; }
        public string JoiningDate { get; set; } = "";
        public string Status { get; set; } = "Active";
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Department
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string DepartmentId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Manager { get; set; } = "";
        public string Location { get; set; } = "";
    }

    public class AttendanceRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string Date { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; } = "";
        public string Notes { get; set; } = "";
    }

    public class PayrollRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public double BaseSalary { get; set; }
        public double Bonus { get; set; }
        public double Deductions { get; set; }
        public double NetSalary { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public class DatabaseService
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Employee> _employees;
        private readonly IMongoCollection<Department> _departments;
        private readonly IMongoCollection<AttendanceRecord> _attendance;
        private readonly IMongoCollection<PayrollRecord> _payroll;
        private readonly IMongoCollection<BsonDocument> _users;
        public bool Connected { get; private set; }
        public string? ConnectionError { get; private set; }

        public DatabaseService()
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
                var client = new MongoClient(settings);
                _db = client.GetDatabase("bank_management_system");
                _employees = _db.GetCollection<Employee>("employees");
                _departments = _db.GetCollection<Department>("departments");
                _attendance = _db.GetCollection<AttendanceRecord>("attendance");
                _payroll = _db.GetCollection<PayrollRecord>("payroll");
                _users = _db.GetCollection<BsonDocument>("users");
                _db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Connected = true;
                EnsureIndexes();
                SeedAdmin();
            }
            catch (Exception ex)
            {
                Connected = false;
                ConnectionError = ex.Message;
            }
        }

        private void EnsureIndexes()
        {
            var empIdx = Builders<Employee>.IndexKeys;
            _employees.Indexes.CreateOne(new CreateIndexModel<Employee>(empIdx.Ascending(x => x.EmployeeId), new CreateIndexOptions { Unique = true }));
            _employees.Indexes.CreateOne(new CreateIndexModel<Employee>(empIdx.Ascending(x => x.Email), new CreateIndexOptions { Unique = true }));
            _employees.Indexes.CreateOne(new CreateIndexModel<Employee>(empIdx.Ascending(x => x.DepartmentId)));

            var attIdx = Builders<AttendanceRecord>.IndexKeys;
            _attendance.Indexes.CreateOne(new CreateIndexModel<AttendanceRecord>(attIdx.Ascending(x => x.EmployeeId).Ascending(x => x.Date), new CreateIndexOptions { Unique = true }));

            var payIdx = Builders<PayrollRecord>.IndexKeys;
            _payroll.Indexes.CreateOne(new CreateIndexModel<PayrollRecord>(payIdx.Ascending(x => x.EmployeeId).Ascending(x => x.Month).Ascending(x => x.Year), new CreateIndexOptions { Unique = true }));
        }

        private string Hash(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private void SeedAdmin()
        {
            var adminFilter = Builders<BsonDocument>.Filter.Eq("username", "admin");
            if (_users.Find(adminFilter).FirstOrDefault() == null)
            {
                var doc = new BsonDocument
                {
                    ["username"] = "admin",
                    ["password_hash"] = Hash("admin123"),
                    ["role"] = "admin",
                    ["employee_id"] = BsonNull.Value,
                    ["created_at"] = DateTime.Now,
                    ["is_active"] = true,
                    ["is_deleted"] = false
                };
                _users.InsertOne(doc);
            }
        }

        public UserInfo? Authenticate(string username, string password)
        {
            if (!Connected) return null;
            var filter = Builders<BsonDocument>.Filter.Eq("username", username)
                & Builders<BsonDocument>.Filter.Ne("is_deleted", true)
                & Builders<BsonDocument>.Filter.Ne("is_active", false);
            var user = _users.Find(filter).FirstOrDefault();
            if (user != null && user.TryGetValue("password_hash", out var passwordHash)
                && passwordHash.IsString
                && string.Equals(passwordHash.AsString, Hash(password), StringComparison.OrdinalIgnoreCase))
            {
                return new UserInfo
                {
                    Username = user["username"].AsString,
                    Role = user["role"].AsString,
                    EmployeeId = user.Contains("employee_id") && user["employee_id"].IsString ? user["employee_id"].AsString : null
                };
            }
            return null;
        }

        public async Task<List<Employee>> GetEmployeesAsync(string? search = null)
        {
            if (!Connected) return new List<Employee>();
            FilterDefinition<Employee> filter = Builders<Employee>.Filter.Eq(x => x.IsDeleted, false);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                filter &= Builders<Employee>.Filter.Or(
                    Builders<Employee>.Filter.Regex(x => x.FirstName, new MongoDB.Bson.BsonRegularExpression(s, "i")),
                    Builders<Employee>.Filter.Regex(x => x.LastName, new MongoDB.Bson.BsonRegularExpression(s, "i")),
                    Builders<Employee>.Filter.Regex(x => x.EmployeeId, new MongoDB.Bson.BsonRegularExpression(s, "i")),
                    Builders<Employee>.Filter.Regex(x => x.Email, new MongoDB.Bson.BsonRegularExpression(s, "i"))
                );
            }
            return await _employees.Find(filter).Limit(500).SortBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
        }

        public async Task InsertEmployeeAsync(Employee e) => await _employees.InsertOneAsync(e);
        public async Task UpdateEmployeeAsync(string employeeId, Employee e) => await _employees.ReplaceOneAsync(x => x.EmployeeId == employeeId, e);
        public async Task SoftDeleteEmployeeAsync(string employeeId) => await _employees.UpdateOneAsync(x => x.EmployeeId == employeeId, Builders<Employee>.Update.Set(x => x.IsDeleted, true).Set(x => x.Status, "Inactive"));
        public Employee? GetEmployeeById(string employeeId) => _employees.Find(x => x.EmployeeId == employeeId && !x.IsDeleted).FirstOrDefault();

        public async Task<List<Department>> GetDepartmentsAsync() => await _departments.Find(_ => true).ToListAsync();
        public async Task InsertDepartmentAsync(Department d) => await _departments.InsertOneAsync(d);
        public async Task UpdateDepartmentAsync(string deptId, Department d) => await _departments.ReplaceOneAsync(x => x.DepartmentId == deptId, d);
        public async Task DeleteDepartmentAsync(string deptId) => await _departments.DeleteOneAsync(x => x.DepartmentId == deptId);
        public Department? GetDepartmentById(string deptId) => _departments.Find(x => x.DepartmentId == deptId).FirstOrDefault();

        public async Task MarkAttendanceAsync(AttendanceRecord a)
        {
            var parts = a.Date.Split('-');
            a.Month = int.Parse(parts[1]);
            a.Year = int.Parse(parts[0]);
            var filter = Builders<AttendanceRecord>.Filter.Eq(x => x.EmployeeId, a.EmployeeId) & Builders<AttendanceRecord>.Filter.Eq(x => x.Date, a.Date);
            var existing = await _attendance.Find(filter).FirstOrDefaultAsync();
            if (existing != null)
                await _attendance.ReplaceOneAsync(filter, a);
            else
                await _attendance.InsertOneAsync(a);
        }
        public async Task<List<AttendanceRecord>> GetAttendanceAsync(string? date = null)
        {
            if (!Connected) return new List<AttendanceRecord>();
            if (!string.IsNullOrWhiteSpace(date))
                return await _attendance.Find(x => x.Date == date).ToListAsync();
            return await _attendance.Find(_ => true).SortByDescending(x => x.Date).Limit(1000).ToListAsync();
        }

        public async Task GeneratePayrollAsync(PayrollRecord p)
        {
            p.NetSalary = p.BaseSalary + p.Bonus - p.Deductions;
            var filter = Builders<PayrollRecord>.Filter.Eq(x => x.EmployeeId, p.EmployeeId) & Builders<PayrollRecord>.Filter.Eq(x => x.Month, p.Month) & Builders<PayrollRecord>.Filter.Eq(x => x.Year, p.Year);
            var existing = await _payroll.Find(filter).FirstOrDefaultAsync();
            if (existing != null)
                await _payroll.ReplaceOneAsync(filter, p);
            else
                await _payroll.InsertOneAsync(p);
        }
        public async Task<List<PayrollRecord>> GetPayrollAsync(int? month = null, int? year = null)
        {
            if (!Connected) return new List<PayrollRecord>();
            var builder = Builders<PayrollRecord>.Filter;
            var filter = builder.Empty;
            if (month.HasValue) filter &= builder.Eq(x => x.Month, month.Value);
            if (year.HasValue) filter &= builder.Eq(x => x.Year, year.Value);
            return await _payroll.Find(filter).SortByDescending(x => x.Year).ThenByDescending(x => x.Month).Limit(1000).ToListAsync();
        }

        public Dictionary<string, string> GetEmployeeNamesMap()
        {
            if (!Connected) return new Dictionary<string, string>();
            return _employees.Find(x => !x.IsDeleted).Project(x => new { x.EmployeeId, x.FirstName, x.LastName }).ToList()
                .ToDictionary(x => x.EmployeeId, x => $"{x.FirstName} {x.LastName}");
        }

        public (int employees, int departments, int presentToday) GetStats()
        {
            if (!Connected) return (0, 0, 0);
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return (
                (int)_employees.CountDocuments(x => !x.IsDeleted),
                (int)_departments.CountDocuments(_ => true),
                (int)_attendance.CountDocuments(x => x.Date == today && x.Status == "Present")
            );
        }
    }
}
