namespace DomainLogic.DTOs.Output;

public record GetFileByIdResultDTO : BaseResultObjectDataDTO<DomainLogic.Entities.File>;
public record GetUserByIdResultDTO : BaseResultObjectDataDTO<DomainLogic.Entities.TelegramUser>;