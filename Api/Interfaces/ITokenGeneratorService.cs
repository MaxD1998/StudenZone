﻿using Domain.Entity;

namespace Api.Interfaces
{
    public interface ITokenGeneratorService
    {
        string GenerateJwt(UserEntity user);

        string GenerateRefreshToken();
    }
}