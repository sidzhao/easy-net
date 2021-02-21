namespace EasyNet.Application
{
    public interface IEntityDto : IEntityDto<int>
    {
    }

    public interface IEntityDto<out TPrimaryKey>
    {
        TPrimaryKey Id { get; }
    }
}
