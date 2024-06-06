using Contract.Abstractions.Messages;

namespace Contract.Services.Material.Create;

public record CreateMaterialCommand(CreateMaterialRequest createMaterialRequest) : ICommand;
