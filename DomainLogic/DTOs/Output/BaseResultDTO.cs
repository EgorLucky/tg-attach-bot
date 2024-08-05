namespace DomainLogic.DTOs.Output;

public record BaseResultDTO
{
    public bool Success { get; init; } = false;
    public string ErrorMessage { get; init; } = "";
    public ErrorType? ErrorType { get; init; }
}

public enum ErrorType { BadRequest, NotFound }

public record BaseResultObjectDataDTO<T>: BaseResultDTO where T : class
{
    public T Result { get; init; } = null;
}

public record BaseResultStructDataDTO<T> : BaseResultDTO where T : struct
{
    public T? Result { get; init; } = null;
}