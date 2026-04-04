namespace CRUD_API.Models
{
    public class DepartmentClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public ICollection<TeacherClass>? Teachers { get; set; }
    }
}
