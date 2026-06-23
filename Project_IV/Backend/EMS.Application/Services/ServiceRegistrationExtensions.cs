using EMS.Application.Services.Interfaces;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EMS.Application.Services
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<ISpeakerRepository, SpeakerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IParticipantEventRepository, ParticipantEventRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISpeakerService, SpeakerService>();
            services.AddScoped<IParticipantService, ParticipantService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddSingleton<JwtService>();
            services.AddSingleton<IPasswordService, PasswordService>();
            return services;
        }
    }
}
