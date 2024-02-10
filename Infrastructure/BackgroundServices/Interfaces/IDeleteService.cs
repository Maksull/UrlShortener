namespace Infrastructure.BackgroundServices.Interfaces;

public interface IDeleteService
{
    Task DeleteOldUrls();
}
