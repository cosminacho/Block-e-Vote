using AutoMapper;
using EVoting.Common.DTOs.VotingDTOs;
using EVoting.Server.Models;

namespace EVoting.Server.Utils
{
    public class AMapperProfile : Profile
    {
        public AMapperProfile()
        {
            CreateMap<InVoterRegisterDTO, User>();
            CreateMap<InVoterRegisterDTO, UserDetail>();
        }

    }
}
