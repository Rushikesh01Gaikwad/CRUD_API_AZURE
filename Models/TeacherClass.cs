namespace CRUD_API.Models
{
    public class TeacherClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public int? DepartmentID { get; set; }
        public string? ProfileImage { get; set; }
        public DepartmentClass? Department { get; set; }

    }
}
