using Contract.Abstractions.Messages;

namespace Contract.Services.Material.Update;

public record UpdateMaterialCommand(UpdateMaterialRequest UpdateMaterialRequest) : ICommand;

