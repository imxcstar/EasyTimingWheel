using FreeSql.DataAnnotations;

namespace EasyTimingCloud.Entities
{
    [Table(Name = "task")]
    public class TaskEntity
    {
        [Column(IsPrimary = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = null!;

        public int Type { get; set; }

        public string Cron { get; set; } = null!;

        public bool Enable { get; set; } = true;

        public DateTime CreateTime { get; set; }
    }
}
