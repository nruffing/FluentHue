namespace FluentHue
{
    using AutoMapper;
    using FluentHue.Contracts;

    internal static class Mapper
    {
        internal static MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<HueLightStateContract, HueLightState>()
                .ForMember(s => s.ColorX, s => s.MapFrom(c => c.Color != null && c.Color.Length > 1 ? c.Color[0] : 0))
                .ForMember(s => s.ColorY, s => s.MapFrom(c => c.Color != null && c.Color.Length > 1 ? c.Color[1] : 0));

            cfg.CreateMap<HueLightState, HueLightStateContract>()
                .ForMember(s => s.Color, s => s.MapFrom(c => new float[] { c.ColorX, c.ColorY }));
        });
    }
}