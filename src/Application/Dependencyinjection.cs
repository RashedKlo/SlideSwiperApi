using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Slides.Commands.CreateSlide;
using Application.Slides.Commands.UpdateSlide;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR - scans all handlers in this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // FluentValidation - scans all validators in this assembly
        services.AddValidatorsFromAssemblyContaining<CreateSlideValidator>();

        services.AddValidatorsFromAssemblyContaining<UpdateSlideValidator>();


        return services;
    }
}